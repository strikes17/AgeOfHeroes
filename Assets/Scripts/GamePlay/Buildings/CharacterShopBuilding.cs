using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public class CharacterShopBuilding : AbstractBuilding
    {
        private int _level = 1;
        public int tier;
        public int variant;
        private float recruitmentsAvailable;
        public string blockedByBuildingName;
        public string basicCharacterName, eliteCharacterName;
        [NonSerialized] public CharacterObject basicCharacterForm, eliteCharacterForm;
        public int upgradeGoldCost, upgradeGemsCost;
        private int maxRecruitments;

        public int RecruitsAvailableInteger
        {
            get => Mathf.FloorToInt(recruitmentsAvailable);
        }

        public override void Init()
        {
            base.Init();
            maxRecruitments = basicCharacterForm.fullStackCount * GlobalVariables.MaxShopRecruitmentsMultiplier;
        }

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public float RecruitmentsAvailable
        {
            get => recruitmentsAvailable;
            set
            {
                recruitmentsAvailable = Mathf.Clamp(value, 0f, maxRecruitments);
            }
        }
    }
}