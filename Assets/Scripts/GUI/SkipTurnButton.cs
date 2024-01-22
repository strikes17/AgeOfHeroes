using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class SkipTurnButton : GUIButtonWidget
    {
        private MapScenarioHandler _mapScenarioHandler;

        protected override void Awake()
        {
            base.Awake();
            _mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
        }

        public void SkipTurnHuman()
        {
            if (Interactable)
                _button.onClick.Invoke();
        }

        public void SkipTurnAI()
        {
            if(InteractableForAI)
                _button.onClick.Invoke();
        }
    }
}