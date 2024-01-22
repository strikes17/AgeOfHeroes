using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using AStar;
using AStar.Options;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

namespace AgeOfHeroes
{
    public class NavigationMap
    {
        public short[,] navigationsMap => _navigationsMap;
        private short[,] _navigationsMap;

        public WorldGrid worldGrid => _worldGrid;
        private WorldGrid _worldGrid;

        private Dictionary<int, PathFinder> allPathFinders = new Dictionary<int, PathFinder>();

        private PlayableTerrain _playableTerrain;


        public void Init(PlayableTerrain playableTerrain)
        {
            _playableTerrain = playableTerrain;

            var mapSize = _playableTerrain.Size;
            _navigationsMap = new short[mapSize.x, mapSize.y];
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = mapSize.y - 1; j >= 0; j--)
                {
                    _navigationsMap[i, j] = 1;
                }
            }

            _worldGrid = new WorldGrid(_navigationsMap);
        }

        public bool IfCellBlocked(int x, int y)
        {
            var mapSize = _playableTerrain.Size;
            if (x < 0 || y < 0 || x >= mapSize.x || y >= mapSize.y)
                return true;
            if (_navigationsMap[x, y] == 1)
                return false;
            return true;
        }

        public void SetCellFree(int x, int y)
        {
            var mapSize = _playableTerrain.Size;
            if (x < 0 || y < 0 || x >= mapSize.x || y >= mapSize.y)
                return;
            _navigationsMap[x, y] = 1;
            UpdateWorldGrid();
        }

        public void BlockCell(int x, int y)
        {
            _navigationsMap[x, y] = 0;
            UpdateWorldGrid();
        }

        private PathFinder GetPathFinder(int hashCode)
        {
            PathFinder pathFinder = null;
            if (allPathFinders.TryGetValue(hashCode, out pathFinder))
            {
                return pathFinder;
            }

            var pathfinderOptions = new PathFinderOptions()
            {
                PunishChangeDirection = false,
                UseDiagonals = false
            };
            pathFinder = new PathFinder(worldGrid, pathfinderOptions);
            allPathFinders.TryAdd(hashCode, pathFinder);
            return pathFinder;
        }

        public List<Vector2Int> FindPath(int hashCode, Vector2Int start, Vector2Int end)
        {
            _worldGrid[start.y, start.x] = 1;
            _worldGrid[end.y, end.x] = 1;
            var pathFinder = GetPathFinder(hashCode);
            Point startPoint = new Point();
            startPoint.X = start.x;
            startPoint.Y = start.y;

            Point endPoint = new Point();
            endPoint.X = end.x;
            endPoint.Y = end.y;

            var path = pathFinder.FindPath(startPoint, endPoint).ToList();
            List<Vector2Int> foundPath = new List<Vector2Int>();
            path.ForEach(x => foundPath.Add(new Vector2Int(x.X, x.Y)));
            _worldGrid[start.y, start.x] = 0;
            _worldGrid[end.y, end.x] = 0;
            return foundPath;
        }

        private void GenerateNavigationsPreview(Vector2Int mapSize)
        {
            Texture2D texture2D = new Texture2D(mapSize.x, mapSize.y);
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    Color pixelColor = _navigationsMap[i, j] == 1 ? Color.black : Color.red;
                    texture2D.SetPixel(i, j, pixelColor);
                }
            }

            texture2D.Apply();

            var png = texture2D.EncodeToPNG();
            File.WriteAllBytes($"{Application.dataPath}/Resources/nav.png", png);
        }

        public void GenerateNavigationsForTerrain(List<(int, int)> staticCollisions)
        {
            foreach (var tile in _playableTerrain.Water.TerrainTiles.Keys)
            {
                var overlayedByGround =
                    _playableTerrain.GroundTilemap.GetTile(new Vector3Int(tile.x, tile.y)) != null;
                if (!overlayedByGround)
                {
                    _navigationsMap[tile.x, tile.y] = 0;
                }
            }

            foreach (var tile in _playableTerrain.ObstaclesLow.TerrainTiles.Keys)
            {
                _navigationsMap[tile.x, tile.y] = 0;
            }

            foreach (var tile in _playableTerrain.Trees.TerrainTiles.Keys)
            {
                _navigationsMap[tile.x, tile.y] = 0;
            }

            foreach (var tile in _playableTerrain.ObstaclesHigh.TerrainTiles.Keys)
            {
                _navigationsMap[tile.x, tile.y] = 0;
            }

            // foreach (var character in _playableTerrain.TerrainTiles)
            // {
            //     _navigationsMap[character.positionX, character.positionY] = 0;
            // }

            foreach (var collision in staticCollisions)
            {
                _navigationsMap[collision.Item1, collision.Item2] = 0;
            }

            UpdateWorldGrid();
        }

        public void UpdateNavigationsMapForCharacter(ControllableCharacter character)
        {
            if (!character.isExistingInWorld)
            {
                navigationsMap[character.Position.x, character.Position.y] = 1;
                _worldGrid[character.Position.y, character.Position.x] = 1;
                return;
            }

            var prevPosition = character.PreviousPosition;
            var currentPosition = character.Position;

            if (prevPosition.x < _playableTerrain.Size.x && prevPosition.y < _playableTerrain.Size.y)
                navigationsMap[prevPosition.x, prevPosition.y] = 1;
            if (currentPosition.x < _playableTerrain.Size.x && currentPosition.y < _playableTerrain.Size.y)
                navigationsMap[currentPosition.x, currentPosition.y] = 0;

            UpdateWorldGrid();
        }

        private void UpdateWorldGrid()
        {
            var mapSize = _playableTerrain.Size;
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    _worldGrid[i, j] = _navigationsMap[j, i];
                }
            }
        }


        private void Update()
        {
            var mapSize = _playableTerrain.Size;
            if (Input.GetKeyDown(KeyCode.G))
            {
                for (int i = 0; i < mapSize.x; i++)
                for (int j = 0; j < mapSize.y; j++)
                    GlobalVariables.__SpawnMarker(new Vector2Int(i, j), Color.red, false, 3f);
            }
        }

        public ControllableCharacter GetRelativeCharacterAtPosition(ControllableCharacter source, Vector2Int position,
            long mask)
        {
            var characterLayerMask = 1 << LayerMask.NameToLayer("Character");
            var overlapPoint =
                Physics2D.OverlapPoint(new Vector2(position.x, position.y),
                    characterLayerMask);
            if (overlapPoint != null)
            {
                var character = overlapPoint.GetComponent<ControllableCharacter>();
                if (character == source) return null;
                bool isAlly = source.playerOwnerColor == character.playerOwnerColor;
                bool lookForAlly = (mask & (long)MagicSpellAllowedTarget.Ally) != 0;
                bool lookForEnemy = (mask & (long)MagicSpellAllowedTarget.Enemy) != 0;
                if ((isAlly && lookForAlly) || (!isAlly && lookForEnemy))
                    return character;
            }

            return null;
        }

        public ControllableCharacter GetCharacterAtPosition(Vector2Int position)
        {
            var characterLayerMask = 1 << LayerMask.NameToLayer("Character");
            var overlapPoint =
                Physics2D.OverlapPoint(new Vector2(position.x, position.y),
                    characterLayerMask);
            if (overlapPoint != null)
            {
                var character = overlapPoint.GetComponent<ControllableCharacter>();
                return character;
            }

            return null;
        }

        public T GetObjectOfType<T>(Vector2Int position, int mask)
        {
            T result;
            int layerMask = 1 << mask;
            var overlapForFilteredObjects = Physics2D.OverlapPoint(position, layerMask);
            if (overlapForFilteredObjects == null) return default(T);

            var target = overlapForFilteredObjects.GetComponent<T>();
            return target;
        }

        public List<T> GetAllObjectsOfType<T>(Vector2Int position, int range, int mask)
        {
            int waveStep = 0;
            List<Vector2Int> _edgeTiles = new List<Vector2Int>();
            List<T> results = new List<T>();
            Vector2Int startingTilePosition = new Vector2Int((int)(position.x), (int)(position.y));
            Vector2Int[] positions = GlobalVariables.CrossNeighbours;
            _edgeTiles.Add(startingTilePosition);
            int layerMask = 1 << mask;
            while (waveStep < range)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector2Int ntpos = edgeTilePosition + positions[i];
                        Vector3Int nt3Pos = new Vector3Int(ntpos.x, ntpos.y, 0);
                        _edgeTiles.Add(ntpos);
                        var overlapForFilteredObjects = Physics2D.OverlapPoint(ntpos, layerMask);
                        if (overlapForFilteredObjects == null) continue;
                        var target = overlapForFilteredObjects.GetComponent<T>();
                        if (target != null)
                            results.Add(target);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                results = results.Distinct().ToList();
                waveStep++;
            }

            return results;
        }

        public List<Vector2Int> GetCellsByFilterInRangeExcludingEmpties(Vector2Int position, int range, int mask)
        {
            int waveStep = 0;
            List<Vector2Int> _edgeTiles = new List<Vector2Int>();
            List<Vector2Int> _resultCells = new List<Vector2Int>();
            Vector2Int startingTilePosition = new Vector2Int((int)(position.x), (int)(position.y));
            Vector2Int[] positions = GlobalVariables.CrossNeighbours;
            _edgeTiles.Add(startingTilePosition);
            int layerMask = 1 << mask;
            while (waveStep < range)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector2Int ntpos = edgeTilePosition + positions[i];
                        Vector3Int nt3Pos = new Vector3Int(ntpos.x, ntpos.y, 0);
                        _edgeTiles.Add(ntpos);
                        var overlapForFilteredObjects =
                            Physics2D.OverlapPoint(ntpos,
                                layerMask);
                        if (overlapForFilteredObjects == null) continue;

                        _resultCells.Add(ntpos);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                _resultCells = _resultCells.Distinct().ToList();
                waveStep++;
            }

            return _resultCells;
        }


        public List<Vector2Int> GetPositionsByFilterInRange(Vector2Int position, int range, int mask)
        {
            int waveStep = 0;
            List<Vector2Int> _edgeTiles = new List<Vector2Int>();
            List<Vector2Int> _resultCells = new List<Vector2Int>();
            Vector2Int startingTilePosition = new Vector2Int((int)(position.x), (int)(position.y));
            Vector2Int[] positions =
            {
                new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0)
            };
            _edgeTiles.Add(startingTilePosition);
            int layerMask = 1 << mask;
            while (waveStep < range)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector2Int ntpos = edgeTilePosition + positions[i];
                        Vector3Int nt3Pos = new Vector3Int(ntpos.x, ntpos.y, 0);
                        var nextLowObstacleTile = _playableTerrain.ObstaclesLowTilemap.GetTile(nt3Pos);
                        var nextTreeTile = _playableTerrain.TreesTilemap.GetTile(nt3Pos);
                        var nextWaterTile = _playableTerrain.WaterTilemap.GetTile(nt3Pos);
                        var nextGroundTile = _playableTerrain.GroundTilemap.GetTile(nt3Pos);
                        var navigationMap = _playableTerrain.TerrainNavigator.NavigationMap;

                        _edgeTiles.Add(ntpos);

                        if (ntpos == startingTilePosition)
                            continue;

                        if (nextGroundTile == null)
                            continue;

                        if (nextWaterTile != null)
                        {
                            if (nextGroundTile == null)
                                continue;
                        }

                        if (nextLowObstacleTile != null)
                            continue;

                        if (nextTreeTile != null)
                            continue;

                        var overlapForFilteredObjects =
                            Physics2D.OverlapPoint(ntpos,
                                layerMask);
                        if (overlapForFilteredObjects != null)
                        {
                            continue;
                        }

                        _resultCells.Add(ntpos);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                _resultCells = _resultCells.Distinct().ToList();
                waveStep++;
            }

            return _resultCells;
        }

        public List<Vector2Int> GetFreeCellsInRange(Vector2Int position, int range)
        {
            int waveStep = 0;
            List<Vector2Int> _edgeTiles = new List<Vector2Int>();
            List<Vector2Int> _resultCells = new List<Vector2Int>();
            Vector2Int startingTilePosition = new Vector2Int((int)(position.x), (int)(position.y));
            Vector2Int[] positions =
            {
                new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0)
            };
            _edgeTiles.Add(startingTilePosition);
            int acLayerMask = 1 << LayerMask.NameToLayer("ActionCell");
            int characterLayerMask = 1 << LayerMask.NameToLayer("Character");
            while (waveStep < range)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector2Int ntpos = edgeTilePosition + positions[i];
                        Vector3Int nt3Pos = new Vector3Int(ntpos.x, ntpos.y, 0);
                        var nextLowObstacleTile = _playableTerrain.ObstaclesLowTilemap.GetTile(nt3Pos);
                        var nextWaterTile = _playableTerrain.WaterTilemap.GetTile(nt3Pos);
                        var nextGroundTile = _playableTerrain.GroundTilemap.GetTile(nt3Pos);
                        var navigationMap = _playableTerrain.TerrainNavigator.NavigationMap;
                        var nextTreeTile = _playableTerrain.TreesTilemap.GetTile(nt3Pos);

                        _edgeTiles.Add(ntpos);
                        if (navigationMap.IfCellBlocked(ntpos.x, ntpos.y))
                        {
                            continue;
                        }

                        if (ntpos == startingTilePosition)
                            continue;

                        if (nextGroundTile == null)
                            continue;

                        if (nextWaterTile != null)
                        {
                            if (nextGroundTile == null)
                                continue;
                        }

                        if (nextLowObstacleTile != null)
                            continue;
                        if (nextTreeTile != null)
                            continue;

                        var overlapForActionCell =
                            Physics2D.OverlapPoint(ntpos,
                                acLayerMask);
                        if (overlapForActionCell != null)
                        {
                            continue;
                        }

                        var neighbourCharacter =
                            Physics2D.OverlapPoint(ntpos,
                                characterLayerMask);
                        if (neighbourCharacter != null)
                        {
                            continue;
                        }

                        // if (_edgeTiles.Contains(ntpos))
                        //     continue;

                        _resultCells.Add(ntpos);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                _resultCells = _resultCells.Distinct().ToList();
                waveStep++;
            }

            return _resultCells;
        }

        public List<ControllableCharacter> GetAllCharactersInRange(Vector2 position, Player sourcePlayer, int range,
            long mask, bool onlyVisibles)
        {
            var color = sourcePlayer.Color;
            var gameManager = GameManager.Instance;
            bool collectAllies = ((mask & (long)MagicSpellAllowedTarget.Ally) != 0);
            bool collectEnemies = ((mask & (long)MagicSpellAllowedTarget.Enemy) != 0);
            var players = gameManager.MapScenarioHandler.players;
            List<ControllableCharacter> allCharacters = new List<ControllableCharacter>();
            foreach (var playerColor in players.Keys)
            {
                if ((playerColor == color && collectAllies) ||
                    (playerColor != color && collectEnemies))
                    allCharacters.AddRange(players[playerColor].controlledCharacters);
            }

            List<ControllableCharacter> suitableCharacters = new List<ControllableCharacter>();

            foreach (var character in allCharacters)
            {
                if (!character.gameObject.activeSelf || !character.isExistingInWorld)
                    continue;
                bool maskMatch = (mask & character.persona) != 0;
                // if (!maskMatch)
                // {
                //     continue;
                // }

                var v3pos = ControllableCharacter.V3Int(position);
                var enemyV3Pos = ControllableCharacter.V3Int(character.transform.position);
                var distance = Mathf.CeilToInt(Vector3Int.Distance(v3pos, enemyV3Pos));
                bool isVisible = character.IsVisibleForPlayer(sourcePlayer);
                bool isInRange = distance <= range;
                bool isHero = (character.persona & (long)MagicSpellAllowedTarget.Hero) != 0;
                bool affectsHeroes = (mask & (long)MagicSpellAllowedTarget.Hero) != 0;
                if (isHero)
                {
                    if (!affectsHeroes) continue;
                }

                if (onlyVisibles)
                {
                    if (isVisible && isInRange)
                        suitableCharacters.Add(character);
                }
                else
                {
                    if (isInRange)
                        suitableCharacters.Add(character);
                }
            }

            return suitableCharacters;
        }
    }
}