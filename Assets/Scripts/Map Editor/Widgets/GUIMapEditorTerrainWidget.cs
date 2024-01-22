using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorTerrainWidget : GUIMapEditorWidget
    {
        private TerrainTileObject _terrainTileObject;
        private int index;

        public void Set(TerrainTileObject terrainTileObject, int index)
        {
            _button.onClick.RemoveAllListeners();
            this.index = index;
            _button.onClick.AddListener(SelectTerrainTile);
            _terrainTileObject = terrainTileObject;
            _image.sprite = _terrainTileObject.Tiles[index].sprite;
        }

        private void SelectTerrainTile()
        {
            MapEditorManager.Instance.SelectedTerrainTileIndex = index;
        }
    }
}