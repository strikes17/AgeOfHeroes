using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICommonDialogueWindow : GUIDialogueWindow
    {
        [SerializeField] protected TMP_Text _descriptionText, _titleText;
        [SerializeField] protected Button _okButton, _noButton;
        public bool DoNotDestroy;

        public Action Applied
        {
            set => applied = value;
        }
        
        public Action Canceled
        {
            set => canceled = value;
        }

        private Action applied, canceled;

        private void Awake()
        {
            _noButton.onClick.AddListener(OnCanceled);
            _okButton.onClick.AddListener(OnApplied);
        }

        public override void CloseAndDestroy()
        {
            if (!DoNotDestroy)
                base.CloseAndDestroy();
            else Hide();
        }

        private void OnApplied()
        {
            applied?.Invoke();
            CloseAndDestroy();
        }
        
        private void OnCanceled()
        {
            canceled?.Invoke();
            CloseAndDestroy();
        }

        public string Description
        {
            set => _descriptionText.text = value;
            get => _descriptionText.text;
        }

        public string Title
        {
            set => _titleText.text = value;
            get => _titleText.text;
        }
    }
}