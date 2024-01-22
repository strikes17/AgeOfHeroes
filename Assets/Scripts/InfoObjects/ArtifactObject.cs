using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "New Artifact Object", menuName = "Create New Artifact Object")]
    public class ArtifactObject : ScriptableObject
    {
        public string Title;
        public Sprite Icon;
        public ArtifactQuality Quality;
        public List<HeroEquipmentPlace> HeroEquipmentPlaces;
        public List<ArtifactModifier> Modifiers;
        public List<string> Spells;
    }
}