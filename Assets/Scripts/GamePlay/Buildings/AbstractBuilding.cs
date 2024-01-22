using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class AbstractBuilding : ICloneable
    {
        public int goldCost, gemsCost;
        public Fraction fraction;
        public string internalName;
        public string title, description;
        protected bool _isBuilt, _isRestricted;
        public List<string> requiredBuiltBuildings = new List<string>();
        [SerializeField] protected int uniqueId;
        protected Player _playerOwner;
        protected Castle _targetCastle;

        public delegate void OnBuildingDelegate(Castle castle);

        public event  OnBuildingDelegate Built
        {
            add => built += value;
            remove => built -= value;
        }
        private event OnBuildingDelegate built;

        public Player PlayerOwner
        {
            get => _playerOwner;
            set => _playerOwner = value;
        }

        public bool IsBuilt
        {
            get => _isBuilt;
        }

        public bool IsRestricted
        {
            get => _isRestricted;
            set => _isRestricted = value;
        }
        
        public virtual void OnBuilt(Castle castle)
        {
            _isBuilt = true;
            _targetCastle = castle;
            built?.Invoke(_targetCastle);
        }

        public virtual void Update()
        {
            
        }
        
        public virtual void Init()
        {
            
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}