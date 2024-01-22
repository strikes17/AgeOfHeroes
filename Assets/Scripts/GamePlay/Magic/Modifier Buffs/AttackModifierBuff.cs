using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class AttackModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _floatModifier.Operation = Operation;
            _controllableCharacter.attackModifiers.Add(_floatModifier);
            modifierInstanceId = _floatModifier.id;
        }
        
        public override void OnExpired(Buff buff)
        {
            _controllableCharacter.attackModifiers.Remove(_floatModifier);
            base.OnExpired(buff);
        }
    }
}