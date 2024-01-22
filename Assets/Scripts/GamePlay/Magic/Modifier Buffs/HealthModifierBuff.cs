using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class HealthModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _floatModifier.Operation = Operation;
            _controllableCharacter.healthModifiers.Add(_floatModifier);
            modifierInstanceId = _floatModifier.id;
        }
        
        public override void OnExpired(Buff buff)
        {
            _controllableCharacter.healthModifiers.Remove(_floatModifier);
            base.OnExpired(buff);
        }
    }
}