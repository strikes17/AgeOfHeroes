using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class HeroInventoryManager
    {
        public delegate void OnArtifactDelegate(Artifact artifactBehaviour);

        public event OnArtifactDelegate ArtifactAdded
        {
            add => artifactAdded += value;
            remove => artifactAdded -= value;
        }
        private event OnArtifactDelegate artifactAdded;
        
        public event OnArtifactDelegate ArtifactRemoved
        {
            add => artifactRemoved += value;
            remove => artifactRemoved -= value;
        }
        private event OnArtifactDelegate artifactRemoved;
        
        private List<Artifact> _equipedArtifacts = new List<Artifact>();

        public List<Artifact> equipedArtifacts => _equipedArtifacts;

        public bool AddArtifact(Artifact artifact)
        {
            Debug.Log($"Adding artifact {artifact.ArtifactObject.Title} instance ");
            var artifactsCount = _equipedArtifacts.Count;
            if (artifactsCount >= GlobalVariables.MaxArtifactsOnHero)
            {
                return false;
            }
            _equipedArtifacts.Add(artifact);
            Debug.Log("added invoked");
            artifactAdded?.Invoke(artifact);
            return true;
        }

        public bool HasFreeSlots()
        {
            return _equipedArtifacts.Count < GlobalVariables.MaxArtifactsOnHero;
        }

        public void RemoveArtifact(Artifact artifact)
        {
            _equipedArtifacts.Remove(artifact);
            artifactRemoved?.Invoke(artifact);
        }
    }
}