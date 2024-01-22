using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static AgeOfHeroes.GUICastleGarnisonEditWidget;

namespace AgeOfHeroes
{
    public class GUICastleGarnisonPanel : GUIDialogueWindow
    {
        [SerializeField] private GUICastleGarnisonCharacterSlotWidget _slotWidgetPrefab;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private GUICastleGarnisonEditWidget _castleGarnisonEditWidget;
        public ControllableCharacter SelectedGarnisonCharacter { set; get; }
        private readonly Dictionary<int, GUICastleGarnisonCharacterSlotWidget> _widgets = new();
        private Mode _mode;
        private Player _targetPlayer;

        public void Init()
        {
            _castleGarnisonEditWidget.ModeChanged = OnModeChanged;
        }

        public override void Show()
        {
            base.Show();
            _castleGarnisonEditWidget.Show();
        }

        public override void Hide()
        {
            base.Hide();
            _targetPlayer?.SetActiveContext(PlayerContext.Default);
            _castleGarnisonEditWidget.Hide();
        }


        private void SetModeForWidget(GUICastleGarnisonCharacterSlotWidget widget, Mode mode)
        {
            if (mode == Mode.Info)
            {
                widget.button.onClick.RemoveAllListeners();
                widget.button.onClick.AddListener(() =>
                {
                    var character = widget.controllableCharacter;
                    SelectedGarnisonCharacter = character;
                    var infoWindow = GameManager.Instance.GUIManager.CreateInfoWindowInstance(character);
                    infoWindow.Show();
                });
            }
            else if (mode == Mode.Edit)
            {
                widget.button.onClick.RemoveAllListeners();
                widget.button.onClick.AddListener(() =>
                {
                    var character = widget.controllableCharacter;
                    SelectedGarnisonCharacter = character;
                });
            }
        }

        public void SetMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Info:
                    _castleGarnisonEditWidget.SetInfoMode();
                    break;
                case Mode.Edit:
                    _castleGarnisonEditWidget.SetEditMode();
                    break;
            }
        }

        private void OnModeChanged(Mode mode)
        {
            _mode = mode;
            SelectedGarnisonCharacter = null;
            foreach (var widget in _widgets)
            {
                SetModeForWidget(widget.Value, mode);
            }

            if (_mode == Mode.Info)
                _targetPlayer?.SetActiveContext(PlayerContext.Default);
            else if (_mode == Mode.Edit)
                _targetPlayer?.SetActiveContext(PlayerContext.PlacingCharacter);
        }

        private void ClearGarnisonSlots()
        {
            foreach (var widget in _widgets.Values)
            {
                Destroy(widget.gameObject);
            }

            _widgets.Clear();
        }

        public void UpdateGarnisonForPlayerCastle(Castle castle)
        {
            if (_targetPlayer != castle.Player) ClearGarnisonSlots();
            _targetPlayer = castle.Player;
            var characters = castle.garnisonCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                var existingSlot = GetCharacterSlot(character);
                GUICastleGarnisonCharacterSlotWidget slot = null;
                bool isPlayerInsideCastle =
                    GameManager.Instance.terrainManager.IsCastleTerrain(_targetPlayer.ActiveTerrain);
                if (existingSlot == null)
                    slot = AddCharacterSlot(character);
                else
                    slot = existingSlot;

                if (castle.GuardsCharacters.Contains(character) && (!isPlayerInsideCastle))
                    slot.button.interactable = false;
                else slot.button.interactable = true;
            }
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
            var slotInstance =
                GameObject.Instantiate(_slotWidgetPrefab, Vector3.zero, Quaternion.identity, _contentRoot);
            slotInstance.controllableCharacter = character;
            character.DeselectAndRemoveControll(character.playerOwnerColor);
            SetModeForWidget(slotInstance, _mode);
            _widgets.TryAdd(character.GetHashCode(), slotInstance);
            return slotInstance;
        }

        public GUICastleGarnisonCharacterSlotWidget GetCharacterSlot(ControllableCharacter controllableCharacter)
        {
            if (_widgets.TryGetValue(controllableCharacter.GetHashCode(),
                    out GUICastleGarnisonCharacterSlotWidget value))
            {
                return value;
            }

            return null;
        }
    }
}