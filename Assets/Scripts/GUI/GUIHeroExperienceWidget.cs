using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroExperienceWidget : GUIBaseWidget
    {
        [SerializeField] private Image _image;

        public float FillValue
        {
            set => _image.fillAmount = value;
        }
    }
}