using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICastleSiegePanel : GUIDialogueWindow
    {
        [SerializeField] private GUICastleGarnisonCharacterSlotWidget _slotWidgetPrefab;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Button _applySiegeButton, _cancelSiegeButton;
        private GUICommonDialogueWindow _startSiegeDialogueWindow, _failedStartSiegeWindow;
        public ControllableCharacter SelectedSiegeCharacter { set; get; }


        private readonly Dictionary<int, GUICastleGarnisonCharacterSlotWidget> _widgets = new();


        public delegate void OnCharacterSiegeEventDelegate(PlayerSiegeData playerSiegeData,
            ControllableCharacter controllableCharacter);

        public event OnCharacterSiegeEventDelegate CharacterApplied
        {
            add => characterApplied += value;
            remove => characterApplied -= value;
        }

        public event OnCharacterSiegeEventDelegate CharacterRemoved
        {
            add => characterRemoved += value;
            remove => characterRemoved -= value;
        }

        private event OnCharacterSiegeEventDelegate characterApplied, characterRemoved;

        public override void Show()
        {
            base.Show();
            GameManager.Instance.GUIManager.RecentCharactersWidget.Locked = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (_startSiegeDialogueWindow != null)
                _startSiegeDialogueWindow.CloseAndDestroy();
            GameManager.Instance.GUIManager.RecentCharactersWidget.Locked = false;
        }

        public void ApplySiegeCharacter(PlayerSiegeData playerSiegeData, ControllableCharacter controllableCharacter)
        {
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            playerSiegeData.appliedCharacters.Add(controllableCharacter);
            Debug.Log($"Applied {controllableCharacter.title}");
            mapScenarioHandler.NewSiegeRoundBegin += controllableCharacter.UpdateBuffsOnTurn;
            characterApplied?.Invoke(playerSiegeData, controllableCharacter);
        }

        public void RemoveSiegeCharacter(PlayerSiegeData playerSiegeData, ControllableCharacter controllableCharacter)
        {
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            playerSiegeData.appliedCharacters.Remove(controllableCharacter);
            mapScenarioHandler.NewSiegeRoundBegin -= controllableCharacter.UpdateBuffsOnTurn;
            Debug.Log($"Removed {controllableCharacter.title}");
            characterRemoved?.Invoke(playerSiegeData, controllableCharacter);
        }

        private bool CheckIfTargetCastleEmpty(CastleSiegeData castleSiegeData)
        {
            var p = castleSiegeData.defendingPlayerData;
            var c = p.Player.controlledCharacters;
            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].InsideGarnison || c[i].IsInsideTheCastle)
                {
                    return false;
                }
            }

            return true;
        }

        private void BeginSiege(CastleSiegeData castleSiegeData)
        {
            characterApplied = characterRemoved = null;
            _startSiegeDialogueWindow?.CloseAndDestroy();
            _cancelSiegeButton.gameObject.SetActive(false);
            _applySiegeButton.gameObject.SetActive(false);
            var guiManager = GameManager.Instance.GUIManager;
            guiManager.CastleQuickButton.Hide();
            guiManager.EscapeSiegeButton.Show();
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var siegedCastle = castleSiegeData.siegedCastle;
            var defendingPlayer = siegedCastle.Player;
            var attackingPlayer = castleSiegeData.attackingPlayerData.Player;
            var allAttackingCharacters = new List<ControllableCharacter>();
            var allDefendingCharacters = new List<ControllableCharacter>();
            var hasNoDefenders = CheckIfTargetCastleEmpty(castleSiegeData);
            if (hasNoDefenders)
            {
                mapScenarioHandler.ChangeCastleOwner(siegedCastle, attackingPlayer);
                Hide();
                return;
            }

            mapScenarioHandler.SetupSiege(attackingPlayer, defendingPlayer);


            var widgetsValues = _widgets.Values;
            foreach (var widget in widgetsValues)
            {
                var character = widget.controllableCharacter;
                character.DeselectAndRemoveControll(attackingPlayer.Color);
                allAttackingCharacters.Add(character);
                castleSiegeData.attackingPlayerData.charsPositionBeforeSiege.TryAdd(character, character.Position);
            }

            CharacterApplied +=
                (playerSiegeData, character) =>
                {
                    ControllableCharacter.OnCharactersEventDelegate stackDemolished = null;
                    ControllableCharacter.OnIntegerChanged
                        quantityChanged = (value) =>
                        {
                            playerSiegeData.AddDefeatedCharacter((character).BaseCharacterObject, value);
                        };
                    stackDemolished = (target, source) =>
                    {
                        playerSiegeData.appliedCharacters.Remove(character);
                        playerSiegeData.Player.CheckAliveCharactersCount(castleSiegeData, playerSiegeData);
                        character.StackDemolished -= stackDemolished;
                        character.QuantityChanged -= quantityChanged;
                    };
                    character.StackDemolished += stackDemolished;
                    character.QuantityChanged += quantityChanged;
                };
            var allCharacters = defendingPlayer.controlledCharacters;
            foreach (var character in allCharacters)
            {
                if (character.InsideGarnison)
                {
                    castleSiegeData.siegedCastle.RemoveFromGarnisonCharacterStack(character);
                    character.Hide();
                    castleSiegeData.defendingPlayerData.charsPositionBeforeSiege.TryAdd(character, character.Position);
                    allDefendingCharacters.Add(character);
                }
                else if (character.IsInsideTheCastle)
                {
                    ApplySiegeCharacter(castleSiegeData.defendingPlayerData, character);
                    castleSiegeData.defendingPlayerData.charsPositionBeforeSiege.TryAdd(character, character.Position);
                    allDefendingCharacters.Add(character);
                }
            }

            castleSiegeData.attackingPlayerData.allCharacters = allAttackingCharacters;
            castleSiegeData.defendingPlayerData.allCharacters = allDefendingCharacters;

            attackingPlayer.actionCellPool.ResetAll();
            siegedCastle.CastleSiegeSetup(castleSiegeData);
            siegedCastle.SiegeEnd += ResetSiegeAndHide;
        }

        public void SetupSiegePanel(PlayerSiegeData playerSiegeData)
        {
            var guiManager = GameManager.Instance.GUIManager;
            ClearSiegeSlots();
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var characters = playerSiegeData.allCharacters;
            foreach (var character in characters)
            {
                mapScenarioHandler.NewSiegeRoundBegin -= character.UpdateBuffsOnTurn;
                mapScenarioHandler.NewSiegeRoundBegin += character.UpdateBuffsOnTurn;
                if (character.IsInsideTheCastle) continue;
                character.PlaceOnTerrain(playerSiegeData.SiegedCastleTerrain);

                AddCharacterSlot(character);
            }

            var player = playerSiegeData.Player;
            player.UpdateVisionForEveryEnemy();
            guiManager.RecentCharactersWidget.SetGUI(null, null);
            guiManager.RecentCharactersWidget.TopTierCharacter = null;
            Show();
            player.SetActiveContext(PlayerContext.PlanningSiege);
        }

        private void ResetSiegeAndHide(CastleSiegeData castleSiegeData)
        {
            Hide();
            _cancelSiegeButton.gameObject.SetActive(true);
            _applySiegeButton.gameObject.SetActive(true);
            ClearSiegeSlots();
        }

        public void ClearSiegeSlots()
        {
            foreach (var widget in _widgets.Values)
            {
                Destroy(widget.gameObject);
            }

            _widgets.Clear();
        }

        public void SetupSiegeData(CastleSiegeData castleSiegeData)
        {
            ClearSiegeSlots();
            _applySiegeButton.onClick.RemoveAllListeners();
            _cancelSiegeButton.onClick.RemoveAllListeners();

            _applySiegeButton.onClick.AddListener((() =>
            {
                bool hasAny = false;
                bool hasAnyHero = false;
                var widgets = _widgets;
                foreach (var widget in widgets)
                {
                    var ch = widget.Value.controllableCharacter;
                    if (ch is not Hero) continue;
                    hasAnyHero = true;
                    break;
                }

                Debug.Log(widgets.Count);
                hasAny = widgets.Count > 0;

                if (!hasAny || !hasAnyHero)
                {
                    _failedStartSiegeWindow = GUIDialogueFactory.CreateCommonDialogueWindow();
                    if (!hasAny)
                    {
                        _failedStartSiegeWindow.Title = "Select characters!";
                        _failedStartSiegeWindow.Description = "No characters are applied to siege";
                    }
                    else if (!hasAnyHero)
                    {
                        _failedStartSiegeWindow.Title = "Hero required!";
                        _failedStartSiegeWindow.Description = "You need to select any hero to siege";
                    }

                    return;
                }

                if (_startSiegeDialogueWindow != null) return;
                Locked = true;
                GameManager.Instance.GUIManager.LeftSidebar.Hide();
                GameManager.Instance.GUIManager.CastleQuickButton.Hide();
                _startSiegeDialogueWindow = GUIDialogueFactory.CreateCommonDialogueWindow();
                _startSiegeDialogueWindow.Title = "Last chance";
                _startSiegeDialogueWindow.Description = "Begin siege?";
                castleSiegeData.attackingPlayerData.Player.isInputLocked = true;
                _startSiegeDialogueWindow.Applied = () =>
                {
                    Locked = false;
                    castleSiegeData.attackingPlayerData.Player.isInputLocked = false;
                    BeginSiege(castleSiegeData);
                };
                _startSiegeDialogueWindow.Canceled = () =>
                {
                    Locked = false;
                    castleSiegeData.attackingPlayerData.Player.isInputLocked = false;
                    foreach (var (key, value) in _widgets)
                    {
                        value.button.interactable = true;
                    }
                };

                foreach (var (key, value) in _widgets)
                {
                    value.button.interactable = false;
                }
            }));

            _cancelSiegeButton.onClick.AddListener(() =>
            {
                Locked = false;
                castleSiegeData.attackingPlayerData.Player.isInputLocked = false;
                castleSiegeData.attackingPlayerData.Player.SetActiveContext(PlayerContext.Default);
                foreach (var widget in _widgets)
                {
                    var character = widget.Value.controllableCharacter;
                    character.Show();
                    character.PlayableTerrain.TerrainNavigator.NavigationMap.BlockCell(character.Position.x,
                        character.Position.y);
                    widget.Value.button.interactable = true;
                }

                ClearSiegeSlots();
                Hide();
            });
        }

        public void RemoveCharacterSlot(ControllableCharacter character)
        {
            GUICastleGarnisonCharacterSlotWidget widget = null;
            int hashcode = character.GetHashCode();
            if (_widgets.TryGetValue(hashcode, out widget))
            {
                _widgets.Remove(hashcode);
                Destroy(widget.gameObject);
            }
        }

        public GUICastleGarnisonCharacterSlotWidget AddCharacterSlot(ControllableCharacter character)
        {
            var widget =
                GameObject.Instantiate(_slotWidgetPrefab, Vector3.zero, Quaternion.identity, _contentRoot);
            character.DeselectWithoutControll(character.playerOwnerColor);
            widget.controllableCharacter = character;
            _widgets.TryAdd(character.GetHashCode(), widget);
            widget.button.onClick.AddListener((() => { SelectedSiegeCharacter = widget.controllableCharacter; }));
            return widget;
        }
    }
}