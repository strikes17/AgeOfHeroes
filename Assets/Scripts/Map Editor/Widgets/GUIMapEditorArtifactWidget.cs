namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorArtifactWidget : GUIMapEditorWidget
    {
        public ArtifactObject ArtifactObject
        {
            set
            {
                artifactObject = value;
                ImageIcon = artifactObject.Icon;
            }
            get => artifactObject;
        }
        private ArtifactObject artifactObject;
    }
}