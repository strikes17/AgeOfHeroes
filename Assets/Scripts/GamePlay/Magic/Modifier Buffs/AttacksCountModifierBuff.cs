using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class AttacksCountModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            var floatModifier = new FloatModifier(value);
            floatModifier.Operation = Operation;
            _controllableCharacter.attacksCountModifiers.Add(floatModifier);
            modifierInstanceId = floatModifier.id;
        }

        public override void OnExpired(Buff buff)
        {
            var modifier = _controllableCharacter.attacksCountModifiers.Where(x => x.id == modifierInstanceId).FirstOrDefault();
            _controllableCharacter.attacksCountModifiers.Remove(modifier);
            base.OnExpired(buff);
        }
    }
}