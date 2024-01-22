using System;
using System.Collections.Generic;

namespace AgeOfHeroes
{
    [Serializable]
    public class CastleInfo : ICloneable
    {
        public string internalName;
        public Fraction Fraction;
        public List<string> Buildings = new List<string>();
        public List<bool> IsBuildingBuilt = new List<bool>();
        public List<bool> IsBuildingRestricted = new List<bool>();
        public List<int> BuildingLevel = new List<int>();

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}