using System.Collections.Generic;
using System.Linq;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableHeroSkillTree
    {
        public int LevelPoints;
        public Dictionary<int, int> PointsPerTier = new Dictionary<int, int>();
        public List<string> LearntSkills = new List<string>();
        
        public SerializableHeroSkillTree(){}

        public SerializableHeroSkillTree(HeroSkillTree heroSkillTree)
        {
            LevelPoints = heroSkillTree.LevelPoints;
            PointsPerTier = heroSkillTree.PointsPerTier;
            LearntSkills = heroSkillTree.LearntSkills.Keys.ToList();
        }
    }
}