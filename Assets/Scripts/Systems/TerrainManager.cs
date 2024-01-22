using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class TerrainManager : AbstractManager
    {
        [SerializeField] private MapTilemapGridReferences _tilemapGridReferencesPrefab;

        private PlayableMap _currentPlayableMap;
        private Tilemap _fogOfWarTilemap;


        public PlayableMap CurrentPlayableMap
        {
            get => _currentPlayableMap;
        }

        public Tilemap FogOfWarTilemap => _fogOfWarTilemap;

        public delegate void OnPlayableTerrainDelegate(PlayableTerrain serializablePlayableTerrain);


        private void Start()
        {
            OnLoaded();
        }

        public Castle GetCastleByTerrain(PlayableTerrain playableTerrain)
        {
            var keys = CurrentPlayableMap.CastlesTerrains.Keys;
            foreach (var key in keys)
            {
                if (key.CastleTerrain == playableTerrain)
                {
                    return key;
                }
            }

            return null;
        }

        public bool IsCastleTerrain(PlayableTerrain playableTerrain)
        {
            return CurrentPlayableMap.CastlesTerrains.ContainsValue(playableTerrain);
        }

        public void HideAllTerrainsExcept(PlayableTerrain playableTerrain)
        {
            _currentPlayableMap.WorldTerrain.SetActive(false);

            foreach (var terrain in _currentPlayableMap.CastlesTerrains)
            {
                terrain.Value.SetActive(false);
            }

            playableTerrain.SetActive(true);
        }

        private void SpawnFogOfWar()
        {
            var fogsOfWarRootTransform = new GameObject($"__fog_of_war_").transform;
            var _fogOfWarObjectPrefab = ResourcesBase.GetPrefab("Special/Fog Of War Grid");
            _fogOfWarTilemap = GameObject.Instantiate(_fogOfWarObjectPrefab, Vector3.zero,
                Quaternion.identity, fogsOfWarRootTransform).transform.GetChild(0).GetComponent<Tilemap>();
        }

        public void LoadMapAsSavedGame(string saveName)
        {
            SpawnFogOfWar();
            _currentPlayableMap = new PlayableMap();
            var players = GameManager.Instance.MapScenarioHandler.AllPlayers;
            LoadSerializableMap(saveName, playableTerrain =>
            {
                _currentPlayableMap.WorldTerrain = playableTerrain;
                players.ForEach(x =>
                {
                    x.fogOfWarController.CreateFogOfWarObjectForTerrain(x, playableTerrain);
                    x.fogOfWarController.SetTilemapReference(_fogOfWarTilemap);
                    x.ActiveTerrain = playableTerrain;
                });
            }, (serializableMap) =>
            {
                var allPlayers = GameManager.Instance.MapScenarioHandler.AllPlayersExcludeNeutral;
                allPlayers.ForEach(x => { x.LoadFromSerializable(serializableMap.Players[x.Color]); });
                GameManager.Instance.MainCamera.SetBounds(_currentPlayableMap.WorldTerrain.Size);
                GameManager.Instance.MapScenarioHandler.LoadGameInitialization(serializableMap.TurnOfPlayerId);
            }, MapCategory.SavedGame);
        }

        public void LoadMapAsNewGame(string mapName)
        {
            SpawnFogOfWar();
            _currentPlayableMap = new PlayableMap();
            var players = GameManager.Instance.MapScenarioHandler.AllPlayers;
            LoadSerializableMap(mapName, playableTerrain =>
            {
                _currentPlayableMap.WorldTerrain = playableTerrain;
                players.ForEach(x =>
                {
                    x.fogOfWarController.CreateFogOfWarObjectForTerrain(x, playableTerrain);
                    x.fogOfWarController.SetTilemapReference(_fogOfWarTilemap);
                    x.ActiveTerrain = playableTerrain;
                });
            }, (serializableMap) =>
            {
                GameManager.Instance.MainCamera.SetBounds(_currentPlayableMap.WorldTerrain.Size);
                GameManager.Instance.MapScenarioHandler.NewGameInitialization();
            });
        }

        public void LoadSerializableMap(string mapName, Action<PlayableTerrain> onTerrainLoaded = null,
            Action<SerializableMap> mapReady = null, MapCategory mapCategory = MapCategory.Original)
        {
            Moroutine.Run(MapSerializerSystem.MapSerializer.LoadMap(mapName, serializableMap =>
            {
                var mapTilemapGridReferences =
                    GameObject.Instantiate(_tilemapGridReferencesPrefab, Vector3.zero, Quaternion.identity);
                mapTilemapGridReferences.name = mapName;
                FillAllTilemaps(mapTilemapGridReferences, serializableMap, (playableTerrain) =>
                {
                    playableTerrain.TerrainNavigator.CreateNavigationMap(playableTerrain,
                        serializableMap.MainTerrain.Collisions);
                    onTerrainLoaded?.Invoke(playableTerrain);
                    var players = GameManager.Instance.MapScenarioHandler.AllPlayers;
                    foreach (var player in players)
                    {
                        player.fogOfWarController.CreateFogOfWarObjectForTerrain(player, playableTerrain);
                        player.ReserveTerrainFog(playableTerrain);
                        if (player.Color == PlayerColor.Neutral) continue;
                        var lowFogTiles = serializableMap.MainTerrain.LowFogOfWarCells;
                        List<Vector2Int> lowFog = null;
                        if (lowFogTiles.TryGetValue(player.Color, out lowFog))
                        {
                            foreach (var cell in lowFog)
                            {
                                player.fogOfWarController.SetFogOfWarCellTypeSingle(new Vector3Int(cell.x, cell.y),
                                    playableTerrain, FogOfWarCellType.Low, true);
                            }
                        }
                    }

                    ;
                    CreateCastles(serializableMap, playableTerrain, mapCategory);
                    CreateEntities(serializableMap.MainTerrain, playableTerrain, () =>
                    {
                        var serializableCastles = serializableMap.MainTerrain.SerializableCastles;
                        var castles = playableTerrain.SpawnedCastles;
                        Dictionary<int, SerializableCastle> dictionary = new Dictionary<int, SerializableCastle>();
                        foreach (var serializableCastle in serializableCastles)
                        {
                            dictionary.TryAdd(serializableCastle.UniqueId, serializableCastle);
                        }

                        SerializableCastle value = null;
                        foreach (var castle in castles)
                        {
                            if (dictionary.TryGetValue(castle.UniqueId, out value))
                            {
                                castle.LoadGarnison(value);
                            }
                        }
                    });
                    mapReady?.Invoke(serializableMap);
                }, mapCategory);
            }, mapCategory));
        }

        private void CreateCastleTerrains(SerializableMap serializableMap,
            Dictionary<Castle, SerializableCastle> castles,
            MapCategory mapCategory)
        {
            foreach (var castle in castles.Keys)
            {
                castle.castleMapName = "hum_castle";
                LoadSerializableMap(castle.castleMapName, (castleTerrain =>
                {
                    castle.CastleTerrain = castleTerrain;
                    _currentPlayableMap.CastlesTerrains.TryAdd(castle, castleTerrain);
                    castleTerrain.SetActive(false);
                }), (castleSerializableMap) =>
                {
                    var owner = castle.PlayerOwnerColor;
                    var player = GameManager.Instance.MapScenarioHandler.players[owner];
                    player.fogOfWarController.ClearDeepFogOfWarEverywhereReserved(castle.CastleTerrain, player);
                    if (mapCategory == MapCategory.SavedGame)
                    {
                        var mapCastleTerrains = serializableMap.CastleTerrains;
                        if (mapCastleTerrains.ContainsKey(castle.UniqueId))
                        {
                            CreateEntities(mapCastleTerrains[castle.UniqueId], castle.CastleTerrain);
                        }
                    }

                    castle.LoadFromSerializable(castles[castle]);
                });
            }
        }

        private List<Castle> CreateCastles(SerializableMap serializableMap, PlayableTerrain playableTerrain,
            MapCategory mapCategory)
        {
            var spawnManager = GameManager.Instance.SpawnManager;
            var serializableCastles = serializableMap.MainTerrain.SerializableCastles;
            Dictionary<Castle, SerializableCastle> castles = new Dictionary<Castle, SerializableCastle>();
            var matchInfo = FindObjectOfType<MatchInfo>();
            foreach (var serializableCastle in serializableCastles)
            {
                var fr = matchInfo.playersFractions[serializableCastle.PlayerOwnerColor];
                serializableCastle.Fraction = fr;
                var fractionObject = ResourcesBase.GetFractionObject(fr);
                serializableCastle.objectName = fractionObject.DefaultCastleObject;
                var castleObject = ResourcesBase.GetCastleObject(fractionObject.DefaultCastleObject);
                serializableCastle.castleInfoName = castleObject.internalName;
                var castle = spawnManager.SpawnCastle(serializableCastle, playableTerrain);
                castle.PlayerOwnerColor = serializableCastle.PlayerOwnerColor;
                castle.UpdateFogOnWorldTerrain(_currentPlayableMap.WorldTerrain);
                castles.Add(castle, serializableCastle);
            }

            CreateCastleTerrains(serializableMap, castles, mapCategory);
            return castles.Keys.ToList();
        }

        private void CreateEntities(DynamicTerrainData dynamicTerrainData, PlayableTerrain playableTerrain,
            Action entitiesCreated = null)
        {
            var matchInfo = FindObjectOfType<MatchInfo>();
            var spawnManager = GameManager.Instance.SpawnManager;
            var serializableCharacters = dynamicTerrainData.SerializableCharacters;
            foreach (var serializableCharacter in serializableCharacters)
            {
                spawnManager.SpawnCharacter(serializableCharacter, playableTerrain);
            }

            var serializableHeroes = dynamicTerrainData.SerializableHeroes;
            foreach (var serializableHero in serializableHeroes)
            {
                if (serializableHero.Fraction == Fraction.None)
                {
                    var fr = matchInfo.playersFractions[serializableHero.PlayerOwnerColor];
                    var fractionObject = ResourcesBase.GetFractionObject(fr);
                    Debug.Log(fractionObject.Fraction);
                    var heroObject = ResourcesBase.GetHeroObject(fractionObject.Heroes[0], fractionObject.Fraction);
                    serializableHero.InitFromObject(heroObject);
                }

                spawnManager.SpawnHero(serializableHero, playableTerrain);
            }

            var serializableArtifacts = dynamicTerrainData.SerializableArtifacts;
            foreach (var serializableArtifact in serializableArtifacts)
            {
                spawnManager.SpawnArtifact(serializableArtifact, playableTerrain);
            }

            var serializableMarkers = dynamicTerrainData.SerializableMarkers;
            foreach (var serializableMarker in serializableMarkers)
            {
                spawnManager.SpawnMarker(serializableMarker, playableTerrain);
            }

            var serializableTreasures = dynamicTerrainData.SerializableTreasures;
            foreach (var serializableTreasure in serializableTreasures)
            {
                spawnManager.SpawnTreasure(serializableTreasure, playableTerrain);
            }

            var serializableDwellings = dynamicTerrainData.SerializableDwellings;
            foreach (var serializableDwelling in serializableDwellings)
            {
                spawnManager.SpawnDwelling(serializableDwelling, playableTerrain);
            }

            entitiesCreated?.Invoke();
        }

        private void FillAllTilemaps(MapTilemapGridReferences mapTilemapGridReferences, SerializableMap serializableMap,
            Action<PlayableTerrain> ready,
            MapCategory mapCategory)
        {
            if (mapCategory == MapCategory.SavedGame)
            {
                Moroutine.Run(MapSerializerSystem.MapSerializer.LoadMap(
                    serializableMap.SerializableMatchInfo.MapInfo.Name, originalSerializableMap =>
                    {
                        FillTilemap(mapTilemapGridReferences.Ground, originalSerializableMap.Ground);
                        FillTilemap(mapTilemapGridReferences.ObstaclesLow, originalSerializableMap.ObstaclesLow);
                        FillTilemap(mapTilemapGridReferences.ObstaclesHigh, originalSerializableMap.ObstaclesHigh);
                        FillTilemap(mapTilemapGridReferences.Trees, originalSerializableMap.Trees);
                        FillTilemap(mapTilemapGridReferences.GroundOverlay, originalSerializableMap.GroundOverlay);
                        FillTilemap(mapTilemapGridReferences.Water, originalSerializableMap.Water);
                        FillTilemap(mapTilemapGridReferences.Tool, originalSerializableMap.Tool);
                        var playableTerrain = new PlayableTerrain(originalSerializableMap, mapTilemapGridReferences);
                        ready?.Invoke(playableTerrain);
                    }, MapCategory.Original));
            }
            else
            {
                FillTilemap(mapTilemapGridReferences.Ground, serializableMap.Ground);
                FillTilemap(mapTilemapGridReferences.ObstaclesLow, serializableMap.ObstaclesLow);
                FillTilemap(mapTilemapGridReferences.ObstaclesHigh, serializableMap.ObstaclesHigh);
                FillTilemap(mapTilemapGridReferences.Trees, serializableMap.Trees);
                FillTilemap(mapTilemapGridReferences.GroundOverlay, serializableMap.GroundOverlay);
                FillTilemap(mapTilemapGridReferences.Water, serializableMap.Water);
                FillTilemap(mapTilemapGridReferences.Tool, serializableMap.Tool);
                var playableTerrain = new PlayableTerrain(serializableMap, mapTilemapGridReferences);
                ready?.Invoke(playableTerrain);
            }
        }

        private void FillTilemap(Tilemap tilemap, SerializableTileMap serializableTileMap)
        {
            var serializableTerrainTiles = serializableTileMap.SerializableTerrainTiles;
            foreach (var serializableTerrainTile in serializableTerrainTiles)
            {
                var position = new Vector3Int(serializableTerrainTile.positionX, serializableTerrainTile.positionY, 0);
                var tileObject = ResourcesBase.GetTerrainTileObject(serializableTerrainTile.TerrainTileObjectName);
                var tile = tileObject.Tiles[serializableTerrainTile.Index];
                tilemap.SetTile(position, tile);
            }
        }
    }
}