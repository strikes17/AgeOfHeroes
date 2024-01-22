using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class TerrainTileMap
    {
        public TerrainTileMap(List<SerializableTerrainTile> serializableTerrainTiles)
        {
            if (serializableTerrainTiles == null) return;
            foreach (var serializableTerrainTile in serializableTerrainTiles)
            {
                var terrainTile = new TerrainTile(serializableTerrainTile);
                TerrainTiles.Add(terrainTile.Position, terrainTile);
            }
        }

        public Dictionary<Vector2Int, TerrainTile> TerrainTiles = new Dictionary<Vector2Int, TerrainTile>();
    }
}