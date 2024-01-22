using System;

namespace AgeOfHeroes.Spell
{
    public class AreaAffectorBuff : Buff
    {
        public int radius;
        public string buffName;
        protected Buff _buff;

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            if (!string.IsNullOrEmpty(buffName))
                _buff = ResourcesBase.GetBuff(buffName);
        }

        public override bool IsNotDebuff()
        {
            return true;
        }
        
    }
}