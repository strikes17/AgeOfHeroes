using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class DefenseModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _floatModifier.Operation = Operation;
            _controllableCharacter.defenseModifiers.Add(_floatModifier);
            modifierInstanceId = _floatModifier.id;
        }
        
        public override void OnExpired(Buff buff)
        {
            _controllableCharacter.defenseModifiers.Remove(_floatModifier);
            base.OnExpired(buff);
        }
    }
}