using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "Hero Object", menuName = "Create New Hero Object", order = 0)]
    public class HeroObject : ControllableCharacterObject
    {
        public Sprite portraitIcon;
        public string personalName;
        public string skillTreeObjectName;
        public int maxLevel;
        public List<int> experiencePerLevel = new List<int>();
    }
}