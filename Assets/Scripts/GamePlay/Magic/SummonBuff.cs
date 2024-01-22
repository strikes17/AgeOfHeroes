using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SummonBuff : Buff
    {
        public int healthValue;
        public Fraction summonFraction;
        public string summonCharacterName;
        [NonSerialized] protected CharacterObject summonCharacterObject;
        [NonSerialized] protected Player playerSummoner;
        [NonSerialized] protected Vector3 position;

        public override bool IsNotDebuff()
        {
            return true;
        }

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            position = magicSpellCombatData.target.transform.position;
            playerSummoner = magicSpellCombatData.source.gameObject.GetComponent<ControllableCharacter>().Player;
            summonCharacterObject = ResourcesBase.GetCharacterObject(summonCharacterName, summonFraction);
        }

        public override void Apply()
        {
            var character = GameManager.Instance.SpawnManager.SpawnCharacter(summonCharacterName, summonCharacterObject.Fraction,
             playerSummoner.Color, ControllableCharacter.V3Int(position), playerSummoner.ActiveTerrain);
            character.ChangeHealthValue(healthValue);
            OnExpired(this);
        }
    }
}