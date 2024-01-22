using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class SaveManager
    {
        public static SaveManager Instance => _instance = _instance == null ? new SaveManager() : _instance;
        private static SaveManager _instance;

        public delegate void SavedMapEventDelegate(SerializableMap serializableMap);

        public void SaveGame(PlayableMap playableMap, string filename)
        {
            var jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var matchInfo = GameManager.Instance.Info;
            //Terrains
            Dictionary<int, DynamicTerrainData> castleTerrains = new Dictionary<int, DynamicTerrainData>();
            DynamicTerrainData worldTerrainData = new DynamicTerrainData(playableMap.WorldTerrain);
            foreach (var castle in playableMap.CastlesTerrains.Keys)
            {
                DynamicTerrainData castleTerrainData = new DynamicTerrainData(playableMap.CastlesTerrains[castle]);
                var castleSerializableMap = new SerializableMap();
                castleSerializableMap.MainTerrain = castleTerrainData;
                castleSerializableMap.SerializableMatchInfo = new SerializableMatchInfo();
                castleTerrains.Add(castle.UniqueId, castleTerrainData);
            }

            //Players
            var serializablePlayers = new Dictionary<PlayerColor, SerializablePlayerData>();
            var allPlayers = mapScenarioHandler.players;
            foreach (var player in allPlayers)
            {
                if (player.Key == PlayerColor.Neutral) continue;
                serializablePlayers.Add(player.Key, new SerializablePlayerData(player.Value));
            }

            //Saved Map
            var savedMap = new SerializableMap()
            {
                MainTerrain = worldTerrainData,
                TurnOfPlayerId = mapScenarioHandler.TurnOfPlayerId,
                SerializableMatchInfo = new SerializableMatchInfo(matchInfo),
                CastleTerrains = castleTerrains,
                Players = serializablePlayers
            };

            //Json
            var json = JsonConvert.SerializeObject(savedMap, jsonSerializerSettings);
            var directory = GlobalStrings.SAVED_GAMES_PATH;
            var filePath = $"{directory}/{filename}.json";
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllText(filePath, json);
        }
    }
}