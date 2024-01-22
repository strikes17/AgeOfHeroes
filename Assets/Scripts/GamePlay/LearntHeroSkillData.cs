namespace AgeOfHeroes
{
    public class LearntHeroSkillData
    {
        public LearntHeroSkillData(Hero hero, HeroSkill heroSkill, HeroSkillType heroSkillType, int tier)
        {
            _heroSkill = heroSkill;
            _tier = tier;
            _heroSkillType = heroSkillType;
            _hero = hero;
        }


        public HeroSkill Skill => _heroSkill;

        public int Tier => _tier;

        public Hero Hero => _hero;

        public HeroSkillType SkillType => _heroSkillType;
        
        public bool SilentMode { get; set; }
        private HeroSkillType _heroSkillType;
        private Hero _hero;
        private HeroSkill _heroSkill;
        private int _tier;
    }
}