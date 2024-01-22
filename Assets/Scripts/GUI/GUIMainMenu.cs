using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class GUIMainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _campaignButton, _customMapsButton, _mapEditorButton, _settingsButton, _exitButton;

        [SerializeField] private Transform _menuRoot, _mapSelectorRoot;
        private GUIMapSelectorMenu _guiMapSelectorMenu;
        [SerializeField] private GUILoadGamesWindow _guiLoadGamesMenu;
        [SerializeField] private List<AudioClip> _buttonClickSounds;
        [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            try
            {
                var directory = GlobalStrings.SAVED_GAMES_PATH;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (IOException e)
            {
            }

            _guiLoadGamesMenu.SavingEnabled = false;
            _menuRoot.gameObject.SetActive(true);
            _mapSelectorRoot.gameObject.SetActive(false);
            _guiMapSelectorMenu = _mapSelectorRoot.GetComponent<GUIMapSelectorMenu>();
            _customMapsButton.onClick.AddListener(OpenMapSelectorMenu);
            _mapEditorButton.onClick.AddListener(OpenMapEditor);
            _campaignButton.onClick.AddListener(() => OpenLoadingGamesWindow());
        }

        // public void LoadGame()
        // {
        //     var matchInfo = MatchInfo.CreateInstance();
        //     var gameLoadAsync = SceneManager.LoadSceneAsync("game", LoadSceneMode.Single);
        //     gameLoadAsync.allowSceneActivation = true;
        //     matchInfo.GameType = GameType.Loaded;
        // }

        public void PlayRandomClickSound()
        {
            var audioClip = _buttonClickSounds[Random.Range(0, _buttonClickSounds.Count)];
            _audioSource.PlayOneShot(audioClip);
        }

        private void OpenMapEditor()
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync("map_editor", LoadSceneMode.Single);
            loadSceneAsync.allowSceneActivation = true;
        }

        private void OpenLoadingGamesWindow()
        {
            _guiMapSelectorMenu.Hide();
            _guiLoadGamesMenu.Show();
        }

        private void OpenMapSelectorMenu()
        {
            _guiMapSelectorMenu.Show();
            _guiLoadGamesMenu.Hide();
        }
    }
}