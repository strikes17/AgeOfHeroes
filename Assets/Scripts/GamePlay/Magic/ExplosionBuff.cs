using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class ExplosionBuff : Buff
    {
        public int radius;
        public int baseDamage, damagePerCaster;
        public string _appliedBuffOnTargetsName;
        protected DamageBuff _appliedBuffOnTargets;
  
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            if (!string.IsNullOrEmpty(_appliedBuffOnTargetsName))
                _appliedBuffOnTargets = ResourcesBase.GetBuff(_appliedBuffOnTargetsName).Clone() as DamageBuff;
        }
        
        public override bool IsNotDebuff()
        {
            return true;
        }

        public override void UpdateState()
        {
            var buffTarget = _target.Character;
            var charactersInRange = buffTarget.GetAllCharactersInRange(radius,
                GlobalVariables.GetCharactersDefaultPersona(MagicSpellAllowedTarget.Enemy), false);
            int totalDamage = baseDamage + damagePerCaster * _caster.Character.Count;
            foreach (var character in charactersInRange)
            {
                if (!character.IsAnAllyTo(_caster.Character))
                {
                    var damageBuffInstance = character.CreateBuff(_appliedBuffOnTargets) as DamageBuff;
                    damageBuffInstance.value = totalDamage;
                    damageBuffInstance.Apply();
                }
            }
            buffTarget.ChangeHealthValue(-totalDamage, _caster.Character);
            base.UpdateState();
        }
    }
}