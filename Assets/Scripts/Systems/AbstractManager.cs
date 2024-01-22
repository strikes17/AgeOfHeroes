using UnityEngine;

namespace AgeOfHeroes
{
    public class AbstractManager : MonoBehaviour
    {
        public bool isReady = false;
        public delegate void OnManagerLoadedDelegate();

        public event OnManagerLoadedDelegate Loaded
        {
            add => loaded += value;
            remove => loaded -= value;
        }

        public void OnLoaded()
        {
            // Debug.Log(GetType().ToString() + " loaded");
            isReady = true;
            loaded?.Invoke();
        }

        private event OnManagerLoadedDelegate loaded;
    }
}