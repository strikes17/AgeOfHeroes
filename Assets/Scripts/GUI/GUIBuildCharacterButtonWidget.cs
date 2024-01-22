using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuildCharacterButtonWidget : MonoBehaviour
    {
        public Button selectButton;

        [SerializeField] private TMP_Text title;
        [SerializeField] private Image backgroundImage, characterImage;
        [SerializeField] private Image goldValueImage, gemsValueImage;
        [SerializeField] private TMP_Text _goldValueText, _gemsValueText;
        public GUISelectionEffect SelectionEffect;

        public TMP_Text goldValueText => _goldValueText;

        public TMP_Text gemsValueText => _gemsValueText;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetGUIInfo(CharacterShopBuilding shopBuilding, ResourceCompound resourceCompound, bool isElite = false)
        {
            _goldValueText.text = resourceCompound.Gold.ToString();
            _gemsValueText.text = resourceCompound.Gems.ToString();
            var characterObject = ResourcesBase.GetCharacterObject(isElite ? shopBuilding.eliteCharacterName : shopBuilding.basicCharacterName, shopBuilding.fraction);
            title.text = characterObject.title;
            characterImage.sprite = characterObject.mainSprite;
        }
    }
}