using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIMapSelectorButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _sizeText, _playersText, _mapNameText;
        [SerializeField] private Image _mapPreviewImage;

        public string sizeText
        {
            get => _sizeText.text;
            set => _sizeText.text = value;
        }

        public string mapName
        {
            get => _mapNameText.text;
            set => _mapNameText.text = value;
        }

        public Color sizeTextColor
        {
            get => _sizeText.color;
            set => _sizeText.color = value;
        }

        public string playersText
        {
            get => _playersText.text;
            set => _playersText.text = value;
        }

        public Color playersTextColor
        {
            get => _playersText.color;
            set => _playersText.color = value;
        }

        public Sprite mapPreviewImage
        {
            get => _mapPreviewImage.sprite;
            set => _mapPreviewImage.sprite = value;
        }

        public Button button => _button;
    }
}