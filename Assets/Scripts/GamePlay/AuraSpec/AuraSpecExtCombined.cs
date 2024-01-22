using System.Collections.Generic;

namespace AgeOfHeroes
{
    public class AuraSpecExtCombined : AuraSpecializationExtensionHeroSkill
    {
        public List<string> extensionsToCombine = new List<string>();
        
        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            foreach (var extensionName in extensionsToCombine)
            {
                HeroSkill heroSkill = ResourcesBase.GetHeroSkill(extensionName);
                var newHeroSkillData = new LearntHeroSkillData(learntHeroSkillData.Hero, heroSkill,
                    learntHeroSkillData.SkillType, learntHeroSkillData.Tier);
                newHeroSkillData.SilentMode = true;
                heroSkill?.OnLearnt(newHeroSkillData);
            }
        }
    }
}