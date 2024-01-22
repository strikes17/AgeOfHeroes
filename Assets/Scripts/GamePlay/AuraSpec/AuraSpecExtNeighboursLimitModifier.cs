namespace AgeOfHeroes
{
    public class AuraSpecExtNeighboursLimitModifier : AuraSpecializationExtensionHeroSkill
    {
        public int newNeighbourLimit;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            _auraSpecializationHeroSkill.neighboursLimit = newNeighbourLimit;
        }
    }
}