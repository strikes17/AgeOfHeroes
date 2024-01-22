using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICombatAbilitiyInfoWidget : MonoBehaviour
    {
        [SerializeField] private Button _clickableIcon;
        [SerializeField] private TMP_Text _abilityName;

        public Sprite AbilityIcon
        {
            set => _clickableIcon.image.sprite = value;
        }

        public string AbilityName
        {
            set => _abilityName.text = value;
            get => _abilityName.text;
        }

        public Button clickableIcon => _clickableIcon;
    }
}