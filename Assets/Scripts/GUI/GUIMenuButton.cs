using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIMenuButton : GUIBaseWidget
    {
        [SerializeField] private Button _button;

        public Button button => _button;
    }
}