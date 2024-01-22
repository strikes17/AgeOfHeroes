using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIArtifactQuickSlotWidget : GUIBaseWidget
    {
        [SerializeField] private Image _artifactIcon;
        [SerializeField] private Button _artifactFramingButton;
        private Artifact _artifact;

        public Artifact Artifact
        {
            get => _artifact;
            set
            {
                _artifact = value;
                if (_artifact == null)
                {
                    _artifactFramingButton.image.color = Colors.whiteColor;
                    _artifactIcon.sprite = ResourcesBase.GetDefaultArtifactSprite();
                    return;
                }

                _artifactFramingButton.image.color = GlobalVariables.ArtifactQualityColors[_artifact.ArtifactObject.Quality];
                _artifactIcon.sprite = _artifact.ArtifactObject.Icon;
            }
        }

        public Button artifactFramingButton => _artifactFramingButton;
    }
}