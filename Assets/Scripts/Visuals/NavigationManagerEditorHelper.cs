using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes
{
    public class NavigationManagerEditorHelper : MonoBehaviour
    {
        public bool visualGridEnabledByDefault = true;
        public bool visualGridEnabledOnStart = false;
        private TerrainManager _terrainManager;

        private void OnEnable()
        {
            _terrainManager = GetComponent<TerrainManager>();
            // _terrainManager.ObstaclesTileMap.GetComponent<TilemapRenderer>().enabled = visualGridEnabledOnStart;
        }

        private void Start()
        {
            // _terrainManager.ObstaclesTileMap.GetComponent<TilemapRenderer>().enabled = visualGridEnabledOnStart;
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     visualGridEnabledByDefault = !visualGridEnabledByDefault;
            // }
            //
            // _terrainManager.ToolTilemap.GetComponent<TilemapRenderer>().enabled = visualGridEnabledByDefault;
        }
    }
}