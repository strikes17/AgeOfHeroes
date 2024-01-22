using System.Collections.Generic;
using AgeOfHeroes.Spell;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableHero : SerializableCharacter
    {
        public int totalExperience;
        public SerializableHeroSkillTree SerializableHeroSkillTree;
        public List<SerializableArtifact> EquipedArtifacts = new List<SerializableArtifact>();

        public SerializableHero() { }

        public SerializableHero(MapEditorHero mapEditorHero)
        {
            var heroObject = mapEditorHero.HeroObject;
            Fraction = mapEditorHero.fraction;
            PlayerOwnerColor = mapEditorHero.playerColor;
            flipX = mapEditorHero.SpriteRenderer.flipX;
            flipY = mapEditorHero.SpriteRenderer.flipY;
            healthLeft = heroObject.startingHealth;
            objectName = heroObject.name;
            positionX = mapEditorHero.Position.x;
            positionY = mapEditorHero.Position.y;
            healthLeft = heroObject.startingHealth;
            movePointsLeft = heroObject.startingMovementPoints;
            retilationsLeft = heroObject.retilationsCount;
            attacksLeft = heroObject.attacksCount;
            manaLeft = heroObject.startingMana;
            isActive = true;
            isVisuallyHidden = false;
        }

        public void InitFromObject(HeroObject heroObject)
        {
            objectName = heroObject.name;
            Fraction = heroObject.Fraction;
            healthLeft = heroObject.startingHealth;
            objectName = heroObject.name;
            healthLeft = heroObject.startingHealth;
            movePointsLeft = heroObject.startingMovementPoints;
            retilationsLeft = heroObject.retilationsCount;
            attacksLeft = heroObject.attacksCount;
            manaLeft = heroObject.startingMana;
        }

        public SerializableHero(Hero hero) : base(hero)
        {
            totalExperience = hero.TotalExperience;
            SerializableHeroSkillTree = new SerializableHeroSkillTree(hero.SkillTree);
            var artifacts = hero.inventoryManager.equipedArtifacts;
            foreach (var artifact in artifacts)
            {
                var serializableArtifact = new SerializableArtifact(artifact);
                EquipedArtifacts.Add(serializableArtifact);
            }
        }
    }


}