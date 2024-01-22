using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroSkillTreeButton : GUIBaseWidget
    {
        [SerializeField] private Button _button;
        [SerializeField] private GUIHeroSkillTreeWindow _guiHeroSkillTreeWindow;
        private Hero _hero;
        
        public void Init()
        {
            // Debug.Log("init skill tree");
            var enabled = GameManager.Instance.GameSettings.heroesLevelEnabled;
            if (!enabled)
                gameObject.SetActive(false);
            _button.onClick.AddListener(SwitchSkillTreeWindow);
            var players = GameManager.Instance.MapScenarioHandler.players;
            foreach (var player in players)
            {
                player.Value.SelectedAnyCharacter += SetWidgetState;
                player.Value.DeselectedAnyCharacter += character => DisableInteraction();
                player.Value.TurnSkipped += player1 => DisableInteraction();
            }
            DisableInteraction();
        }

        private void SetWidgetState(ControllableCharacter controllableCharacter)
        {
            bool isAnAlly = controllableCharacter.playerOwnerColor ==
                            GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;
            bool isHero = controllableCharacter is Hero;
            if (isAnAlly && isHero)
            {
                _hero = controllableCharacter as Hero;
                EnableInteraction();
            }
            else
            {
                DisableInteraction();
            }
        }
        
        private void EnableInteraction()
        {
            _button.interactable = true;
        }

        private void DisableInteraction()
        {
            _button.interactable = false;
            _hero = null;
        }

        public override void Hide()
        {
            base.Hide();
            _hero = null;
        }

        private void SwitchSkillTreeWindow()
        {
            bool isOpened = _guiHeroSkillTreeWindow.gameObject.activeSelf;
            if (isOpened)
            {
                Debug.Log("Hide window");
                _guiHeroSkillTreeWindow.Hide();
                return;
            }
            Debug.Log("Show window");
            _guiHeroSkillTreeWindow.Setup(_hero);
            _guiHeroSkillTreeWindow.Show();
        }
    }
}