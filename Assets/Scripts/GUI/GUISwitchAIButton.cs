using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISwitchAIButton : GUIButtonWidget
    {
        protected override void Awake()
        {
            base.Awake();
            _button.onClick.AddListener(() =>
            {
                var playerHuman = GameManager.Instance.MapScenarioHandler.players[PlayerColor.Red];
                playerHuman.AIControlls = !playerHuman.AIControlls;
                if (playerHuman.AIControlls)
                    playerHuman.AIPlayerController.ProcessAI();
            });
        }
    }
}