namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorHeroWidget : GUIMapEditorWidget
    {
        private HeroObject _heroObject;

        public void Set(HeroObject heroObject)
        {
            _button.onClick.RemoveAllListeners();
            _heroObject = heroObject;
            _image.sprite = _heroObject.mainSprite;
            _button.onClick.AddListener(SelectHero);
        }

        private void SelectHero()
        {
            MapEditorManager.Instance.SelectedHeroObject = _heroObject;
        }
    }
}