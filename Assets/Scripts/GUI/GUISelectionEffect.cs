using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISelectionEffect : MonoBehaviour
    {
        [SerializeField] private Material selectionMaterial;
        [SerializeField] private List<Image> imagesToApply;
        
        public void ApplyEffect()
        {
            foreach (var img in imagesToApply)
            {
                img.material = selectionMaterial;
            }
        }

        public void RemoveEffect()
        {
            foreach (var img in imagesToApply)
            {
                img.material = null;
            }
        }
    }
}