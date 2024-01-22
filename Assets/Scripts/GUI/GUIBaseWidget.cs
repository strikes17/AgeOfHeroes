using UnityEngine;

namespace AgeOfHeroes
{
    public class GUIBaseWidget : MonoBehaviour
    {
        public bool isHidden => gameObject.activeSelf;
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Toggle()
        {
            if(isHidden)Show();
            else Hide();
        }
    }
}