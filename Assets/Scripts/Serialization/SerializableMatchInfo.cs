using System.Collections.Generic;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableMatchInfo
    {
        public SerializableMatchInfo(){}
        public SerializableMatchInfo(MatchInfo matchInfo)
        {
            MapInfo = matchInfo.MapInfo;
            isPlayerHuman = matchInfo.isPlayerHuman;
            totalPlayerCount = matchInfo.totalPlayerCount;
            MatchType = matchInfo.MatchType;
            Difficulty = matchInfo.Difficulty;
            playersFractions = matchInfo.playersFractions;
        }

        public Dictionary<PlayerColor, Fraction> playersFractions = new Dictionary<PlayerColor, Fraction>();
        public MapInfo MapInfo = new MapInfo();
        public Dictionary<PlayerColor, PlayerType> isPlayerHuman = new Dictionary<PlayerColor, PlayerType>();
        public uint totalPlayerCount;
        public MatchType MatchType = MatchType.Local;
        public MatchDifficulty Difficulty = MatchDifficulty.Easy;
    }
}