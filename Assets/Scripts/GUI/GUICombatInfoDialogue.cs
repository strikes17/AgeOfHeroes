using System;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICombatInfoDialogue : GUIDialogueWindow
    {
        public Image _sourceBannerImage, _targetBannerImage;
        public TMP_Text sourceDamageText, targetRetilationText, killQuantityText;
        public Button _closeButton, _attackButton, _minimizeButton;
        public GUICombatInfoMinimizedDialogue _minimizedDialogue;
        [SerializeField] private GUICharacterInfoButton _sourceCharacterInfoButton, _targetCharacterInfoButton;
        [SerializeField] private Image _sourceCharacterImage, _targetCharacterImage;
        public bool WasMinimized;
        protected CombatData _combatData;

        public virtual void SetGUIInfo(CombatData combatData)
        {
            _combatData = combatData;
            var offensiveCharacter = combatData.offensiveCharacter;
            var defensiveCharacter = combatData.defensiveCharacter;
            var offensivePlayer = GameManager.Instance.MapScenarioHandler.players[offensiveCharacter.playerOwnerColor];
            var defensivePlayer = GameManager.Instance.MapScenarioHandler.players[defensiveCharacter.playerOwnerColor];

            _sourceBannerImage.sprite = offensivePlayer.Banner;
            _targetBannerImage.sprite = defensivePlayer.Banner;
            _sourceCharacterImage.sprite = offensiveCharacter.GetMainSprite();
            _targetCharacterImage.sprite = defensiveCharacter.GetMainSprite();

            sourceDamageText.text = combatData.totalDamage.ToString();
            killQuantityText.text = combatData.killQuantity.ToString();
            targetRetilationText.text = combatData.willRetilate ? "YES" : "NO";

            _sourceCharacterInfoButton.newWindow = true;
            _targetCharacterInfoButton.newWindow = true;


            _sourceCharacterInfoButton.SetGUI(offensiveCharacter);
            _targetCharacterInfoButton.SetGUI(defensiveCharacter);

            _minimizedDialogue.SetGUIInfo(combatData);
        }

        protected virtual void Awake()
        {
            _minimizeButton.onClick.AddListener(MinimizeWindow);
            _attackButton.onClick.AddListener(Attack);
            _sourceCharacterInfoButton.Test();
            _targetCharacterInfoButton.Test();
        }

        protected virtual void Attack()
        {
            Moroutine.Run(_combatData.offensiveCharacter.IECombatAttackProcess(_combatData));
            Hide();
        }

        protected virtual void Start()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        private void MinimizeWindow()
        {
            Hide();
            WasMinimized = true;
            _minimizedDialogue.Show();
        }

        public override void Show()
        {
            base.Show();
            if (WasMinimized)
            {
                MinimizeWindow();
            }
        }
        
    }
}