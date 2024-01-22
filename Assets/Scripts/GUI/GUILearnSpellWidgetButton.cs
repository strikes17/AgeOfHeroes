using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUILearnSpellWidgetButton : GUIBaseWidget
    {
        public Image Image;
        public Button Button;
        public string magicSpellName;

        public void ApplySelectionEffect()
        {
            Button.image.color = Color.green;
        }

        public void RemoveSelectionEffect()
        {
            Button.image.color = Color.white;
        }
    }
}