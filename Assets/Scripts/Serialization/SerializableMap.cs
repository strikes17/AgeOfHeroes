using System.Collections.Generic;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableMap
    {
        public SerializableMap(){}
        public Dictionary<PlayerColor, SerializablePlayerData> Players;
        public SerializableMatchInfo SerializableMatchInfo;
        public PlayerColor TurnOfPlayerId;
        public DynamicTerrainData MainTerrain;
        public Dictionary<int, DynamicTerrainData> CastleTerrains;
        
        public SerializableTileMap Background,
            Water,
            Ground,
            Roads,
            ObstaclesLow,
            ObstaclesHigh,
            Tool,
            Trees,
            GroundOverlay;
    }
}