using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUIRecentCharactersWidget : MonoBehaviour
    {
        [SerializeField] private GUICharacterWidgetButton _selectedCharacterWidget, _previousCharacterWidget, _topTierCharacter;
        private bool _locked;

        public ControllableCharacter TopTierCharacter
        {
            get
            {
                return _topTierCharacter.ReferencedCharacter;
            }
            set => _topTierCharacter.ReferencedCharacter = value;
        }

        public bool Locked
        {
            get => Locked;
            set
            {
                _locked = value;
                _selectedCharacterWidget.Locked = _locked;
                _previousCharacterWidget.Locked = _locked;
                _topTierCharacter.Locked = _locked;
            }
        }

        public void SetGUI(ControllableCharacter selectedCharacter, ControllableCharacter previousCharacter)
        {
            if (selectedCharacter == null)
                _selectedCharacterWidget.ReferencedCharacter = null;
            if (previousCharacter == null)
                _previousCharacterWidget.ReferencedCharacter = null;
            if (selectedCharacter == _selectedCharacterWidget.ReferencedCharacter)
                return;
            _selectedCharacterWidget.ReferencedCharacter = selectedCharacter;
            if (previousCharacter != null)
                _previousCharacterWidget.ReferencedCharacter = previousCharacter;
        }
    }
}