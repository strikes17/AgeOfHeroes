using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public class HeroSkillTree
    {

        public Dictionary<int, Dictionary<string, HeroSkill>> HeroSkills => _heroSkills;

        public Dictionary<string, HeroSkill> LearntSkills => _learntSkills;
        public Dictionary<string, ModifierHeroSkill> LearntModifiersSkills => _learntModifierSkills;

        public Dictionary<int, int> PointsPerTier => _pointsPerTier;

        private Dictionary<int, Dictionary<string, HeroSkill>> _heroSkills =
            new Dictionary<int, Dictionary<string, HeroSkill>>();

        private Dictionary<string, HeroSkill> _learntSkills = new Dictionary<string, HeroSkill>();
        private Dictionary<string, ModifierHeroSkill> _learntModifierSkills = new Dictionary<string, ModifierHeroSkill>();

        private Dictionary<int, int> _pointsPerTier = new Dictionary<int, int>();

        private int _levelPoints;

        public int LevelPoints
        {
            get => _levelPoints;
            set => _levelPoints = value;
        }

        public event HeroSkill.OnHeroSkillEventDelegate SkillLearnt
        {
            add => skillLearnt += value;
            remove => skillLearnt -= value;
        }

        protected event HeroSkill.OnHeroSkillEventDelegate skillLearnt;

        public void LoadFromSerializable(Hero hero, SerializableHeroSkillTree serializableHeroSkillTree)
        {
            _levelPoints = serializableHeroSkillTree.LevelPoints;
            var learntSkills = serializableHeroSkillTree.LearntSkills;
            foreach (var heroSkillName in learntSkills)
            {
                var heroSkill = ResourcesBase.GetHeroSkill(heroSkillName);
                var heroSkillInstance = heroSkill.Clone() as HeroSkill;
                _learntSkills.TryAdd(heroSkillName, heroSkillInstance);
                LearntHeroSkillData learntHeroSkillData = new LearntHeroSkillData(hero, heroSkill, HeroSkillType.Common, 0);
                learntHeroSkillData.SilentMode = true;
                heroSkillInstance.OnLearnt(learntHeroSkillData);
            }

            var pointsPerTier = serializableHeroSkillTree.PointsPerTier;
            foreach (var p in pointsPerTier.Keys)
            {
                _pointsPerTier[p] = pointsPerTier[p];
            }
        }

        public HeroSkillTree(HeroSkillTreeObject heroSkillTreeObject)
        {
            var tree = heroSkillTreeObject.HeroSkills;
            var keys = tree.Keys;
            foreach (var tier in keys)
            {
                _pointsPerTier.TryAdd(tier, 0);
                _heroSkills.TryAdd(tier, new Dictionary<string, HeroSkill>());
                foreach (var skillName in tree[tier])
                {
                    var heroSkill = ResourcesBase.GetHeroSkill(skillName);
                    var heroSkillInstance = heroSkill.Clone() as HeroSkill;
                    heroSkillInstance.Init();
                    heroSkillInstance.Learnt += OnSkillLearnt;
                    _heroSkills[tier].TryAdd(skillName, heroSkillInstance);
                }
            }
        }

        public int GetPoints(int tier)
        {
            int points = 0;
            points = _pointsPerTier[tier];
            return points;
        }

        public void ChangePoints(int tier, int points)
        {
            if (tier == 0) return;
            _pointsPerTier[tier] += points;
        }

        public void Update()
        {
            var keys = _learntSkills.Keys;
            foreach (var key in keys)
            {
                _learntSkills[key].Update();
            }
        }

        private void OnSkillLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            int points = learntHeroSkillData.SkillType == HeroSkillType.Common ? 1 : 3;
            int tier = learntHeroSkillData.Tier;
            ChangePoints(tier, points);
            ChangePoints(tier - 1, -1);
            _learntSkills.TryAdd(learntHeroSkillData.Skill.internalName, learntHeroSkillData.Skill);
            skillLearnt?.Invoke(learntHeroSkillData);
        }

        public bool HasLearntSkill(string internalName)
        {
            return _learntSkills.ContainsKey(internalName);
        }

        public HeroSkill GetLearntSkill(string internalName)
        {
            HeroSkill heroSkill = null;
            if (_learntSkills.TryGetValue(internalName, out heroSkill))
            {
                return heroSkill;
            }

            return null;
        }
    }
}