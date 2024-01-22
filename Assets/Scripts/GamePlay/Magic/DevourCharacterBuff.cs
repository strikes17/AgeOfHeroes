using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class DevourCharacterBuff : Buff
    {
        public float healthMultiplier;

        public override bool IsNotDebuff()
        {
            return false;
        }

        public override void UpdateState()
        {
            bool isTargetCorpse = _target.GetComponent<Corpse>() != null;
            bool isTargetCharacter = _target.GetComponent<ControllableCharacter>() != null;
            var targetCharacter = _target.Character;
            var healthValue = (int)(healthMultiplier * targetCharacter.SpawnedCount * targetCharacter.BaseCharacterObject.startingHealth);
            Debug.Log($"{_caster.Character.title} feast: {healthValue}");
            if (isTargetCorpse)
            {
                targetCharacter.CreateBuff(ResourcesBase.GetBuff("destroy_corpse1")).Apply();
            }
            else if (isTargetCharacter)
            {
                targetCharacter.ChangeHealthValue((int)-99999, _caster.Character);
            }

            _caster.Character.ChangeHealthValue(healthValue);

            base.UpdateState();
        }
    }
}