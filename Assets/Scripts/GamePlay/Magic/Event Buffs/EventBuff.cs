namespace AgeOfHeroes.Spell
{
    public class EventBuff : Buff
    {
        protected ControllableCharacter _buffTarget;

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _buffTarget = _target.Character;
        }

        public override bool IsNotDebuff()
        {
            return true;
        }
    }
}