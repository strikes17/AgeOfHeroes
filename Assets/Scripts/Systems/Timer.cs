using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public class Timer
    {
        public Action expired, cycleFinished;
        protected string _internalName;

        protected float cycleTime, updateTime;
        protected int totalCycles, currentCycle;

        protected bool isStopped = false;

        public Timer(string name, float time, Action cycleFinished, Action expired, int cycles)
        {
            isStopped = false;
            cycleTime = time;
            _internalName = name;
            this.expired = expired;
            this.cycleFinished = cycleFinished;
            totalCycles = cycles;
            currentCycle = 0;
        }

        public string InternalName => _internalName;

        public bool Finished => isStopped;

        public void Stop()
        {
            expired?.Invoke();
            isStopped = true;
        }

        public void Update()
        {
            if (isStopped) return;
            updateTime += Time.deltaTime;
            if (updateTime < cycleTime)
                return;
            currentCycle++;
            cycleFinished?.Invoke();
            updateTime = 0f;
            if (currentCycle >= totalCycles && totalCycles > 0)
            {
                expired?.Invoke();
                isStopped = true;
                return;
            }
        }
    }
}