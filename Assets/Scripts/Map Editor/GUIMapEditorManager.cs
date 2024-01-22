using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorManager : MonoBehaviour
    {
        [SerializeField] private Button _newMapButton,
            _loadMapButton,
            _characterButton,
            _terrainButton;
        
        [SerializeField] private Button _decorButton,
            _playerColorToolButton,
            _treasureButton,
            _structuresButton,
            _heroButton,
            _exitEditorButton,
            _saveMapButton,
            _markerToolButton;

        [SerializeField] private GUIMapEditorMarkersView _markersView;
        [SerializeField] private GUIMapEditorCharactersView _charactersView;
        [SerializeField] private GUIMapEditorHeroesView _heroesView;
        [SerializeField] private GUIMapEditorTerrainView _terrainView;
        [SerializeField] private GUIMapEditorHelperView _helperView;
        [SerializeField] private GUIMapEditorPlayerColorView _playerColorView;
        [SerializeField] private GUIMapEditorStructuresView _structuresView;
        [SerializeField] private GUIMapEditorTreasuresView _treasuresView;
        [SerializeField] private GUIMapEditorCharacterInfoWindow _characterInfoWindowPrefab;
        [SerializeField] private GUICreateMapWindow _createMapWindow;
        [SerializeField] private GUIMapEditorLoadMapDialogue _loadMapDialogue;

        private void Awake()
        {
            _characterButton.onClick.AddListener(EnableCharactersTool);
            _terrainButton.onClick.AddListener(EnableTerrainTool);
            _structuresButton.onClick.AddListener(EnableStructuresTool);
            _heroButton.onClick.AddListener(EnableHeroesTool);
            _playerColorToolButton.onClick.AddListener(_playerColorView.SwitchState);
            _newMapButton.onClick.AddListener(_createMapWindow.Show);
            _exitEditorButton.onClick.AddListener(ExitEditor);
            _saveMapButton.onClick.AddListener(SaveMap);
            _loadMapButton.onClick.AddListener(_loadMapDialogue.Show);
            _treasureButton.onClick.AddListener(EnableTreasuresTool);
            _markerToolButton.onClick.AddListener(EnableMarkersTool);
            _createMapWindow.Hide();
            _loadMapDialogue.Hide();
            Init();
        }

        private void SaveMap()
        {
            MapEditorManager.Instance.SaveMap();
        }

        private void ExitEditor()
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync("main_menu", LoadSceneMode.Single);
        }

        private void Init()
        {
            DisableAllTools();
        }

        public GUIMapEditorCharacterInfoWindow CreateMapCharacterInfoWindowInstance(MapEditorCharacter character)
        {
            var characterInfoWindowInstance = GameObject.Instantiate(_characterInfoWindowPrefab,
                Vector3.zero, Quaternion.identity, transform);
            characterInfoWindowInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            characterInfoWindowInstance.Set(character);
            characterInfoWindowInstance.Show();
            return characterInfoWindowInstance;
        }


        private void DisableAllTools()
        {
            _charactersView.Hide();
            _terrainView.Hide();
            _treasuresView.Hide();
            _structuresView.Hide();
            _heroesView.Hide();
            _markersView.Hide();
            _helperView.FractionSelectionChanged -= UpdateCharactersToolPanel;
            _helperView.TerrainSetSelectionChanged -= UpdateTerrainToolPanel;
            _helperView.StructureTypeSelectionChanged -= UpdateStructuresToolPanel;
            _helperView.MarkerSelectionChanged -= UpdateMarkersToolPanel;
        }
        
        public void EnableMarkersTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Marker;
            DisableAllTools();
            _markersView.Show();
            _helperView.MarkerSelectionChanged += UpdateMarkersToolPanel;
            _helperView.ShowForMarkers();
        }

        public void EnableTreasuresTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Treasure;
            DisableAllTools();
            _treasuresView.Show();
            _treasuresView.ShowForTreasures();
            _helperView.TreasureModeSelectionChanged += UpdateTreasureToolPanel;
            _helperView.ShowForTreasures();
        }

        private void EnableStructuresTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Structure;
            DisableAllTools();
            _structuresView.Show();
            _helperView.StructureTypeSelectionChanged += UpdateStructuresToolPanel;
            _helperView.ShowForStructures();
        }

        private void EnableTerrainTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Terrain;
            DisableAllTools();
            _terrainView.Show();
            _helperView.TerrainSetSelectionChanged += UpdateTerrainToolPanel;
            _helperView.ShowForTerrainTool();
        }

        private void EnableCharactersTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Character;
            DisableAllTools();
            _charactersView.Show();
            _helperView.FractionSelectionChanged += UpdateCharactersToolPanel;
            _helperView.ShowForCharacters();
        }

        private void EnableHeroesTool()
        {
            MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Hero;
            DisableAllTools();
            _heroesView.Show();
            _helperView.FractionSelectionChanged += UpdateHeroesToolPanel;
            _helperView.ShowForCharacters();
        }

        private void UpdateTreasureToolPanel(uint mode)
        {
            if (mode == 0)
            {
                _treasuresView.ShowForTreasures();
                MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Treasure;
            }
            else if (mode == 1)
            {
                _treasuresView.ShowForArtifacts();
                MapEditorManager.Instance.SelectedToolMode = MapEditorToolMode.Artifact;
            }
        }

        private void UpdateMarkersToolPanel(MarkerType markerType)
        {
            _markersView.UpdateGUI(markerType);
        }
        
        private void UpdateCharactersToolPanel(Fraction fraction)
        {
            _charactersView.UpdateGUI(fraction);
        }

        private void UpdateHeroesToolPanel(Fraction fraction)
        {
            _heroesView.UpdateGUI(fraction);
        }

        private void UpdateTerrainToolPanel(TerrainTileObject terrainTileObject)
        {
            _terrainView.UpdateGUI(terrainTileObject);
        }

        private void UpdateStructuresToolPanel(StructureType structureType)
        {
            _structuresView.Set(structureType);
            _structuresView.Show();
        }
    }
}