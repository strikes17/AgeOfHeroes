using System;
using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public enum GameType
    {
        New,
        Loaded
    }

    public class MatchInfo : MonoBehaviour
    {
        public static MatchInfo CreateInstance()
        {
            var matchInfoPrefab = ResourcesBase.GetPrefab("Match Info");
            var matchInfoInstance = GameObject.Instantiate(matchInfoPrefab, Vector3.zero, Quaternion.identity).GetComponent<MatchInfo>();
            return matchInfoInstance;
        }

        public static void Destroy(MatchInfo matchInfo)
        {
            Destroy(matchInfo?.gameObject);
        }
        public MapInfo MapInfo = new MapInfo();
        public Dictionary<PlayerColor, PlayerType> isPlayerHuman = new Dictionary<PlayerColor, PlayerType>();
        public Dictionary<PlayerColor, Fraction> playersFractions = new Dictionary<PlayerColor, Fraction>();
        public Dictionary<PlayerColor, string> playersHeroes = new Dictionary<PlayerColor, string>();
        public uint totalPlayerCount;
        public MatchType MatchType = MatchType.Local;
        public MatchDifficulty Difficulty = MatchDifficulty.Easy;
        private GameType _gameType;

        public GameType GameType
        {
            get => _gameType;
            set => _gameType = value;
        }

        public void FromSerializable(SerializableMatchInfo serializableMatchInfo)
        {
            MapInfo = serializableMatchInfo.MapInfo;
            isPlayerHuman = serializableMatchInfo.isPlayerHuman;
            totalPlayerCount = serializableMatchInfo.totalPlayerCount;
            MatchType = serializableMatchInfo.MatchType;
            Difficulty = serializableMatchInfo.Difficulty;
            playersFractions = serializableMatchInfo.playersFractions;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}