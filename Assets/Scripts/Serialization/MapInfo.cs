using System;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapInfo
    {
        public string previewTexturePath;
        public int sizeX, sizeY;
        public string Name;
        public int maxPlayersCount;
        public int maxHumanPlayersCount;
        [NonSerialized] private Texture2D _previewTexture;
        public MapCategory MapCategory = MapCategory.Custom;
    }
}