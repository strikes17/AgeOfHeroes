using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuildRedirectRequirementWidget : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        public Color FrameColor
        {
            set => _button.image.color = value;
        }
        
        public Sprite Icon
        {
            set => _image.sprite = value;
        }

        public Button button => _button;
    }
}