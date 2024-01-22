using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class CastleTierSpecialBuilding : SpecialBuilding
    {
        public class CastleGuardInfo
        {
            public int FightingPoints;
            public bool IsElite;
        }

        public int tier;
        public string nextTierUpgrade;
        public int goldIncome;
        public float gemsIncome;
        public float growthMultiplier;
        public Dictionary<int, CastleGuardInfo> StartingGuards = new Dictionary<int, CastleGuardInfo>();

        public override void Init()
        {
            base.Init();
        }

        public override void OnBuilt(Castle castle)
        {
            base.OnBuilt(castle);
            castle.Tier = tier;
            if (string.IsNullOrEmpty(nextTierUpgrade))
                return;
            castle.SpecialBuildings[internalName].IsRestricted = true;
            var nextTierBuilding = castle.SpecialBuildings[nextTierUpgrade];
            nextTierBuilding.IsRestricted = false;
            castle.UpdateGuards(StartingGuards);
        }
    }
}