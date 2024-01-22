using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorTerrainSetWidget : GUIMapEditorWidget
    {
        private TerrainTileObject _terrainTileObject;

        public void Set(TerrainTileObject terrainTileObject)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(SelecteTerrainTileObject);
            _terrainTileObject = terrainTileObject;
            _image.sprite = _terrainTileObject.Icon;
        }

        private void SelecteTerrainTileObject()
        {
            MapEditorManager.Instance.SelectedTerrainTileObject = _terrainTileObject;
        }
    }
}