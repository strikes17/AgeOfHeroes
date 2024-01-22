using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace AgeOfHeroes
{
    public class PlayerFogOfWarController : MonoBehaviour
    {
        [SerializeField] private Tile _fogOfWarDeepTile;
        [SerializeField] private Tile _fogOfWarLowTile;
        [SerializeField] private Tile _fogOfWarClearTile;

        private Dictionary<int, FogOfWarObject> fogOfWarObjects = new Dictionary<int, FogOfWarObject>();
        private Tilemap _tilemap;

        public FogOfWarObject GetFogOfWarObject(int playableTerrainUniqueId)
        {
            FogOfWarObject fogOfWarObject = null;
            if (fogOfWarObjects.TryGetValue(playableTerrainUniqueId, out fogOfWarObject))
            {
                return fogOfWarObject;
            }

            return null;
        }

        private void Awake()
        {
            // terrainManager.WorldTerrainReady += terrain => {  };
        }

        public void SetTilemapReference(Tilemap tilemap)
        {
            _tilemap = tilemap;
        }

        public void CreateFogOfWarObjectForTerrain(Player player, PlayableTerrain playableTerrain)
        {
            if(fogOfWarObjects.ContainsKey(playableTerrain.UniqueId))return;
            var fogOfWarObject = new FogOfWarObject(playableTerrain.Size.x, playableTerrain.Size.y);
            fogOfWarObject.name = $"{player.Color} fog for {playableTerrain.Name}";
            fogOfWarObjects.TryAdd(playableTerrain.UniqueId, fogOfWarObject);
        }
        
        public void ShowFogOfWarForTerrain(PlayableTerrain terrain)
        {
            // var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            UpdateTilemapForTerrain(terrain);
        }
        

        public void ClearDeepFogOfWarEverywhereReserved(PlayableTerrain terrain, Player player)
        {
            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            for (int i = 0; i < terrain.Size.x; i++)
            {
                for (int j = 0; j < terrain.Size.y; j++)
                {
                    var position = new Vector3Int(i, j, 0);
                    player.OpenedFogDictionary[terrain].Add(position);
                    SetFogOfWarCellType(position, fogOfWarObject, FogOfWarCellType.Clear, false);
                }
            }
        }


        public FogOfWarCellType GetCellTypeAtPosition(Vector3Int position, PlayableTerrain playableTerrain)
        {
            var fogOfWarObject = GetFogOfWarObject(playableTerrain.UniqueId);
            return fogOfWarObject.GetCell(new Vector2Int(position.x, position.y));
        }

        public bool IsCharacterInsideFogOfWar(ControllableCharacter character)
        {
            var terrain = character.PlayableTerrain;
            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            var pos = new Vector2Int(character.Position.x, character.Position.y);
            bool insideFog = fogOfWarObject.GetCell(pos) != FogOfWarCellType.Clear;
            return insideFog;
        }

        public void UpdateVisionAtAreaForPlayer(Player player, Vector2Int center, PlayableTerrain terrain, int radius,
            bool ignoreHighObstacles = false,
            bool unrevealFog = false)
        {
            var newOpenedFogOfWarCells = CollectFogCellsAround(center, terrain, radius);
            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            foreach (var newCell in newOpenedFogOfWarCells)
            {
                if (!player.OpenedFogDictionary[terrain].Contains(newCell))
                {
                    player.OpenedFogDictionary[terrain].Add(newCell);
                    SetFogOfWarCellType(newCell, fogOfWarObject, FogOfWarCellType.Clear, player.isHuman);
                }
            }
        }

        public void UpdateVisionForCharacterOnDeath(ControllableCharacter character)
        {
            var terrain = character.PlayableTerrain;
            //New opened fog cells
            var newOpenedFogOfWarCells =
                CollectFogCellsAround(character.Position, terrain, character.VisionValue);

            List<Vector3Int> cellsToHideInFog = new List<Vector3Int>();
            var allCharacters = character.Player.controlledCharacters;
            foreach (var candidateToHideInFog in newOpenedFogOfWarCells)
            {
                bool candidateFailed = false;
                foreach (var c in allCharacters)
                {
                    if (c == character) continue;
                    if (c.OpenedFogDictionary[c.PlayableTerrain].Contains(candidateToHideInFog) ||
                        c.Player.OpenedFogDictionary[c.PlayableTerrain].Contains(candidateToHideInFog))
                    {
                        candidateFailed = true;
                        break;
                    }
                }

                character.OpenedFogDictionary[terrain].Remove(candidateToHideInFog);
                if (!candidateFailed)
                    cellsToHideInFog.Add(candidateToHideInFog);
            }

            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);

            //Hide old cells in low fog of war
            foreach (var position in cellsToHideInFog)
            {
                fogOfWarObject.SetCell(new Vector2Int(position.x, position.y), FogOfWarCellType.Low);
            }
        }

        public void UpdateVisionForCharacter(ControllableCharacter character)
        {
            // return;
            //New opened fog cells
            var isHuman = character.Player.isHuman;
            var terrain = character.PlayableTerrain;
            var newOpenedFogOfWarCells =
                CollectFogCellsAround(character.Position, terrain, character.VisionValue);

            //Cells probably to hide in fog
            List<Vector3Int> candidatesToHide = new List<Vector3Int>();
            foreach (var oldCell in character.OpenedFogDictionary[terrain])
            {
                bool isFaded = !newOpenedFogOfWarCells.Contains(oldCell);
                if (isFaded)
                {
                    candidatesToHide.Add(oldCell);
                }
            }

            //Cells 100% percent to hide in fog
            List<Vector3Int> cellsToHideInFog = new List<Vector3Int>();
            var allCharacters = character.Player.controlledCharacters;
            foreach (var candidateToHideInFog in candidatesToHide)
            {
                bool candidateFailed = false;
                foreach (var c in allCharacters)
                {
                    if (c == character) continue;
                    if (c.OpenedFogDictionary[terrain].Contains(candidateToHideInFog) || character.Player.OpenedFogDictionary[terrain].Contains(candidateToHideInFog))
                    {
                        candidateFailed = true;
                        break;
                    }
                }

                character.OpenedFogDictionary[terrain].Remove(candidateToHideInFog);
                if (!candidateFailed)
                    cellsToHideInFog.Add(candidateToHideInFog);
            }

            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            //Show new cells, remove fog from em
            foreach (var newCell in newOpenedFogOfWarCells)
            {
                if (!character.OpenedFogDictionary[terrain].Contains(newCell))
                {
                    character.OpenedFogDictionary[terrain].Add(newCell);
                    SetFogOfWarCellType(newCell, fogOfWarObject, FogOfWarCellType.Clear, isHuman);
                }
            }

            //Hide old cells in low fog of war
            foreach (var position in cellsToHideInFog)
            {
                SetFogOfWarCellType(position, fogOfWarObject, FogOfWarCellType.Low, isHuman);
            }

            return;
        }

        private List<Vector3Int> CollectFogCellsAround(Vector2Int center, PlayableTerrain terrain, int radius)
        {
            var position = center;
            int waveStep = 0;
            Vector3Int startingTilePosition = new Vector3Int((int)(position.x), (int)(position.y));
            Vector3Int[] positions =
            {
                new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, 0, 0)
            };
            List<Vector3Int> _edgeTiles = new List<Vector3Int>();
            List<Vector3Int> _resultCells = new List<Vector3Int>();

            _edgeTiles.Add(startingTilePosition);
            while (waveStep < radius)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector3Int neighbourTilePosition = edgeTilePosition + positions[i];

                        if (_edgeTiles.Contains(neighbourTilePosition))
                            continue;
                        if (neighbourTilePosition.x >= terrain.Size.x ||
                            neighbourTilePosition.y >= terrain.Size.y)
                            continue;
                        if (neighbourTilePosition.x < 0 || neighbourTilePosition.y < 0)
                            continue;
                        if (terrain.ObstaclesHighTilemap
                                .GetTile(neighbourTilePosition) == null)
                            _edgeTiles.Add(neighbourTilePosition);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                _resultCells.AddRange(_edgeTiles);

                waveStep++;
            }

            return _resultCells;
        }

        public void ClearCellForPlayer(Vector3Int position, PlayableTerrain terrain, Player player)
        {
            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            SetFogOfWarCellType(position, fogOfWarObject, FogOfWarCellType.Clear, player.isHuman);
            var fog = player.OpenedFogDictionary[terrain];
            if (!fog.Contains(position))
                fog.Add(position);
        }

        public void UpdateTilemapForTerrain(PlayableTerrain playableTerrain)
        {
            var fogOfWarObject = GetFogOfWarObject(playableTerrain.UniqueId);
            var fogMap = fogOfWarObject.FogMap;
            var fogMapSize = fogOfWarObject.Size;
            for (int i = 0; i < fogMapSize.x; i++)
            for (int j = 0; j < fogMapSize.y; j++)
            {
                Tile tile = GetTileByFogType((FogOfWarCellType)fogMap[i, j]);
                var position = new Vector3Int(i, j, 0);
                _tilemap.SetTile(position, tile);
            }
        }

        private Tile GetTileByFogType(FogOfWarCellType fogOfWarCellType)
        {
            Tile tile = null;
            switch (fogOfWarCellType)
            {
                case FogOfWarCellType.Clear:
                    tile = _fogOfWarClearTile;
                    break;
                case FogOfWarCellType.Low:
                    tile = _fogOfWarLowTile;
                    break;
                case FogOfWarCellType.Deep:
                    tile = _fogOfWarDeepTile;
                    break;
            }

            return tile;
        }
        
        public void SetFogOfWarCellTypeSingle(Vector3Int position, PlayableTerrain terrain,
            FogOfWarCellType fogOfWarCellType, bool updateTilemap = true)
        {
            var fogOfWarObject = GetFogOfWarObject(terrain.UniqueId);
            fogOfWarObject.SetCell(new Vector2Int(position.x, position.y), fogOfWarCellType);
            if (!updateTilemap) return;
            var tile = GetTileByFogType(fogOfWarCellType);
            _tilemap.SetTile(position, tile);
        }

        private void SetFogOfWarCellType(Vector3Int position, FogOfWarObject fogOfWarObject,
            FogOfWarCellType fogOfWarCellType, bool updateTilemap = true)
        {
            fogOfWarObject.SetCell(new Vector2Int(position.x, position.y), fogOfWarCellType);
            if (!updateTilemap) return;
            var tile = GetTileByFogType(fogOfWarCellType);
            _tilemap.SetTile(position, tile);
        }
    }
}