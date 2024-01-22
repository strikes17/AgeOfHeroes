using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISiegeCharacterLossWidget : GUIBaseWidget
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _quantityText;

        public int Quantity
        {
            set => _quantityText.text = value.ToString();
        }

        public Sprite Icon
        {
            set => _image.sprite = value;
        }
    }
}