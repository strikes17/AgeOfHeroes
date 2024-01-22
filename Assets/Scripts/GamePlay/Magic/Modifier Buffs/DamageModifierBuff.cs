using System.Linq;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class DamageModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            var floatModifier = new FloatModifier(value);
            floatModifier.Operation = Operation;
            _controllableCharacter.damageModifiers.Add(floatModifier);
            modifierInstanceId = floatModifier.id;
        }

        public override void OnExpired(Buff buff)
        {
            var modifier = _controllableCharacter.damageModifiers.Where(x => x.id == modifierInstanceId).FirstOrDefault();
            _controllableCharacter.damageModifiers.Remove(modifier);
            base.OnExpired(buff);
        }
    }
}