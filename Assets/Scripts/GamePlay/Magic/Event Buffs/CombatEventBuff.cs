namespace AgeOfHeroes.Spell
{
    public class CombatEventBuff : EventBuff
    {
        public string onAttackPerformedBuffName, onAttackRecievedBuffName;
        public bool isBuffAppliesOnSelf;
        protected Buff _onAttackPerformedBuff, _onAttackRecievedBuff;

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            if (!string.IsNullOrEmpty(onAttackPerformedBuffName))
                _onAttackPerformedBuff = ResourcesBase.GetBuff(onAttackPerformedBuffName);
            if (!string.IsNullOrEmpty(onAttackRecievedBuffName))
                _onAttackRecievedBuff = ResourcesBase.GetBuff(onAttackRecievedBuffName);
        }

        protected virtual void OnAttackPerformed(CombatData combatData)
        {
            var buffTarget = isBuffAppliesOnSelf ? combatData.offensiveCharacter : combatData.defensiveCharacter;
            buffTarget.CreateBuff(_onAttackPerformedBuff).Apply();
        }

        protected virtual void OnAttackRecieved(CombatData combatData)
        {
            var buffTarget = isBuffAppliesOnSelf ? combatData.defensiveCharacter : combatData.offensiveCharacter;
            buffTarget.CreateBuff(_onAttackRecievedBuff).Apply();
        }

        public override void Apply()
        {
            base.Apply();
            if (_onAttackPerformedBuff != null)
                _buffTarget.AttackPerformed += OnAttackPerformed;
            if (_onAttackRecievedBuff != null)
                _buffTarget.AttackRecieved += OnAttackRecieved;
        }

        public override void OnExpired(Buff buff)
        {
            base.OnExpired(buff);
            if (_onAttackPerformedBuff != null)
                _buffTarget.AttackPerformed -= OnAttackPerformed;
            if (_onAttackRecievedBuff != null)
                _buffTarget.AttackRecieved -= OnAttackRecieved;
        }
    }
}