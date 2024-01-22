using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICharacterWidgetButton : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _characterImage;
        private ControllableCharacter _referencedCharacter;
        private bool _locked;

        public bool Locked
        {
            get => Locked;
            set
            {
                _locked = value;
                _selectButton.interactable = !_locked;
            }
        }

        public ControllableCharacter ReferencedCharacter
        {
            get => _referencedCharacter;
            set
            {
                _referencedCharacter = value;
                if (_referencedCharacter != null)
                    _characterImage.sprite = _referencedCharacter.GetMainSprite();
                else
                    _characterImage.sprite = ResourcesBase.GetDefaultCharacterSprite();
            }
        }

        private void SelectCharacterOnMap()
        {
            if (_referencedCharacter == null)
                return;
            var position = _referencedCharacter.transform.position;
            GameManager.Instance.MainCamera.MoveToPosition(position);
            _referencedCharacter.SelectAndGainControll(_referencedCharacter.playerOwnerColor);
        }

        private void Awake()
        {
            _selectButton.onClick.AddListener(SelectCharacterOnMap);
        }
    }
}