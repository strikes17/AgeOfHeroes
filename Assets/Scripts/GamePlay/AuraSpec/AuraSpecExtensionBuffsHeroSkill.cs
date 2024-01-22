using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class AuraSpecExtensionBuffsHeroSkill : AuraSpecializationExtensionHeroSkill
    {
        public List<string> onAlliesBuffNames = new List<string>();
        public List<string> onSelfBuffNames = new List<string>();
        private List<Buff> _onAlliesBuffSources, _onSelfBuffSources;
        private Dictionary<ControllableCharacter, List<Buff>> _onAlliesBuffInstances;
        private Dictionary<ControllableCharacter, List<Buff>> _onSelfBuffInstances;
        private ControllableCharacter _self;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            _self = learntHeroSkillData.Hero;
            _onAlliesBuffSources = new List<Buff>();
            foreach (var buffName in onAlliesBuffNames)
            {
                var buffSource = ResourcesBase.GetBuff(buffName);
                _onAlliesBuffSources.Add(buffSource.Clone() as Buff);
            }

            _onSelfBuffSources = new List<Buff>();
            foreach (var buffName in onSelfBuffNames)
            {
                var buffSource = ResourcesBase.GetBuff(buffName);
                _onSelfBuffSources.Add(buffSource.Clone() as Buff);
            }

            _onSelfBuffInstances = new Dictionary<ControllableCharacter, List<Buff>>();
            _onAlliesBuffInstances = new Dictionary<ControllableCharacter, List<Buff>>();
            base.OnLearnt(learntHeroSkillData);
        }

        protected override void EnteredZone(ControllableCharacter controllableCharacter)
        {
            // base.EnteredZone(controllableCharacter);
            // if (_auraSpecializationHeroSkill.NeighboursCount > _auraSpecializationHeroSkill.neighboursLimit - 1) return;
            List<Buff> _alliesBuffClones = new List<Buff>();
            foreach (var buff in _onAlliesBuffSources)
            {
                var buffClone = controllableCharacter.CreateBuff(buff);
                _alliesBuffClones.Add(buffClone);
                buffClone.Apply();
            }

            _onAlliesBuffInstances.TryAdd(controllableCharacter, _alliesBuffClones);

            List<Buff> _selfBuffClones = new List<Buff>();
            foreach (var buff in _onSelfBuffSources)
            {
                var buffClone = _self.CreateBuff(buff);
                _selfBuffClones.Add(buffClone);
                buffClone.Apply();
            }

            _onSelfBuffInstances.TryAdd(controllableCharacter, _selfBuffClones);
        }

        protected override void ExitedZone(ControllableCharacter controllableCharacter)
        {
            // base.ExitedZone(controllableCharacter);
            List<Buff> alliesInstancesList = null;
            if (_onAlliesBuffInstances.TryGetValue(controllableCharacter, out alliesInstancesList))
            {
                for (int i = 0; i < alliesInstancesList.Count; i++)
                {
                    controllableCharacter.DestroyBuff(alliesInstancesList[i]);
                }

                _onAlliesBuffInstances.Remove(controllableCharacter);
            }

            List<Buff> selfInstancesList = null;
            if (_onSelfBuffInstances.TryGetValue(controllableCharacter, out selfInstancesList))
            {
                for (int i = 0; i < selfInstancesList.Count; i++)
                {
                    _self.DestroyBuff(selfInstancesList[i]);
                }

                _onSelfBuffInstances.Remove(controllableCharacter);
            }
        }
    }
}