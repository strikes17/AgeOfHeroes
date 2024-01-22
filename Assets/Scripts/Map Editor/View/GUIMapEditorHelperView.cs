using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorHelperView : MonoBehaviour
    {
        [SerializeField] private List<FractionObject> fractionsObjects;

        // [SerializeField] private List<TerrainTileObject> terrainTileObjects;
        [SerializeField] private GUIMapEditorFractionWidget _fractionWidgetPrefab;
        [SerializeField] private GUIMapEditorTerrainSetWidget _terrainSetWidgetPrefab;
        [FormerlySerializedAs("_structerWidgetPrefab")] [SerializeField] private GUIMapEditorStructureTypeWidget structureTypeWidgetPrefab;
        [SerializeField] private GameObject _commonWidgetPrefab;
        [SerializeField] private Transform _contentTransform;
        private List<GUIMapEditorFractionWidget> _fractionWidgetsList = new List<GUIMapEditorFractionWidget>();
        private List<GUIMapEditorTerrainSetWidget> _terrainSetWidgets = new List<GUIMapEditorTerrainSetWidget>();
        private List<GUIMapEditorStructureTypeWidget> _structerWidgets = new List<GUIMapEditorStructureTypeWidget>();
        private List<TerrainTileObject> terrainTileObjects = new List<TerrainTileObject>();
        private Button _aiMarkersButton, _artifactsButton;
        
        public event OnMarkerSelectionDelegate MarkerSelectionChanged
        {
            add => markerModeSelectionChanged += value;
            remove => markerModeSelectionChanged -= value;
        }

        public event OnFractionSelectionDelegate FractionSelectionChanged
        {
            add => fractionSelectionChanged += value;
            remove => fractionSelectionChanged -= value;
        }

        public event OnTerrainSetSelectionDelegate TerrainSetSelectionChanged
        {
            add => terrainSetSelectionChanged += value;
            remove => terrainSetSelectionChanged -= value;
        }

        public event OnStructureSelectionDelegate StructureTypeSelectionChanged
        {
            add => structureTypeSelectionChanged += value;
            remove => structureTypeSelectionChanged -= value;
        }

        public event OnTreasureModeDelegate TreasureModeSelectionChanged
        {
            add => treasureModeSelectionChanged += value;
            remove => treasureModeSelectionChanged -= value;
        }
        public delegate void OnMarkerSelectionDelegate(MarkerType markerType);

        public delegate void OnFractionSelectionDelegate(Fraction fraction);

        public delegate void OnTerrainSetSelectionDelegate(TerrainTileObject terrainTileObject);

        public delegate void OnStructureSelectionDelegate(StructureType structureType);

        public delegate void OnTreasureModeDelegate(uint treasureMode);

        private event OnStructureSelectionDelegate structureTypeSelectionChanged;
        private event OnFractionSelectionDelegate fractionSelectionChanged;
        private event OnTerrainSetSelectionDelegate terrainSetSelectionChanged;
        private event OnTreasureModeDelegate treasureModeSelectionChanged;
        private event OnMarkerSelectionDelegate markerModeSelectionChanged;

        private int mode = 0, prevMode = 0;
        
        private void OnMarkerTypeChanged(MarkerType markerType)
        {
            markerModeSelectionChanged.Invoke(markerType);
        }

        private void OnFractionSelectionChanged(Fraction fraction)
        {
            fractionSelectionChanged.Invoke(fraction);
        }

        private void OnTerrainSetSelectionChanged(TerrainTileObject terrainTileObject)
        {
            terrainSetSelectionChanged.Invoke(terrainTileObject);
        }

        private void OnStructureTypeSelectionChanged(StructureType structureType)
        {
            structureTypeSelectionChanged.Invoke(structureType);
        }

        private void Clear()
        {
            if (mode == 1)
            {
                for (int i = 0; i < _fractionWidgetsList.Count; i++)
                {
                    Destroy(_fractionWidgetsList[i].gameObject);
                }

                _fractionWidgetsList.Clear();
            }
            else if (mode == 2)
            {
                for (int i = 0; i < _terrainSetWidgets.Count; i++)
                {
                    Destroy(_terrainSetWidgets[i].gameObject);
                }

                _terrainSetWidgets.Clear();
            }

            else if (mode == 3)
            {
                for (int i = 0; i < _structerWidgets.Count; i++)
                {
                    Destroy(_structerWidgets[i].gameObject);
                }

                _structerWidgets.Clear();
            }

            else if (mode == 4)
            {
                Destroy(_aiMarkersButton.gameObject);
                Destroy(_artifactsButton.gameObject);
            }
        }

        public void Hide()
        {
            Clear();
        }
        
        public void ShowForMarkers()
        {
            if (mode != 5)
                Clear();
            prevMode = mode;
            mode = 5;
            if (mode == prevMode)
                return;
            _aiMarkersButton = GameObject.Instantiate(_commonWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform).GetComponent<Button>();
            _aiMarkersButton.transform.GetChild(0).GetComponent<Image>().sprite = ResourcesBase.GetSprite("map_editor_treasures+treasure");
            _aiMarkersButton.onClick.AddListener((() => OnMarkerTypeChanged(MarkerType.AI)));
            
        }

        public void ShowForTreasures()
        {
            if (mode != 4)
                Clear();
            prevMode = mode;
            mode = 4;
            if (mode == prevMode)
                return;
            _aiMarkersButton = GameObject.Instantiate(_commonWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform).GetComponent<Button>();
            _aiMarkersButton.transform.GetChild(0).GetComponent<Image>().sprite = ResourcesBase.GetSprite("map_editor_treasures+treasure");
            _aiMarkersButton.onClick.AddListener((() => treasureModeSelectionChanged.Invoke(0)));

            _artifactsButton = GameObject.Instantiate(_commonWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform).GetComponent<Button>();
            _artifactsButton.transform.GetChild(0).GetComponent<Image>().sprite = ResourcesBase.GetSprite("map_editor_treasures+artifact");
            _artifactsButton.onClick.AddListener((() => treasureModeSelectionChanged.Invoke(1)));
        }

        public void ShowForStructures()
        {
            if (mode != 3)
                Clear();
            prevMode = mode;
            mode = 3;
            if (mode == prevMode)
                return;

            var castleWidget = GameObject.Instantiate(structureTypeWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
            Sprite castleSprite = ResourcesBase.GetSprite("Castles/castles_atlas+castle_undefined");
            castleWidget.Set(StructureType.Castle, castleSprite);
            castleWidget.SelectButton.onClick.AddListener(() => OnStructureTypeSelectionChanged(StructureType.Castle));

            var shopBuildingsWidget = GameObject.Instantiate(structureTypeWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
            Sprite shopBuildingsSprite = ResourcesBase.GetSprite("Castles/castles_atlas+castle_undefined");
            shopBuildingsWidget.Set(StructureType.Dwell, shopBuildingsSprite);
            shopBuildingsWidget.SelectButton.onClick.AddListener(() => OnStructureTypeSelectionChanged(StructureType.Dwell));

            _structerWidgets.Add(castleWidget);
            _structerWidgets.Add(shopBuildingsWidget);
        }

        public void ShowForTerrainTool()
        {
            if (mode != 2)
                Clear();
            prevMode = mode;
            mode = 2;
            if (mode == prevMode)
                return;
            terrainTileObjects = MapEditorDatabase.Instance.GetAllTerrainTileObjects();
            int index = 0;
            foreach (var terrainTileObject in terrainTileObjects)
            {
                var terrainWidgetInstance = GameObject.Instantiate(_terrainSetWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                terrainWidgetInstance.name = terrainTileObject.name;
                terrainWidgetInstance.Set(terrainTileObject);
                terrainWidgetInstance.SelectButton.onClick.AddListener(() => OnTerrainSetSelectionChanged(terrainTileObject));
                _terrainSetWidgets.Add(terrainWidgetInstance);
            }

            if (MapEditorManager.Instance.SelectedTerrainTileObject == null)
                MapEditorManager.Instance.SelectedTerrainTileObject = terrainTileObjects[0];
        }

        public void ShowForCharacters()
        {
            if (mode != 1)
                Clear();
            prevMode = mode;
            mode = 1;
            if (mode == prevMode)
                return;
            foreach (var fractionObject in fractionsObjects)
            {
                var fractionWidgetInstance = GameObject.Instantiate(_fractionWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                fractionWidgetInstance.Set(fractionObject);
                fractionWidgetInstance.SelectButton.onClick.AddListener(() => OnFractionSelectionChanged(fractionObject.Fraction));
                _fractionWidgetsList.Add(fractionWidgetInstance);
            }
        }

        public void ShowForHeroes()
        {
            if (mode != 4)
                Clear();
            prevMode = mode;
            mode = 4;
            if (mode == prevMode)
                return;
            foreach (var fractionObject in fractionsObjects)
            {
                var fractionWidgetInstance = GameObject.Instantiate(_fractionWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                fractionWidgetInstance.Set(fractionObject);
                fractionWidgetInstance.SelectButton.onClick.AddListener(() => OnFractionSelectionChanged(fractionObject.Fraction));
                _fractionWidgetsList.Add(fractionWidgetInstance);
            }
        }
    }
}