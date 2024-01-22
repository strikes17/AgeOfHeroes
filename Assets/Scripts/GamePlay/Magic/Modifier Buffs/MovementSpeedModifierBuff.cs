using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class MovementSpeedModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            var floatModifier = new FloatModifier(value);
            floatModifier.Operation = Operation;
            _controllableCharacter.moveSpeedModifiers.Add(floatModifier);
            modifierInstanceId = floatModifier.id;

        }

        public override void OnExpired(Buff buff)
        {
            var modifier = _controllableCharacter.moveSpeedModifiers.Where(x => x.id == modifierInstanceId).FirstOrDefault();
            _controllableCharacter.moveSpeedModifiers.Remove(modifier);
            base.OnExpired(buff);
        }
    }
}