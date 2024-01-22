using System;
using AgeOfHeroes.Spell;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISpellBookButtonWidget : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        private ControllableCharacter _referencedCharacter;
        private GUISpellBook _guiSpellBook;

        private void Awake()
        {
            var enabled = GameManager.Instance.GameSettings.spellBookEnabled;
            if(!enabled)
                gameObject.SetActive(false);
            _button.onClick.AddListener(OpenSpellbookGUI);
            _guiSpellBook = GameManager.Instance.GUISpellBook;
            _button.interactable = false;
            _guiSpellBook.HideHotbar();
            Show();
        }

        public void UpdateState(ControllableCharacter character)
        {
            _referencedCharacter = character;
            if (_referencedCharacter == null)
            {
                _button.interactable = false;
                _guiSpellBook.HideHotbar();
                return;
            }
            var hasSpellbook = character.spellBook != null;
            if (hasSpellbook)
            {
                _guiSpellBook.UpdateHotbar(_referencedCharacter.spellBook.HotbarMagicSpells);
                _guiSpellBook.ShowHotbar();
                _button.interactable = true;
                _guiSpellBook.SetSpellBook(_referencedCharacter.spellBook);
            }
            else
            {
                _guiSpellBook.HideHotbar();
                _button.interactable = false;
            }
        }

        public void OpenSpellbookGUI()
        {
            var selectedCharacter = GameManager.Instance.MapScenarioHandler.CurrentPlayer.selectedCharacter;
            _guiSpellBook.SwitchState(selectedCharacter);
        }
    }
}