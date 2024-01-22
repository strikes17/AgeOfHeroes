using System;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    [Serializable]
    public class ArtifactModifier : ICloneable
    {
        public HeroModifieableStat StatType;
        public ModifierOperation operation;
        public float value;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}