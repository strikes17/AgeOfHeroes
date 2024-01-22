using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.AI;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using Mirror;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace AgeOfHeroes
{
    public enum PlayerContext
    {
        Default,
        UsingSpell,
        Awaiting,
        PlacingCharacter,
        EditingGarnison,
        PlanningSiege
    }

    public abstract class Player : NetworkBehaviour
    {
        public int FightingPoints
        {
            get
            {
                int points = 0;
                foreach (var character in controlledCharacters)
                {
                    points += character.FightingPoints;
                }

                return points;
            }
        }
        
        public abstract void OnSiegeStartFighting();
        public abstract void OnSiegeEnded();

        public virtual void CheckAliveCharactersCount(CastleSiegeData castleSiegeData, PlayerSiegeData playerSiegeData)
        {
            if(castleSiegeData.SiegeResult == SiegeResult.AttackerFlee)return;
            var characters = playerSiegeData.appliedCharacters;
            Debug.Log($"{playerSiegeData.Player.Color} count: {characters.Count}");
            if (characters.Count <= 0)
            {
                castleSiegeData.loserPlayer = playerSiegeData.Player;
                castleSiegeData.siegedCastle.OnSiegeEnded(castleSiegeData);
                castleSiegeData.SiegeResult = castleSiegeData.loserPlayer == this
                    ? SiegeResult.AttackerLost
                    : SiegeResult.AttackerWin;
            }
        }

        public virtual void SetupSiegeGuards(CastleSiegeData siegeData)
        {
            var guards = siegeData.siegedCastle.GuardsCharacters;
            var markers = siegeData.SiegedCastleTerrain.SpawnedMarkers;
            foreach (var guard in guards)
            {
                foreach (var marker in markers)
                {
                    var guardMarker = marker as AIGuardMarker;
                    if (guardMarker == null) continue;
                    if (guard.GetTier() != guardMarker.Tier) continue;
                    var position = new Vector2Int(guardMarker.Position.x, guardMarker.Position.y);
                    bool cellBlocked = siegeData.SiegedCastleTerrain.TerrainNavigator.NavigationMap.IfCellBlocked(
                        guardMarker.Position.x,
                        guardMarker.Position.y);
                    if (cellBlocked) continue;
                    guard.Position = position;
                }
            }
        }

        protected bool isBanished = false;

        public bool AIControlls
        {
            get => _aiControlls;
            set
            {
                var skipTurnButton = GameManager.Instance.GUIManager.skipTurnButton;
                _aiControlls = value;
                skipTurnButton.InteractableForAI = true;
            }
        }
        protected bool _aiControlls;

        public int Gold
        {
            set
            {
                gold = value < 0 ? 0 : value > 50000 ? 50000 : value;
                goldChanged?.Invoke(gold);
            }
            get => gold;
        }

        public int Gems
        {
            set
            {
                gems = value < 0 ? 0 : value > 100 ? 100 : value;
                gemsChanged?.Invoke(gems);
            }
            get => gems;
        }

        public Dictionary<PlayableTerrain, List<Vector3Int>> OpenedFogDictionary =
            new Dictionary<PlayableTerrain, List<Vector3Int>>();

        public delegate void OnResourceValueChangedEventDelegate(int deltaValue);

        public delegate void PlayerEventDelegate(Player player);

        public delegate void OnControllableCharacterEventDelegate(ControllableCharacter controllableCharacter);

        public delegate void OnPlayerTerrainDelegate(PlayableTerrain playableTerrain);

        private event PlayerEventDelegate banished;

        public event PlayerEventDelegate Banished
        {
            add => banished += value;
            remove => banished -= value;
        }

        protected virtual void OnPlayerBanished(Player player)
        {
            banished?.Invoke(player);
        }

        public ControllableCharacter CheckTopTierCharacter()
        {
            int maxTier = -1;
            foreach (var c in controlledCharacters)
            {
                var tier = c.GetTier();
                if (tier > maxTier)
                {
                    maxTier = tier;
                    topTierCharacter = c;
                }
            }

            return topTierCharacter;
        }

        public ControllableCharacter TopTierSiegeCharacter
        {
            get
            {
                int maxTier = -1;
                foreach (var c in controlledCharacters)
                {
                    var tier = c.GetTier();
                    if (tier > maxTier && c.IsInsideTheCastle)
                    {
                        maxTier = tier;
                        topTierCharacter = c;
                    }
                }

                return topTierCharacter;
            }
        }

        public event OnControllableCharacterEventDelegate CharacterSpawned
        {
            add => characterSpawned += value;
            remove => characterSpawned -= value;
        }

        public event OnControllableCharacterEventDelegate CharacterDestroyed
        {
            add => characterDestroyed += value;
            remove => characterDestroyed -= value;
        }

        public event OnControllableCharacterEventDelegate SelectedAnyCharacter
        {
            add => selectedAnyCharacter += value;
            remove => selectedAnyCharacter -= value;
        }

        public event OnControllableCharacterEventDelegate DeselectedAnyCharacter
        {
            add => deselectedAnyCharacter += value;
            remove => deselectedAnyCharacter -= value;
        }

        public void OnSelectedAnyCharacter(ControllableCharacter controllableCharacter)
        {
            selectedAnyCharacter?.Invoke(controllableCharacter);
        }

        public void OnDeselectedAnyCharacter(ControllableCharacter controllableCharacter)
        {
            deselectedAnyCharacter?.Invoke(controllableCharacter);
        }

        protected event OnControllableCharacterEventDelegate characterSpawned, characterDestroyed;
        protected event OnControllableCharacterEventDelegate selectedAnyCharacter, deselectedAnyCharacter;

        public event OnResourceValueChangedEventDelegate GemsChanged
        {
            add => gemsChanged += value;
            remove => gemsChanged -= value;
        }

        public event OnResourceValueChangedEventDelegate GoldChanged
        {
            add => goldChanged += value;
            remove => goldChanged -= value;
        }

        protected event OnResourceValueChangedEventDelegate goldChanged;
        protected event OnResourceValueChangedEventDelegate gemsChanged;

        public event PlayerEventDelegate TurnRecieved
        {
            add => turnRecieved += value;
            remove => turnRecieved -= value;
        }

        public event PlayerEventDelegate TurnSkipped
        {
            add => turnSkipped += value;
            remove => turnSkipped -= value;
        }

        public event OnPlayerTerrainDelegate ActiveTerrainChanged
        {
            add => activeTerrainChanged += value;
            remove => activeTerrainChanged -= value;
        }

        protected event OnPlayerTerrainDelegate activeTerrainChanged;

        protected event PlayerEventDelegate turnRecieved;
        protected event PlayerEventDelegate turnSkipped;
        [SerializeField] protected PlayerActionCellPool _actionCellPool;
        [SerializeField] protected PlayerFogOfWarController _fogOfWarController;
        protected int gold, gems;
        public bool isHuman;
        public PlayerColor Color;
        public Color realColor;

        public ControllableCharacter selectedCharacter,
            previousSelectedCharacter,
            topTierCharacter;

        public Castle SelectedCastle;

        public Castle LastSelectedCastle
        {
            get => _lastSelectedCastle;
            set
            {
                _lastSelectedCastle = value;
                if (_lastSelectedCastle != null)
                {
                    _GUIManager.CastleQuickButton.Show();
                }
                else
                {
                    _GUIManager.CastleQuickButton.Hide();
                }
            }
        }

        protected Castle _lastSelectedCastle; 
        public List<ControllableCharacter> controlledCharacters = new List<ControllableCharacter>();
        public Hero MainHero;
        public List<Castle> controlledCastles = new List<Castle>();
        protected GUISpellBook _guiSpellBook;
        protected bool isInteractionWithWorldEnabled = false, _isInputLocked = false;
        protected MagicSpellCombatData _magicSpellCombatData;
        protected GUILeftSidebar _guiLeftSidebar;
        protected int topTierCharacterValue = -1;
        protected Sprite _bannerSprite;
        protected bool _isPassive = false;
        protected TerrainManager _terrainManager;
        protected BasicAIPlayerController _basicAIPlayerController;
        protected PlayableTerrain _activeTerrain;
        protected CastleSiegeData _castleSiegeData;
        protected Fraction _fraction;

        protected GUIManager _GUIManager;
        public Sprite Banner => _bannerSprite;

        public bool isPassive
        {
            get => _isPassive;
            set => _isPassive = value;
        }

        public PlayerFogOfWarController fogOfWarController => _fogOfWarController;

        public PlayerActionCellPool actionCellPool => _actionCellPool;

        public bool isInputLocked
        {
            get => _isInputLocked;
            set => _isInputLocked = value;
        }

        public PlayableTerrain ActiveTerrain
        {
            get => _activeTerrain;
            set
            {
                if (value != null)
                    if (_activeTerrain != value)
                    {
                        _activeTerrain = value;
                        OnActiveTerrainChanged(_activeTerrain);
                        _terrainManager.HideAllTerrainsExcept(_activeTerrain);
                    }
            }
        }

        public bool IsInsideTheCastle
        {
            get { return _terrainManager.CurrentPlayableMap.IsCastleTerrain(ActiveTerrain); }
        }

        public MagicSpellCombatData SpellCombatData => _magicSpellCombatData;


        public BasicAIPlayerController AIPlayerController => _basicAIPlayerController;

        public Fraction Fraction
        {
            get => _fraction;
            set => _fraction = value;
        }

        public CastleSiegeData SiegeData => _castleSiegeData;

        public bool IsBanished
        {
            get => isBanished;
        }

        public delegate void OnContextEventDelegate(PlayerContext context);

        public virtual void OnCastleBeingSeigedBegin(CastleSiegeData siegeData)
        {
        }

        public virtual void OnEnemyCastleSiegeBegin(CastleSiegeData siegeData)
        {
        }

        public virtual void OnCastleBeingSeigedPlanningStage(CastleSiegeData siegeData)
        {
            _castleSiegeData = siegeData;
        }

        public virtual void OnEnemyCastleSiegePlanningStage(CastleSiegeData siegeData)
        {
            _castleSiegeData = siegeData;
        }

        protected event OnContextEventDelegate ContextChanged
        {
            add => contextChanged += value;
            remove => contextChanged -= value;
        }

        protected event OnContextEventDelegate contextChanged;
        public PlayerContext activePlayerContext;

        public void Banish()
        {
            GameManager.Instance.GUIManager.GameResultWindow.SetPlayerLoseState(Color);
            for (int i = 0; i < controlledCharacters.Count; i++)
            {
                controlledCharacters[i].playerOwnerColor = PlayerColor.Neutral;
            }

            isBanished = true;
            OnPlayerBanished(this);
        }

        public void SetAIController(BasicAIPlayerController aiPlayerController)
        {
            _basicAIPlayerController = aiPlayerController;
            // _basicAIPlayerController.TurnEnded +=
            //     (playerColorId) => GameManager.Instance.MapScenarioHandler.SkipPlayerTurn();
            _basicAIPlayerController.TurnEnded += (PlayerColor playerColor) =>
                GameManager.Instance.GUIManager.skipTurnButton.SkipTurnAI();
        }

        public virtual void OnActiveTerrainChanged(PlayableTerrain playableTerrain)
        {
            _guiLeftSidebar.GUICastleGarnisonPanel.SetMode(GUICastleGarnisonEditWidget.Mode.Info);
            _guiLeftSidebar.GUICastleGarnisonPanel.Hide();
            SetActiveContext(PlayerContext.Default);
            fogOfWarController.ShowFogOfWarForTerrain(playableTerrain);
            GameManager.Instance.MainCamera.SetBounds(playableTerrain.Size);
            activeTerrainChanged?.Invoke(playableTerrain);
        }

        public void ReserveTerrainFog(PlayableTerrain playableTerrain)
        {
            OpenedFogDictionary.TryAdd(playableTerrain, new List<Vector3Int>());
            foreach (var controllableCharacter in controlledCharacters)
            {
                controllableCharacter.ReserveTerrainFog(playableTerrain);
            }
        }

        public virtual void Init()
        {
            _terrainManager = GameManager.Instance.terrainManager;
            _bannerSprite = ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[Color]);
            Gold = 100;
            Gems = 0;
            _GUIManager = GameManager.Instance.GUIManager;
            _guiSpellBook = GameManager.Instance.GUISpellBook;
            _guiSpellBook.Opening += DisableInteractionWithWorld;
            _guiSpellBook.Closed += EnableInteractionWithWorld;
            _guiLeftSidebar = _GUIManager.LeftSidebar;
            _GUIManager.GameResultWindow.PlayerWon += color =>
            {
                _GUIManager.skipTurnButton.RemoveAllListeners();
                _GUIManager.PauseMenuWindow.Locked = true;
                _GUIManager.HeroPortraitWidget.portraitWidgetMini.Disable();
                _GUIManager.HeroPortraitWidget.Disable();
                GameManager.Instance.GUIManager.skipTurnButton.Interactable = false;
                _GUIManager.RecentCharactersWidget.Locked = true;
                DisableInteractionWithWorld();
                _GUIManager.BgLockImage.gameObject.SetActive(true);
            };
        }

        public void LoadFromSerializable(SerializablePlayerData serializablePlayerData)
        {
            Gold = serializablePlayerData.Gold;
            Gems = serializablePlayerData.Gems;
            AIPlayerController.LoadFromSerializable(serializablePlayerData.AIPlayerData);
        }

        protected void OnPlacingCharactersContext(PlayerContext playerContext)
        {
            if (playerContext != PlayerContext.PlacingCharacter)
                return;
            Debug.Log($"Placing CHARS CONTEXT");
            if (IsInsideTheCastle)
                return;
            var mask = LayerMask.NameToLayer("ActionCell");
            var filteredCells =
                ActiveTerrain.TerrainNavigator.NavigationMap.GetPositionsByFilterInRange(LastSelectedCastle.Position, 3,
                    mask);
            List<Vector2Int> l = new List<Vector2Int>();
            for (int i = 0; i < filteredCells.Count; i++)
            {
                var characterAtCell =
                    ActiveTerrain.TerrainNavigator.NavigationMap.GetCharacterAtPosition(filteredCells[i]);
                var hasCharacterAtCell = characterAtCell != null;
                if (hasCharacterAtCell)
                    if (!characterAtCell.IsAnAllyTo(this))
                        continue;
                var hasBlockAtCell =
                    ActiveTerrain.TerrainNavigator.NavigationMap.IfCellBlocked(filteredCells[i].x,
                        filteredCells[i].y);
                if ((!hasBlockAtCell || (hasBlockAtCell && hasCharacterAtCell)))
                    l.Add(filteredCells[i]);
            }

            actionCellPool.CreateActionCellsAtPositions(l, GlobalVariables.EditCharactersPositionActionCell);
        }

        protected void OnPlanningSiegeContext(PlayerContext playerContext)
        {
            if (playerContext != PlayerContext.PlanningSiege)
                return;
            actionCellPool.ResetAll();
        }

        protected void OnDefaultContext(PlayerContext playerContext)
        {
            if (playerContext != PlayerContext.Default)
                return;
            actionCellPool.ResetAll();
            _GUIManager.SiegePanel.Hide();
            _GUIManager.CancelSpellButtonWidget.Hide();
        }

        protected void OnUsingSpellContext(PlayerContext context)
        {
            if (context != PlayerContext.UsingSpell)
                return;
            actionCellPool.ResetAll();
            _GUIManager.CancelSpellButtonWidget.Show();
            var caster = _magicSpellCombatData.source.Character;
            var magicSpell = _magicSpellCombatData.magicSpell;
            bool spellTargetsEmtyCell =
                (_magicSpellCombatData.magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Empty) != 0;
            if (spellTargetsEmtyCell)
            {
                var freeCells =
                    ActiveTerrain.TerrainNavigator.NavigationMap.GetFreeCellsInRange(caster.Position,
                        magicSpell.castRange);
                actionCellPool.CreateActionCellsAtPositions(freeCells, GlobalVariables.CastSpellActionCell);
                return;
            }

            var neighbours = caster.GetAllCharactersInRange(magicSpell.castRange, magicSpell.allowedTarget, true);
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i] == _magicSpellCombatData.source && !_magicSpellCombatData.magicSpell.selfCast)
                {
                    neighbours.RemoveAt(i);
                    i--;
                    continue;
                }

                var onlyTheseCharacters = _magicSpellCombatData.magicSpell.AffectsOnlyTheseCharacters;
                if (onlyTheseCharacters.Count > 0)
                {
                    if (onlyTheseCharacters.Contains(neighbours[i].BaseCharacterObject.name))
                        continue;
                    neighbours.RemoveAt(i);
                    i--;
                }
            }

            bool allowedOnCorpse = ((magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.AliveCorpse) != 0) ||
                                   ((magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.UndeadCorpse) != 0);
            if (allowedOnCorpse)
            {
                var corpses = caster.GetAllCorpsesInRange(magicSpell.castRange, magicSpell.allowedTarget);
                neighbours.AddRange(corpses);
            }

            actionCellPool.CreateActionCellsOnCharacters(neighbours, GlobalVariables.CastSpellActionCell);
        }

        public void DisableInteractionWithWorld()
        {
            isInteractionWithWorldEnabled = false;
        }

        public void EnableInteractionWithWorld()
        {
            isInteractionWithWorldEnabled = true;
        }

        protected void OnPlayerNewGameStarted()
        {
            // foreach (var castle in controlledCastles)
            // {
            //     castle.Selected += OnPlayerSelectedCastle;
            //     castle.Deselected += OnPlayerDeselectedCastle;
            // }
            LastSelectedCastle = null;
        }

        public void OnNewDayForPlayer()
        {
            controlledCastles.ForEach(x => x.OnNewDayBegin());
            controlledCharacters.ForEach(x => x.OnNewDayBegin());
        }

        public void OnNewSiegeRoundForPlayer()
        {
            controlledCharacters.ForEach(x => x.OnNewDayBegin());
        }

        public void OnPlayerCharacterStackDemolished(ControllableCharacter demolishedCharacter,
            ControllableCharacter characterSource = null)
        {
            if (demolishedCharacter is Hero)
            {
                if (!demolishedCharacter.isExistingInWorld)
                    controlledCharacters.Remove(demolishedCharacter);
                return;
            }

            characterDestroyed?.Invoke(demolishedCharacter);
            demolishedCharacter.Selected -= OnPlayerSelectedCharacter;
            demolishedCharacter.Deselected -= OnPlayerDeselectedCharacter;
            demolishedCharacter.StackDemolished -= OnPlayerCharacterStackDemolished;
            demolishedCharacter.StackBanished -= OnPlayerCharacterStackDemolished;
            demolishedCharacter.PlayableTerrain.SpawnedCharacters.Remove(demolishedCharacter);
            controlledCharacters.Remove(demolishedCharacter);
            // if (!demolishedCharacter.InsideGarnison)
            demolishedCharacter.PlayableTerrain.TerrainNavigator.NavigationMap.SetCellFree(
                demolishedCharacter.Position.x, demolishedCharacter.Position.y);
            CheckTopTierCharacter();
        }

        public void OnCastleSpawned(Castle castle)
        {
            castle.Selected += OnPlayerSelectedCastle;
            castle.Deselected += OnPlayerDeselectedCastle;
            controlledCastles.Add(castle);
        }

        public void OnCorpseSpawned(Corpse corpse)
        {
        }

        public void OnCharacterSpawned(ControllableCharacter character)
        {
            characterSpawned?.Invoke(character);
            character.Selected += OnPlayerSelectedCharacter;
            character.Deselected += OnPlayerDeselectedCharacter;
            character.StackDemolished += OnPlayerCharacterStackDemolished;
            character.StackBanished += OnPlayerCharacterStackDemolished;
            character.PlayableTerrain.TerrainNavigator.NavigationMap.BlockCell(character.Position.x,
                character.Position.y);
            bool isHero = character.GetType() == typeof(Hero);
            if (isHero)
            {
                MainHero = (Hero)character;
            }

            int tier = character.GetTier();
            if (tier > topTierCharacterValue)
            {
                topTierCharacterValue = tier;
                topTierCharacter = character;
            }

            controlledCharacters.Add(character);
            CheckTopTierCharacter();
        }

        public void OnPlayerSelectedCharacter(ControllableCharacter character)
        {
            var playerColor = character.playerOwnerColor;
            previousSelectedCharacter = selectedCharacter;
            previousSelectedCharacter?.DeselectAndRemoveControll(playerColor);
            selectedCharacter = character;
            _GUIManager.RecentCharactersWidget.SetGUI(selectedCharacter,
                previousSelectedCharacter);
            _GUIManager.CharacterInfoButton.SetGUI(selectedCharacter);
            GameManager.Instance.guiSpellBookButtonWidget.UpdateState(character);
        }

        public void OnPlayerDeselectedCharacter(ControllableCharacter character)
        {
            selectedCharacter = selectedCharacter == character ? null : selectedCharacter;
            GameManager.Instance.guiSpellBookButtonWidget.UpdateState(selectedCharacter);
        }

        public void OnPlayerSelectedCastle(Castle castle)
        {
            LastSelectedCastle = SelectedCastle = castle;
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.RemoveAllListeners();
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.AddListener(castle.EnterCastle);
            _guiLeftSidebar.GUICharacterShopMenu.UpdateShopGUI(castle);
            _guiLeftSidebar.GUISpecialBuildsMenu.UpdateShopGUI(castle);
            _guiLeftSidebar.GUICastleGarnisonPanel.UpdateGarnisonForPlayerCastle(castle);
            _guiLeftSidebar.Show();
        }

        public void OnPlayerDeselectedCastle(Castle castle)
        {
            SelectedCastle = null;
        }

        public void UpdateVisionForEveryEnemy()
        {
            // Debug.Log($"UVFEE FOR {Color}");
            var allPlayer = GameManager.Instance.MapScenarioHandler.AllPlayers;
            foreach (var player in allPlayer)
            {
                if (player == this) continue;
                var enemies = player.controlledCharacters;
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    if (enemy.IsVisibleForPlayer(this))
                    {
                        enemy.VisuallyShow();
                    }
                    else
                    {
                        enemy.VisuallyHide();
                    }
                }
            }
        }

        public void UpdateVisionForAllCharacters()
        {
            foreach (var character in controlledCharacters)
            {
                if (character.isExistingInWorld && character.gameObject.activeSelf)
                    fogOfWarController.UpdateVisionForCharacter(character);
            }
        }

        public virtual void ContinueTurn()
        {
        }

        public virtual void OnPlayerRecievedTurn()
        {
            if (controlledCharacters.Count <= 0 && controlledCastles.Count <= 0 && Color != PlayerColor.Neutral)
            {
                Banish();
                return;
            }

            turnRecieved?.Invoke(this);
            foreach (var character in controlledCharacters)
            {
                if (!character.gameObject.activeSelf) continue;
                character.OnNewTurnBegin();
            }

            if (!GameManager.Instance.MapScenarioHandler.IsSiegeStarted)
                foreach (var castle in controlledCastles)
                {
                    if (!castle.gameObject.activeSelf) continue;
                    castle.OnNewTurnBegin();
                }

            if (GameManager.Instance.GameSettings.noFogOfWar)
                fogOfWarController.ClearDeepFogOfWarEverywhereReserved(ActiveTerrain, this);
        }

        public virtual void OnPlayerEndedTurn()
        {
            turnSkipped?.Invoke(this);
        }

        public void SetActiveContext(PlayerContext context)
        {
            if (activePlayerContext != context)
                OnContextChanged(context);
            activePlayerContext = context;
        }

        protected void OnContextChanged(PlayerContext context)
        {
            if (context == PlayerContext.UsingSpell)
            {
                _guiSpellBook.Close();
            }

            contextChanged?.Invoke(context);
        }

        public void PrepareSpell(MagicSpell magicSpell)
        {
            _magicSpellCombatData = new MagicSpellCombatData();
            _magicSpellCombatData.magicSpell = magicSpell;
            _magicSpellCombatData.source = selectedCharacter;
            _magicSpellCombatData.magicSpell.CreatePrepareVisuals();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (GameManager.Instance.MapScenarioHandler.CurrentPlayer != this)
                    return;
                var cells = selectedCharacter.OpenedFogDictionary[ActiveTerrain];
                foreach (var c in cells)
                {
                    GlobalVariables.__SpawnMarker(new Vector2Int(c.x, c.y), UnityEngine.Color.red, false, 3f);
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (GameManager.Instance.MapScenarioHandler.CurrentPlayer != this)
                    return;
                var screenToWorldPoint = GameManager.Instance.MainCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                var castle = _activeTerrain.TerrainNavigator.NavigationMap.GetObjectOfType<Castle>(
                    new Vector2Int(Mathf.RoundToInt(screenToWorldPoint.x), Mathf.RoundToInt(screenToWorldPoint.y)),
                    LayerMask.NameToLayer("Building"));
                if (castle != null)
                {
                    GameManager.Instance.MapScenarioHandler.ChangeCastleOwner(castle, this);
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                foreach (var character in controlledCharacters)
                {
                    Hero hero = character as Hero;
                    if (hero == null) continue;
                    hero.TotalExperience += 100;
                }

                Gold += 300;
                Gems += 2;
            }
#endif

            switch (activePlayerContext)
            {
                case PlayerContext.Default:
                    UpdateDefaultContext();
                    break;
                case PlayerContext.UsingSpell:
                    UpdateUsingSpellContext();
                    break;
                case PlayerContext.Awaiting:
                    UpdateAwaitingContext();
                    break;
                case PlayerContext.PlacingCharacter:
                    UpdatePlacingCharacterContext();
                    break;
                case PlayerContext.PlanningSiege:
                    UpdatePlanningSiegeContext();
                    break;
            }
        }

        protected void UpdatePlanningSiegeContext()
        {
            if (_isInputLocked) return;

            if (!isInteractionWithWorldEnabled)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                bool clickedOnUI = GameManager.Instance.GUIRaycaster.HasAnyUI();
                if (clickedOnUI)
                    return;
                var worldPoint = GameManager.Instance.MainCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                worldPoint.z = 0f;
                var gridWorldPosition =
                    new Vector2Int(Mathf.RoundToInt(worldPoint.x), Mathf.RoundToInt(worldPoint.y));
                bool clickedOutOfBounds = gridWorldPosition.x > _activeTerrain.Size.x ||
                                          gridWorldPosition.y > _activeTerrain.Size.y ||
                                          gridWorldPosition.x < 0 || gridWorldPosition.y < 0;
                if (clickedOutOfBounds) return;
                if (IsInsideTheCastle)
                {
                    var siegeTile = ResourcesBase.GetTile("siege_attacker");
                    var siegePanel = _GUIManager.SiegePanel;


                    var toolsTilemap = _activeTerrain.ToolTilemap;
                    var tileAtPosition =
                        toolsTilemap.GetTile(new Vector3Int(gridWorldPosition.x, gridWorldPosition.y, 0));

                    bool isCellBlocked =
                        _activeTerrain.TerrainNavigator.NavigationMap.IfCellBlocked(gridWorldPosition.x,
                            gridWorldPosition.y);
                    var attackingPlayerData = _castleSiegeData.attackingPlayerData;
                    var defendingPlayerData = _castleSiegeData.defendingPlayerData;
                    if (isCellBlocked)
                    {
                        var characterAtCell =
                            _activeTerrain.TerrainNavigator.NavigationMap.GetCharacterAtPosition(gridWorldPosition);
                        if (characterAtCell != null)
                        {
                            if (!characterAtCell.IsAnAllyTo(this))
                                return;
                            var widget = siegePanel.AddCharacterSlot(characterAtCell);
                            _activeTerrain.TerrainNavigator.NavigationMap.SetCellFree(gridWorldPosition.x,
                                gridWorldPosition.y);
                            characterAtCell.Hide();
                            characterAtCell.DeselectAndRemoveControll(Color);
                            bool isAttackingPlayer = attackingPlayerData.Player == this;
                            if (isAttackingPlayer)
                                siegePanel.RemoveSiegeCharacter(attackingPlayerData, characterAtCell);
                            else
                                siegePanel.RemoveSiegeCharacter(defendingPlayerData, characterAtCell);
                        }
                    }
                    else
                    {
                        var selectedSiegeCharacter = siegePanel.SelectedSiegeCharacter;
                        if (selectedSiegeCharacter == null) return;
                        bool inSiegeTile = (tileAtPosition == siegeTile);
                        bool b = ((inSiegeTile && attackingPlayerData.Player == this) ||
                                  (!inSiegeTile && defendingPlayerData.Player == this));
                        if (!b) return;
                        selectedSiegeCharacter.ResetWorldPosition(gridWorldPosition);
                        selectedSiegeCharacter.gameObject.SetActive(true);
                        siegePanel.SelectedSiegeCharacter = null;
                        siegePanel.RemoveCharacterSlot(selectedSiegeCharacter);
                        bool isAttackingPlayer = attackingPlayerData.Player == this;
                        if (isAttackingPlayer)
                            siegePanel.ApplySiegeCharacter(attackingPlayerData, selectedSiegeCharacter);
                        else
                            siegePanel.ApplySiegeCharacter(defendingPlayerData, selectedSiegeCharacter);
                    }
                }
                else
                {
                    int layerMask = 1 << LayerMask.NameToLayer("ActionCell");
                    var results = GameManager.Instance.MainCamera.Raycast2DSorted(layerMask);
                    var raycast = results.FirstOrDefault();
                    if (raycast.collider != null)
                    {
                        var clickable = raycast.collider.GetComponent<IClickTarget>();
                        if (clickable != null)
                            Moroutine.Run(clickable.OnClicked(this));
                    }
                }
            }
        }

        protected void UpdateUsingSpellContext()
        {
            if (_isInputLocked) return;

            if (!isInteractionWithWorldEnabled)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                bool clickedOnUI = GameManager.Instance.GUIRaycaster.HasAnyUI();
                if (clickedOnUI)
                {
                    var hotbarSpellSlot = GameManager.Instance.GUIRaycaster.GetUIElement<GUIHotbarSpellSlot>();
                    if (hotbarSpellSlot != null)
                    {
                        _guiSpellBook.spellBook.SetHotbarMagicSpell(hotbarSpellSlot.ID,
                            _magicSpellCombatData.magicSpell);
                        _guiSpellBook.UpdateHotbar(_guiSpellBook.spellBook.HotbarMagicSpells);
                        SetActiveContext(PlayerContext.Default);
                    }

                    return;
                }

                int layerMask = 1 << LayerMask.NameToLayer("ActionCell");
                var results = GameManager.Instance.MainCamera.Raycast2DSorted(layerMask);
                var raycast = results.FirstOrDefault();
                if (raycast.collider != null)
                {
                    var actionCell = raycast.collider.GetComponent<ActionCell>();
                    if (actionCell != null)
                        if (actionCell.Variant.GetType() == GlobalVariables.CastSpellActionCell.GetType())
                            Moroutine.Run(actionCell.OnClicked(this));
                }
            }
        }

        protected void UpdateAwaitingContext()
        {
        }

        protected void UpdatePlacingCharacterContext()
        {
            if (_isInputLocked) return;

            if (!isInteractionWithWorldEnabled)
                return;
            if (IsInsideTheCastle)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    bool clickedOnUI = GameManager.Instance.GUIRaycaster.HasAnyUI();
                    if (clickedOnUI)
                        return;
                    var worldPoint = GameManager.Instance.MainCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                    worldPoint.z = 0f;
                    var gridWorldPosition =
                        new Vector2Int(Mathf.RoundToInt(worldPoint.x), Mathf.RoundToInt(worldPoint.y));
                    bool clickedOutOfBounds = gridWorldPosition.x > _activeTerrain.Size.x ||
                                              gridWorldPosition.y > _activeTerrain.Size.y ||
                                              gridWorldPosition.x < 0 || gridWorldPosition.y < 0;
                    if (clickedOutOfBounds) return;
                    int layerMask = 1 << LayerMask.NameToLayer("Character");
                    var results = GameManager.Instance.MainCamera.Raycast2DSorted(layerMask);
                    var raycast = results.FirstOrDefault();
                    if (raycast.collider != null)
                    {
                        var character = raycast.collider.GetComponent<ControllableCharacter>();
                        if (character != null)
                        {
                            character.PlaceOnTerrain(_terrainManager.CurrentPlayableMap.WorldTerrain);
                            SelectedCastle.AddGarnisonCharacterStack(character);
                        }
                    }
                    else
                    {
                        bool blockedByTool =
                            ActiveTerrain.ToolTilemap.GetTile(new Vector3Int(Mathf.RoundToInt(worldPoint.x),
                                Mathf.RoundToInt(worldPoint.y))) != null;
                        if (blockedByTool) return;
                        var garnison = _GUIManager.LeftSidebar.GUICastleGarnisonPanel;
                        var character = garnison.SelectedGarnisonCharacter;
                        if (character != null)
                        {
                            // garnison.RemoveCharacterSlot(character);
                            garnison.SelectedGarnisonCharacter = null;
                            character.PlaceOnTerrain(ActiveTerrain);
                            character.Position = gridWorldPosition;
                            character.gameObject.SetActive(true);
                            SelectedCastle.RemoveFromGarnisonCharacterStack(character);
                        }
                    }
                }

                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                bool clickedOnUI = GameManager.Instance.GUIRaycaster.HasAnyUI();
                if (clickedOnUI)
                    return;
                int layerMask = 1 << LayerMask.NameToLayer("ActionCell");
                var results = GameManager.Instance.MainCamera.Raycast2DSorted(layerMask);
                var raycast = results.FirstOrDefault();
                if (raycast.collider != null)
                {
                    var clickable = raycast.collider.GetComponent<IClickTarget>();
                    if (clickable != null)
                        Moroutine.Run(clickable.OnClicked(this));
                }
            }
        }

        protected void UpdateDefaultContext()
        {
            if (_isInputLocked) return;

            if (!isInteractionWithWorldEnabled)
                return;
            if (GameManager.Instance.MapScenarioHandler.TurnOfPlayerId != Color)
                return;
            // if (Input.GetKeyDown(KeyCode.B))
            // {
            //     Debug.Log($"Player {Color.ToString()} called B Command");
            //     controlledCastles[0].Tier++;
            // }


            if (Input.GetMouseButtonDown(0))
            {
                bool clickedOnUI = GameManager.Instance.GUIRaycaster.HasAnyUI();
                if (clickedOnUI)
                    return;
                int layerMask = 1 << LayerMask.NameToLayer("ActionCell");
                layerMask |= 1 << LayerMask.NameToLayer("Character");
                layerMask |= 1 << LayerMask.NameToLayer("SpriteGUI");
                layerMask |= 1 << LayerMask.NameToLayer("Building");
                var results = GameManager.Instance.MainCamera.Raycast2DSorted(layerMask);
                var raycast = results.FirstOrDefault();
                if (raycast.collider != null)
                {
                    var clickable = raycast.collider.GetComponent<IClickTarget>();
                    if (clickable != null)
                        Moroutine.Run(clickable.OnClicked(this));
                }
            }
        }
    }
}