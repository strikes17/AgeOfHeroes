using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableTileMap
    {
        public SerializableTileMap(){}
        public SerializableTileMap(SerializableTerrainTile[,] matrix, Vector2Int size)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var terrainTileSource = matrix[i, j];
                    if (terrainTileSource == null) continue;
                    SerializableTerrainTiles.Add(terrainTileSource);
                }
            }
        }

        public List<SerializableTerrainTile> SerializableTerrainTiles = new List<SerializableTerrainTile>();
    }
}