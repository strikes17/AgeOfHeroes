using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class MoveActionCell : AbstractActionCell
    {
        public MoveActionCell()
        {
            _color = new Color(0.4f, 1f, 0.42f);
            _sprite = ResourcesBase.GetSprite("action_cell_move");
        }
        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;
            var lastSelectedCharacter =
                GameManager.Instance.MapScenarioHandler.players[turnOfPlayerId].selectedCharacter;
            if (lastSelectedCharacter.playerOwnerColor != turnOfPlayerId)
                return;
            Moroutine.Run(lastSelectedCharacter.MoveToPosition(actionCell.Position));
        }
    }
}