using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuyCharacterDialogue : GUIDialogueWindow
    {
        [SerializeField] private Button closeDialogueButton;
        [SerializeField] private Button raiseQuantityMaxButton;
        [SerializeField] private Button makeDealButton;
        [SerializeField] private Slider quantitySlider;
        [SerializeField] private TMP_Text goldPriceText, gemsPriceText, quantityText;
        [SerializeField] private GUIBuyCharacterButtonWidget basicCharacterButtonWidget, eliteCharacterButtonWidget;
        [SerializeField] private GUICharacterInfoButton _characterInfoButton;
        [SerializeField] private GUICharacterInfoWindow _characterInfoWindow;

        private ResourceCompound basicCharacterPrice, eliteCharacterPrice, selectedCharacterPrice;
        private CharacterObject basicCharacterObject, eliteCharacterObject, selectedCharacterObject;
        private CharacterShopBuilding _shopBuilding;
        private CastleTierIncomeValues _castleTierIncomeValues;
        private int dealQuantity;
        private Castle _targetCastle;

        private int currentGoldPriceValue, currentGemsPriceValue;

        private void Awake()
        {
            quantitySlider.onValueChanged.AddListener(OnSliderValueChanged);
            closeDialogueButton.onClick.AddListener(Hide);
            makeDealButton.onClick.AddListener(Deal);
            raiseQuantityMaxButton.onClick.AddListener(RaiseQuantityMax);
        }

        public override void Show()
        {
            int level = _shopBuilding.Level;
            if (level == 1)
                SelectBasicForm();
            else if (level == 2)
                SelectEliteForm();
            RaiseQuantityMax();
            if (_shopBuilding.RecruitmentsAvailable == 0)
            {
                makeDealButton.image.color = Colors.unavailableElementDarkGray;
                raiseQuantityMaxButton.image.color = Colors.unavailableElementDarkGray;
                makeDealButton.interactable = false;
                raiseQuantityMaxButton.interactable = false;
            }
            else
            {
                makeDealButton.image.color = Colors.whiteColor;
                raiseQuantityMaxButton.image.color = Colors.whiteColor;
                makeDealButton.interactable = true;
                raiseQuantityMaxButton.interactable = true;
            }

            base.Show();
        }

        private void RaiseQuantityMax()
        {
            quantitySlider.value = quantitySlider.maxValue;
            UpdatePriceValue(quantitySlider.value);
        }

        public override void Hide()
        {
            base.Hide();
            _targetCastle = null;
            basicCharacterButtonWidget.Hide();
            eliteCharacterButtonWidget.Hide();
        }

        private void Deal()
        {
            var player = GameManager.Instance.MapScenarioHandler.players[_targetCastle.PlayerOwnerColor];

            if (dealQuantity <= 0)
                return;
            var playerGold = player.Gold;
            var playerGems = player.Gems;
            bool hasEnoughGold = true;
            bool hasEnoughGems = true;

            if (playerGold < currentGoldPriceValue)
                hasEnoughGold = false;
            if (playerGems < currentGemsPriceValue)
                hasEnoughGems = false;

            if (!hasEnoughGold)
            {
                goldPriceText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            if (!hasEnoughGems)
            {
                gemsPriceText.GetComponent<GUITextSpecialEffect>().Flicker(0.5f, Color.red, 0.25f, 2, false);
                return;
            }

            player.Gold -= currentGoldPriceValue;
            player.Gems -= currentGemsPriceValue;
            _shopBuilding.RecruitmentsAvailable -= dealQuantity;
            var spawnManager = GameManager.Instance.SpawnManager;
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;

            var position = player.LastSelectedCastle.spawnPoint.position;

            var guiManagerLeftSidebar = GameManager.Instance.GUIManager.LeftSidebar;
            var guiCastleGarnisonPanel = guiManagerLeftSidebar.GUICastleGarnisonPanel;
            var garnisonPanel = guiCastleGarnisonPanel;
            bool putInsideCastle = player.ActiveTerrain == _targetCastle.CastleTerrain;

            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            // var targetTerrain = putInsideCastle ? _targetCastle.CastleTerrain : playableMap.WorldTerrain;
            var targetTerrain = playableMap.WorldTerrain;
            var spawnedCharacter = spawnManager.SpawnCharacter(selectedCharacterObject.name, _shopBuilding.fraction, turnOfPlayerId, ControllableCharacter.V3Int(position), targetTerrain, (int)quantitySlider.value);
            
            _targetCastle.AddGarnisonCharacterStack(spawnedCharacter);

            guiManagerLeftSidebar.GUICharacterShopMenu.UpdateShopGUI(_targetCastle);
            guiCastleGarnisonPanel.SetMode(GUICastleGarnisonEditWidget.Mode.Edit);
            guiCastleGarnisonPanel.Show();
            Hide();
        }

        private void Update()
        {
            if (!gameObject.activeSelf)
                return;
            dealQuantity = (int)quantitySlider.value;
            quantityText.text = dealQuantity.ToString();
            if (dealQuantity <= 0)
            {
                makeDealButton.interactable = false;
                makeDealButton.image.color = Colors.unavailableElementDarkGray;
            }
            else
            {
                makeDealButton.interactable = true;
                makeDealButton.image.color = Colors.whiteColor;
            }
        }

        private void SelectBasicForm()
        {
            basicCharacterButtonWidget.SelectionEffect.ApplyEffect();
            eliteCharacterButtonWidget.SelectionEffect.RemoveEffect();
            selectedCharacterObject = basicCharacterObject;
            selectedCharacterPrice = basicCharacterPrice;
            _characterInfoButton.SetGUI(selectedCharacterObject, _targetCastle.IncomeValues);
            _characterInfoWindow.SetCharacterInfoForCastle(selectedCharacterObject, _castleTierIncomeValues);
            UpdatePriceValue(quantitySlider.value);
        }

        private void SelectEliteForm()
        {
            basicCharacterButtonWidget.SelectionEffect.RemoveEffect();
            eliteCharacterButtonWidget.SelectionEffect.ApplyEffect();
            selectedCharacterObject = eliteCharacterObject;
            selectedCharacterPrice = eliteCharacterPrice;
            _characterInfoButton.SetGUI(selectedCharacterObject, _targetCastle.IncomeValues);
            _characterInfoWindow.SetCharacterInfoForCastle(selectedCharacterObject, _castleTierIncomeValues);
            UpdatePriceValue(quantitySlider.value);
        }

        private void OnSliderValueChanged(float value)
        {
            UpdatePriceValue(value);
        }

        private void UpdatePriceValue(float value)
        {
            int intValue = (int)value;
            currentGoldPriceValue = selectedCharacterPrice.Gold * intValue;
            currentGemsPriceValue = selectedCharacterPrice.Gems * intValue;
            goldPriceText.text = currentGoldPriceValue.ToString();
            gemsPriceText.text = currentGemsPriceValue.ToString();
        }

        private void OpenUpgradingMenu(Castle castle, CharacterShopBuilding shopBuilding)
        {
            GameManager.Instance.GUIManager.LeftSidebar.GUICharacterShopMenu.OpenBuildUpgradeCharacterDialogueWindow(castle, shopBuilding);
        }

        public void SetGUIInfo(Castle castle, CharacterShopBuilding shopBuilding, ResourceCompound basicCharacterPrice, ResourceCompound eliteCharacterPrice)
        {
            _castleTierIncomeValues = castle.IncomeValues;
            _shopBuilding = shopBuilding;
            _targetCastle = castle;
            this.basicCharacterObject = ResourcesBase.GetCharacterObject(shopBuilding.basicCharacterName, shopBuilding.fraction);
            this.basicCharacterPrice = basicCharacterPrice;
            basicCharacterButtonWidget.selectButton.onClick.RemoveAllListeners();
            basicCharacterButtonWidget.SetGUIInfo(basicCharacterObject);
            basicCharacterButtonWidget.CharacterImage.sprite = basicCharacterObject.mainSprite;
            basicCharacterButtonWidget.Show();
            bool hasEliteForm = basicCharacterObject.hasEliteForm;
            if (hasEliteForm)
            {
                this.eliteCharacterObject = ResourcesBase.GetCharacterObject(shopBuilding.eliteCharacterName, _shopBuilding.fraction);
                this.eliteCharacterPrice = eliteCharacterPrice;
                eliteCharacterButtonWidget.selectButton.onClick.RemoveAllListeners();
                eliteCharacterButtonWidget.SetGUIInfo(eliteCharacterObject);
                eliteCharacterButtonWidget.CharacterImage.sprite = eliteCharacterObject.mainSprite;
                eliteCharacterButtonWidget.Show();
            }

            if (shopBuilding.Level == 1)
            {
                basicCharacterButtonWidget.selectButton.onClick.AddListener(SelectBasicForm);
                eliteCharacterButtonWidget.selectButton.onClick.AddListener(() => OpenUpgradingMenu(castle, shopBuilding));
                eliteCharacterButtonWidget.DarkenColor();
            }
            else
            {
                basicCharacterButtonWidget.selectButton.onClick.AddListener(SelectBasicForm);
                eliteCharacterButtonWidget.selectButton.onClick.AddListener(SelectEliteForm);
                eliteCharacterButtonWidget.NormalColor();
            }

            int fullStackCount = shopBuilding.basicCharacterForm.fullStackCount;
            int recruitmentsAvailable = Mathf.FloorToInt(shopBuilding.RecruitmentsAvailable);
            int maxValue = recruitmentsAvailable > fullStackCount ? fullStackCount : recruitmentsAvailable;
            quantitySlider.maxValue = maxValue;
        }
    }
}