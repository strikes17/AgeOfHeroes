using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class SpecialBuilding : AbstractBuilding
    {
        private string _iconPath;

        [NonSerialized] public Sprite EditorIcon;

        public void SetIcon(Sprite sprite)
        {
            _icon = sprite;
        }

        public Sprite GetIcon()
        {
            if (_icon != null)
                return _icon;
            if (!string.IsNullOrEmpty(_iconPath))
                _icon = ResourcesBase.GetSprite(_iconPath);
            return _icon;
        }

        private Sprite _icon;

        public string IconPath
        {
            get => _iconPath;
            set => _iconPath = value;
        }

        public virtual void SpecialAction(Castle _castle)
        {
        }
    }
}