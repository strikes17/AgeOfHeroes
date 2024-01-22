using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUIHeroSkillTreeColumn : GUIBaseWidget
    {
        private GUIHeroSkillCell _specializationCellPrefab, _connectionCellPrefab, _skillCellPrefab;

        private List<GUIHeroSkillCell> _cells = new List<GUIHeroSkillCell>();

        public List<GUIHeroSkillCell> Cells => _cells;

        private GUIHeroSkillTreeWindow _window;

        private Hero _hero;

        public void Init(GUIHeroSkillTreeWindow window, Hero hero)
        {
            _hero = hero;
            _window = window;
            _specializationCellPrefab = ResourcesBase.GetPrefab("GUI/Skill Tree/Specialization Widget")
                .GetComponent<GUIHeroSkillCell>();
            _skillCellPrefab = ResourcesBase.GetPrefab("GUI/Skill Tree/Skill Widget")
                .GetComponent<GUIHeroSkillCell>();
        }

        public void UpdateState()
        {
            foreach (var cell in _cells)
            {
                if (_hero.SkillTree.HasLearntSkill(cell.Skill.internalName) || _hero.SkillTree.LevelPoints <= 0)
                {
                    cell.SetDisabled();
                }
                else
                {
                    int previousTier = cell.tier - 1;
                    if (previousTier == 0) continue;
                    // Debug.Log($"points on prevTier {previousTier}= {_hero.SkillTree.GetPoints(previousTier)}");
                    bool hasEnoughPoints = _hero.SkillTree.GetPoints(previousTier) > 0;
                    if (hasEnoughPoints)
                        cell.SetEnabled();
                    else 
                        cell.SetDisabled();
                }
            }
        }

        public void CreateSpecializationColumn(HeroSkill heroSkill, int columnTier)
        {
            var instance =
                GameObject.Instantiate(_specializationCellPrefab, Vector3.zero, Quaternion.identity, transform);
            _cells.Add(instance);
            var skillCell = instance.GetComponent<GUIHeroSkillCell>();
            skillCell.tier = columnTier;
            skillCell.HeroSkillType = HeroSkillType.Spec;
            skillCell.Init(heroSkill);
            skillCell.Clicked += OnSkillCellClicked;
        }

        private void OnSkillCellClicked(GUIHeroSkillCell skillCell)
        {
            var dialogueWindow = _window.DialogueWindow;
            dialogueWindow.Description = skillCell.Skill.description;
            dialogueWindow.Title = skillCell.Skill.title;
            dialogueWindow.Applied = () =>
            {
                LearntHeroSkillData heroSkillData =
                    new LearntHeroSkillData(_hero, skillCell.Skill, skillCell.HeroSkillType, skillCell.tier);
                heroSkillData.SilentMode = false;
                skillCell.Skill.OnLearnt(heroSkillData);
                _hero.SkillTree.LevelPoints--;
                _window.UpdateTree();
            };
            dialogueWindow.Show();
        }


        public void CreateConnectionsColumn(int variant)
        {
            for (int i = 0; i < 3; i++)
            {
                var instance = GameObject.Instantiate(_connectionCellPrefab, Vector3.zero, Quaternion.identity,
                    transform);
                _cells.Add(instance);
                var connectionCell = instance.GetComponent<GUIHeroSkillCellConnection>();
                connectionCell.SetConnectionVariant(variant, i);
            }
        }

        public void CreateSkillsColumn(List<HeroSkill> skills, int columnTier)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                var instance = GameObject.Instantiate(_skillCellPrefab, Vector3.zero, Quaternion.identity,
                    transform);
                _cells.Add(instance);
                var heroSkill = skills[i];
                var skillCell = instance.GetComponent<GUIHeroSkillCell>();
                skillCell.HeroSkillType = HeroSkillType.Common;
                skillCell.tier = columnTier;
                skillCell.Init(heroSkill);
                skillCell.Clicked += OnSkillCellClicked;
            }
        }
    }
}