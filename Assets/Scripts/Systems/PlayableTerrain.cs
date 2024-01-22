using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class BackgroundTerrain
    {
        private Dictionary<TerrainTileMapType, Tilemap> _tilemaps = new Dictionary<TerrainTileMapType, Tilemap>();

        public Dictionary<TerrainTileMapType, Tilemap> Tilemaps => _tilemaps;

        public void SetActive(bool bActive)
        {
            var values = _tilemaps.Values;
            foreach (var value in values)
            {
                value.gameObject.SetActive(bActive);
            }
        }
    }

    public class PlayableTerrain
    {
        public TerrainTileMap Background, Water, Ground, Roads, ObstaclesLow, ObstaclesHigh, Tool, Trees;
        public Tilemap ToolTilemap => MapTilemapGridReferences?.Tool;
        public Tilemap ObstaclesLowTilemap => MapTilemapGridReferences?.ObstaclesLow;
        public Tilemap ObstaclesHighTilemap => MapTilemapGridReferences?.ObstaclesHigh;
        public Tilemap WaterTilemap => MapTilemapGridReferences?.Water;
        public Tilemap GroundTilemap => MapTilemapGridReferences.Ground;
        public Tilemap TreesTilemap => MapTilemapGridReferences?.Trees;
        private Vector2Int _size;

        private BackgroundTerrain _backgroundTerrain;
        public string Name => _mapTilemapGridReferences?.name;

        public Vector2Int Size => _size;

        public TerrainNavigator TerrainNavigator
        {
            get => _terrainNavigator;
        }

        public SerializableMap serializableMap
        {
            get => _serializableMap;
        }

        public MapTilemapGridReferences MapTilemapGridReferences
        {
            get => _mapTilemapGridReferences;
        }

        public List<Hero> Heroes
        {
            get => SpawnedCharacters.OfType<Hero>().ToList();
        }

        public List<Corpse> SpawnedCorpses = new List<Corpse>();
        public List<ControllableCharacter> SpawnedCharacters = new List<ControllableCharacter>();
        public List<Castle> SpawnedCastles = new List<Castle>();
        public List<AbstractTreasure> SpawnedTreasures = new List<AbstractTreasure>();
        public List<ArtifactBehaviour> SpawnedArtifacts = new List<ArtifactBehaviour>();
        public List<AbstractMarker> SpawnedMarkers = new List<AbstractMarker>();
        public List<DwellBuildingBehaviour> SpawnedDwellings = new List<DwellBuildingBehaviour>();

        public int UniqueId => GetHashCode();

        private int _id;

        public PlayableTerrain(SerializableMap serializablePlayableMap,
            MapTilemapGridReferences mapTilemapGridReferences)
        {
            _terrainNavigator = new TerrainNavigator();
            _serializableMap = serializablePlayableMap;
            _mapTilemapGridReferences = mapTilemapGridReferences;
            _size = _serializableMap.MainTerrain.Size;
            Background = new TerrainTileMap(_serializableMap.Background?.SerializableTerrainTiles);
            Water = new TerrainTileMap(_serializableMap.Water.SerializableTerrainTiles);
            Ground = new TerrainTileMap(_serializableMap.Ground.SerializableTerrainTiles);
            Roads = new TerrainTileMap(_serializableMap.Roads?.SerializableTerrainTiles);
            ObstaclesLow = new TerrainTileMap(_serializableMap?.ObstaclesLow.SerializableTerrainTiles);
            ObstaclesHigh = new TerrainTileMap(_serializableMap?.ObstaclesHigh.SerializableTerrainTiles);
            Tool = new TerrainTileMap(_serializableMap.Tool?.SerializableTerrainTiles);
            Trees = new TerrainTileMap(_serializableMap.Trees?.SerializableTerrainTiles);
            SpawnBackgroundTerrain(this);
        }

        private void SpawnBackgroundTerrain(PlayableTerrain playableTerrain)
        {
            _backgroundTerrain = new BackgroundTerrain();
            var bgTerrainPrefab = ResourcesBase.GetPrefab("Background Terrain");
            var bgTerrainInstance = GameObject.Instantiate(bgTerrainPrefab, Vector3.zero, Quaternion.identity);
            var bgTile = ResourcesBase.GetTile("worldbg");
            var bgTile2 = ResourcesBase.GetTile("worldbg_2");
            Tilemap groundTilemap = bgTerrainInstance.transform.Find("Tilemap").GetComponent<Tilemap>();
            Tilemap obstaclesLowTilemap = bgTerrainInstance.transform.Find("TilemapMountains").GetComponent<Tilemap>();
            for (int i = -5; i < playableTerrain.Size.x + 5; i++)
            {
                for (int j = -5; j < playableTerrain.Size.y + 5; j++)
                {
                    bool a = (i >= 0 && i < playableTerrain.Size.x);
                    bool b = (j >= 0 && j < playableTerrain.Size.y);
                    if (a && b) continue;
                    groundTilemap.SetTile(new Vector3Int(i, j, 0), bgTile);
                    float rand = Random.Range(0f, 1f);
                    if (rand < 0.25f)
                        obstaclesLowTilemap.SetTile(new Vector3Int(i, j, 0), bgTile2);
                }
            }

            _backgroundTerrain.Tilemaps.TryAdd(TerrainTileMapType.Ground, groundTilemap);
            _backgroundTerrain.Tilemaps.TryAdd(TerrainTileMapType.ObstacleLow, obstaclesLowTilemap);
        }

        public void __PrintCharacters()
        {
            Debug.Log($"{_mapTilemapGridReferences.name} <--");
            SpawnedCharacters.ForEach(x => Debug.Log($"{x.title}"));
            Debug.Log($"{_mapTilemapGridReferences.name} -->");
        }

        public void SetActive(bool bActive)
        {
            foreach (var controllableCharacter in SpawnedCharacters)
            {
                if (bActive) controllableCharacter.Show();
                else controllableCharacter.Hide();
            }

            foreach (var spawnedCastle in SpawnedCastles)
            {
                spawnedCastle.gameObject.SetActive(bActive);
            }

            foreach (var spawnedTreasure in SpawnedTreasures)
            {
                spawnedTreasure.gameObject.SetActive(bActive);
            }

            foreach (var spawnedArtifact in SpawnedArtifacts)
            {
                spawnedArtifact.gameObject.SetActive(bActive);
            }

            foreach (var spawnedCorpse in SpawnedCorpses)
            {
                spawnedCorpse.gameObject.SetActive(bActive);
            }

            foreach (var spawnedMarker in SpawnedMarkers)
            {
                spawnedMarker.gameObject.SetActive(bActive);
            }
            
            foreach (var spawnedDwelling in SpawnedDwellings)
            {
                spawnedDwelling.gameObject.SetActive(bActive);
            }

            _backgroundTerrain.SetActive(bActive);
            _mapTilemapGridReferences.gameObject.SetActive(bActive);
        }

        private TerrainNavigator _terrainNavigator;
        private SerializableMap _serializableMap;
        private MapTilemapGridReferences _mapTilemapGridReferences;
    }
}