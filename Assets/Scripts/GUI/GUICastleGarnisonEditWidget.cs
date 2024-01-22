using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICastleGarnisonEditWidget : GUIBaseWidget
    {
        [SerializeField] private Button _infoButton, _editButton;

        public enum Mode
        {
            Info, Edit
        }
        public Action<Mode> ModeChanged;

        private void Awake()
        {
            _infoButton.onClick.AddListener(() => SetEditMode());
            _editButton.onClick.AddListener(() => SetInfoMode());
            SetInfoMode();
        }

        public void SetEditMode()
        {
            _infoButton.gameObject.SetActive(false);
            _editButton.gameObject.SetActive(true);
            ModeChanged?.Invoke(Mode.Edit);
        }

        public void SetInfoMode()
        {
            _infoButton.gameObject.SetActive(true);
            _editButton.gameObject.SetActive(false);
            ModeChanged?.Invoke(Mode.Info);
        }
    }
}