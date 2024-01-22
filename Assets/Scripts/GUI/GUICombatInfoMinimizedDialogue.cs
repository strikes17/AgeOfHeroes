using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICombatInfoMinimizedDialogue : GUICombatInfoDialogue
    {
        [SerializeField]private GUICombatInfoDialogue _maximizedDialogue;

        protected override void Awake()
        {
            _minimizeButton.onClick.AddListener(MaximizeWindow);
            _attackButton.onClick.AddListener(Attack);
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void SetGUIInfo(CombatData combatData)
        {
            _combatData = combatData;
            var offensiveCharacter = combatData.offensiveCharacter;
            var defensiveCharacter = combatData.defensiveCharacter;
            var offensivePlayer = GameManager.Instance.MapScenarioHandler.players[offensiveCharacter.playerOwnerColor];
            var defensivePlayer = GameManager.Instance.MapScenarioHandler.players[defensiveCharacter.playerOwnerColor];

            _sourceBannerImage.sprite = offensivePlayer.Banner;
            _targetBannerImage.sprite = defensivePlayer.Banner;

            sourceDamageText.text = combatData.totalDamage.ToString();
            killQuantityText.text = combatData.killQuantity.ToString();
            targetRetilationText.text = combatData.willRetilate ? "YES" : "NO";
        }

        private void MaximizeWindow()
        {
            Hide();
            _maximizedDialogue.WasMinimized = false;
            _maximizedDialogue.Show();
        }
    }
}