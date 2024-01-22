namespace AgeOfHeroes
{
    public class AuraSpecExtensionModifierHeroSkill : AuraSpecializationExtensionHeroSkill
    {
        public int operationOnSelf, operationOnAllies;
        public float valueOnSelf, valueOnAllies;
        
        protected override void UpdateSpecializationBuffs()
        {
            // var selfBaseValue = _auraSpecializationHeroSkill.AppliedBuffOnSelf.BaseValue;
            // float selfValue = _auraSpecializationHeroSkill.AppliedBuffOnSelf.value;
            // selfValue = operationOnSelf == 0 ? selfBaseValue * valueOnSelf : selfValue + valueOnSelf;
            // _auraSpecializationHeroSkill.AppliedBuffOnSelf.value = selfValue;
            //
            // var allyBaseValue = _auraSpecializationHeroSkill.AppliedBuffOnAllies.BaseValue;
            // float allyValue = _auraSpecializationHeroSkill.AppliedBuffOnAllies.value;
            // allyValue = operationOnAllies == 0 ? allyBaseValue * valueOnAllies : allyValue + valueOnAllies;
            // _auraSpecializationHeroSkill.AppliedBuffOnAllies.value = allyValue;
        }
    }
}