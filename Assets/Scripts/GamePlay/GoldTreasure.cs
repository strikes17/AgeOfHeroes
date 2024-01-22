using UnityEngine;

namespace AgeOfHeroes
{
    public class GoldTreasure : AbstractCollectable
    {
        public Vector2Int goldValueRange;

        private GameManager _gameManager;
        private int goldValue, expValue;

        public override void ShowDialogue(Hero heroCollector)
        {
            var dialogue = GUIDialogueFactory.CreateTreasureDialogue();
            dialogue.messageTMPText.text = $"Поздравляем, вы нашли {goldValue} золота!";
            var playerColor = heroCollector.playerOwnerColor;
            _gameManager.MapScenarioHandler.players[playerColor].DisableInteractionWithWorld();
            dialogue.SetOnOkButtonEvent(() =>
            {
                OnCollected(heroCollector);
                GivePlayerTreasure(playerColor);
                dialogue.Hide();
            });
        }

        public override void Init()
        {
            _gameManager = GameManager.Instance;
            goldValue = Random.Range(goldValueRange.x, goldValueRange.y);
        }

        private void GivePlayerTreasure(PlayerColor playerColor)
        {
            _gameManager.MapScenarioHandler.players[playerColor].EnableInteractionWithWorld();
            GameManager.Instance.MapScenarioHandler.players[playerColor].Gold += goldValue;
        }
    }
}