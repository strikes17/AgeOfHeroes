using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using Mono.Cecil;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "Character Object", menuName = "Create New Character Object", order = 0)]
    public class CharacterObject : ControllableCharacterObject
    {
        public int goldPrice, gemsPrice;
        public float baseGrowthPerDay;
        public int fullStackCount;
        public bool hasEliteForm;
        public bool isAnEliteForm;
        public CharacterObject eliteFormObject;
    }
}