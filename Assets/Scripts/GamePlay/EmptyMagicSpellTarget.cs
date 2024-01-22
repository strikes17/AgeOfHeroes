using System;
using System.Collections;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class EmptyMagicSpellTarget : ControllableCharacter
    {
        private void Start()
        {
            Moroutine.Run(IEAutoDestroy());
        }

        private IEnumerator IEAutoDestroy()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }
}