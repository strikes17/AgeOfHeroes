namespace AgeOfHeroes.Spell
{
    public class NetherPulseBuff : Buff
    {
        public int radius;
        public int damagePerAlly, healPerEnemy;
        public string buffOnAlivesName, buffOnUndeadName;
        protected DamageBuff _buffOnAlives;
        protected HealingBuff _buffOnUndead;

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            if (!string.IsNullOrEmpty(buffOnAlivesName))
                _buffOnAlives = ResourcesBase.GetBuff(buffOnAlivesName).Clone() as DamageBuff;

            if (!string.IsNullOrEmpty(buffOnUndeadName))
                _buffOnUndead = ResourcesBase.GetBuff(buffOnUndeadName).Clone() as HealingBuff;
        }

        public override bool IsNotDebuff()
        {
            return true;
        }

        public override void UpdateState()
        {
            var buffTarget = _target.Character;
            var charactersInRange = buffTarget.GetAllCharactersInRange(radius,
                GlobalVariables.GetCharactersDefaultPersona(
                    MagicSpellAllowedTarget.Ally | MagicSpellAllowedTarget.Enemy), false);
            int friendsCount = 0, enemiesCount = 0;
            foreach (var character in charactersInRange)
            {
                if (character.IsAnAllyTo(buffTarget))
                {
                    if ((character.TargetTypeMask & (long)MagicSpellAllowedTarget.Undead) != 0)
                        friendsCount++;
                }
                else
                {
                    if ((character.TargetTypeMask & (long)MagicSpellAllowedTarget.Alive) != 0)
                        enemiesCount++;
                }
            }

            int totalDamage = friendsCount * damagePerAlly;
            int totalHeal = enemiesCount * healPerEnemy;
            foreach (var character in charactersInRange)
            {
                if (character.IsAnAllyTo(buffTarget))
                {
                    if ((character.TargetTypeMask & (long)MagicSpellAllowedTarget.Undead) != 0)
                    {
                        var _buffOnUndeadInstance = character.CreateBuff(_buffOnUndead) as HealingBuff;
                        _buffOnUndeadInstance.value = totalHeal;
                        _buffOnUndeadInstance.Apply();
                    }
                }
                else
                {
                    if ((character.TargetTypeMask & (long)MagicSpellAllowedTarget.Alive) != 0)
                    {
                        var _buffOnAlivesInstance = character.CreateBuff(_buffOnAlives) as DamageBuff;
                        _buffOnAlivesInstance.value = totalDamage;
                        _buffOnAlivesInstance.Apply();
                    }
                }
            }
            base.UpdateState();
        }
    }
}