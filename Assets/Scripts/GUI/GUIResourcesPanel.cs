using System;
using AgeOfHeroes;
using TMPro;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUIResourcesPanel : MonoBehaviour
    {
        public TMP_Text goldTMPText, gemsTMPText;

        public void UpdateGUI(Player player)
        {
            if (GameManager.Instance.MapScenarioHandler.TurnOfPlayerId != player.Color)
                return;
            int goldValue = player.Gold;
            int gemsValue = player.Gems;
            goldTMPText.text = goldValue.ToString();
            gemsTMPText.text = gemsValue.ToString();
        }
    }
}