using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorManager : MonoBehaviour
    {
        public static MapEditorManager Instance => instance;

        private static MapEditorManager instance;

        public MapEditorToolMode SelectedToolMode;
        public CharacterObject SelectedCharacterObject;
        public HeroObject SelectedHeroObject;
        public MapEditorCharacter SelectedMapCharacter;
        public MapEditorHero SelectedMapHero;
        public FractionObject SelectedFractionObject;
        public TerrainTileObject SelectedTerrainTileObject;
        public Fraction SelectedFraction = Fraction.Human;
        public PlayerColor SelectedPlayerColor = PlayerColor.Red;
        public int SelectedTerrainTileIndex = -1;
        public CastleObject SelectedCastleObject;
        public DwellBuilding SelectedDwellBuilding;
        public StructureType SelectedStructureType = StructureType.Castle;
        public TreasureObject SelectedTreasureObject;
        public ArtifactObject SelectedArtifactObject;
        public MapEditorArtifact SelectedArtifact;
        public MapEditorTreasure SelectedTreasure;
        public MapEditorAIMarker SelectedMarker;

        [SerializeField] private MapEditorWorldGridManager _worldGridManager;
        [SerializeField] private GUIMapEditorManager _guiMapEditorManager;
        [SerializeField] private GUIRaycaster _guiRaycaster;
        [SerializeField] private AdvancedCamera _camera;

        public GUIRaycaster GUIRaycaster => _guiRaycaster;
        public AdvancedCamera MainCamera => _camera;

        public GUIMapEditorManager GUIMapEditorManager => _guiMapEditorManager;

        private SerializableMap _serializableMap;

        private void Awake()
        {
            instance = GetComponent<MapEditorManager>();
        }

        public void CreateNewMap(string mapName, int sizeX, int sizeY)
        {
            Vector2Int mapSize = new Vector2Int(sizeX, sizeY);
            _serializableMap = new SerializableMap();
            _serializableMap.MainTerrain = new DynamicTerrainData();
            _serializableMap.MainTerrain.Size = mapSize;
            _serializableMap.SerializableMatchInfo = new SerializableMatchInfo();
            _serializableMap.SerializableMatchInfo.MapInfo.Name = mapName;
            _serializableMap.SerializableMatchInfo.MapInfo.sizeX = sizeX;
            _serializableMap.SerializableMatchInfo.MapInfo.sizeY = sizeY;

#if UNITY_EDITOR
            _serializableMap.SerializableMatchInfo.MapInfo.MapCategory = MapCategory.Original;
#endif
            _worldGridManager.CreateNewMap(mapSize);
        }


        public void LoadMap(SerializableMap serializableMap)
        {
            _serializableMap = serializableMap;
            _worldGridManager.LoadMap(_serializableMap);
        }

        public string GenerateMapPreview(Vector2Int mapSize)
        {
            Texture2D mapPreview = new Texture2D(mapSize.x, mapSize.y);
            var ground = _worldGridManager.groundTileMapMatrix;
            var water = _worldGridManager.waterTileMapMatrix;
            var obstaclesLow = _worldGridManager.obstaclesLowTileMapMatrix;
            Color emptyColor = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < mapPreview.width; i++)
            {
                for (int j = 0; j < mapPreview.height; j++)
                {
                    var position = new Vector3Int(i, j, 0);
                    if (water[i, j] != null)
                    {
                        var waterTerrainTileMaterial = ResourcesBase
                            .GetTerrainTileObject(water[i, j].TerrainTileObjectName).MaterialName;
                        var waterColor = GlobalVariables.TerrainColors[waterTerrainTileMaterial];
                        mapPreview.SetPixel(i, j, waterColor);
                    }

                    if (ground[i, j] != null)
                    {
                        var groundTerrainTileMaterial = ResourcesBase
                            .GetTerrainTileObject(ground[i, j].TerrainTileObjectName).MaterialName;
                        var groundColor = GlobalVariables.TerrainColors[groundTerrainTileMaterial];
                        mapPreview.SetPixel(i, j, groundColor);
                    }

                    if (obstaclesLow[i, j] != null)
                    {
                        var obstaclesLowTerrainTileMaterial = ResourcesBase
                            .GetTerrainTileObject(obstaclesLow[i, j].TerrainTileObjectName).MaterialName;
                        var obstaclesLowColor = GlobalVariables.TerrainColors[obstaclesLowTerrainTileMaterial];
                        mapPreview.SetPixel(i, j, obstaclesLowColor);
                    }
                }
            }

            mapPreview.Apply();
            var mapPng = mapPreview.EncodeToPNG();
            string mapName = _serializableMap.SerializableMatchInfo.MapInfo.Name;
            string previewPathFormat = $"Previews/{mapName}.png";
            var mapPreviewPath = string.Empty;
            var currentDevice = GlobalVariables.CurrentDevice;
            currentDevice = DeviceType.Editor;
            if (currentDevice == DeviceType.Editor)
            {
                mapPreviewPath = $"{GlobalStrings.ORIGINAL_MAPS_DIRECTORY}/{previewPathFormat}";
            }
            else
            {
                mapPreviewPath = $"{GlobalStrings.USER_MAPS_DIRECTORY}/{previewPathFormat}";
            }

            File.WriteAllBytes(mapPreviewPath, mapPng);
            return previewPathFormat;
        }

        public void SaveMap()
        {
            List<PlayerColor> playerColors = new List<PlayerColor>();
            var mapInfo = _serializableMap.SerializableMatchInfo.MapInfo;
            string mapName = mapInfo.Name;
            var size = _serializableMap.MainTerrain.Size;
            var mainTerrain = _serializableMap.MainTerrain;
            mainTerrain.Size = size;
            _serializableMap.Tool = new SerializableTileMap(_worldGridManager.toolTileMapMatrix, size);
            _serializableMap.Background =
                new SerializableTileMap(_worldGridManager.backgroundTileMapMatrix, size);
            _serializableMap.Ground = new SerializableTileMap(_worldGridManager.groundTileMapMatrix, size);
            _serializableMap.Water = new SerializableTileMap(_worldGridManager.waterTileMapMatrix, size);
            _serializableMap.ObstaclesLow =
                new SerializableTileMap(_worldGridManager.obstaclesLowTileMapMatrix, size);
            _serializableMap.ObstaclesHigh =
                new SerializableTileMap(_worldGridManager.obstaclesHighTileMapMatrix, size);
            _serializableMap.Trees = new SerializableTileMap(_worldGridManager.treesTileMapMatrix, size);
            _serializableMap.GroundOverlay =
                new SerializableTileMap(_worldGridManager.groundOverlayTileMapMatrix, size);
            _serializableMap.Roads = new SerializableTileMap();
            mainTerrain.SerializableCastles = new List<SerializableCastle>();
            mainTerrain.SerializableCharacters = new List<SerializableCharacter>();
            mainTerrain.SerializableHeroes = new List<SerializableHero>();
            mainTerrain.SerializableTreasures = new List<SerializableTreasure>();
            mainTerrain.SerializableArtifacts = new List<SerializableArtifact>();
            mainTerrain.SerializableMarkers = new List<SerializableMarker>();
            mainTerrain.SerializableDwellings = new List<SerializableDwelling>();
            mainTerrain.Collisions = new List<(int, int)>();
            _serializableMap.CastleTerrains = new Dictionary<int, DynamicTerrainData>();
            var charactersDictionary = _worldGridManager.charactersDictionary;
            foreach (var key in charactersDictionary.Keys)
            {
                MapEditorEntity mapEditorEntity = null;
                if (!charactersDictionary.TryGetValue(key, out mapEditorEntity))
                {
                    Debug.Log($"failed to get character at position {key}");
                    continue;
                }

                MapEditorCharacter mapEditorCharacter = (MapEditorCharacter)mapEditorEntity;

                if (mapEditorEntity == null) continue;
                if (!playerColors.Contains(mapEditorCharacter.PlayerOwnerColor))
                    playerColors.Add(mapEditorCharacter.PlayerOwnerColor);
                var serializableCharacter = new SerializableCharacter(mapEditorCharacter);
                mainTerrain.SerializableCharacters.Add(serializableCharacter);
            }

            var heroesDictionary = _worldGridManager.heroesDictionary;
            foreach (var key in heroesDictionary.Keys)
            {
                MapEditorEntity mapEditorEntity = null;
                if (!heroesDictionary.TryGetValue(key, out mapEditorEntity))
                {
                    Debug.Log($"failed to get hero at position {key}");
                    continue;
                }

                MapEditorHero mapEditorHero = (MapEditorHero)mapEditorEntity;

                if (mapEditorEntity == null) continue;
                var serializableHero = new SerializableHero(mapEditorHero);

                mainTerrain.SerializableHeroes.Add(serializableHero);
            }

            var castlesDictionary = _worldGridManager.castlesDictionary;
            foreach (var key in castlesDictionary.Keys)
            {
                var mapEditorCastle = castlesDictionary[key] as MapEditorCastle;
                var serializableCastle = new SerializableCastle(mapEditorCastle);
                mainTerrain.SerializableCastles.Add(serializableCastle);
                if (!playerColors.Contains(mapEditorCastle.PlayerOwnerColor))
                    playerColors.Add(mapEditorCastle.PlayerOwnerColor);
            }

            var treasuresDictionary = _worldGridManager.treasuresDictionary;
            foreach (var key in treasuresDictionary.Keys)
            {
                var mapEditorTreasure = treasuresDictionary[key] as MapEditorTreasure;
                var serializableTreasure = new SerializableTreasure(mapEditorTreasure);
                mainTerrain.SerializableTreasures.Add(serializableTreasure);
            }

            var artifactsDictionary = _worldGridManager.artifactsDictionary;
            foreach (var key in artifactsDictionary.Keys)
            {
                var mapEditorArtifact = artifactsDictionary[key] as MapEditorArtifact;
                var serializableArtifact = new SerializableArtifact(mapEditorArtifact);
                mainTerrain.SerializableArtifacts.Add(serializableArtifact);
            }

            var markersDictionary = _worldGridManager.markersDictionary;
            foreach (var key in markersDictionary.Keys)
            {
                var mapEditorAIMarker = markersDictionary[key] as MapEditorAIMarker;
                var serializableMarker = new SerializableMarker(mapEditorAIMarker as MapEditorAISiegeGuardMarker);
                mainTerrain.SerializableMarkers.Add(serializableMarker);
            }

            var dwellingsDictionary = _worldGridManager.dwellingsDictionary;
            foreach (var key in dwellingsDictionary.Keys)
            {
                var mapEditorDwelling = dwellingsDictionary[key] as MapEditorDwelling;
                var type = mapEditorDwelling.DwellBuilding.GetType();
                if (type == typeof(ResourceIncomeDwell))
                {
                    var serializableDwelling = new SerializableResourceDwelling(mapEditorDwelling);
                    mainTerrain.SerializableDwellings.Add(serializableDwelling);
                }
                else if (type == typeof(CharacterIncomeDwell))
                {
                    var serializableDwelling = new SerializableCharactersDwelling(mapEditorDwelling);
                    mainTerrain.SerializableDwellings.Add(serializableDwelling);
                }
            }

            var collisions = _worldGridManager.collisions;
            foreach (var key in collisions.Keys)
            {
                if (_worldGridManager.artifactsDictionary.ContainsKey(new Vector3Int(key.x, key.y, 0)) ||
                    _worldGridManager.treasuresDictionary.ContainsKey(new Vector3Int(key.x, key.y, 0)) ||
                    _worldGridManager.markersDictionary.ContainsKey(new Vector3Int(key.x, key.y, 0)))
                {
                    continue;
                }

                mainTerrain.Collisions.Add((key.x, key.y));
            }

            var previewPath = GenerateMapPreview(size);
            mapInfo.previewTexturePath = previewPath;

            int totalPlayersCount = 0;
            playerColors.Remove(PlayerColor.Neutral);
            playerColors.ForEach(x =>
            {
                totalPlayersCount++;
                _serializableMap.MainTerrain.LowFogOfWarCells.TryAdd(x, new List<Vector2Int>());
            });

            mapInfo.maxPlayersCount = totalPlayersCount;
            mapInfo.maxHumanPlayersCount = totalPlayersCount;

            _serializableMap.MainTerrain = mainTerrain;

            JsonSerializerSettings jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();
            var jsonObject = JsonConvert.SerializeObject(_serializableMap, jsonSerializerSettings);

            bool unityEditor = GlobalVariables.CurrentDevice == DeviceType.Editor;
            //!!!!!!!! REMOVE LINE BEFORE RELEASE
            unityEditor = true;
            //
            string filePath = string.Empty;
            if (unityEditor)
            {
                filePath = $"{GlobalStrings.ORIGINAL_MAPS_DIRECTORY}/{mapName}.json";
            }
            else
            {
                filePath = $"{GlobalStrings.USER_MAPS_DIRECTORY}/{mapName}.json";
            }

            Debug.Log(filePath);
            File.WriteAllText(filePath, jsonObject);
            Debug.Log($"Saved to {filePath}");
        }
    }
}