using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class Artifact : ICloneable
    {
        public ArtifactObject ArtifactObject;
        public List<ArtifactModifier> Modifiers = new List<ArtifactModifier>();
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}