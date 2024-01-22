using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AdvancedCamera : MonoBehaviour
{
    public float speed, panSpeed;
    public Camera Camera;
    [SerializeField] private Vector2 boundsOffset;
    private Vector2 mousePosition, prevMousePosition, mouseMoveVector, boundsMax, boundsMin;
    private Transform _followingToTransform;
    [SerializeField] private float followSpeed = 2f;
    private Tilemap _groundTilemap, _waterTilemap;
    private TerrainTileMap _groundTerrain, _waterTerrain;
    private bool terrainsReady;

    public delegate void OnCameraFollowerDelegate(Transform follower);

    private void OnFollowerChanged(Transform newFollower)
    {
    }

    public void Setup(List<Player> allPlayers)
    {
        foreach (var player in allPlayers)
        {
            player.ActiveTerrainChanged += (terrain =>
            {
                _groundTilemap = terrain.GroundTilemap;
                _waterTilemap = terrain.WaterTilemap;
                _groundTerrain = terrain.Ground;
                _waterTerrain = terrain.Water;
                terrainsReady = true;
            });
        }
    }

    public RaycastHit2D[] Raycast2DSorted(int layerMask)
    {
        var input = Input.mousePosition;
        var worldSpaceInput = Camera.ScreenPointToRay(input);
        var results = Physics2D.RaycastAll(worldSpaceInput.origin, worldSpaceInput.direction, Mathf.Infinity, layerMask,
            -1f, 100f);
        results = results.OrderBy(x => x.collider.gameObject.layer).ToArray();
        return results;
    }

    public RaycastHit2D Raycast2DInputPosition(int layerMask)
    {
        var input = Input.mousePosition;
        var worldSpaceInput = Camera.ScreenPointToRay(input);
        return Raycast2D(worldSpaceInput.origin, worldSpaceInput.direction, layerMask);
    }

    public RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, int layerMask)
    {
        var raycast = Physics2D.Raycast(origin, direction, Mathf.Infinity, layerMask, -1f, 100f);
        return raycast;
    }

    void Update()
    {
        if (_followingToTransform == null)
        {
            prevMousePosition = mousePosition;
            mousePosition = Input.mousePosition;

            var horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            var vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

            mouseMoveVector = -(mousePosition - prevMousePosition).normalized;
            if (Input.GetMouseButton(2))
            {
                transform.Translate(mouseMoveVector * 0.01f * panSpeed, Space.World);
            }
            else
            {
                transform.Translate(new Vector3(horizontal, vertical, 0f), Space.World);
            }

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundsMin.x, boundsMax.x),
                Mathf.Clamp(transform.position.y, boundsMin.y, boundsMax.y), transform.position.z);
        }
        else
        {
            var targetPosition = new Vector3(Mathf.Clamp(_followingToTransform.position.x, boundsMin.x, boundsMax.x),
                Mathf.Clamp(_followingToTransform.position.y, boundsMin.y, boundsMax.y), -10f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed);
        }

        if (!terrainsReady)
            return;
        var tileWorldPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y), 0);
        var terrainTilePosition = new Vector2Int(tileWorldPosition.x, tileWorldPosition.y);
        var groundTile = _groundTilemap.GetTile(tileWorldPosition);
        var waterTile = _waterTilemap.GetTile(tileWorldPosition);
        if (groundTile != null)
        {
            TerrainTile terrainTile = null;
            if (_groundTerrain.TerrainTiles.TryGetValue(terrainTilePosition, out terrainTile))
            {
                var terrainTileObject = ResourcesBase.GetTerrainTileObject(terrainTile.TerrainTileObjectName);
                if (terrainTileObject == null) return;
                GameManager.Instance.MusicManager.PlayTerrainAmbience(terrainTileObject.MaterialName);
            }
        }
        else
        {
            TerrainTile terrainTile = null;
            if (_waterTerrain.TerrainTiles.TryGetValue(terrainTilePosition, out terrainTile))
            {
                var terrainTileObject = ResourcesBase.GetTerrainTileObject(terrainTile.TerrainTileObjectName);
                if (terrainTileObject == null) return;
                GameManager.Instance.MusicManager.PlayTerrainAmbience(terrainTileObject.MaterialName);
            }
        }
    }

    public void SetBounds(Vector2Int mapSize)
    {
        if (mapSize.x < 20)
            boundsOffset.x = 0f;
        if (mapSize.y < 20)
            boundsOffset.y = 0f;
        boundsMin = boundsOffset;
        boundsMax = new Vector2(mapSize.x - boundsOffset.x, mapSize.y - boundsOffset.y);
    }

    public void SetFollowTo(Transform transform)
    {
        if (_followingToTransform?.GetHashCode() != transform.GetHashCode())
            OnFollowerChanged(transform);
        _followingToTransform = transform;
    }

    public void StopFollowing()
    {
        _followingToTransform = null;
    }

    public void MoveToPosition(Vector3 position)
    {
        position.z = -10f;
        transform.position = position;
    }
}