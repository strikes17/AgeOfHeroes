using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorCharacterInfoWindow : MonoBehaviour
    {
        [SerializeField] private Button _okButton, _closeButton;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _quantityTextInput;
        private MapEditorCharacter _character;
        private int _quantity;

        public delegate void OnWindowEventDelegate();

        public OnWindowEventDelegate Closed
        {
            get => _closed;
            set => _closed = value;
        }

        private OnWindowEventDelegate _closed;

        private void Awake()
        {
            _okButton.onClick.AddListener(Apply);
            _closeButton.onClick.AddListener(Hide);
        }

        private void Apply()
        {
            string quantityStringFix = _quantityTextInput.text;
            quantityStringFix = quantityStringFix.Replace((char) 8203, ' ');
            int result = 0;
            int.TryParse(quantityStringFix, out result);
            _quantity = result;
            _character.countInStack = _quantity;
            Hide();
        }

        public void Set(MapEditorCharacter character)
        {
            _character = character;
            _titleText.text = _character.CharacterObject.title;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _closed?.Invoke();
            Destroy(gameObject);
        }
    }
}