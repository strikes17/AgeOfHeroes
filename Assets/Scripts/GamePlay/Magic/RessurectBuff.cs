using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class RessurectBuff : Buff
    {
        public int healthValue;
        public bool changeCharacter;
        [NonSerialized] protected Corpse _targetCorpse;
        [NonSerialized] protected CharacterObject _characterObject;
        [NonSerialized] protected Player playerSummoner;
        [NonSerialized] protected Vector2Int position;

        public override bool IsNotDebuff()
        {
            return true;
        }

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            var p = ControllableCharacter.V3Int(magicSpellCombatData.target.transform.position);
            position = new Vector2Int(p.x, p.y);
            playerSummoner = magicSpellCombatData.source.gameObject.GetComponent<ControllableCharacter>().Player;
            _characterObject = (CharacterObject)magicSpellCombatData.target.GetComponent<ControllableCharacter>()
                .BaseCharacterObject;
        }

        protected virtual void ChangeHealthValueOfCharacter(ControllableCharacter character, int value)
        {
            character.ChangeHealthValue(value);
        }

        public override void UpdateState()
        {
            bool isTargetCorpse = _target.GetComponent<Corpse>() != null;
            if (isTargetCorpse)
            {
                int maxHealth = _characterObject.startingHealth;
                int count = healthValue / maxHealth + 1;
                int healthLeft = healthValue % maxHealth;
                var character = GameManager.Instance.SpawnManager.SpawnCharacter(_characterObject,
                    _characterObject.Fraction, playerSummoner.Color, position,
                    playerSummoner.ActiveTerrain, count);
                ChangeHealthValueOfCharacter(character, -(maxHealth - healthLeft));
                OnExpired(this);
                _target.Character.CreateBuff(ResourcesBase.GetBuff("destroy_corpse1")).Apply();
            }
            else
            {
                ChangeHealthValueOfCharacter(_target.Character, healthValue);
            }
            Debug.Log(healthValue);
            base.UpdateState();
        }
    }
}