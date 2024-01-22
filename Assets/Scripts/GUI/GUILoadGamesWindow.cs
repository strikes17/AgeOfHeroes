using System.IO;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Newtonsoft.Json;
using AgeOfHeroes.MapEditor;

namespace AgeOfHeroes
{
    public class GUILoadGamesWindow : GUIDialogueWindow
    {
        [SerializeField] protected Button closeButton, saveButton, loadButton;
        [SerializeField] protected Transform root;
        [SerializeField] protected GUISavedGameWidget savedGameWidgetPrefab;
        [SerializeField] protected TMP_Text saveGameNameField;
        [SerializeField] private TMP_InputField inputField;
        private bool savingEnabled;

        private GUISavedGameWidget _selectedWidget;

        protected List<GUISavedGameWidget> savedGameWidgets = new List<GUISavedGameWidget>();

        public bool SavingEnabled
        {
            get => savingEnabled;
            set
            {
                savingEnabled = value;
                saveButton.gameObject.SetActive(value);
            }
        }

        protected virtual void Awake()
        {
            closeButton.onClick.AddListener(Hide);
            saveButton.onClick.AddListener(() =>
            {
                bool yes = saveGameNameField.text.Contains((char)8203) && saveGameNameField.text.Length == 1;
                var inputName = yes ? string.Empty : saveGameNameField.text.Replace($"{8203}", string.Empty);
                Debug.Log(inputName.Length);
                if (string.IsNullOrEmpty(inputName))
                    return;
                var map = GameManager.Instance.terrainManager.CurrentPlayableMap;
                SaveManager.Instance.SaveGame(map, $"{inputName}");
                UpdateSavedGames();
            });
            loadButton.onClick.AddListener(() =>
            {
                Debug.Log(_selectedWidget.TargetFileName);
                GlobalVariables.SetString("targetLoadingMap", _selectedWidget.TargetFileName);
                MatchInfo.Destroy(FindObjectOfType<MatchInfo>());
                var matchInfo = MatchInfo.CreateInstance();
                var gameLoadAsync = SceneManager.LoadSceneAsync("game", LoadSceneMode.Single);
                gameLoadAsync.allowSceneActivation = true;
                matchInfo.GameType = GameType.Loaded;
            });
        }

        public override void Show()
        {
            loadButton.gameObject.SetActive(false);
            base.Show();
            UpdateSavedGames();
        }

        private void UpdateSavedGames()
        {
            var directory = GlobalStrings.SAVED_GAMES_PATH;
            var files = Directory.GetFiles(directory, "*.json");
            for (int i = 0; i < savedGameWidgets.Count; i++)
            {
                Destroy(savedGameWidgets[i].gameObject);
            }
            savedGameWidgets.Clear();
            foreach (var file in files)
            {
                var widget = Instantiate(savedGameWidgetPrefab, root);
                savedGameWidgets.Add(widget);
                Debug.Log(file);
                var text = File.ReadAllText(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                var date = File.GetLastWriteTime(file);
                var settings = GlobalVariables.GetDefaultSerializationSettings();
                var json = JsonConvert.DeserializeObject<SerializableMap>(text, settings);
                var name = json.SerializableMatchInfo.MapInfo.Name;
                var previewPath = json.SerializableMatchInfo.MapInfo.previewTexturePath;
                var mapInfo = json.SerializableMatchInfo.MapInfo;
                GlobalVariables.LoadMapPreviewIcon(mapInfo.previewTexturePath, mapInfo.sizeX, mapInfo.sizeY, mapInfo.MapCategory,
                 (sprite) => widget.Icon = sprite);
                widget.MapNameText = name;
                widget.SaveNameText = fileName;
                widget.LastDateText = date.ToShortDateString();
                widget.TargetFileName = fileName;
                widget.Button.onClick.AddListener(() =>
                {
                    foreach (var w in savedGameWidgets)
                    {
                        w.VisualSelectionOff();
                    }
                    _selectedWidget = widget;
                    _selectedWidget.VisualSelectionOn();
                    loadButton.gameObject.SetActive(true);
                    inputField.text = _selectedWidget.TargetFileName;
                });
            }
        }
    }
}