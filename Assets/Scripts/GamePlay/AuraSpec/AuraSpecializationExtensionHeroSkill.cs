namespace AgeOfHeroes
{
    public abstract class AuraSpecializationExtensionHeroSkill : HeroSkill
    {
        public string baseSpecializationInternalName;
        protected AuraSpecializationHeroSkill _auraSpecializationHeroSkill;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            _auraSpecializationHeroSkill = learntHeroSkillData.Hero.SkillTree.GetLearntSkill(baseSpecializationInternalName) as AuraSpecializationHeroSkill;
            UpdateSpecializationBuffs();
            _auraSpecializationHeroSkill.EnteredZone += EnteredZone;
            _auraSpecializationHeroSkill.ExitedZone += ExitedZone;
            _auraSpecializationHeroSkill.RefreshBuff();
        }

        protected virtual void UpdateSpecializationBuffs()
        {
        }

        protected virtual void EnteredZone(ControllableCharacter controllableCharacter)
        {
        }

        protected virtual void ExitedZone(ControllableCharacter controllableCharacter)
        {
        }
    }
}