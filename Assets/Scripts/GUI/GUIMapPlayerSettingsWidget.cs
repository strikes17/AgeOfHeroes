using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIMapPlayerSettingsWidget : GUIBaseWidget
    {
        [SerializeField] private Button bannerButton, playerTypeButton, fractionButton, heroButton;
        [SerializeField] private Image bannerIcon, playerTypeIcon, fractionIcon, heroIcon;
        [SerializeField] private TMP_Text playerTypeText;
        private PlayerColor playerColor;
        private PlayerType playerType = PlayerType.Human;
        private Fraction fraction = Fraction.Human;
        private HeroObject heroObject;
        public bool FractionPredefined { get; set; }

        public PlayerColor PlayerColor
        {
            get => playerColor;
            set
            {
                playerColor = value;
                bannerIcon.sprite = ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[playerColor]);
            }
        }

        public PlayerType PlayerType
        {
            get => playerType;
            set
            {
                playerType = value;
                playerTypeText.text = GlobalVariables.PlayerTypes[playerType];
                if (playerType == PlayerType.None)
                {
                    fractionButton.interactable = false;
                    heroButton.interactable = false;
                }
                else
                {
                    if (!FractionPredefined)
                        fractionButton.interactable = true;
                    heroButton.interactable = true;
                }
            }
        }

        public Fraction Fraction
        {
            get => fraction;
            set
            {
                fraction = value;
                fractionIcon.sprite = ResourcesBase.GetSprite(GlobalVariables.FractionIcons[Fraction]);
                if (FractionPredefined)
                    fractionButton.interactable = false;
                var fractionObject = ResourcesBase.GetFractionObject(fraction);
                HeroObject ho = ResourcesBase.GetHeroObject(fractionObject.Heroes[0], fractionObject.Fraction);
                heroIcon.sprite = ho.portraitIcon;
                heroObject = ho;
            }
        }

        private void Awake()
        {
            playerTypeText.text = GlobalVariables.PlayerTypes[playerType];
            fractionIcon.sprite = ResourcesBase.GetSprite(GlobalVariables.FractionIcons[Fraction]);
            playerTypeButton.onClick.AddListener(() =>
            {
                PlayerType = playerType == PlayerType.AIHard ? PlayerType.Human : playerType + 1;
            });
            fractionButton.onClick.AddListener(() =>
            {
                Fraction = Fraction == Fraction.Undead ? Fraction.Human : Fraction + 1;
                if (Fraction == Fraction.None)
                {
                    heroButton.interactable = false;
                }
                else
                {
                    heroButton.interactable = true;
                }
            });
        }
    }
}