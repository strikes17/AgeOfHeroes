using System.Collections;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class AbstractMagicSpellTarget : MonoBehaviour
    {
        public abstract IEnumerator RecieveOffsensiveMagicSpell(MagicSpellCombatData magicSpellCombatData);
        public abstract IEnumerator RecieveFriendlyMagicSpell(MagicSpellCombatData magicSpellCombatData);
        public abstract IEnumerator SelfCastMagicSpell(MagicSpellCombatData magicSpellCombatData);
        public long persona;

        protected List<Buff> appliedNegativeBuffs = new List<Buff>();
        protected List<Buff> appliedPositiveBuffs = new List<Buff>();

        public ControllableCharacter Character => GetComponent<ControllableCharacter>();

        public List<Buff> AllAppliedBuffs
        {
            get
            {
                var appliedBuffs = new List<Buff>();
                appliedBuffs.AddRange(appliedPositiveBuffs);
                appliedBuffs.AddRange(appliedNegativeBuffs);
                return appliedBuffs;
            }
        }

        public List<Buff> AppliedNegativeBuffs => appliedNegativeBuffs;

        public List<Buff> AppliedPositiveBuffs => appliedPositiveBuffs;

        public virtual IEnumerator UpdateBuffsState()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < appliedNegativeBuffs.Count; i++)
            {
                Debug.Log($"buff {appliedNegativeBuffs[i].title} has duration left: {appliedNegativeBuffs[i].durationLeft}");
                appliedNegativeBuffs?[i].UpdateState();
            }

            for (int i = 0; i < appliedPositiveBuffs.Count; i++)
                appliedPositiveBuffs?[i].UpdateState();
        }
    }
}