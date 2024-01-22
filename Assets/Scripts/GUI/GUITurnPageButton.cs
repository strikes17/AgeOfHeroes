using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUITurnPageButton : Button
    {
        public bool turnRight;

        public void AddOnClickListener(Action action)
        {
            onClick.AddListener(() => { action.Invoke(); });
        }

        public void ClearAllListeners()
        {
            onClick.RemoveAllListeners();
        }
    }
}