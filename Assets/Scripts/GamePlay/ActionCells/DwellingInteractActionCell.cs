using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class DwellingInteractActionCell : AbstractActionCell
    {
        public DwellingInteractActionCell()
        {
            _color = new Color(1f, 0.76f, 0.18f);
            _sprite = ResourcesBase.GetSprite("action_cell_base");
        }
        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;
            var lastSelectedCharacter = player.selectedCharacter;
            if (lastSelectedCharacter.playerOwnerColor != turnOfPlayerId)
                return;
            int layerMask = 1 << LayerMask.NameToLayer("Building");
            var target = Physics2D.OverlapBox(new Vector2(actionCell.transform.position.x, actionCell.transform.position.y),
                new Vector2(0.25f, 0.25f), 0f, layerMask);
            if (target != null)
            {
                var dwellBuildingBehaviour = target.GetComponent<DwellBuildingBehaviour>();
                lastSelectedCharacter.DeselectAndRemoveControll(player.Color);
                Moroutine.Run(lastSelectedCharacter.IEDwellInteract(dwellBuildingBehaviour)).WaitForComplete();
            }
        }
    }
}