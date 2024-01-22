using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class TimersSystem : MonoBehaviour
    {
        public static TimersSystem Instance
        {
            get => _instance;
        }

        private static TimersSystem _instance;
        private List<Timer> _timers = new List<Timer>();

        public void RunTimer(Timer timer)
        {
            _timers.Add(timer);
        }

        private void Awake()
        {
            _instance = GetComponent<TimersSystem>();
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                var timer = _timers[i];
                timer.Update();
                if (timer.Finished)
                {
                    _timers.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}