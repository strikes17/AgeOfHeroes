using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISiegeResultDialogue : GUIDialogueWindow
    {
        [SerializeField] private GUISiegeCharacterLossWidget _lossWidgetPrefab;
        [SerializeField] private Transform _attackerContentRoot, _defenderContentRoot;
        [SerializeField] private Image _attackerBannerImage, _defenderBannerImage;
        [SerializeField] private Image _attackerBannerImageBottom, _defenderBannerImageBottom;
        [SerializeField] private TMP_Text _descriptionText, _titleText;
        [SerializeField] private Button _okButton;

        private List<GUISiegeCharacterLossWidget> _widgets = new List<GUISiegeCharacterLossWidget>();

        public Button OkButton => _okButton;

        private void Awake()
        {
            _okButton.onClick.AddListener(Hide);
        }

        public override void Show()
        {
            base.Show();
            GameManager.Instance.GUIManager.BgLockImage.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            GameManager.Instance.GUIManager.BgLockImage.gameObject.SetActive(false);
        }

        public void Init(CastleSiegeData castleSiegeData)
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }

            _widgets.Clear();

            var attackingPlayerData = castleSiegeData.attackingPlayerData;
            var attackerDefeatedCharacters = attackingPlayerData.defeatedCharactersCount;
            var keys = attackerDefeatedCharacters.Keys;
            var attackerBannerSprite =
                ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[attackingPlayerData.Player.Color]);
            _attackerBannerImage.sprite = attackerBannerSprite;
            _attackerBannerImageBottom.sprite = attackerBannerSprite;
            foreach (var key in keys)
            {
                var quantity = attackerDefeatedCharacters[key];
                var widgetInstance = Instantiate(_lossWidgetPrefab, _attackerContentRoot);
                widgetInstance.Icon = key.mainSprite;
                widgetInstance.Quantity = quantity;
                _widgets.Add(widgetInstance);
            }
            
            var defenderPlayerData = castleSiegeData.defendingPlayerData;
            var defenderDefeatedCharacters = defenderPlayerData.defeatedCharactersCount;
            keys = defenderDefeatedCharacters.Keys;
            var defenderBannerSprite =
                ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[defenderPlayerData.Player.Color]);
            _defenderBannerImage.sprite = defenderBannerSprite;
            _defenderBannerImageBottom.sprite = defenderBannerSprite;
            foreach (var key in keys)
            {
                var quantity = defenderDefeatedCharacters[key];
                var widgetInstance = Instantiate(_lossWidgetPrefab, _defenderContentRoot);
                widgetInstance.Icon = key.mainSprite;
                widgetInstance.Quantity = quantity;
                _widgets.Add(widgetInstance);
            }
        }
    }
}