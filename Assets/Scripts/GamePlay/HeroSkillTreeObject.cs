using System;
using System.Collections.Generic;

namespace AgeOfHeroes
{
    [Serializable]
    public class HeroSkillTreeObject
    {
        public string internalName;
        public Dictionary<int, List<string>> HeroSkills => _heroSkills;
        
        private Dictionary<int, List<string>> _heroSkills =
            new Dictionary<int, List<string>>();
    }
}