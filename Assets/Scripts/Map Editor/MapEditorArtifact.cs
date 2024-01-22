using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorArtifact : MapEditorEntity
    {
        private ArtifactObject _artifactObject;

        public ArtifactObject ArtifactObject => _artifactObject;
        public string ArtifactObjectName => _artifactObject.name;

        public void Init(ArtifactObject artifactObject)
        {
            _artifactObject = artifactObject;
            _spriteRenderer.sprite = _artifactObject.Icon;
        }
    }
}