using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class AuraSpecializationHeroSkill : HeroSkill
    {

        public List<string> onAlliesBuffNames = new List<string>();
        public List<string> onSelfBuffNames = new List<string>();
        private List<ModifierBuff> _onAlliesBuffSources, _onSelfBuffSources;
        private Dictionary<ControllableCharacter, List<ModifierBuff>> _onAlliesBuffInstances;
        private Dictionary<ControllableCharacter, List<ModifierBuff>> _onSelfBuffInstances;
        private Hero _hero;
        public bool tierDependent;
        public int neighboursLimit;
        private List<ControllableCharacter> _oldNeighbours;
        private int _neighboursCount;

        public int NeighboursCount => _neighboursCount;

        private void OnEnteredZone(ControllableCharacter controllableCharacter)
        {
            if(_neighboursCount > neighboursLimit - 1)return;
            _neighboursCount++;
            List<ModifierBuff> _alliesBuffClones = new List<ModifierBuff>();
            foreach (var buff in _onAlliesBuffSources)
            {
                var buffClone = controllableCharacter.CreateBuff(buff) as ModifierBuff;
                if (tierDependent) buffClone.Modifier.value = controllableCharacter.GetTier();
                _alliesBuffClones.Add(buffClone);
                buffClone.Apply();
            }
            _onAlliesBuffInstances.TryAdd(controllableCharacter, _alliesBuffClones);
            
            List<ModifierBuff> _selfBuffClones = new List<ModifierBuff>();
            foreach (var buff in _onSelfBuffSources)
            {
                var buffClone = _hero.CreateBuff(buff) as ModifierBuff;
                if (tierDependent) buffClone.Modifier.value = controllableCharacter.GetTier();
                _selfBuffClones.Add(buffClone);
                buffClone.Apply();
            }
            _onSelfBuffInstances.TryAdd(controllableCharacter, _selfBuffClones);
            enteredZone?.Invoke(controllableCharacter);
        }

        private void OnExitedZone(ControllableCharacter controllableCharacter)
        {
            _neighboursCount--;
            List<ModifierBuff> alliesInstancesList = null;
            if (_onAlliesBuffInstances.TryGetValue(controllableCharacter, out alliesInstancesList))
            {
                for (int i = 0; i < alliesInstancesList.Count; i++)
                {
                    controllableCharacter.DestroyBuff(alliesInstancesList[i]);
                }
                _onAlliesBuffInstances.Remove(controllableCharacter);
            }
            
            List<ModifierBuff> selfInstancesList = null;
            if (_onSelfBuffInstances.TryGetValue(controllableCharacter, out selfInstancesList))
            {
                for (int i = 0; i < selfInstancesList.Count; i++)
                {
                    _hero.DestroyBuff(selfInstancesList[i]);
                }
                _onSelfBuffInstances.Remove(controllableCharacter);
            }
            exitedZone?.Invoke(controllableCharacter);
        }

        public event ControllableCharacter.OnCharacterEventDelegate EnteredZone
        {
            add => enteredZone += value;
            remove => enteredZone -= value;
        }

        public event ControllableCharacter.OnCharacterEventDelegate ExitedZone
        {
            add => exitedZone += value;
            remove => exitedZone -= value;
        }

        private event ControllableCharacter.OnCharacterEventDelegate enteredZone, exitedZone;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            _oldNeighbours = new List<ControllableCharacter>();
            _hero = learntHeroSkillData.Hero;
            _onAlliesBuffSources = new List<ModifierBuff>();
            foreach (var buffName in onAlliesBuffNames)
            {
                var buffSource = ResourcesBase.GetBuff(buffName);
                _onAlliesBuffSources.Add(buffSource.Clone() as ModifierBuff);
            }
            
            _onSelfBuffSources = new List<ModifierBuff>();
            foreach (var buffName in onSelfBuffNames)
            {
                var buffSource = ResourcesBase.GetBuff(buffName);
                _onSelfBuffSources.Add(buffSource.Clone() as ModifierBuff);
            }

            _onSelfBuffInstances = new Dictionary<ControllableCharacter, List<ModifierBuff>>();
            _onAlliesBuffInstances = new Dictionary<ControllableCharacter, List<ModifierBuff>>();
        }

        public void RefreshBuff()
        {
            foreach (var controllableCharacter in _oldNeighbours)
            {
                OnExitedZone(controllableCharacter);
            }
            _oldNeighbours.Clear();
        }
        
        public override void Update()
        {
            var newNeighbours = _hero.CheckNeighbourCellsForCharacters(_hero.transform.position,
                GlobalVariables.QuadNeighbours, (long)MagicSpellAllowedTarget.Ally);
            foreach (var newN in newNeighbours)
            {
                if (!_oldNeighbours.Contains(newN))
                {
                    _oldNeighbours.Add(newN);
                    OnEnteredZone(newN);
                }
            }

            for (int i = 0; i < _oldNeighbours.Count; i++)
            {
                var oldN = _oldNeighbours[i];
                if (!newNeighbours.Contains(oldN))
                {
                    _oldNeighbours.Remove(oldN);
                    OnExitedZone(oldN);
                }
            }
        }
    }
}