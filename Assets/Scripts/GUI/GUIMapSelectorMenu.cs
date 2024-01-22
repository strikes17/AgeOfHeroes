using System;
using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIMapSelectorMenu : MonoBehaviour
    {
        [SerializeField] private GUIMapSelectorButton _mapSelectorButtonPrefab;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private List<MapInfo> mapInfoList = new List<MapInfo>();
        [SerializeField] private Button closeButton, playButton;
        [SerializeField] private Transform mapDescriptionPanel, mapPlayerSettingsRoot;
        [SerializeField] private Image mapDescriptionIcon;
        [SerializeField] private TMP_Text mapDescriptionName;
        [SerializeField] private GUIMapPlayerSettingsWidget mapPlayerSettingsPrefab;
        private List<GUIMapSelectorButton> _instantiatedMapSelectors = new List<GUIMapSelectorButton>();
        private List<GUIMapPlayerSettingsWidget> mapPlayerSettings = new List<GUIMapPlayerSettingsWidget>();

        private void Awake()
        {
            mapDescriptionPanel.gameObject.SetActive(false);
            closeButton.onClick.AddListener(Hide);
        }

        public void Hide()
        {
            for (int i = 0; i < _instantiatedMapSelectors.Count; i++)
            {
                Destroy(_instantiatedMapSelectors[i].gameObject);
            }
            mapDescriptionPanel.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            _instantiatedMapSelectors.Clear();
            mapInfoList.Clear();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            _ = Moroutine.Run(MapSerializerSystem.MapSerializer.GetAvailableMaps((maps) =>
            {
                Debug.Log(maps.Count);
                mapInfoList.Clear();
                maps.ForEach(x =>
                {
                    var mapInfo = x.SerializableMatchInfo.MapInfo;
                    mapInfoList.Add(mapInfo);
                    var mapSelectorInstance = GameObject.Instantiate(_mapSelectorButtonPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                    mapSelectorInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    MapSize mapSize = GlobalVariables.CalculateMapSize(new Vector2Int(mapInfo.sizeX, mapInfo.sizeY));
                    mapSelectorInstance.sizeText = mapSize.ToString();
                    mapSelectorInstance.mapName = mapInfo.Name;
                    mapSelectorInstance.sizeTextColor = GlobalVariables.MapSizeColors[mapSize];
                    mapSelectorInstance.playersText = $"{mapInfo.maxPlayersCount} PLAYERS";
                    mapSelectorInstance.playersTextColor = GlobalVariables.PlayerCountColors[mapInfo.maxPlayersCount];
                    mapSelectorInstance.button.onClick.AddListener(() => SelectMap(x));
                    var mapCategory = mapInfo.MapCategory;
                    GlobalVariables.LoadMapPreviewIcon(mapInfo.previewTexturePath, mapInfo.sizeX, mapInfo.sizeY, mapCategory, (sprite) =>
                    { mapSelectorInstance.mapPreviewImage = sprite; });
                    _instantiatedMapSelectors.Add(mapSelectorInstance);
                });
                gameObject.SetActive(true);
            }));
        }


        private void SelectMap(SerializableMap serializableMap)
        {
            for (int i = 0; i < mapPlayerSettings.Count; i++)
            {
                Destroy(mapPlayerSettings[i].gameObject);
            }
            mapPlayerSettings.Clear();
            mapDescriptionPanel.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
            var mapInfo = serializableMap.SerializableMatchInfo.MapInfo;
            GlobalVariables.LoadMapPreviewIcon(mapInfo.previewTexturePath, mapInfo.sizeX, mapInfo.sizeY, mapInfo.MapCategory, (sprite) => mapDescriptionIcon.sprite = sprite);

            mapDescriptionName.text = mapInfo.Name;
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => StartMap(serializableMap));
            var playersCount = mapInfo.maxPlayersCount;
            var castles = serializableMap.MainTerrain.SerializableCastles;
            for (int i = 0; i < playersCount; i++)
            {
                Fraction playerFraction = Fraction.None;
                var widget = Instantiate(mapPlayerSettingsPrefab, mapPlayerSettingsRoot);
                widget.PlayerColor = (PlayerColor)i;
                mapPlayerSettings.Add(widget);
                foreach (var castle in castles)
                {
                    if ((int)castle.PlayerOwnerColor == i)
                    {
                        playerFraction = castle.Fraction;
                        break;
                    }
                }
                if (i != 0)
                    widget.PlayerType = PlayerType.AIEasy;
                widget.FractionPredefined = playerFraction != Fraction.None;
                if(playerFraction == Fraction.None)playerFraction = Fraction.Human;
                widget.Fraction = playerFraction;
            }
        }

        private void StartMap(SerializableMap serializableMap)
        {
            var matchInfoInstance = MatchInfo.CreateInstance();
            var mapInfo = serializableMap.SerializableMatchInfo.MapInfo;
            matchInfoInstance.MapInfo = mapInfo;
            matchInfoInstance.totalPlayerCount = (uint)mapInfo.maxPlayersCount;
            for (int i = 0; i < mapPlayerSettings.Count; i++)
            {
                matchInfoInstance.isPlayerHuman.TryAdd((PlayerColor)i, mapPlayerSettings[i].PlayerType);
                matchInfoInstance.playersFractions.TryAdd((PlayerColor)i, mapPlayerSettings[i].Fraction);
            }

            matchInfoInstance.MatchType = MatchType.Local;
            matchInfoInstance.GameType = GameType.New;
            var gameLoadAsync = SceneManager.LoadSceneAsync("game", LoadSceneMode.Single);
            gameLoadAsync.allowSceneActivation = true;
            // gameLoadAsync.completed += operation => SceneManager.UnloadSceneAsync("main_menu");
        }
    }
}