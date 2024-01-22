using System;
using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SingleTargetedMagicSpell : MagicSpell
    {
        [NonSerialized] public AbstractMagicSpellTarget _magicSpellTarget;

        public SingleTargetedMagicSpell(SingleTargetedMagicSpellObject magicSpellObject) : base(magicSpellObject)
        {
        }

        public override void CreatePrepareVisuals()
        {
        }

        public override void DestroyPrepareVisuals()
        {
        }

        public override bool TryCastSpell(MagicSpellCombatData _magicSpellCombatData)
        {
            var moroutine = CheckCastConditions(_magicSpellCombatData);
            if (moroutine != null)
            {
                var character = _magicSpellCombatData.source.Character;
                int manaLeft = character.ManaLeft;
                int manaCost = _magicSpellCombatData.magicSpell.ManaCost;
                if (manaLeft < manaCost)
                {
                    Debug.Log($"Not enough mana for {_magicSpellCombatData.magicSpell.title}");
                    return false;
                }

                _magicSpellCombatData.magicSpell.SetOnCooldown();
                character.ManaLeft -= manaCost;
                moroutine.Run();
                return true;
            }

            return false;
        }

        public override Moroutine CheckCastConditions(MagicSpellCombatData _magicSpellCombatData)
        {
            Moroutine moroutine = null;
            var magicSpell = _magicSpellCombatData.magicSpell;
            if (magicSpell == null) return null;
            long nonCharacterBitMask = (long)(MagicSpellAllowedTarget.Prop | MagicSpellAllowedTarget.Building |
                                              MagicSpellAllowedTarget.Empty | MagicSpellAllowedTarget.Tree |
                                              MagicSpellAllowedTarget.Corpse);
            List<Buff> appliedBuffs = new List<Buff>();
            appliedBuffs.AddRange(magicSpell.appliedNegativeBuffsOnTarget);
            appliedBuffs.AddRange(magicSpell.appliedPositiveBuffsOnTarget);
            foreach (var buff in appliedBuffs)
            {
                var success = buff.CheckConditions(_magicSpellCombatData);
                if (!success)
                {
                    Debug.Log($"failed cast at buff conditions {buff.internalName}");
                    return null;
                }
            }

            bool isTargetCharacter = (_magicSpellCombatData.target.persona & nonCharacterBitMask) == 0;
            if (isTargetCharacter)
            {
                var targetCharacterComponent = _magicSpellCombatData.target.GetComponent<ControllableCharacter>();
                var sourceCharacterComponent = _magicSpellCombatData.source.GetComponent<ControllableCharacter>();
                if (targetCharacterComponent != null)
                {
                    bool spellTargetsAllies = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Ally) != 0;
                    bool spellTargetsEnemies = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Enemy) != 0;
                    bool spellTargetsAlives = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Alive) != 0;
                    bool spellTargetsUndead = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Undead) != 0;
                    bool spellTargetsHero = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Hero) != 0;
                    bool spellTargetsMechanism =
                        (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Mechanism) != 0;
                    bool spellTargetsSummon = (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.Summon) != 0;
                    bool spellTargetsUndeadCorpse =
                        (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.UndeadCorpse) != 0;
                    bool spellTargetsAliveCorpse =
                        (magicSpell.allowedTarget & (long)MagicSpellAllowedTarget.AliveCorpse) != 0;

                    bool isTargetAnEnemy = !targetCharacterComponent.IsAnAllyTo(sourceCharacterComponent);
                    bool isTargetSelf = targetCharacterComponent == _magicSpellCombatData.source;

                    long targetMask = _magicSpellCombatData.target.persona;

                    bool isTargetAlive = (targetMask & (long)MagicSpellAllowedTarget.Alive) != 0;
                    bool isTargetUndead = (targetMask & (long)MagicSpellAllowedTarget.Undead) != 0;
                    bool isTargetHero = (targetMask & (long)MagicSpellAllowedTarget.Hero) != 0;
                    bool isTargetMechanism = (targetMask & (long)MagicSpellAllowedTarget.Mechanism) != 0;
                    bool isTargetSummon = (targetMask & (long)MagicSpellAllowedTarget.Summon) != 0;
                    bool isTargetUndeadCorpse = (targetMask & (long)MagicSpellAllowedTarget.UndeadCorpse) != 0;
                    bool isTargetAliveCorpse = (targetMask & (long)MagicSpellAllowedTarget.AliveCorpse) != 0;

                    bool canUseSpell = (spellTargetsAlives && isTargetAlive) ||
                                       (spellTargetsUndead && isTargetUndead) || (spellTargetsHero && isTargetHero) ||
                                       (spellTargetsMechanism && isTargetMechanism) ||
                                       (spellTargetsSummon && isTargetSummon) ||
                                       (spellTargetsUndeadCorpse && isTargetUndeadCorpse) ||
                                       (spellTargetsAliveCorpse && isTargetAliveCorpse);

                    if (!canUseSpell)
                    {
                        Debug.Log($"Target of spell {targetCharacterComponent.name} is not allowed target!");
                        return null;
                    }

                    if (isTargetSelf)
                        if (magicSpell.selfCast)
                        {
                            moroutine = Moroutine.Create(
                                targetCharacterComponent.SelfCastMagicSpell(_magicSpellCombatData));
                            return moroutine;
                        }

                    if (spellTargetsAllies && spellTargetsEnemies)
                    {
                        _magicSpellCombatData.target = targetCharacterComponent;
                        if (isTargetAnEnemy)
                            moroutine = Moroutine.Create(
                                targetCharacterComponent.RecieveOffsensiveMagicSpell(_magicSpellCombatData));
                        else
                            moroutine = Moroutine.Create(
                                targetCharacterComponent.RecieveFriendlyMagicSpell(_magicSpellCombatData));
                        return moroutine;
                    }

                    if (spellTargetsEnemies)
                    {
                        if (isTargetAnEnemy)
                        {
                            _magicSpellCombatData.target = targetCharacterComponent;
                            moroutine = Moroutine.Create(
                                targetCharacterComponent.RecieveOffsensiveMagicSpell(_magicSpellCombatData));
                            return moroutine;
                        }
                        else
                        {
                            Debug.Log($"Target of spell {targetCharacterComponent.name} is not an enemy!");
                        }
                    }
                    else if (spellTargetsAllies)
                    {
                        if (!isTargetAnEnemy)
                        {
                            moroutine = Moroutine.Create(
                                targetCharacterComponent.RecieveFriendlyMagicSpell(_magicSpellCombatData));
                            return moroutine;
                        }
                        else
                        {
                            Debug.Log($"Target of spell {targetCharacterComponent.name} is not an ally!");
                        }
                    }
                }
            }
            else
            {
                var targetMagicSpellTargetComponent =
                    _magicSpellCombatData.target.GetComponent<AbstractMagicSpellTarget>();
                moroutine = Moroutine.Create(
                    targetMagicSpellTargetComponent.RecieveOffsensiveMagicSpell(_magicSpellCombatData));
            }

            return moroutine;
        }
    }
}