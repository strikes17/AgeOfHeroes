using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroPortraitWidgetMini : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _portraitButton;
        [SerializeField] private GUIHeroPortraitWidget _portraitWidget;

        public Sprite PortraitIcon
        {
            set => _image.sprite = value;
            get => _image.sprite;
        }

        private void Awake()
        {
            _portraitButton.onClick.AddListener(Hide);
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _portraitWidget.gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _portraitWidget.Locked = false;
            _portraitWidget.gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}