using UnityEngine;

namespace AgeOfHeroes
{
    public class TerrainTile
    {
        public TerrainTile(SerializableTerrainTile serializableTerrainTile)
        {
            Position = new Vector2Int(serializableTerrainTile.positionX, serializableTerrainTile.positionY);
            Index = serializableTerrainTile.Index;
            TerrainTileObjectName = serializableTerrainTile.TerrainTileObjectName;
        }
        public int Index = -1;
        public string TerrainTileObjectName;
        public Vector2Int Position;
    }


}