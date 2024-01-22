using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class GUILeftSidebarAbstractMenu : MonoBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}