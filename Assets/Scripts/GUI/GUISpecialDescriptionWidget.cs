using TMPro;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUISpecialDescriptionWidget : GUIBaseWidget
    {
        [SerializeField] private TMP_Text _description;

        public string Description
        {
            set => _description.text = value;
            get => _description.text;
        }
        
    }
}