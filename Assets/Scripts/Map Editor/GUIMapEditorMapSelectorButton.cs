using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorMapSelectorButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _mapNameText;
        [SerializeField] private Image _mapPreviewImage;
        [SerializeField] private SerializableMap _serializableMap;

        public string mapNameText
        {
            get => _mapNameText.text;
            set => _mapNameText.text = value;
        }

        public Sprite mapPreviewImage
        {
            get => _mapPreviewImage.sprite;
            set => _mapPreviewImage.sprite = value;
        }

        public Button button => _button;

        public SerializableMap serializableMap
        {
            get => _serializableMap;
            set => _serializableMap = value;
        }
    }
}