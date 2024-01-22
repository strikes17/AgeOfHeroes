using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class ModifyRessurectBuffHeroSkill : ModifySpellHeroSkill
    {
        public int operation;
        public float value;
        public override void Modify(LearntHeroSkillData learntHeroSkillData)
        {
            base.Modify(learntHeroSkillData);
            foreach (var buff in _magicSpell.appliedPositiveBuffsOnTarget)
            {
                Debug.Log(buff.GetHashCode());
                var ressurectBuff = buff as RessurectBuff;
                var val = operation == 0
                    ? ressurectBuff.healthValue * value
                    : ressurectBuff.healthValue + value;
                ressurectBuff.healthValue = Mathf.RoundToInt(val);
                Debug.Log(ressurectBuff.healthValue);
            }
            
            foreach (var buff in _magicSpell.appliedNegativeBuffsOnTarget)
            {
                Debug.Log(buff.GetHashCode());
                var ressurectBuff = buff as RessurectBuff;
                var val = operation == 0
                    ? ressurectBuff.healthValue * value
                    : ressurectBuff.healthValue + value;
                ressurectBuff.healthValue = Mathf.RoundToInt(val);
                Debug.Log(ressurectBuff.healthValue);
            }
        }
    }
}