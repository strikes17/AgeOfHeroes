using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuffInfoPopup : GUIDialogueWindow
    {
        [SerializeField] private TMP_Text _titleText, _descriptionText;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        public string TItle
        {
            set => _titleText.text = value;
        }

        public string Description
        {
            set => _descriptionText.text = value;
        }
    }
}