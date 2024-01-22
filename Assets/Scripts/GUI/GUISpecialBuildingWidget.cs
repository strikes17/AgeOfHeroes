using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISpecialBuildingWidget : GUIBaseWidget
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        
        private SpecialBuilding _specialBuilding;

        public Sprite Icon
        {
            set => _image.sprite = value;
        }

        public SpecialBuilding specialBuilding
        {
            get => _specialBuilding;
            set
            {
                _specialBuilding = value;
                Icon = ResourcesBase.GetSprite(_specialBuilding.IconPath);
            }
        }

        public Button button => _button;
    }
}