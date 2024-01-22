using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public abstract class MagicSpellObject : ICloneable
    {
        public string internalName, title, description;
        public long allowedTarget;
        public int castRange;
        public bool selfCast;
        public int baseCooldown, baseChargesCount, baseManaCost;
        public string iconName;
        [NonSerialized] public Sprite Icon;

        [JsonIgnore]
        public Sprite SpriteIcon
        {
            get
            {
                Icon = Icon == null ? ResourcesBase.GetSprite(iconName) : Icon;
                return Icon;
            }
        }
       
        public List<string> appliedNegativeBuffsOnCaster = new List<string>();
        public List<string> appliedPositiveBuffsOnTarget = new List<string>();
        public List<string> appliedPositiveBuffsOnCaster = new List<string>();
        public List<string> appliedNegativeBuffsOnTarget = new List<string>();
        public List<string> affectsOnlyTheseCharacters = new List<string>();
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    
    public class SingleTargetedMagicSpellObject : MagicSpellObject
    {
        [NonSerialized] public AbstractMagicSpellTarget _magicSpellTarget;
    }

    public class AreaTargetedMagicSpellObject : MagicSpellObject
    {
        public int maxRange;
        public int rangeSlices;
        public int powerReducePerSlice;
        public bool powerDependsOnDistance;
        [NonSerialized] public List<AbstractMagicSpellTarget> _spellTargets;
    }


}