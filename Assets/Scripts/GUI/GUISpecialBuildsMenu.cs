using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUISpecialBuildsMenu : GUILeftSidebarAbstractMenu
    {
        [SerializeField] private GUISpecialBuildingWidget _widgetPrefab;
        [SerializeField] private GUISpecialBuildingDialogue _specialBuildingDialogue;
        [SerializeField] private GUISpecialBuildingInfoWindow _specialBuildingInfoWindow;
        [SerializeField] private Transform _contentTransform;
        private List<GUISpecialBuildingWidget> _widgets = new List<GUISpecialBuildingWidget>();

        public void UpdateShopGUI(Castle castle)
        {
            Clear();
            var specialBuildings = castle.SpecialBuildings;
            foreach (var building in specialBuildings)
            {
                if(building.Value.IsRestricted)
                    continue;
                var widgetInstance = GameObject.Instantiate(_widgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                var specialBuilding = building.Value;
                specialBuilding.Built -= UpdateShopGUI; 
                specialBuilding.Built += UpdateShopGUI;
                widgetInstance.specialBuilding = specialBuilding;
                _widgets.Add(widgetInstance);
                if (specialBuilding.IsBuilt)
                {
                    widgetInstance.button.onClick.AddListener((() => OpenSpecialBuildingInfoWindow(castle, specialBuilding)));
                }
                else
                {
                    widgetInstance.button.onClick.AddListener((() => OpenBuildingDialogue(castle, specialBuilding)));
                }
            }
        }

        private void OpenSpecialBuildingInfoWindow(Castle castle, SpecialBuilding specialBuilding)
        {
            _specialBuildingInfoWindow.SetGUI(castle, specialBuilding);
            _specialBuildingInfoWindow.Show();
        }

        private void OpenBuildingDialogue(Castle castle, SpecialBuilding specialBuilding)
        {
            _specialBuildingDialogue.SetGUI(castle, specialBuilding);
            _specialBuildingDialogue.Show();
        }

        private void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }

            _widgets.Clear();
        }
    }
}