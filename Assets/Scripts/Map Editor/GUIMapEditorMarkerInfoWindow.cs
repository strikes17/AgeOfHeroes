using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorMarkerInfoWindow : GUICommonDialogueWindow
    {
        [SerializeField] private TMP_InputField _quantityText, _tierText;
        [SerializeField] private Toggle _eliteToggle;


        public int Quantity
        {
            get => int.Parse(_quantityText.text.Replace((char)8203, ' '));
            set => _quantityText.text = value.ToString();
        }
        
        public int Tier
        {
            get => int.Parse(_tierText.text.Replace((char)8203, ' '));
            set => _tierText.text = value.ToString();
        }

        public bool IsElite
        {
            get => _eliteToggle.isOn;
            set => _eliteToggle.isOn = value;
        }
    }
}