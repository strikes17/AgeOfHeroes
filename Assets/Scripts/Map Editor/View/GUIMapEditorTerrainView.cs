using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorTerrainView : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private TerrainTileObject defaultTerrainTileObject;
        [SerializeField] private GUIMapEditorTerrainWidget _terrainWidgetPrefab;
        private List<GUIMapEditorTerrainWidget> _terrainWidgets = new List<GUIMapEditorTerrainWidget>();

        private void Clear()
        {
            for (int i = 0; i < _terrainWidgets.Count; i++)
            {
                Destroy(_terrainWidgets[i].gameObject);
            }

            _terrainWidgets.Clear();
        }

        public void UpdateGUI(TerrainTileObject terrainTileObject)
        {
            Clear();
            int index = 0;
            foreach (var tile in terrainTileObject.Tiles)
            {
                var terrainWidgetInstance = GameObject.Instantiate(_terrainWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                terrainWidgetInstance.name = tile.name;
                terrainWidgetInstance.Set(terrainTileObject, index++);
                _terrainWidgets.Add(terrainWidgetInstance);
            }
        }

        public void Show()
        {
            var lastSelectedTerrainTileObject = MapEditorManager.Instance.SelectedTerrainTileObject == null ? defaultTerrainTileObject : MapEditorManager.Instance.SelectedTerrainTileObject;
            UpdateGUI(lastSelectedTerrainTileObject);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
        }
    }
}