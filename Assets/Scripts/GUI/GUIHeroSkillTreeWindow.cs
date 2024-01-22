using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroSkillTreeWindow : GUIDialogueWindow
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private GUICommonDialogueWindow _dialogueWindow;
        [SerializeField] private Transform _view;
        [SerializeField] private GUIHeroSkillTreeColumn _columnPrefab;
        private int lastVisitorId;

        private List<GUIHeroSkillTreeColumn> _columns = new List<GUIHeroSkillTreeColumn>();

        public GUICommonDialogueWindow DialogueWindow => _dialogueWindow;

        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
            _dialogueWindow.DoNotDestroy = true;
            // Hide();
        }
        
        public override void Hide()
        {
            base.Hide();
            _dialogueWindow.Hide();
        }

        private void Clear()
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                Destroy(_columns[i].gameObject);
            }

            _columns.Clear();
        }

        private void Fill(Hero hero)
        {
            var tree = hero.SkillTree.HeroSkills;
            var tierKeys = tree.Keys;
            List<HeroSkill> skills = new List<HeroSkill>();
            foreach (var tier in tierKeys)
            {
                var skillNames = tree[tier].Keys;
                foreach (var skillName in skillNames)
                {
                    var skill = tree[tier][skillName];
                    skills.Add(skill);
                }

                var column = CreateColumn(hero);
                int connectionVariant = 0;
                if (skills.Count == 1)
                {
                    column.CreateSpecializationColumn(skills[0], tier);
                    connectionVariant = 1;
                }
                else
                {
                    column.CreateSkillsColumn(skills, tier);
                    connectionVariant = 2;
                }

                _columns.Add(column);
                column.UpdateState();
                skills.Clear();
            }
        }

        public void UpdateTree()
        {
            foreach (var column in _columns)
            {
                column.UpdateState();
            }
        }

        private GUIHeroSkillTreeColumn CreateColumn(Hero hero)
        {
            var column = GameObject.Instantiate(_columnPrefab, Vector3.zero, Quaternion.identity, _view);
            column.Init(this, hero);
            return column;
        }

        public void Setup(Hero hero)
        {
            var visitorId = hero.SkillTree.GetHashCode();
            if (lastVisitorId != visitorId)
            {
                lastVisitorId = visitorId;
                Clear();
                Fill(hero);
            }

            UpdateTree();
        }
    }
}