using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorStructureTypeWidget : GUIMapEditorWidget
    {
        private StructureType _structureType;

        public void Set(StructureType structureType, Sprite icon)
        {
            _button.onClick.RemoveAllListeners();
            _structureType = structureType;
            _image.sprite = icon;
            _button.onClick.AddListener(SelectStructureType);
        }

        private void SelectStructureType()
        {
            MapEditorManager.Instance.SelectedStructureType = _structureType;
        }
    }
}