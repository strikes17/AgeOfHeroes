using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuyCharacterButtonWidget : MonoBehaviour
    {
        public TMP_Text title;
        public Button selectButton;
        public Image BackgroundImage, CharacterImage;
        public GUISelectionEffect SelectionEffect;

        public void SetGUIInfo(CharacterObject characterObject)
        {
            title.text = characterObject.title;
            CharacterImage.sprite = characterObject.mainSprite;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void DarkenColor()
        {
            CharacterImage.color = Colors.unavailableElementDarkGray;
        }

        public void NormalColor()
        {
            CharacterImage.color = Colors.whiteColor;
        }
    }
}