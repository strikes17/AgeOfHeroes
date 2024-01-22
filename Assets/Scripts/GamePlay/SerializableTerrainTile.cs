using UnityEngine;

namespace AgeOfHeroes
{

    public class SerializableTerrainTile
    {
        public SerializableTerrainTile(Vector3Int position, int index, string terrainTileObjectName)
        {
            positionX = position.x;
            positionY = position.y;
            Index = index;
            TerrainTileObjectName = terrainTileObjectName;
        }

        public int positionX, positionY;
        public int Index = -1;
        public string TerrainTileObjectName;
    }
}