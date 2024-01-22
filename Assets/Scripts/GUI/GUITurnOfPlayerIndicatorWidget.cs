using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUITurnOfPlayerIndicatorWidget : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public Color ImageColor
        {
            get => _image.color;
            set => _image.color = value;
        }
    }
}