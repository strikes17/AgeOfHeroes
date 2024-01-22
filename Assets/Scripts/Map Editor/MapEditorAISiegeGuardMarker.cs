using System;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorAISiegeGuardMarker : MapEditorAIMarker
    {
        public int Tier;
        public int Quantity;
        public int Level;
        public AIGuardMarkerCharacterType GuardMarkerCharacterType;
        public AIGuardMarkerPlayerStateType GuardMarkerPlayerStateType;

        private void Start()
        {
            Tier = 1;
        }

        public override void OnClicked()
        {
            base.OnClicked();
            var window = GUIDialogueFactory.CreateMapEditorMarkerInfoWindow(1001);
            if(window == null)return;
            window.Quantity = Quantity;
            window.Tier = Tier;
            window.IsElite = Level == 2 ? true : false;
            window.Applied = () => GetDialogueWindowInfo(window);
        }

        private void GetDialogueWindowInfo(GUIMapEditorMarkerInfoWindow window)
        {
            Tier = window.Tier;
            Quantity = window.Quantity;
            Level = window.IsElite ? 2 : 1;
        }
    }
}