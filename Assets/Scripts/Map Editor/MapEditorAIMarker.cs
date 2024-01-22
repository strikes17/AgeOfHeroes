namespace AgeOfHeroes.MapEditor
{
    public class MapEditorAIMarker : MapEditorEntity
    {
        public virtual void OnClicked()
        {
            MapEditorManager.Instance.SelectedMarker = this;
        }
    }
}