using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class CastSpellActionCell : AbstractActionCell
    {
        public CastSpellActionCell()
        {
            _color = new Color(0f, 0.48f, 0.83f);
            _sprite = ResourcesBase.GetSprite("action_cell_base");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var _magicSpellCombatData = player.SpellCombatData;
            int charLayerMask = 1 << LayerMask.NameToLayer("Character");
            charLayerMask |= 1 << LayerMask.NameToLayer("Corpse");
            bool targetIsEmptyCell = (_magicSpellCombatData.magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Empty) != 0;
            if (targetIsEmptyCell)
            {
                var emptyMagicSpellTargetPrefab = ResourcesBase.GetPrefab("EMST");
                var emptyMagicSpellTargetInstance = GameObject.Instantiate(emptyMagicSpellTargetPrefab,
                    actionCell.transform.position, Quaternion.identity);
                _magicSpellCombatData.target =
                    emptyMagicSpellTargetInstance.GetComponent<AbstractMagicSpellTarget>();
                bool isCastSuccessful = _magicSpellCombatData.magicSpell.TryCastSpell(_magicSpellCombatData);
                if (isCastSuccessful)
                {
                    player.selectedCharacter.DeselectAndRemoveControll(player.Color);
                    player.SetActiveContext(PlayerContext.Default);
                }
                return;
            }
            var target = Physics2D.OverlapBox(
                new Vector2(actionCell.transform.position.x, actionCell.transform.position.y),
                new Vector2(0.25f, 0.25f), 0f, charLayerMask);
            if(target == null)return;
            var magicSpellTarget = target.GetComponent<AbstractMagicSpellTarget>();
            if (magicSpellTarget != null)
            {
                _magicSpellCombatData.target = magicSpellTarget;
                bool isCastSuccessful = _magicSpellCombatData.magicSpell.TryCastSpell(_magicSpellCombatData);
                if (isCastSuccessful)
                {
                    player.selectedCharacter.DeselectAndRemoveControll(player.Color);
                    player.SetActiveContext(PlayerContext.Default);
                }
            }
        }
    }
}