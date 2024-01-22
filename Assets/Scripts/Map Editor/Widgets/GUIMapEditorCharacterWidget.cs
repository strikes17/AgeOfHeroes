namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorCharacterWidget : GUIMapEditorWidget
    {
        private CharacterObject _characterObject;

        public void Set(CharacterObject characterObject)
        {
            _button.onClick.RemoveAllListeners();
            _characterObject = characterObject;
            _image.sprite = _characterObject.mainSprite;
            _button.onClick.AddListener(SelectCharacter);
        }

        private void SelectCharacter()
        {
            MapEditorManager.Instance.SelectedCharacterObject = _characterObject;
        }
    }
}