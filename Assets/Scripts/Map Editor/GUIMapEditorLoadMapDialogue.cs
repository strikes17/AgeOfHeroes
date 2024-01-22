using System;
using System.Collections.Generic;
using System.IO;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorLoadMapDialogue : MonoBehaviour
    {
        [SerializeField] private Button _originalTab, _customTab, _downloadedTab;
        [SerializeField] private Button _okButton, _noButton;
        [SerializeField] private ScrollRect _originalScrollbar, _customScrollbar, _downloadedScrollbar;
        [SerializeField] private Transform _originalScrollbarContent, _customScrollbarContent, _downloadedScrollbarContent;
        [SerializeField] private GUIMapEditorMapSelectorButton _mapSelectorButtonPrefab;
        private List<GUIMapEditorMapSelectorButton> _mapSelectors = new List<GUIMapEditorMapSelectorButton>();
        private GUIMapEditorMapSelectorButton _selectorButton;

        private void Awake()
        {
            _okButton.onClick.AddListener(OnOkButtonClick);
            _noButton.onClick.AddListener(Hide);
            _originalTab.onClick.AddListener(SetOriginalMapsView);
            _customTab.onClick.AddListener(SetCustomMapsView);
            _downloadedTab.onClick.AddListener(SetDownloadedMapsView);
            LoadMaps();
        }

        private void SetOriginalMapsView()
        {
            DisableAllMapViews();
            _originalScrollbar.gameObject.SetActive(true);
            _originalTab.image.color = Colors.whiteColor;
        }

        private void SetCustomMapsView()
        {
            DisableAllMapViews();
            _customScrollbar.gameObject.SetActive(true);
            _customTab.image.color = Colors.whiteColor;
        }

        private void SetDownloadedMapsView()
        {
            DisableAllMapViews();
            _downloadedScrollbar.gameObject.SetActive(true);
            _downloadedTab.image.color = Colors.whiteColor;
        }

        private void DisableAllMapViews()
        {
            _originalTab.image.color = Colors.unavailableElementDarkGray;
            _customTab.image.color = Colors.unavailableElementDarkGray;
            _downloadedTab.image.color = Colors.unavailableElementDarkGray;
            _originalScrollbar.gameObject.SetActive(false);
            _customScrollbar.gameObject.SetActive(false);
            _downloadedScrollbar.gameObject.SetActive(false);
        }

        private void OnOkButtonClick()
        {
            MapEditorManager.Instance.LoadMap(_selectorButton.serializableMap);
            Hide();
        }

        private void LoadMaps()
        {
            Moroutine.Run(MapSerializerSystem.MapSerializer.GetAvailableMaps((maps =>
            {
                foreach (var map in maps)
                {
                    var mapInfo = map.SerializableMatchInfo.MapInfo;
                    var mapCategory = mapInfo.MapCategory;
                    Transform targetContent = null;
                    switch (mapCategory)
                    {
                        case MapCategory.Original:
                            targetContent = _originalScrollbarContent;
                            break;
                        case MapCategory.Custom:
                            targetContent = _customScrollbarContent;
                            break;
                        case MapCategory.Downloaded:
                            targetContent = _downloadedScrollbarContent;
                            break;
                    }

                    var mapSelectorInstance = GameObject.Instantiate(_mapSelectorButtonPrefab, Vector3.zero, Quaternion.identity, targetContent);
                    mapSelectorInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    mapSelectorInstance.serializableMap = map;
                    mapSelectorInstance.mapNameText = map.SerializableMatchInfo.MapInfo.Name;
                    mapSelectorInstance.button.onClick.AddListener(() => { _selectorButton = mapSelectorInstance; });
                    GlobalVariables.LoadMapPreviewIcon(mapInfo.previewTexturePath, mapInfo.sizeX, mapInfo.sizeY, mapCategory, (sprite) =>
                    {
                        mapSelectorInstance.mapPreviewImage = sprite;
                        _mapSelectors.Add(mapSelectorInstance);
                    });
                }
            })));
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetOriginalMapsView();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}