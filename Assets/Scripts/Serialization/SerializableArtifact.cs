namespace AgeOfHeroes.MapEditor
{
    public class SerializableArtifact : SerializableEntity
    {
        public int CurrentOwnerId;
        public SerializableArtifact()
        {
        }
        
        public SerializableArtifact(MapEditorArtifact mapEditorArtifact)
        {
            objectName = mapEditorArtifact.ArtifactObjectName;
            positionX = mapEditorArtifact.Position.x;
            positionY = mapEditorArtifact.Position.y;
        }

        public SerializableArtifact(ArtifactBehaviour artifactBehaviour)
        {
            UniqueId = artifactBehaviour.UniqueId;
            objectName = artifactBehaviour.artifactObject.name;
            positionX = artifactBehaviour.Position.x;
            positionY = artifactBehaviour.Position.y;

        }

        public SerializableArtifact(Artifact artifact)
        {
            objectName = artifact.ArtifactObject.name;
        }
    }
}