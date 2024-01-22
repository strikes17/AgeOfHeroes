using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class GUIDialogueWindow : MonoBehaviour
    {
        public delegate void GUIWidgetEventDelegate();

        public delegate void OnTreasureEventDelegate();

        public event GUIWidgetEventDelegate Opened
        {
            add => opened += value;
            remove => opened -= value;
        }

        public event GUIWidgetEventDelegate Closed
        {
            add => closed += value;
            remove => closed -= value;
        }
        
        public bool Locked { get; set; }

        private event GUIWidgetEventDelegate closed;
        private event GUIWidgetEventDelegate opened;

        public virtual void Show()
        {
            if(gameObject.activeSelf || Locked)
                return;
            // Debug.Log("Show!");
            gameObject.SetActive(true);
            opened?.Invoke();
        }

        public virtual void Hide()
        {
            if(!gameObject.activeSelf || Locked)
                return;
            // Debug.Log("Hide!");
            gameObject?.SetActive(false);
            closed?.Invoke();
        }

        public virtual void CloseAndDestroy()
        {
            Hide();
            Destroy(gameObject);
        }

        public void Toggle()
        {
            if (gameObject.activeSelf)
                Hide();
            else Show();
        }

        public virtual void Darken()
        {
        }
    }
}