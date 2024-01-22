using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class DamageBlockEventBuff : CombatEventBuff
    {
        public int value;
        protected override void OnAttackRecieved(CombatData combatData)
        {
            if (combatData.totalDamage <= value)
            {
                combatData.totalDamage = 0;
            }
        }

        public override void Apply()
        {
            base.Apply();
            _buffTarget.AttackRecieved += OnAttackRecieved;
        }

        public override void OnExpired(Buff buff)
        {
            base.OnExpired(buff);
            _buffTarget.AttackRecieved -= OnAttackRecieved;
        }
    }
}