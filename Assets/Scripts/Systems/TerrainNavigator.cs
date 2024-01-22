using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes
{
    public class TerrainNavigator
    {
        private NavigationMap _navigationMap;

        public NavigationMap NavigationMap { get => _navigationMap; set => _navigationMap = value; }

        public void CreateNavigationMap(PlayableTerrain terrain, List<(int, int)> staticCollisions)
        {
            _navigationMap = new NavigationMap();
            _navigationMap.Init(terrain);
            _navigationMap.GenerateNavigationsForTerrain(staticCollisions);
        }
    }
}