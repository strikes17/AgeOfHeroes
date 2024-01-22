using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISpecialBuildingDialogue : GUISpecialBuildingInfoWindow
    {
        [SerializeField] protected Button _dealButton;

        protected override void Awake()
        {
            base.Awake();
            _dealButton.onClick.AddListener(Deal);
        }

        private void Deal()
        {
            var playerGold = _targetCastle.Player.Gold;
            var playerGems = _targetCastle.Player.Gems;
            var goldCost = _specialBuilding.goldCost;
            var gemsCost = _specialBuilding.gemsCost;
            bool hasEnoughGold = false;
            bool hasEnoughGems = false;
            if (playerGold >= goldCost)
                hasEnoughGold = true;
            if (playerGems >= gemsCost)
                hasEnoughGems = true;
            if (!hasEnoughGold || !hasEnoughGems)
            {
                if (!hasEnoughGold)
                    _goldText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                if (!hasEnoughGems)
                    _gemsText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            _targetCastle.Player.Gold -= goldCost;
            _targetCastle.Player.Gems -= gemsCost;

            Hide();
            _targetCastle.BuildSpecialBuilding(_specialBuilding);
        }

        public override void SetGUI(Castle castle, SpecialBuilding specialBuilding)
        {
            _targetCastle = castle;
            _specialBuilding = specialBuilding;
            _titleText.text = _specialBuilding.title;
            _descriptionText.text = _specialBuilding.description;
            _buildingIcon.sprite = _specialBuilding.GetIcon();
            _goldText.text = _specialBuilding.goldCost.ToString();
            _gemsText.text = _specialBuilding.gemsCost.ToString();
        }
    }
}