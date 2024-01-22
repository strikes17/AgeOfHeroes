using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "New Terrain Tile Object", menuName = "Create New Terrain Tile Object")]
    public class TerrainTileObject : ScriptableObject
    {
        public Sprite Icon;
        public List<Tile> Tiles;
        public TerrainTileMaterialName MaterialName;
        public TerrainTileMapType TileMapType;
        public float baseMovementPenalty;
    }
}