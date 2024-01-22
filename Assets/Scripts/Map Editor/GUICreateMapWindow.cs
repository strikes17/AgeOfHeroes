using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUICreateMapWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mapNameInputText;
        [SerializeField] private TMP_Dropdown _mapSizeDropdown;
        [SerializeField] private Button _okButton, _noButton;
        private List<string> mapSizesStrings = new List<string>();

        private void Awake()
        {
            _okButton.onClick.AddListener(OnOkButtonClicked);
            _noButton.onClick.AddListener(Hide);
            mapSizesStrings = Enum.GetNames(typeof(MapSize)).ToList();
        }

        public void OnOkButtonClicked()
        {
            var selectedSize = mapSizesStrings[_mapSizeDropdown.value];
            var mapSizeValue = Enum.Parse<MapSize>(selectedSize);
            var mapSize = GlobalVariables.GetMapSizeVector2Int(mapSizeValue);
            string mapNameFix = _mapNameInputText.text;
            mapNameFix = mapNameFix.Replace($"{(char) 8203}", string.Empty);
            if (string.IsNullOrEmpty(mapNameFix))
            {
                return;
            }

            MapEditorManager.Instance.CreateNewMap(mapNameFix, mapSize.x, mapSize.y);
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _mapSizeDropdown.options.Clear();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var mapSize in mapSizesStrings)
            {
                options.Add(new TMP_Dropdown.OptionData(mapSize));
            }

            _mapSizeDropdown.AddOptions(options);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}