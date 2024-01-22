namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorCastleWidget : GUIMapEditorStructureWidget
    {
        private CastleObject _castleObject;

        public void Set(CastleObject castleObject)
        {
            _button.onClick.RemoveAllListeners();
            _castleObject = castleObject;
            _image.sprite = _castleObject.tierSprite[_castleObject.maxTiers - 1];
            _button.onClick.AddListener(SelectCastleObject);
        }

        private void SelectCastleObject()
        {
            MapEditorManager.Instance.SelectedCastleObject = _castleObject;
        }
    }
}