using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuffInfoWidget : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _roundsLeftText;
        [SerializeField] private Button _button;

        public int RoundsLeft
        {
            set { _roundsLeftText.text = value <= 0 ? "--" : value.ToString(); }
        }

        public bool IsNotDebuff
        {
            set => _roundsLeftText.color = value ? Colors.positiveBuffColor : Colors.negativeBuffColor;
        }

        public Sprite BuffIcon
        {
            set => _image.sprite = value;
        }

        public Button button => _button;
    }
}