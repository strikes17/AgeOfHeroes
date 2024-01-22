using System;
using AgeOfHeroes.MapEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIPauseMenuWindow : GUIDialogueWindow
    {
        [SerializeField] private Button _continueButton, _settingsButton, _loadButton, _mainMenuButton;
        [SerializeField] private GUILoadGamesWindow loadGamesWindow;
        [SerializeField] private Button menuButton;
        private bool locked;

        public bool Locked
        {
            get => locked;
            set
            {
                locked = value;
                menuButton.interactable = !locked;
            }
        }

        private void Awake()
        {
            loadGamesWindow.SavingEnabled = true;
            _continueButton.onClick.AddListener(Hide);
            _settingsButton.onClick.AddListener(OpenSettingsWindow);
            _loadButton.onClick.AddListener(OpenLoadGameWindow);
            _mainMenuButton.onClick.AddListener(LoadMainMenu);
        }

        public void LoadMainMenu()
        {
            Destroy(GameObject.FindObjectOfType<MatchInfo>().gameObject);
            var loadSceneAsync = SceneManager.LoadSceneAsync("main_menu", LoadSceneMode.Single);
            loadSceneAsync.allowSceneActivation = true;
        }

        private void OpenSettingsWindow()
        {
            if (locked) return;

        }


        private void OpenLoadGameWindow()
        {
            if (locked) return;
            loadGamesWindow.Show();
        }

        public override void Show()
        {
            if (locked) return;
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}