using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICancelSpellButtonWidget : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        [SerializeField] private GUISpellBookButtonWidget _spellBookButton;

        public Button Button1 => _button;

        private void Awake()
        {
            Hide();
        }

        public override void Hide()
        {
            base.Hide();
            _spellBookButton.Show();
        }

        public override void Show()
        {
            base.Show();
            _spellBookButton.Hide();
        }
    }
}