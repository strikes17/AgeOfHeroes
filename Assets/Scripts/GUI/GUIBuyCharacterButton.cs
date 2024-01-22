using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuyCharacterButton : MonoBehaviour
    {
        public Button DialButton;
        public bool opened;
        [Range(1, 6)] public int tier;
        [Range(1, 2)] public int variant;
        [SerializeField] private Image quantityBackgroundImage;

        public bool QuantityBackgroundVisibility
        {
            get => quantityBackgroundImage.gameObject.activeSelf;
            set => quantityBackgroundImage.gameObject.SetActive(value);
        }

        public Color QuantityBGColor
        {
            set => quantityBackgroundImage.color = value;
        }

        public TMP_Text quantityText;
        public Image Image;
    }
}