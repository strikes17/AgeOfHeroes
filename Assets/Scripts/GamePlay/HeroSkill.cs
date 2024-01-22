using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class HeroSkill : ICloneable
    {
        public string title;
        public string internalName;
        public string description;
        public string spriteIconPath;
        [NonSerialized] public Sprite spriteIcon;

        public delegate void OnHeroSkillEventDelegate(LearntHeroSkillData learntHeroSkillData);

        public event OnHeroSkillEventDelegate Learnt
        {
            add => learnt += value;
            remove => learnt -= value;
        }

        protected event OnHeroSkillEventDelegate learnt;

        public virtual void Init()
        {
            spriteIcon = ResourcesBase.GetSprite(spriteIconPath);
        }

        public virtual void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            learnt?.Invoke(learntHeroSkillData);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public virtual void Update()
        {
        }
    }
}