using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableCharacter : SerializableEntity
    {
        public int quantity;
        public Fraction Fraction;
        public PlayerColor PlayerOwnerColor;
        public bool flipX, flipY;
        public bool isActive, isVisuallyHidden;

        public List<SerializableBuff> appliedPositiveBuffs = new List<SerializableBuff>(),
            appliedNegativeBuffs = new List<SerializableBuff>();

        public int movePointsLeft, retilationsLeft, attacksLeft, manaLeft, shootsLeft, healthLeft;
        public SerializableSpellBook SerializableSpellBook;

        public SerializableCharacter()
        {
        }

        public SerializableCharacter(MapEditorCharacter mapEditorCharacter)
        {
            var characterObject = mapEditorCharacter.CharacterObject;
            UniqueId = 0;
            quantity = mapEditorCharacter.countInStack;
            Fraction = mapEditorCharacter.fraction;
            PlayerOwnerColor = mapEditorCharacter.PlayerOwnerColor;
            flipX = mapEditorCharacter.SpriteRenderer.flipX;
            flipY = mapEditorCharacter.SpriteRenderer.flipY;
            healthLeft = characterObject.startingHealth;
            movePointsLeft = characterObject.startingMovementPoints;
            retilationsLeft = characterObject.retilationsCount;
            attacksLeft = characterObject.attacksCount;
            manaLeft = characterObject.startingMana;
            shootsLeft = characterObject.shoots;
            objectName = characterObject.name;
            positionX = mapEditorCharacter.Position.x;
            positionY = mapEditorCharacter.Position.y;
            isActive = true;
            isVisuallyHidden = false;
        }

        public SerializableCharacter(ControllableCharacter controllableCharacter)
        {
            UniqueId = controllableCharacter.UniqueId;
            isActive = controllableCharacter.gameObject.activeSelf;
            isVisuallyHidden = !controllableCharacter._spriteRenderer.enabled;
            quantity = controllableCharacter.Count;
            Fraction = controllableCharacter.fraction;
            PlayerOwnerColor = controllableCharacter.playerOwnerColor;
            flipX = controllableCharacter._spriteRenderer.flipX;
            flipY = controllableCharacter._spriteRenderer.flipY;
            var positiveBuffs = controllableCharacter.AppliedPositiveBuffs;
            foreach (var buff in positiveBuffs)
            {
                if (!buff.savingState)
                    continue;
                SerializableBuff serializableBuff = new SerializableBuff(buff);
                appliedPositiveBuffs.Add(serializableBuff);
            }

            var negativeBuffs = controllableCharacter.AppliedNegativeBuffs;
            foreach (var buff in negativeBuffs)
            {
                if (!buff.savingState)
                    continue;
                SerializableBuff serializableBuff = new SerializableBuff(buff);
                appliedNegativeBuffs.Add(serializableBuff);
            }

            healthLeft = controllableCharacter.HealthLeft;
            movePointsLeft = controllableCharacter.MovementPointsLeft;
            retilationsLeft = controllableCharacter.RetilationsLeft;
            attacksLeft = controllableCharacter.AttacksLeft;
            manaLeft = controllableCharacter.ManaLeft;
            shootsLeft = controllableCharacter.ShootsLeft;
            objectName = controllableCharacter.BaseCharacterObject.name;
            positionX = controllableCharacter.Position.x;
            positionY = controllableCharacter.Position.y;
            var spellBook = controllableCharacter.spellBook;
            if (spellBook != null)
                SerializableSpellBook = new SerializableSpellBook(spellBook);
        }
    }
}