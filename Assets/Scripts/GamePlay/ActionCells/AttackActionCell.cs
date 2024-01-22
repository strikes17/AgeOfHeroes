using UnityEngine;

namespace AgeOfHeroes
{
    public class AttackActionCell : AbstractActionCell
    {
        public AttackActionCell()
        {
            _color = new Color(0.83f, 0f, 0f);
            _sprite = ResourcesBase.GetSprite("action_cell_attack");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;
            var selectedCharacter =
                GameManager.Instance.MapScenarioHandler.players[turnOfPlayerId].selectedCharacter;
            if (selectedCharacter.playerOwnerColor != turnOfPlayerId)
                return;
            int charLayerMask = 1 << LayerMask.NameToLayer("Character");
            var target = Physics2D.OverlapBox(
                new Vector2(actionCell.transform.position.x, actionCell.transform.position.y),
                new Vector2(0.25f, 0.25f), 0f, charLayerMask);
            if (target != null)
            {
                var targetChar = target.GetComponent<ControllableCharacter>();
                if (targetChar == null)
                    return;
                CombatData combatData = new CombatData();
                combatData.offensiveCharacter = selectedCharacter;
                combatData.defensiveCharacter = targetChar;
                combatData.performedOnRetilation = false;
                combatData.totalDamage = targetChar.CalculateIncomingDamage(combatData);
                combatData.killQuantity = targetChar.CalculateQuantityChangeInStack(combatData);
                int killQuantity = combatData.killQuantity;
                bool willStackBeAlive = killQuantity < combatData.defensiveCharacter.Count;
                bool doesDefenderHasMeleeRetilations = combatData.defensiveCharacter.RetilationsLeft > 0;
                bool isOffenderShooter = combatData.offensiveCharacter.isShooter;
                bool isDefenderShooterAndCanRetilate = combatData.defensiveCharacter.isShooter &&
                                                       combatData.defensiveCharacter.RangedRetilationsLeft > 0;
                int distance = Mathf.CeilToInt(combatData.offensiveCharacter.Distance(combatData.defensiveCharacter));
                combatData.willRetilate = willStackBeAlive &&
                                          ((doesDefenderHasMeleeRetilations &&
                                            distance <= combatData.defensiveCharacter.attackRange) ||
                                           (isOffenderShooter && isDefenderShooterAndCanRetilate));

                var combatInfoDialogue = GameManager.Instance.GUIManager.CombatInfoDialogue;

                combatInfoDialogue.SetGUIInfo(combatData);
                combatInfoDialogue.Show();
                selectedCharacter.DeselectAndRemoveControll(selectedCharacter.playerOwnerColor);
            }
            else
            {
                Debug.Log($"{selectedCharacter.name} couldn't have found target! At spot {actionCell.name}");
            }
        }
    }
}