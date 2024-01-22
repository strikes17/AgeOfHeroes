using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class CollectActionCell : AbstractActionCell
    {
        public CollectActionCell()
        {
            _color = new Color(1f, 0.76f, 0.18f);
            _sprite = ResourcesBase.GetSprite("action_cell_base");
        }
        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;
            var selectedCharacter = player.selectedCharacter;
            if (selectedCharacter.playerOwnerColor != turnOfPlayerId)
                return;
            int treasureLayerMask = 1 << LayerMask.NameToLayer("Treasure");
            var target = Physics2D.OverlapBox(new Vector2(actionCell.transform.position.x, actionCell.transform.position.y),
                new Vector2(0.25f, 0.25f), 0f, treasureLayerMask);
            if (target != null)
            {
                var targetTreasure = target.GetComponent<AbstractCollectable>();
                selectedCharacter.DeselectAndRemoveControll(player.Color);
                Moroutine.Run(selectedCharacter.IECollectTreasure(targetTreasure)).WaitForComplete();
                // Moroutine.Run(lastSelectedCharacter.MoveToPosition(transform.position));
            }
        }
    }
}