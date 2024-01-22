using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUINecroRaiseWidget : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _imageIcon;
        protected CharacterObject _characterObject;
        public Button SelectionButton => _button;

        public void Select()
        {
            _button.image.color = Color.green;
        }

        public void Deselect()
        {
            _button.image.color = Color.white;
        }

        public CharacterObject CharacterObject
        {
            get => _characterObject;
            set
            {
                _characterObject = value;
                _imageIcon.sprite = _characterObject.mainSprite;
            }
        }
    }
}