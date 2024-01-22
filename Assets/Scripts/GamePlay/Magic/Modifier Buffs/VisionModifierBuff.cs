using System.Linq;

namespace AgeOfHeroes.Spell
{
    public class VisionModifierBuff : ModifierBuff
    {
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            var m = new FloatModifier(value);
            m.Operation = Operation;
            _controllableCharacter.visionModifiers.Add(m);
            modifierInstanceId = m.id;
        }
        
        public override void OnExpired(Buff buff)
        {
            var modifier = _controllableCharacter.visionModifiers.Where(x => x.id == modifierInstanceId).FirstOrDefault();
            _controllableCharacter.visionModifiers.Remove(modifier);
            base.OnExpired(buff);
        }
    }
}