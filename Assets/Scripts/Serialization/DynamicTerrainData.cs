using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class DynamicTerrainData
    {
        public string playableTerrainName;
        public Vector2Int Size;
        public List<(int, int)> Collisions = new List<(int, int)>();
        public List<SerializableCharacter> SerializableCharacters = new List<SerializableCharacter>();
        public List<SerializableHero> SerializableHeroes = new List<SerializableHero>();
        public List<SerializableCastle> SerializableCastles = new List<SerializableCastle>();
        public List<SerializableTreasure> SerializableTreasures = new List<SerializableTreasure>();
        public List<SerializableArtifact> SerializableArtifacts = new List<SerializableArtifact>();
        public List<SerializableMarker> SerializableMarkers = new List<SerializableMarker>();
        public List<SerializableDwelling> SerializableDwellings = new List<SerializableDwelling>();
        public Dictionary<PlayerColor, List<Vector2Int>> LowFogOfWarCells = new Dictionary<PlayerColor, List<Vector2Int>>();
        
        public DynamicTerrainData(){}

        public DynamicTerrainData(PlayableTerrain playableTerrain)
        {
            playableTerrainName = playableTerrain.Name;
            Size = playableTerrain.Size;
            var navigationsMap = playableTerrain.TerrainNavigator.NavigationMap.navigationsMap;
            for (int i = 0; i < playableTerrain.Size.x; i++)
            {
                for (int j = 0; j < playableTerrain.Size.y; j++)
                {
                    var nav = navigationsMap[i, j];
                    if (nav == 0)
                        Collisions.Add((i, j));
                }
            }
            
            var allPlayers = GameManager.Instance.MapScenarioHandler.players;
            foreach (var player in allPlayers)
            {
                if(player.Key == PlayerColor.Neutral)continue;
                var fogOfWarObject = player.Value.fogOfWarController.GetFogOfWarObject(playableTerrain.UniqueId);
                LowFogOfWarCells.TryAdd(player.Key, fogOfWarObject.GetAllCellsOfType(FogOfWarCellType.Low));
            }

            var spawnedCharacters = playableTerrain.SpawnedCharacters;
            foreach (var spawnedCharacter in spawnedCharacters)
            {
                if (spawnedCharacter is Hero)
                {
                    SerializableHero serializableHero = new SerializableHero(spawnedCharacter as Hero);
                    SerializableHeroes.Add(serializableHero);
                }
                else
                {
                    SerializableCharacter serializableCharacter = new SerializableCharacter(spawnedCharacter);
                    SerializableCharacters.Add(serializableCharacter);
                }
            }

            var spawnedTreasures = playableTerrain.SpawnedTreasures;
            foreach (var spawnedTreasure in spawnedTreasures)
            {
                var serializableTreasure = new SerializableTreasure(spawnedTreasure);
                SerializableTreasures.Add(serializableTreasure);
            }

            var spawnedArtifacts = playableTerrain.SpawnedArtifacts;
            foreach (var spawnedArtifact in spawnedArtifacts)
            {
                var serializableArtifact = new SerializableArtifact(spawnedArtifact);
                SerializableArtifacts.Add(serializableArtifact);
            }

            var spawnedCastles = playableTerrain.SpawnedCastles;
            foreach (var spawnedCastle in spawnedCastles)
            {
                var serializableCastle = new SerializableCastle(spawnedCastle);
                SerializableCastles.Add(serializableCastle);
            }
            
            var spawnedMarkers = playableTerrain.SpawnedMarkers;
            foreach (var abstractMarker in spawnedMarkers)
            {
                var serializableMarker = new SerializableMarker(abstractMarker as AIGuardMarker);
                SerializableMarkers.Add(serializableMarker);
            }

            var spawnedDwellings = playableTerrain.SpawnedDwellings;
            foreach (var spawnedDwelling in spawnedDwellings)
            {
                var type = spawnedDwelling.Building.GetType();
                SerializableDwelling serializableDwelling = null;
                if (type == typeof(ResourceIncomeDwell))
                {
                    serializableDwelling = new SerializableResourceDwelling(spawnedDwelling);
                }
                else if (type == typeof(CharacterIncomeDwell))
                {
                    serializableDwelling = new SerializableCharactersDwelling(spawnedDwelling);
                }
                SerializableDwellings.Add(serializableDwelling);
            }
        }
    }
}