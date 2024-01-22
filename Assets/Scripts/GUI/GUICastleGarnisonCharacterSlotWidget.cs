using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICastleGarnisonCharacterSlotWidget : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Image _countImage;
        [SerializeField] private TMP_Text _countText;
        private ControllableCharacter _controllableCharacter;

        public Button button => _button;

        public ControllableCharacter controllableCharacter
        {
            get => _controllableCharacter;
            set
            {
                _controllableCharacter = value;
                _image.sprite = _controllableCharacter.GetMainSprite();
                _countImage.color = GlobalVariables.playerColors[controllableCharacter.playerOwnerColor];
                _countText.text = _controllableCharacter.Count.ToString();
            }
        }

    }
}