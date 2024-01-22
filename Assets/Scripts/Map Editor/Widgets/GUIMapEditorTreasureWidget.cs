using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorTreasureWidget : GUIMapEditorWidget
    {
        public TreasureObject TreasureObject
        {
            set
            {
                _treasureObject = value;
                ImageIcon = _treasureObject.Icon;
            }
            get => _treasureObject;
        }
        private TreasureObject _treasureObject;
    }
}