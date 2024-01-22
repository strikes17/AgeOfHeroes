using System.Collections.Generic;

namespace AgeOfHeroes
{
    public class PlayableMap
    {
        public PlayableTerrain WorldTerrain;
        public Dictionary<Castle, PlayableTerrain> CastlesTerrains = new Dictionary<Castle, PlayableTerrain>();

        public bool IsCastleTerrain(PlayableTerrain playableTerrain)
        {
            return CastlesTerrains.ContainsValue(playableTerrain);
        }
    }
}