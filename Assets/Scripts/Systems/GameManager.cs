using System;
using System.IO;
using AgeOfHeroes.AI;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using Mirror;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GameManager : AbstractManager
    {
        [SerializeField] private AdvancedCamera _mainCamera;
        [SerializeField] private Transform actionCellsPool;
        public TerrainManager terrainManager => _terrainManager;
        public SpawnManager SpawnManager => _spawnManager;
        public MapScenarioHandler MapScenarioHandler => _mapScenarioHandler;
        public SoundManager SoundManager => _soundManager;
        public GUISpellBook GUISpellBook => _guiSpellBook;
        public GUIResourcesPanel GUIResourcesPanel => _guiResourcesPanel;
        public GUIRaycaster GUIRaycaster => _guiRaycaster;

        [SerializeField] private GUIRaycaster _guiRaycaster;
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private SpawnManager _spawnManager;
        [SerializeField] private MapScenarioHandler _mapScenarioHandler;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private GUISpellBook _guiSpellBook;
        [SerializeField] private GUIResourcesPanel _guiResourcesPanel;
        [SerializeField] private GUIManager _guiManager;
        [SerializeField] private GUISpellBookButtonWidget _guiSpellBookButtonWidget;
        [SerializeField] private AIManager _aiManager;
        [SerializeField] private MusicManager _musicManager;
        [SerializeField] private GameSettings _gameSettings;    

        private MatchInfo _matchInfo;

        public static GameManager Instance => _instance;
        private static GameManager _instance;

        public GUIManager GUIManager => _guiManager;

        public NetworkManager NetworkManager => _networkManager;

        public AdvancedCamera MainCamera => _mainCamera;

        public AIManager aiManager => _aiManager;

        public GUISpellBookButtonWidget guiSpellBookButtonWidget => _guiSpellBookButtonWidget;

        public PlayableTerrain WorldTerrain => _terrainManager.CurrentPlayableMap.WorldTerrain;

        public MusicManager MusicManager => _musicManager;

        public MatchInfo Info => _matchInfo;

        public GameSettings GameSettings { get => _gameSettings;  }

        private void Awake()
        {
            BasicSetup();
            Preload();
            _matchInfo = GameObject.FindObjectOfType<MatchInfo>();
            switch (_matchInfo.GameType)
            {
                case GameType.New:
                    StartNewGame();
                    break;
                case GameType.Loaded:
                    LoadGame();
                    break;
            }
        }

        public void StartNewGame()
        {
            Loaded += OnNewGame;
        }

        public void LoadGame()
        {
            Loaded += OnLoadGame;
        }

        private void OnNewGame()
        {
            MapScenarioHandler.SpawnPlayers();
            terrainManager.LoadMapAsNewGame(_matchInfo.MapInfo.Name);
        }

        private void OnLoadGame()
        {
            var targetLoadingFileName = GlobalVariables.GetString("targetLoadingMap");
            Moroutine.Run(MapSerializerSystem.MapSerializer.LoadMap(targetLoadingFileName, serializableMap =>
            {
                var serializableMatchInfo = serializableMap.SerializableMatchInfo;
                _matchInfo = MatchInfo.CreateInstance();
                _matchInfo.FromSerializable(serializableMatchInfo);
                MapScenarioHandler.SpawnPlayers();
                terrainManager.LoadMapAsSavedGame(targetLoadingFileName);
            }, MapCategory.SavedGame));
        }

        private void BasicSetup()
        {
            Application.targetFrameRate = 300;
            _instance = this;
        }

        private void Update()
        {
            if (isReady)
                return;
            bool isEveryManagerReady = terrainManager.isReady && _spawnManager.isReady && _soundManager.isReady &&
                                       _musicManager.isReady;
            if (isEveryManagerReady)
                OnLoaded();
        }

        private void Preload()
        {
            //TO DO Move to another place
            _guiSpellBook.Init();
            ResourcesBase.LoadPrefabAsync("GUI/SpellSlot");
        }
    }
}