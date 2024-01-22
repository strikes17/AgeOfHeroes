using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUICharacterShopMenu : GUILeftSidebarAbstractMenu
    {
        public List<GUIBuyCharacterButton> buyCharactersButtons;
        public GUIBuyCharacterDialogue GUIBuyCharacterDialogue;
        public GUIBuildCharacterDialogue GUIBuildCharacterDialogue;

        [SerializeField] private GameObject buyCharactersButtonPrefab, buyCharacterDialoguePrefab, buildCharacterDialoguePrefab;

        [SerializeField] private Canvas _canvas;

        public event Castle.OnCastleEventDelegate CastleMenuOpened
        {
            add => castleMenuOpened += value;
            remove => castleMenuOpened -= value;
        }

        protected void Awake()
        {
            base.Awake();
        }

        private event Castle.OnCastleEventDelegate castleMenuOpened;

        public override void Hide()
        {
            GUIBuyCharacterDialogue?.Hide();
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
        }

        private void Clear()
        {
            for (int i = 0; i < buyCharactersButtons.Count; i++)
            {
                Destroy(buyCharactersButtons[i].gameObject);
            }

            buyCharactersButtons.Clear();
        }

        public void OpenBuyCharacterDialogueWindow(Castle castle, CharacterShopBuilding shopBuilding)
        {
            GUIBuildCharacterDialogue?.Hide();
            if (GUIBuyCharacterDialogue == null)
            {
                GUIBuyCharacterDialogue = GameObject.Instantiate(buyCharacterDialoguePrefab, Vector3.zero,
                    Quaternion.identity,
                    _canvas.transform).GetComponent<GUIBuyCharacterDialogue>();
                GUIBuyCharacterDialogue.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            ResourceCompound basicCharacterPrice = new ResourceCompound()
            {
                Gold = shopBuilding.basicCharacterForm.goldPrice,
                Gems = shopBuilding.basicCharacterForm.gemsPrice
            };

            ResourceCompound eliteCharacterPrice = new ResourceCompound()
            {
                Gold = shopBuilding.eliteCharacterForm.goldPrice,
                Gems = shopBuilding.eliteCharacterForm.gemsPrice
            };
            var characterObject = ResourcesBase.GetCharacterObject(shopBuilding.basicCharacterName, shopBuilding.fraction);
            GUIBuyCharacterDialogue.SetGUIInfo(castle, shopBuilding, basicCharacterPrice, eliteCharacterPrice);
            GUIBuyCharacterDialogue.Show();
        }

        public void OpenBuildUpgradeCharacterDialogueWindow(Castle castle, CharacterShopBuilding shopBuilding)
        {
            GUIBuyCharacterDialogue?.Hide();
            if (GUIBuildCharacterDialogue == null)
            {
                GUIBuildCharacterDialogue = GameObject.Instantiate(buildCharacterDialoguePrefab, Vector3.zero,
                    Quaternion.identity, _canvas.transform).GetComponent<GUIBuildCharacterDialogue>();
                GUIBuildCharacterDialogue.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            ResourceCompound upgradedShopPrice = new ResourceCompound()
            {
                Gold = shopBuilding.upgradeGoldCost,
                Gems = shopBuilding.upgradeGemsCost
            };
            GUIBuildCharacterDialogue.SetUpgradeGUIInfo(castle, shopBuilding, upgradedShopPrice);
            GUIBuildCharacterDialogue.Show();
        }

        public void OpenBuildCharacterDialogueWindow(Castle castle, CharacterShopBuilding firstShop, CharacterShopBuilding secondShop)
        {
            GUIBuyCharacterDialogue?.Hide();
            if (GUIBuildCharacterDialogue == null)
            {
                GUIBuildCharacterDialogue = GameObject.Instantiate(buildCharacterDialoguePrefab, Vector3.zero,
                    Quaternion.identity, _canvas.transform).GetComponent<GUIBuildCharacterDialogue>();
                GUIBuildCharacterDialogue.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            ResourceCompound firstShopPrice = new ResourceCompound()
            {
                Gold = firstShop.goldCost,
                Gems = firstShop.gemsCost
            };

            ResourceCompound secondShopPrice = new ResourceCompound()
            {
                Gold = secondShop == null ? 0 : secondShop.goldCost,
                Gems = secondShop == null ? 0 : secondShop.gemsCost
            };

            GUIBuildCharacterDialogue.SetBuildGUIInfo(castle, firstShop, secondShop, firstShopPrice, secondShopPrice);
            GUIBuildCharacterDialogue.Show();
        }

        public void UpdateShopGUI(Castle castle)
        {
            Clear();
            var shopBuildings = castle.ShopBuildings.Values.OrderBy(x => x.tier).ToList();
            List<CharacterShopBuilding> conflictedShops = new List<CharacterShopBuilding>();
            foreach (var shopBuilding in shopBuildings)
            {
                for (int i = shopBuildings.IndexOf(shopBuilding) + 1; i < shopBuildings.Count; i++)
                {
                    var nextShopBuilding = shopBuildings[i];
                    if (nextShopBuilding.internalName == shopBuilding.blockedByBuildingName)
                    {
                        var confictingPair = (shopBuilding, nextShopBuilding);
                        conflictedShops.Add(shopBuilding);
                        conflictedShops.Add(nextShopBuilding);
                        AddConflictingGUIButtonInstance(castle, castle.PlayerOwnerColor, confictingPair);
                        break;
                    }
                }

                if (!conflictedShops.Contains(shopBuilding))
                {
                    AddNonConflictingGUIButtonInstance(castle, castle.PlayerOwnerColor, shopBuilding);
                }
            }
        }

        private void AddNonConflictingGUIButtonInstance(Castle castle, PlayerColor playerColor, CharacterShopBuilding characterShopBuilding)
        {
            var buttonInstance = GameObject.Instantiate(buyCharactersButtonPrefab, Vector3.zero,
                Quaternion.identity, transform).GetComponent<GUIBuyCharacterButton>();

            CharacterObject characterObject = null;

            characterObject = characterShopBuilding.Level == 2 ? characterShopBuilding.eliteCharacterForm : characterShopBuilding.basicCharacterForm;
            if (characterShopBuilding.IsBuilt)
            {
                buttonInstance.DialButton.onClick.RemoveAllListeners();
                buttonInstance.DialButton.onClick.AddListener(() => OpenBuyCharacterDialogueWindow(castle, characterShopBuilding));
                buttonInstance.Image.sprite = characterObject.mainSprite;
                buttonInstance.name = characterObject.name;
                buttonInstance.quantityText.text = characterShopBuilding.RecruitsAvailableInteger.ToString();
                buttonInstance.QuantityBGColor = GlobalVariables.playerColors[playerColor];
            }
            else
            {
                buttonInstance.DialButton.onClick.RemoveAllListeners();
                buttonInstance.DialButton.onClick.AddListener(() => OpenBuildCharacterDialogueWindow(castle, characterShopBuilding, null));
                buttonInstance.name = "not built";
                buttonInstance.quantityText.gameObject.SetActive(false);
                buttonInstance.QuantityBackgroundVisibility = false;
            }

            buyCharactersButtons.Add(buttonInstance);
        }

        private void AddConflictingGUIButtonInstance(Castle castle, PlayerColor playerColor, (CharacterShopBuilding, CharacterShopBuilding) conflictingPair)
        {
            var buttonInstance = GameObject.Instantiate(buyCharactersButtonPrefab, Vector3.zero,
                Quaternion.identity, transform).GetComponent<GUIBuyCharacterButton>();

            var firstShop = conflictingPair.Item1;
            var secondShop = conflictingPair.Item2;

            CharacterObject characterObject = null;

            CharacterShopBuilding builtShop = firstShop.IsBuilt ? firstShop : secondShop.IsBuilt ? secondShop : null;

            if (builtShop != null)
            {
                characterObject = builtShop.Level == 2 ? builtShop.eliteCharacterForm : builtShop.basicCharacterForm;
                buttonInstance.DialButton.onClick.AddListener(() => OpenBuyCharacterDialogueWindow(castle, builtShop));
                buttonInstance.Image.sprite = characterObject.mainSprite;
                buttonInstance.name = characterObject.name;
                buttonInstance.quantityText.text = builtShop.RecruitsAvailableInteger.ToString();
                buttonInstance.quantityText.gameObject.SetActive(true);
                buttonInstance.QuantityBackgroundVisibility = true;
            }
            else
            {
                buttonInstance.DialButton.onClick.AddListener(() => OpenBuildCharacterDialogueWindow(castle, firstShop, secondShop));
                buttonInstance.name = "not built";
                buttonInstance.quantityText.gameObject.SetActive(false);
                buttonInstance.QuantityBackgroundVisibility = false;
            }

            buttonInstance.QuantityBGColor = GlobalVariables.playerColors[playerColor];

            buyCharactersButtons.Add(buttonInstance);
        }
    }
}