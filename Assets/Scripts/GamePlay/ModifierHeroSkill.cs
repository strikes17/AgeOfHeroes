using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class ModifierHeroSkill : HeroSkill, ICloneable
    {
        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            learntHeroSkillData.Hero.SkillTree.LearntModifiersSkills.TryAdd(internalName, this);
        }

        public virtual void Modify(LearntHeroSkillData learntHeroSkillData)
        {
            
        }
    }
}