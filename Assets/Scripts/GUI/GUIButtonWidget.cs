using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public abstract class GUIButtonWidget : GUIBaseWidget
    {
        protected Button _button;
        protected bool _interactable;
        
        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            Interactable = true;
        }
        
        public bool InteractableForAI { get; set; }

        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                _button.interactable = _interactable;
                if (_interactable)
                    _button.image.color = Colors.whiteColor;
                else
                    _button.image.color = Colors.unavailableElementDarkGray;
            }
        }
        
        public void AddListener(UnityAction action)
        {
            _button.onClick.AddListener(action);
        }

        public void RemoveAllListeners()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}