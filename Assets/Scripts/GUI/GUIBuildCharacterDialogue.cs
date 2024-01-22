using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuildCharacterDialogue : GUIDialogueWindow
    {
        [SerializeField] private GUIBuildCharacterButtonWidget _firstShopWidget, _secondShopWidget;
        [SerializeField] private Button closeButton, dialButton;
        [SerializeField] private GUICharacterInfoButton _characterInfoButton;
        [SerializeField] private GUICharacterInfoWindow _characterInfoWindow;
        private GUIBuildCharacterButtonWidget _selectedShopWidget;
        private Castle _targetCastle;
        private CharacterShopBuilding _selectedShopBuilding, _shopVariantOne, _shopVariantTwo;
        private ResourceCompound _firstShopPrice, _secondShopPrice, _selectedShopPrice;

        private void Awake()
        {
            var canvas = GameObject.Find("Canvas");
            closeButton.onClick.AddListener(Hide);
            _characterInfoButton.newWindow = true;
        }

        private void OnDestroy()
        {
            _firstShopWidget.selectButton.onClick.RemoveAllListeners();
            _secondShopWidget.selectButton.onClick.RemoveAllListeners();
        }

        private bool CheckRequirementsForBuilding(Castle castle, CharacterShopBuilding characterShopBuilding)
        {
            var requiredBuiltBuildings = characterShopBuilding.requiredBuiltBuildings;
            foreach (var requiredShopName in requiredBuiltBuildings)
            {
                var shopBuilding = castle.ShopBuildings[requiredShopName];
                Debug.Log($"{shopBuilding.internalName}:{shopBuilding.IsBuilt} is required to build {characterShopBuilding.internalName}");
                if (shopBuilding.IsBuilt && !shopBuilding.IsRestricted)
                    return true;
            }

            return false;
        }

        private void SelectFirstShop()
        {
            _selectedShopPrice = _firstShopPrice;
            _selectedShopWidget = _firstShopWidget;
            _selectedShopBuilding = _shopVariantOne;
            _firstShopWidget.SelectionEffect.ApplyEffect();
            _secondShopWidget.SelectionEffect.RemoveEffect();
            _characterInfoButton.SetGUI(_selectedShopBuilding.basicCharacterForm, _targetCastle.IncomeValues);
            _characterInfoWindow.SetCharacterInfoForCastle(_selectedShopBuilding.basicCharacterForm, _targetCastle.IncomeValues);
        }

        private void SelectSecondShop()
        {
            _selectedShopPrice = _secondShopPrice;
            _selectedShopWidget = _secondShopWidget;
            _selectedShopBuilding = _shopVariantTwo;
            _firstShopWidget.SelectionEffect.RemoveEffect();
            _secondShopWidget.SelectionEffect.ApplyEffect();
            _characterInfoButton.SetGUI(_selectedShopBuilding.basicCharacterForm, _targetCastle.IncomeValues);
            _characterInfoWindow.SetCharacterInfoForCastle(_selectedShopBuilding.basicCharacterForm, _targetCastle.IncomeValues);
        }

        private void TryBuildSelectedShop()
        {
            var player = GameManager.Instance.MapScenarioHandler.players[_targetCastle.PlayerOwnerColor];

            var playerGold = player.Gold;
            var playerGems = player.Gems;
            bool hasEnoughGold = true;
            bool hasEnoughGems = true;

            var goldPrice = _selectedShopPrice.Gold;
            var gemsPrice = _selectedShopPrice.Gems;

            if (playerGold < goldPrice)
                hasEnoughGold = false;
            if (playerGems < gemsPrice)
                hasEnoughGems = false;

            bool requirementsAccomplished = CheckRequirementsForBuilding(_targetCastle, _selectedShopBuilding);

            if (!requirementsAccomplished)
            {
                var names = _selectedShopBuilding.requiredBuiltBuildings;
                var requiredBuildings = new List<AbstractBuilding>();
                var allBuildings = _targetCastle.AllBuildingsList;
                foreach (var building in allBuildings)
                {
                    bool isRequired = names.Contains(building.internalName);
                    if (isRequired)
                    {
                        requiredBuildings.Add(building);
                    }
                }

                var failedDialogue = GUIDialogueFactory.CreateBuildFailedDialogue();
                failedDialogue.RequiredBuildingsAreNotBuilt(_selectedShopBuilding, requiredBuildings);
                return;
            }

            if (!hasEnoughGold)
            {
                _firstShopWidget.goldValueText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            if (!hasEnoughGems)
            {
                _firstShopWidget.gemsValueText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            player.Gold -= goldPrice;
            player.Gems -= gemsPrice;
            _targetCastle.BuildCharacterShop(_selectedShopBuilding);
            GameManager.Instance.GUIManager.LeftSidebar.GUICharacterShopMenu.UpdateShopGUI(_targetCastle);
            Hide();
        }

        private void UpgradeSelectedShop()
        {
            var player = GameManager.Instance.MapScenarioHandler.players[_targetCastle.PlayerOwnerColor];

            var playerGold = player.Gold;
            var playerGems = player.Gems;
            bool hasEnoughGold = true;
            bool hasEnoughGems = true;

            var goldPrice = _selectedShopPrice.Gold;
            var gemsPrice = _selectedShopPrice.Gems;

            if (playerGold < goldPrice)
                hasEnoughGold = false;
            if (playerGems < gemsPrice)
                hasEnoughGems = false;

            if (!hasEnoughGold)
            {
                _selectedShopWidget.goldValueText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            if (!hasEnoughGems)
            {
                _selectedShopWidget.gemsValueText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            player.Gold -= goldPrice;
            player.Gems -= gemsPrice;
            Debug.Log("Upgraded shop!");
            _targetCastle.UpgradeCharacterShop(_selectedShopBuilding);
            GameManager.Instance.GUIManager.LeftSidebar.GUICharacterShopMenu.UpdateShopGUI(_targetCastle);
            Hide();
        }

        public void SetBuildGUIInfo(Castle castle, CharacterShopBuilding firstShop, CharacterShopBuilding secondShop, ResourceCompound firstShopPrice, ResourceCompound secondShopPrice)
        {
            this._firstShopPrice = firstShopPrice;
            this._secondShopPrice = secondShopPrice;
            _shopVariantOne = firstShop;
            _shopVariantTwo = secondShop;
            _targetCastle = castle;

            _firstShopWidget.SetGUIInfo(_shopVariantOne, firstShopPrice);
            _firstShopWidget.Show();

            _secondShopWidget.Show();

            _firstShopWidget.selectButton.onClick.AddListener(SelectFirstShop);

            if (secondShop != null)
            {
                _secondShopWidget.SetGUIInfo(_shopVariantTwo, secondShopPrice);
                _secondShopWidget.selectButton.onClick.AddListener(SelectSecondShop);
            }
            else
                _secondShopWidget.Hide();

            dialButton.onClick.RemoveAllListeners();
            dialButton.onClick.AddListener(TryBuildSelectedShop);

            SelectFirstShop();
        }

        public void SetUpgradeGUIInfo(Castle castle, CharacterShopBuilding shopToUpgrade, ResourceCompound priceOfShopUpgrade)
        {
            dialButton.onClick.RemoveAllListeners();
            _selectedShopPrice = _firstShopPrice = priceOfShopUpgrade;
            _shopVariantOne = shopToUpgrade;
            _selectedShopWidget = _firstShopWidget;
            _selectedShopBuilding = _shopVariantOne;
            _firstShopWidget.SelectionEffect.ApplyEffect();
            _targetCastle = castle;
            _firstShopWidget.SetGUIInfo(_shopVariantOne, priceOfShopUpgrade, true);
            _secondShopWidget.Hide();
            _characterInfoButton.SetGUI(_selectedShopBuilding.eliteCharacterForm, castle.IncomeValues);
            _characterInfoWindow.SetCharacterInfoForCastle(_selectedShopBuilding.basicCharacterForm, _targetCastle.IncomeValues);
            dialButton.onClick.RemoveAllListeners();
            dialButton.onClick.AddListener(UpgradeSelectedShop);
        }

        public override void Hide()
        {
            dialButton.onClick.RemoveAllListeners();
            _firstShopWidget.selectButton.onClick.RemoveAllListeners();
            _secondShopWidget.selectButton.onClick.RemoveAllListeners();
            _targetCastle = null;
            _selectedShopBuilding = _shopVariantOne = _shopVariantTwo = null;
            _selectedShopWidget = null;
            _secondShopWidget.Show();
            base.Hide();
        }
    }
}