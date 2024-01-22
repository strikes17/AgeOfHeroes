using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorWorldGridManager : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private Material linesMaterial;
        [SerializeField] private MapEditorCharacter _mapEditorCharacterPrefab;
        [SerializeField] private MapEditorHero _mapEditorHeroPrefab;
        [SerializeField] private MapEditorCastle _mapEditorCastlePrefab;
        [SerializeField] private MapEditorDwelling _mapEditorDwellingPrefab;
        [SerializeField] private MapEditorSprite _failEditorSpritePrefab;
        [SerializeField] private MapEditorTreasure _mapEditorTreasurePrefab;
        [SerializeField] private MapEditorArtifact _mapEditorArtifactPrefab;
        [SerializeField] private MapEditorAIMarker _mapEditorAIMarkerPrefab;

        [SerializeField] private Tilemap _groundTilemap,
            _waterTilemap,
            _backgroundTilemap,
            _roadsTilemap,
            _obstaclesLow,
            _obstaclesHigh,
            _toolTilemap,
            _treesTilemap,
            _groundOverlayTilemap;

        [SerializeField] private TerrainTileObject defaultTerrain, _deepWaterTerrain;
        private List<Tilemap> allTilemaps = new List<Tilemap>();
        private Transform _gridLinesRoot;
        private Transform _mapCharactersRoot;
        private Camera _camera;

        private SerializableTerrainTile[,] _groundTileMapMatrix,
            _waterTileMapMatrix,
            _backgroundTileMapMatrix,
            _obstaclesLowTileMapMatrix,
            _obstaclesHighTileMapMatrix,
            _toolTileMapMatrix,
            _treesTileMapMatrix,
            _groundOverlayMapMatrix;

        private float paintBrushDelay = 0.2f, paintBrushUpdateTime = 0f;
        public SerializableTerrainTile[,] toolTileMapMatrix => _toolTileMapMatrix;

        public SerializableTerrainTile[,] groundTileMapMatrix => _groundTileMapMatrix;

        public SerializableTerrainTile[,] waterTileMapMatrix => _waterTileMapMatrix;

        public SerializableTerrainTile[,] backgroundTileMapMatrix => _backgroundTileMapMatrix;

        public SerializableTerrainTile[,] obstaclesLowTileMapMatrix => _obstaclesLowTileMapMatrix;

        public SerializableTerrainTile[,] obstaclesHighTileMapMatrix => _obstaclesHighTileMapMatrix;

        public SerializableTerrainTile[,] treesTileMapMatrix => _treesTileMapMatrix;

        public SerializableTerrainTile[,] groundOverlayTileMapMatrix => _groundOverlayMapMatrix;


        public Dictionary<Vector3Int, bool> collisions => _collisions;

        public Dictionary<Vector3Int, MapEditorEntity> castlesDictionary => _mapEditorEntities[typeof(MapEditorCastle)];

        public Dictionary<Vector3Int, MapEditorEntity> charactersDictionary =>
            _mapEditorEntities[typeof(MapEditorCharacter)];

        public Dictionary<Vector3Int, MapEditorEntity> heroesDictionary => _mapEditorEntities[typeof(MapEditorHero)];

        public Dictionary<Vector3Int, MapEditorEntity> treasuresDictionary =>
            _mapEditorEntities[typeof(MapEditorTreasure)];

        public Dictionary<Vector3Int, MapEditorEntity> artifactsDictionary =>
            _mapEditorEntities[typeof(MapEditorArtifact)];

        public Dictionary<Vector3Int, MapEditorEntity> markersDictionary =>
            _mapEditorEntities[typeof(MapEditorAIMarker)];

        public Dictionary<Vector3Int, MapEditorEntity> dwellingsDictionary =>
            _mapEditorEntities[typeof(MapEditorDwelling)];

        public Tilemap toolTilemap => _toolTilemap;

        public Tilemap groundTilemap => _groundTilemap;

        public Tilemap waterTilemap => _waterTilemap;

        public Tilemap obstaclesLow => _obstaclesLow;

        public Tilemap obstaclesHigh => _obstaclesHigh;

        public Tilemap groundOverlayTilemap => _groundOverlayTilemap;

        public Tilemap trees => _treesTilemap;

        private Dictionary<Vector3Int, bool> _collisions = new Dictionary<Vector3Int, bool>();

        public Dictionary<Type, Dictionary<Vector3Int, MapEditorEntity>> _mapEditorEntities;

        private int startIndex = 0;

        private void Start()
        {
            _mapEditorEntities = new Dictionary<Type, Dictionary<Vector3Int, MapEditorEntity>>()
            {
                { typeof(MapEditorCastle), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorCharacter), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorHero), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorTreasure), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorArtifact), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorAIMarker), new Dictionary<Vector3Int, MapEditorEntity>() },
                { typeof(MapEditorDwelling), new Dictionary<Vector3Int, MapEditorEntity>() }
            };
            _mapCharactersRoot = new GameObject("map_characters").transform;
            _camera = MapEditorManager.Instance.MainCamera.Camera;
            allTilemaps.Add(_toolTilemap);
            allTilemaps.Add(_groundTilemap);
            allTilemaps.Add(_waterTilemap);
            allTilemaps.Add(_backgroundTilemap);
            allTilemaps.Add(_roadsTilemap);
            allTilemaps.Add(_obstaclesLow);
            allTilemaps.Add(_obstaclesHigh);
            allTilemaps.Add(_treesTilemap);
            allTilemaps.Add(_groundOverlayTilemap);
        }

        public void GenerateMap(Vector2Int size)
        {
            for (int i = startIndex; i < mapSize.x; i++)
            {
                for (int j = startIndex; j < mapSize.y; j++)
                {
                    SetTile(_deepWaterTerrain, 0, new Vector3Int(i, j));
                    // SetTile(defaultTerrain, (int) TileSide.CENTER, new Vector3Int(i, j));
                }
            }
        }

        public void CreateNewMap(Vector2Int size)
        {
            ClearOpenedMap();
            GenerateMatrices(size);
            mapSize = size;
            CreateGrid();
            GenerateMap(size);
            MapEditorManager.Instance.MainCamera.SetBounds(size);
        }

        private void ClearOpenedMap()
        {
            MapEditorCharacter character = null;
            var types = _mapEditorEntities.Keys;
            foreach (var type in types)
            {
                var values = _mapEditorEntities[type].Values;
                foreach (var value in values)
                {
                    Destroy(value.gameObject);
                }
            }

            for (int i = startIndex; i < mapSize.x; i++)
            for (int j = startIndex; j < mapSize.y; j++)
            {
                foreach (var tilemap in allTilemaps)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }

            _collisions.Clear();
        }

        private void GenerateMatrices(Vector2Int size)
        {
            _toolTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _groundTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _waterTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _backgroundTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _obstaclesLowTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _obstaclesHighTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _treesTileMapMatrix = new SerializableTerrainTile[size.x, size.y];
            _groundOverlayMapMatrix = new SerializableTerrainTile[size.x, size.y];
        }

        public void LoadMap(SerializableMap serializableMap)
        {
            var mainTerrain = serializableMap.MainTerrain;
            Vector2Int sizeOfNewMap = mainTerrain.Size;
            ClearOpenedMap();
            mapSize = sizeOfNewMap;
            GenerateMatrices(mapSize);
            CreateGrid();
            MapEditorManager.Instance.MainCamera.SetBounds(mapSize);
            LoadTileMap(serializableMap.Background);
            LoadTileMap(serializableMap.Ground);
            LoadTileMap(serializableMap.Water);
            LoadTileMap(serializableMap.ObstaclesLow);
            LoadTileMap(serializableMap.ObstaclesHigh);
            LoadTileMap(serializableMap.Roads);
            LoadTileMap(serializableMap.Tool);
            LoadTileMap(serializableMap.Trees);
            LoadTileMap(serializableMap.GroundOverlay);

            var serializableCharacters = mainTerrain.SerializableCharacters;
            foreach (var serializableCharacter in serializableCharacters)
            {
                LoadMapCharacter(serializableCharacter);
            }

            var serializableCastles = mainTerrain.SerializableCastles;
            foreach (var serializableCastle in serializableCastles)
            {
                LoadMapCastle(serializableCastle);
            }

            var serializableHeroes = mainTerrain.SerializableHeroes;
            foreach (var serializableHero in serializableHeroes)
            {
                LoadMapHero(serializableHero);
            }

            var serializableTreasures = mainTerrain.SerializableTreasures;
            foreach (var serializableTreasure in serializableTreasures)
            {
                LoadMapTreasure(serializableTreasure);
            }

            var serializableArtifacts = mainTerrain.SerializableArtifacts;
            foreach (var serializableArtifact in serializableArtifacts)
            {
                LoadMapArtifact(serializableArtifact);
            }

            var serializableMarkers = mainTerrain.SerializableMarkers;
            foreach (var serializableMarker in serializableMarkers)
            {
                LoadMapMarker(serializableMarker);
            }
            
            var serializableDwellings = mainTerrain.SerializableDwellings;
            Debug.Log(serializableDwellings.Count);
            foreach (var serializableDwelling in serializableDwellings)
            {
                LoadMapDwelling(serializableDwelling);
            }
        }

        private void LoadTileMap(SerializableTileMap serializableTileMap)
        {
            if (serializableTileMap == null) return;
            foreach (var tile in serializableTileMap.SerializableTerrainTiles)
            {
                if (tile == null) continue;
                var terrainTileObject = ResourcesBase.GetTerrainTileObject(tile.TerrainTileObjectName);
                if (terrainTileObject == null) continue;
                var position = new Vector3Int(tile.positionX, tile.positionY, 0);
                SetTile(terrainTileObject, tile.Index, position);
            }
        }

        private void Update()
        {
            OnLeftMouseDown();
            OnRightMouseDown();

            if (Input.GetMouseButton(0))
            {
                if (!IsPaintingBrushRequested())
                    return;
                if (MapEditorManager.Instance.GUIRaycaster.HasAnyUI())
                    return;
                var inputWorldPosition = InputWorldPositionRounded();
                if (!IsInputPositionValid(inputWorldPosition))
                    return;
                UseCreationToolAtPosition(inputWorldPosition);
                return;
            }

            if (Input.GetMouseButton(1))
            {
                if (!IsPaintingBrushRequested())
                    return;
                if (MapEditorManager.Instance.GUIRaycaster.HasAnyUI())
                    return;
                var inputWorldPosition = InputWorldPositionRounded();
                if (!IsInputPositionValid(inputWorldPosition))
                    return;
                UseDestructionToolAtPosition(inputWorldPosition);
                return;
            }

            paintBrushUpdateTime = 0f;
        }

        private void OnLeftMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (MapEditorManager.Instance.GUIRaycaster.HasAnyUI())
                    return;
                var inputWorldPosition = InputWorldPositionRounded();
                if (!IsInputPositionValid(inputWorldPosition))
                    return;
                UseCreationToolAtPosition(inputWorldPosition);
            }
        }

        private void OnRightMouseDown()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (MapEditorManager.Instance.GUIRaycaster.HasAnyUI())
                    return;
                var inputWorldPosition = InputWorldPositionRounded();
                if (!IsInputPositionValid(inputWorldPosition))
                    return;
                UseDestructionToolAtPosition(inputWorldPosition);
            }
        }

        private (Tilemap, SerializableTerrainTile[,]) GetTerrain(TerrainTileMapType terrainTileMapType)
        {
            Tilemap targetTileMap = null;
            SerializableTerrainTile[,] targetTileMapMatrix = null;
            switch (terrainTileMapType)
            {
                case TerrainTileMapType.Ground:
                    targetTileMap = _groundTilemap;
                    targetTileMapMatrix = _groundTileMapMatrix;
                    break;
                case TerrainTileMapType.WaterShallow:
                    targetTileMap = _waterTilemap;
                    targetTileMapMatrix = _waterTileMapMatrix;
                    break;
                case TerrainTileMapType.WaterDeep:
                    targetTileMap = _waterTilemap;
                    targetTileMapMatrix = _waterTileMapMatrix;
                    break;
                case TerrainTileMapType.ObstacleLow:
                    targetTileMap = _obstaclesLow;
                    targetTileMapMatrix = _obstaclesLowTileMapMatrix;
                    break;
                case TerrainTileMapType.ObstacleHigh:
                    targetTileMap = _obstaclesHigh;
                    targetTileMapMatrix = _obstaclesHighTileMapMatrix;
                    break;
                case TerrainTileMapType.Tool:
                    targetTileMap = _toolTilemap;
                    targetTileMapMatrix = _toolTileMapMatrix;
                    break;
                case TerrainTileMapType.Trees:
                    targetTileMap = _treesTilemap;
                    targetTileMapMatrix = _treesTileMapMatrix;
                    break;
                case TerrainTileMapType.GroundOverlay:
                    targetTileMap = _groundOverlayTilemap;
                    targetTileMapMatrix = _groundOverlayMapMatrix;
                    break;
            }

            return (targetTileMap, targetTileMapMatrix);
        }

        #region SetTileAdvanced REGION

        public void SetTileAdvanced(TerrainTileObject terrainTileObject, Vector3Int position)
        {
            if (terrainTileObject == null)
                return;
            var tileMaptype = terrainTileObject.TileMapType;
            Tilemap targetTileMap = null;
            SerializableTerrainTile[,] targetTileMapMatrix = null;
            var terrain = GetTerrain(terrainTileObject.TileMapType);
            targetTileMap = terrain.Item1;
            targetTileMapMatrix = terrain.Item2;

            var crossNeighbours = GlobalVariables.QuadNeighbours;
            List<Vector2Int> neighbours = new List<Vector2Int>();
            Dictionary<TileDirection, TileState> tileStates = new Dictionary<TileDirection, TileState>();
            for (int i = 0; i < crossNeighbours.Length; i++)
            {
                var neighbour = crossNeighbours[i];
                var neighbourTilePosition = new Vector3Int(neighbour.x, neighbour.y) + position;
                var neighbourTile = targetTileMapMatrix[neighbourTilePosition.x, neighbourTilePosition.y];
                TileState targetTileState;
                if (neighbourTile == null)
                {
                    targetTileState = TileState.Empty;
                }
                else
                {
                    targetTileState = TileState.Filled;
                }

                tileStates.TryAdd((TileDirection)(1 << i), targetTileState);
            }

            TileSide tileSide = TileSide.NONE;
            var tileRules = GlobalVariables.TileRules;
            int directionMask = 0;
            TileDirection direction;
            for (int i = 0; i < tileStates.Count; i++)
            {
                int dir = (int)tileStates[(TileDirection)(1 << i)];
                directionMask |= dir;
            }

            direction = (TileDirection)directionMask;
            (TileCondition, TileState) tt;
            foreach (var tileRule in tileRules)
            {
                tileRule.Key.Rules.TryGetValue(direction, out tt);
            }
        }

        private TileDirection GetNeighbourDirection(Vector2Int sourceTile, Vector2Int neighbourTile)
        {
            int directionX = 0;
            int directionY = 0;
            if (neighbourTile.x > sourceTile.x)
            {
                directionX = 1;
            }
            else if (neighbourTile.x < sourceTile.x)
            {
                directionX = -1;
            }

            if (neighbourTile.y > sourceTile.y)
            {
                directionY = 1;
            }
            else if (neighbourTile.y < sourceTile.y)
            {
                directionY = -1;
            }

            if (directionX == 1)
            {
                switch (directionY)
                {
                    case -1:
                        return TileDirection.RIGHT_BOT;
                    case 0:
                        return TileDirection.RIGHT;
                    case 1:
                        return TileDirection.RIGHT_TOP;
                }
            }
            else if (directionX == 0)
            {
                switch (directionY)
                {
                    case -1:
                        return TileDirection.BOT;
                    case 0:
                        return TileDirection.CENTER;
                    case 1:
                        return TileDirection.TOP;
                }
            }
            else if (directionX == -1)
            {
                switch (directionY)
                {
                    case -1:
                        return TileDirection.LEFT_BOT;
                    case 0:
                        return TileDirection.LEFT;
                    case 1:
                        return TileDirection.LEFT_TOP;
                }
            }

            return TileDirection.CENTER;
        }

        #endregion

        public void SetTilePatch(TerrainTileObject terrainTileObject, Vector3Int position)
        {
            if (terrainTileObject == null)
                return;
            var quadNeighbours = GlobalVariables.QuadNeighbours;
            var tileMaptype = terrainTileObject.TileMapType;
            Tilemap targetTileMap = null;
            SerializableTerrainTile[,] targetTileMapMatrix = null;
            switch (tileMaptype)
            {
                case TerrainTileMapType.Ground:
                    targetTileMap = _groundTilemap;
                    targetTileMapMatrix = _groundTileMapMatrix;
                    break;
                case TerrainTileMapType.WaterShallow:
                    targetTileMap = _waterTilemap;
                    targetTileMapMatrix = _waterTileMapMatrix;
                    break;
                case TerrainTileMapType.WaterDeep:
                    targetTileMap = _waterTilemap;
                    targetTileMapMatrix = _waterTileMapMatrix;
                    break;
                case TerrainTileMapType.ObstacleLow:
                    targetTileMap = _obstaclesLow;
                    targetTileMapMatrix = _obstaclesLowTileMapMatrix;
                    break;
                case TerrainTileMapType.ObstacleHigh:
                    targetTileMap = _obstaclesHigh;
                    targetTileMapMatrix = _obstaclesHighTileMapMatrix;
                    break;
            }

            TileSide tileSide = TileSide.NONE;
            Dictionary<(TileSide, TileSide), TileSide> rules = new Dictionary<(TileSide, TileSide), TileSide>()
            {
                { (TileSide.CENTER_TOP, TileSide.CENTER_TOP), TileSide.CENTER_TOP },
                { (TileSide.CENTER_TOP, TileSide.RIGHT_TOP), TileSide.CENTER_TOP },
                { (TileSide.CENTER_TOP, TileSide.RIGHT_CENTER), TileSide.TR_CORNER },
                { (TileSide.CENTER_TOP, TileSide.RIGHT_BOT), TileSide.TR_CORNER },
                { (TileSide.CENTER_TOP, TileSide.CENTER_BOT), TileSide.CENTER },
                { (TileSide.CENTER_TOP, TileSide.LEFT_BOT), TileSide.TL_CORNER },
                { (TileSide.CENTER_TOP, TileSide.LEFT_CENTER), TileSide.TL_CORNER },
                { (TileSide.CENTER_TOP, TileSide.LEFT_TOP), TileSide.CENTER_TOP },
                { (TileSide.CENTER_TOP, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.RIGHT_TOP, TileSide.RIGHT_TOP), TileSide.RIGHT_TOP },
                { (TileSide.RIGHT_TOP, TileSide.RIGHT_CENTER), TileSide.RIGHT_CENTER },
                { (TileSide.RIGHT_TOP, TileSide.RIGHT_BOT), TileSide.RIGHT_CENTER },
                { (TileSide.RIGHT_TOP, TileSide.CENTER_BOT), TileSide.BR_CORNER },
                { (TileSide.RIGHT_TOP, TileSide.LEFT_BOT), TileSide.LT_BR_MERGE },
                { (TileSide.RIGHT_TOP, TileSide.LEFT_CENTER), TileSide.TL_CORNER },
                { (TileSide.RIGHT_TOP, TileSide.LEFT_TOP), TileSide.CENTER_TOP },
                { (TileSide.RIGHT_TOP, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.RIGHT_CENTER, TileSide.RIGHT_CENTER), TileSide.RIGHT_CENTER },
                { (TileSide.RIGHT_CENTER, TileSide.RIGHT_BOT), TileSide.RIGHT_CENTER },
                { (TileSide.RIGHT_CENTER, TileSide.CENTER_BOT), TileSide.BR_CORNER },
                { (TileSide.RIGHT_CENTER, TileSide.LEFT_BOT), TileSide.BR_CORNER },
                { (TileSide.RIGHT_CENTER, TileSide.LEFT_CENTER), TileSide.CENTER },
                { (TileSide.RIGHT_CENTER, TileSide.LEFT_TOP), TileSide.TR_CORNER },
                { (TileSide.RIGHT_CENTER, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.RIGHT_BOT, TileSide.RIGHT_BOT), TileSide.RIGHT_BOT },
                { (TileSide.RIGHT_BOT, TileSide.CENTER_BOT), TileSide.CENTER_BOT },
                { (TileSide.RIGHT_BOT, TileSide.LEFT_BOT), TileSide.CENTER_BOT },
                { (TileSide.RIGHT_BOT, TileSide.LEFT_CENTER), TileSide.BL_CORNER },
                { (TileSide.RIGHT_BOT, TileSide.LEFT_TOP), TileSide.LB_TR_MERGE },
                { (TileSide.RIGHT_BOT, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.CENTER_BOT, TileSide.CENTER_BOT), TileSide.CENTER_BOT },
                { (TileSide.CENTER_BOT, TileSide.LEFT_BOT), TileSide.CENTER_BOT },
                { (TileSide.CENTER_BOT, TileSide.LEFT_CENTER), TileSide.BL_CORNER },
                { (TileSide.CENTER_BOT, TileSide.LEFT_TOP), TileSide.BL_CORNER },
                { (TileSide.CENTER_BOT, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.LEFT_BOT, TileSide.LEFT_BOT), TileSide.LEFT_BOT },
                { (TileSide.LEFT_BOT, TileSide.LEFT_CENTER), TileSide.LEFT_CENTER },
                { (TileSide.LEFT_BOT, TileSide.LEFT_TOP), TileSide.LEFT_CENTER },
                { (TileSide.LEFT_BOT, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.LEFT_CENTER, TileSide.LEFT_CENTER), TileSide.LEFT_CENTER },
                { (TileSide.LEFT_CENTER, TileSide.LEFT_TOP), TileSide.LEFT_CENTER },
                { (TileSide.LEFT_CENTER, TileSide.CENTER), TileSide.CENTER },
                //
                { (TileSide.LEFT_TOP, TileSide.LEFT_TOP), TileSide.LEFT_TOP },
                { (TileSide.LEFT_TOP, TileSide.CENTER), TileSide.CENTER },
            };
            foreach (var neighbour in quadNeighbours)
            {
                tileSide++;
                Vector3Int neighbourPosition = new Vector3Int(neighbour.x, neighbour.y) + position;
                var neighbourTileObject = targetTileMapMatrix[neighbourPosition.x, neighbourPosition.y];
                if (neighbourTileObject == null || neighbourTileObject.TerrainTileObjectName != terrainTileObject.name)
                {
                    SetTile(terrainTileObject, (int)tileSide, neighbourPosition);
                    continue;
                }
                else
                {
                    var neighbourTileSide = (TileSide)neighbourTileObject.Index;
                    TileSide targetTileSide;
                    if (rules.TryGetValue((neighbourTileSide, tileSide), out targetTileSide))
                    {
                        SetTile(terrainTileObject, (int)targetTileSide, neighbourPosition);
                        continue;
                    }
                    else if (rules.TryGetValue((tileSide, neighbourTileSide), out targetTileSide))
                    {
                        SetTile(terrainTileObject, (int)targetTileSide, neighbourPosition);
                        continue;
                    }
                    else
                    {
                        SetTile(terrainTileObject, (int)TileSide.CENTER, neighbourPosition);
                    }
                }
            }

            SetTile(terrainTileObject, (int)TileSide.CENTER, position);
        }

        private void SetTile(TerrainTileObject terrainTileObject, int tileIndex, Vector3Int position)
        {
            if (terrainTileObject == null || tileIndex == -1)
                return;
            var tileMaptype = terrainTileObject.TileMapType;
            var pair = GetTerrain(tileMaptype);
            Tilemap targetTileMap = pair.Item1;
            SerializableTerrainTile[,] targetTileMapMatrix = pair.Item2;
            targetTileMapMatrix[position.x, position.y] =
                new SerializableTerrainTile(position, tileIndex, terrainTileObject.name);
            targetTileMap?.SetTile(position, terrainTileObject.Tiles[tileIndex]);
        }

        private void ClearTile(Vector3Int position)
        {
            var selectedTileObject = MapEditorManager.Instance.SelectedTerrainTileObject;
            if (selectedTileObject == null)
                return;
            var tileMaptype = selectedTileObject.TileMapType;
            var pair = GetTerrain(tileMaptype);
            Tilemap targetTileMap = pair.Item1;
            SerializableTerrainTile[,] targetTileMapMatrix = pair.Item2;
            targetTileMapMatrix[position.x, position.y] = null;
            targetTileMap.SetTile(position, null);
        }

        private bool IsPaintingBrushRequested()
        {
            paintBrushUpdateTime += Time.deltaTime;
            if (paintBrushUpdateTime < paintBrushDelay)
                return false;
            return true;
        }

        private void UseCreationToolAtPosition(Vector3Int position)
        {
            var toolMode = MapEditorManager.Instance.SelectedToolMode;
            MapEditorEntity mapEditorEntity = null;
            switch (toolMode)
            {
                case MapEditorToolMode.Character:
                    if (_mapEditorEntities[typeof(MapEditorCharacter)].TryGetValue(position, out mapEditorEntity))
                    {
                        mapEditorEntity.OnClicked();
                        return;
                    }

                    var characterObject = MapEditorManager.Instance.SelectedCharacterObject;
                    var playerColor = MapEditorManager.Instance.SelectedPlayerColor;
                    if (characterObject != null)
                        CreateMapCharacter(characterObject, position, playerColor, characterObject.fullStackCount);
                    break;
                case MapEditorToolMode.Terrain:
                    var selectedTileIndex = MapEditorManager.Instance.SelectedTerrainTileIndex;
                    var selectedTileObject = MapEditorManager.Instance.SelectedTerrainTileObject;
                    if (selectedTileObject != null)
                        // SetTileAdvanced(selectedTileObject, position);
                        // SetTilePatch(selectedTileObject, position);
                        SetTile(selectedTileObject, selectedTileIndex, position);
                    break;
                case MapEditorToolMode.Structure:
                    var structureType = MapEditorManager.Instance.SelectedStructureType;
                    switch (structureType)
                    {
                        case StructureType.Castle:
                            var selectedCastleObject = MapEditorManager.Instance.SelectedCastleObject;
                            if (selectedCastleObject != null)
                                CreateMapCastle(selectedCastleObject, position,
                                    MapEditorManager.Instance.SelectedPlayerColor);
                            break;
                        case StructureType.Dwell:
                            var selectedDwellBuilding = MapEditorManager.Instance.SelectedDwellBuilding;
                            if (selectedDwellBuilding != null)
                                CreateMapDwelling(selectedDwellBuilding, position,
                                    MapEditorManager.Instance.SelectedPlayerColor);
                            break;
                    }

                    break;
                case MapEditorToolMode.Hero:
                    if (_mapEditorEntities[typeof(MapEditorHero)].TryGetValue(position, out mapEditorEntity))
                    {
                        mapEditorEntity.OnClicked();
                        return;
                    }

                    var heroObject = MapEditorManager.Instance.SelectedHeroObject;
                    var heroPlayerColor = MapEditorManager.Instance.SelectedPlayerColor;
                    if (heroObject != null)
                        CreateMapHero(heroObject, position, heroPlayerColor);
                    break;
                case MapEditorToolMode.Treasure:
                    MapEditorTreasure mapEditorTreasure = null;
                    if (_mapEditorEntities[typeof(MapEditorTreasure)].TryGetValue(position, out mapEditorEntity))
                    {
                        mapEditorEntity.OnClicked();
                        return;
                    }

                    var treasureObject = MapEditorManager.Instance.SelectedTreasureObject;
                    if (treasureObject != null)
                        CreateMapTreasure(treasureObject, position);
                    break;
                case MapEditorToolMode.Artifact:
                    MapEditorArtifact mapEditorArtifact = null;
                    if (_mapEditorEntities[typeof(MapEditorArtifact)].TryGetValue(position, out mapEditorEntity))
                    {
                        mapEditorEntity.OnClicked();
                        return;
                    }

                    var artifactObject = MapEditorManager.Instance.SelectedArtifactObject;
                    if (artifactObject != null)
                        CreateMapArtifact(artifactObject, position);
                    break;
                case MapEditorToolMode.Marker:
                    if (_mapEditorEntities[typeof(MapEditorAIMarker)].TryGetValue(position, out mapEditorEntity))
                    {
                        (mapEditorEntity as MapEditorAISiegeGuardMarker).OnClicked();
                        return;
                    }

                    CreateMapAIMarker(position);
                    break;
            }
        }

        private void UseDestructionToolAtPosition(Vector3Int position)
        {
            var toolMode = MapEditorManager.Instance.SelectedToolMode;
            switch (toolMode)
            {
                case MapEditorToolMode.Character:
                    DestroyMapEntity<MapEditorCharacter>(position);
                    break;
                case MapEditorToolMode.Terrain:
                    ClearTile(position);
                    break;
                case MapEditorToolMode.Structure:
                    var structureType = MapEditorManager.Instance.SelectedStructureType;
                    switch (structureType)
                    {
                        case StructureType.Castle:
                            DestroyMapEntity<MapEditorCastle>(position);
                            break;
                        case StructureType.Dwell:
                            DestroyMapEntity<MapEditorDwelling>(position);
                            break;
                    }

                    break;
                case MapEditorToolMode.Hero:
                    DestroyMapEntity<MapEditorHero>(position);
                    break;
                case MapEditorToolMode.Treasure:
                    DestroyMapEntity<MapEditorTreasure>(position);
                    break;
                case MapEditorToolMode.Artifact:
                    DestroyMapEntity<MapEditorArtifact>(position);
                    break;
                case MapEditorToolMode.Marker:
                    DestroyMapEntity<MapEditorAIMarker>(position);
                    break;
            }
        }

        private List<MapEditorSprite> _failedPositionsSprites = new List<MapEditorSprite>();

        private MapEditorDwelling CreateMapDwelling(DwellBuilding dwellBuilding, Vector3Int position,
            PlayerColor playerColor)
        {
            var mapEditorDwelling = GameObject.Instantiate(_mapEditorDwellingPrefab, position, Quaternion.identity);
            mapEditorDwelling.PlayerColor = playerColor;
            mapEditorDwelling.DwellBuilding = dwellBuilding;
            _mapEditorEntities[typeof(MapEditorDwelling)].Add(position, mapEditorDwelling);
            _collisions.TryAdd(position, true);
            return mapEditorDwelling;
        }

        private MapEditorCastle CreateMapCastle(CastleObject selectedCastleObject, Vector3Int position,
            PlayerColor playerColor)
        {
            bool hasCollision = false;
            if (_collisions.TryGetValue(position, out hasCollision))
                return null;
            _failedPositionsSprites.Clear();

            int startPointX = -2, endPointX = 3;
            int startPointY = 0, endPointY = 3;
            List<Vector3Int> failedPositions = new List<Vector3Int>();
            bool success = true;
            for (int i = startPointX; i <= endPointX; i++)
            {
                for (int j = startPointY; j <= endPointY; j++)
                {
                    Vector3Int pos = new Vector3Int(position.x + i, position.y + j);
                    bool ifBoundX = (pos.x < 0 || pos.x >= mapSize.x);
                    bool ifBoundY = (pos.y < 0 || pos.y >= mapSize.y);
                    if (ifBoundX || ifBoundY)
                    {
                        failedPositions.Add(pos);
                        success = false;
                        continue;
                    }

                    bool ifCollision = false;
                    _collisions.TryGetValue(pos, out ifCollision);
                    if (ifCollision)
                    {
                        
                        // failedPositions.Add(pos);
                        // success = false;
                        Debug.Log($"Failed! at {pos}");
                    }
                }
            }

            if (!success)
            {
                foreach (var fp in failedPositions)
                {
                    var instance = GameObject.Instantiate(_failEditorSpritePrefab, Vector3.zero, Quaternion.identity);
                    instance.transform.position = new Vector3(fp.x, fp.y);
                    instance.CreateWithLifeTime(1f);
                    _failedPositionsSprites.Add(instance);
                }

                return null;
            }

            for (int i = startPointX; i < endPointX; i++)
            {
                for (int j = startPointY; j < endPointY; j++)
                {
                    Vector3Int pos = new Vector3Int(position.x + i, position.y + j);
                    _collisions.Add(pos, true);
                }
            }

            var castleInstance = GameObject.Instantiate(_mapEditorCastlePrefab, position, Quaternion.identity);
            castleInstance.name = $"{selectedCastleObject.internalName}_{Random.Range(0, 100).ToString()}";
            castleInstance.castleObject = selectedCastleObject;
            castleInstance.Tier = selectedCastleObject.maxTiers;
            castleInstance.Fraction = selectedCastleObject.fraction;
            castleInstance.PlayerOwnerColor = playerColor;
            _mapEditorEntities[typeof(MapEditorCastle)].Add(position, castleInstance);
            _collisions.TryAdd(position, true);
            Debug.Log($"created castle {castleInstance.PlayerOwnerColor}");
            return castleInstance;
        }

        private void DestroyMapCastle(Vector3Int position)
        {
            bool hasCollision = false;
            if (!_collisions.TryGetValue(position, out hasCollision))
                return;
            MapEditorEntity mapEditorEntity = null;
            if (!_mapEditorEntities[typeof(MapEditorCastle)].TryGetValue(position, out mapEditorEntity))
            {
                Debug.Log("Failed to destroy terrain castle!");
                return;
            }

            int startPointX = -3, endPointX = 3;
            int startPointY = 0, endPointY = 4;
            for (int i = startPointX; i < endPointX; i++)
            {
                for (int j = startPointY; j < endPointY; j++)
                {
                    Vector3Int pos = new Vector3Int(position.x + i, position.y + j);
                    _collisions.Remove(pos);
                }
            }

            Destroy(mapEditorEntity.gameObject);
            _mapEditorEntities[typeof(MapEditorCastle)].Remove(position);
            _collisions.Remove(position);
        }

        private void CreateMapHero(HeroObject heroObject, Vector3Int position, PlayerColor playerColor)
        {
            if (!IsPositionFree(position))
                return;
            var heroInstance =
                GameObject.Instantiate(_mapEditorHeroPrefab, position, Quaternion.identity, _mapCharactersRoot);
            heroInstance.Init(heroObject);
            heroInstance.playerColor = playerColor;

            _mapEditorEntities[typeof(MapEditorHero)].TryAdd(position, heroInstance);
            _collisions.TryAdd(position, true);
        }

        private void CreateMapTreasure(TreasureObject treasureObject, Vector3Int position)
        {
            if (!IsPositionFree(position))
                return;
            var treasureInstance = GameObject.Instantiate(_mapEditorTreasurePrefab, position, Quaternion.identity,
                _mapCharactersRoot);
            treasureInstance.Init(treasureObject);
            treasureInstance.name = $"{treasureObject.name}_{treasureInstance.GetInstanceID()}";

            _mapEditorEntities[typeof(MapEditorTreasure)].TryAdd(position, treasureInstance);
            _collisions.TryAdd(position, true);
        }

        private MapEditorAIMarker CreateMapAIMarker(Vector3Int position)
        {
            if (!IsPositionFree(position))
                return null;
            var mapEditorAIMarkerInstance = GameObject.Instantiate(_mapEditorAIMarkerPrefab, position,
                Quaternion.identity,
                _mapCharactersRoot);
            mapEditorAIMarkerInstance.name = $"ai_marker_{mapEditorAIMarkerInstance.GetInstanceID()}";

            _mapEditorEntities[typeof(MapEditorAIMarker)].TryAdd(position, mapEditorAIMarkerInstance);
            _collisions.TryAdd(position, true);
            return mapEditorAIMarkerInstance;
        }

        private bool IsPositionFree(Vector3Int position)
        {
            bool hasCollision = false;
            if (_collisions.TryGetValue(position, out hasCollision))
                return false;
            var hasAnyGroundBelow = _groundTilemap.GetTile(position) != null;
            if (!hasAnyGroundBelow)
                return false;
            return true;
        }

        private void CreateMapArtifact(ArtifactObject artifactObject, Vector3Int position)
        {
            if (!IsPositionFree(position))
                return;
            var artifactInstance = GameObject.Instantiate(_mapEditorArtifactPrefab, position, Quaternion.identity,
                _mapCharactersRoot);
            artifactInstance.Init(artifactObject);
            artifactInstance.name = $"{artifactObject.name}_{artifactInstance.GetInstanceID()}";

            _mapEditorEntities[typeof(MapEditorArtifact)].TryAdd(position, artifactInstance);
            _collisions.TryAdd(position, true);
        }

        private void CreateMapCharacter(CharacterObject characterObject, Vector3Int position, PlayerColor playerColor,
            int quantity)
        {
            if (!IsPositionFree(position))
                return;
            var characterInstance = GameObject.Instantiate(_mapEditorCharacterPrefab, position, Quaternion.identity,
                _mapCharactersRoot);
            characterInstance.Init(characterObject);
            characterInstance.name = $"{characterObject.title}_{characterInstance.GetInstanceID()}";
            characterInstance.PlayerOwnerColor = playerColor;
            characterInstance.countInStack = quantity;

            _mapEditorEntities[typeof(MapEditorCharacter)].TryAdd(position, characterInstance);
            _collisions.TryAdd(position, true);
        }

        private void LoadMapCharacter(SerializableCharacter serializableCharacter)
        {
            var position = new Vector3Int(serializableCharacter.positionX, serializableCharacter.positionY, 0);
            var characterObject = ResourcesBase.GetCharacterObject(serializableCharacter.objectName,
                serializableCharacter.Fraction);
            CreateMapCharacter(characterObject, position, serializableCharacter.PlayerOwnerColor,
                serializableCharacter.quantity);
        }

        private void LoadMapHero(SerializableHero serializableHero)
        {
            var position = new Vector3Int(serializableHero.positionX, serializableHero.positionY, 0);
            var heroObject = ResourcesBase.GetHeroObject(serializableHero.objectName, serializableHero.Fraction);
            CreateMapHero(heroObject, position, serializableHero.PlayerOwnerColor);
        }

        private void LoadMapCastle(SerializableCastle serializableCastle)
        {
            var position = new Vector3Int(serializableCastle.positionX, serializableCastle.positionY, 0);
            var castleObject = ResourcesBase.GetCastleObject(serializableCastle.objectName);
            var mapEditorCastle = CreateMapCastle(castleObject, position, serializableCastle.PlayerOwnerColor);
            mapEditorCastle.UniqueId = serializableCastle.UniqueId;
        }

        private void LoadMapTreasure(SerializableTreasure serializableTreasure)
        {
            var position = new Vector3Int(serializableTreasure.positionX, serializableTreasure.positionY, 0);
            var treasureObject = ResourcesBase.GetTreasureObject(serializableTreasure.objectName);
            CreateMapTreasure(treasureObject, position);
        }

        private void LoadMapArtifact(SerializableArtifact serializableArtifact)
        {
            var position = new Vector3Int(serializableArtifact.positionX, serializableArtifact.positionY, 0);
            var artifactObject = ResourcesBase.GetArtifactObject(serializableArtifact.objectName);
            CreateMapArtifact(artifactObject, position);
        }

        private void LoadMapDwelling(SerializableDwelling serializableDwelling)
        {
            var position = new Vector3Int(serializableDwelling.positionX, serializableDwelling.positionY, 0);
            CreateMapDwelling(ResourcesBase.GetBuilding(serializableDwelling.objectName) as DwellBuilding, position,
                serializableDwelling.PlayerColor);
        }

        private void LoadMapMarker(SerializableMarker serializableMarker)
        {
            var position = new Vector3Int(serializableMarker.positionX, serializableMarker.positionY, 0);
            var marker = CreateMapAIMarker(position) as MapEditorAISiegeGuardMarker;
            marker.GuardMarkerCharacterType = serializableMarker.GuardMarkerCharacterType;
            marker.GuardMarkerPlayerStateType = serializableMarker.GuardMarkerPlayerStateType;
            marker.Quantity = serializableMarker.Quantity;
            marker.Tier = serializableMarker.Tier;
            marker.Level = serializableMarker.Level;
        }

        private void DestroyMapEntity<T>(Vector3Int position) where T : MapEditorEntity
        {
            bool hasCollision = false;
            if (!_collisions.TryGetValue(position, out hasCollision))
                return;
            MapEditorEntity mapEditorEntity = null;
            if (!_mapEditorEntities[typeof(T)].TryGetValue(position, out mapEditorEntity))
                return;
            Destroy(mapEditorEntity.gameObject);
            _mapEditorEntities[typeof(T)].Remove(position);
            _collisions.Remove(position);
        }
        //
        // private void DestroyMapCharacter(Vector3Int position)
        // {
        //     bool hasCollision = false;
        //     if (!_collisions.TryGetValue(position, out hasCollision))
        //         return;
        //     MapEditorEntity mapEditorEntity = null;
        //     if (!_mapEditorEntities[typeof(MapEditorCharacter)].TryGetValue(position, out mapEditorEntity))
        //         return;
        //     Destroy(mapEditorEntity.gameObject);
        //     _mapEditorEntities[typeof(MapEditorCharacter)].Remove(position);
        //     _collisions.Remove(position);
        // }
        //
        // private void DestroyMapHero(Vector3Int position)
        // {
        //     bool hasCollision = false;
        //     if (!_collisions.TryGetValue(position, out hasCollision))
        //         return;
        //     MapEditorEntity mapEditorEntity = null;
        //     if (!_mapEditorEntities[typeof(MapEditorHero)].TryGetValue(position, out mapEditorEntity))
        //         return;
        //     Destroy(mapEditorEntity.gameObject);
        //     _mapEditorEntities[typeof(MapEditorHero)].Remove(position);
        //     _collisions.Remove(position);
        // }
        //
        // private void DestroyMapTreasure(Vector3Int position)
        // {
        //     bool hasCollision = false;
        //     if (!_collisions.TryGetValue(position, out hasCollision))
        //         return;
        //     MapEditorEntity mapEditorEntity = null;
        //     if (!_mapEditorEntities[typeof(MapEditorTreasure)].TryGetValue(position, out mapEditorEntity))
        //         return;
        //     Destroy(mapEditorEntity.gameObject);
        //     _mapEditorEntities[typeof(MapEditorTreasure)].Remove(position);
        //     _collisions.Remove(position);
        // }
        //
        // private void DestroyMapArtifact(Vector3Int position)
        // {
        //     bool hasCollision = false;
        //     if (!_collisions.TryGetValue(position, out hasCollision))
        //         return;
        //     MapEditorEntity mapEditorEntity = null;
        //     if (!_mapEditorEntities[typeof(MapEditorArtifact)].TryGetValue(position, out mapEditorEntity))
        //         return;
        //     Destroy(mapEditorEntity.gameObject);
        //     _mapEditorEntities[typeof(MapEditorArtifact)].Remove(position);
        //     _collisions.Remove(position);
        // }

        private Vector3Int InputWorldPositionRounded()
        {
            var inputPosition = Input.mousePosition;
            var inputWorldPosition = _camera.ScreenToWorldPoint(inputPosition);
            Vector3Int result = new Vector3Int(Mathf.RoundToInt(inputWorldPosition.x),
                Mathf.RoundToInt(inputWorldPosition.y), 0);
            return result;
        }

        private bool IsInputPositionValid(Vector3Int position)
        {
            if (position.x < startIndex || position.x > mapSize.x - 1)
                return false;
            if (position.y < startIndex || position.y > mapSize.y - 1)
                return false;
            return true;
        }

        public void CreateGrid()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Destroy(lines[i]);
            }

            lines.Clear();
            _gridLinesRoot = new GameObject("grid_main").transform;
            Vector3 startPosition = new Vector3(-0.5f, -0.5f, 0f);
            float deltaX = 1f, deltaY = 1f;
            Vector3 lineStart = startPosition;
            Vector3 lineEnd = startPosition;

            lineEnd.y = mapSize.y - 0.5f;
            for (int i = 0; i <= mapSize.x; i++)
            {
                CreateLine(lineStart, lineEnd);
                lineEnd.x += deltaX;
                lineStart.x += deltaX;
            }

            lineStart = startPosition;

            lineEnd.y = -0.5f;
            lineEnd.x = mapSize.x - 0.5f;
            for (int i = 0; i <= mapSize.y; i++)
            {
                CreateLine(lineStart, lineEnd);
                lineEnd.y += deltaY;
                lineStart.y += deltaY;
            }
        }

        private List<GameObject> lines = new List<GameObject>();

        private void CreateLine(Vector3 startPosition, Vector3 endPosition)
        {
            var lineRenderer = new GameObject($"grid_{startPosition} - {endPosition}").AddComponent<LineRenderer>();
            lines.Add(lineRenderer.gameObject);
            lineRenderer.transform.SetParent(_gridLinesRoot);
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.material = linesMaterial;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
        }
    }
}