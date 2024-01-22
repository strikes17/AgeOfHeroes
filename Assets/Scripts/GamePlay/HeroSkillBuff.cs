using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class HeroSkillBuff : HeroSkill
    {
        public List<string> onSelfBuffNames = new List<string>();
        private List<Buff> _onSelfBuffInstances;
        private List<Buff> _onSelfBuffSources;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            _onSelfBuffSources = new List<Buff>();
            foreach (var buffName in onSelfBuffNames)
            {
                var buffSource = ResourcesBase.GetBuff(buffName);
                _onSelfBuffSources.Add(buffSource);
            }

            _onSelfBuffInstances = new List<Buff>();
            
            foreach (var buff in _onSelfBuffSources)
            {
                var buffClone = learntHeroSkillData.Hero.CreateBuff(buff);
                _onSelfBuffInstances.Add(buffClone);
                buffClone.Apply();
            }
        }
    }
}