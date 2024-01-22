namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorDwellWidget : GUIMapEditorStructureWidget
    {
        private DwellBuilding _dwellBuilding;

        public void Set(DwellBuilding dwellBuilding)
        {
            _button.onClick.RemoveAllListeners();
            _dwellBuilding = dwellBuilding.Clone() as DwellBuilding;
            _image.sprite = dwellBuilding.GetIcon();
            _button.onClick.AddListener(SelectDwellBuilding);
        }

        private void SelectDwellBuilding()
        {
            MapEditorManager.Instance.SelectedDwellBuilding = _dwellBuilding;
        }
    }
}