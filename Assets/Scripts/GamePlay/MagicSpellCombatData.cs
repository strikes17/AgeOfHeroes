using AgeOfHeroes.Spell;

namespace AgeOfHeroes
{
    public class MagicSpellCombatData : AbstractData
    {
        public AbstractMagicSpellTarget source, target;
        public MagicSpell magicSpell;
    }
}