using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIArtifactStatWidget : GUIBaseWidget
    {
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _statImage;

        public Sprite Icon
        {
            set => _statImage.sprite = value;
        }

        public string Description
        {
            set => _descriptionText.text = value;
            get => _descriptionText.text;
        }
    }
}