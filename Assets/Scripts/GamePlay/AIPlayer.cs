using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class AIPlayer : Player
    {
        public override void OnActiveTerrainChanged(PlayableTerrain playableTerrain)
        {
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlayerRecievedTurn()
        {
            base.OnPlayerRecievedTurn();
            _GUIManager.PauseMenuWindow.Locked = true;
            _GUIManager.HeroPortraitWidget.portraitWidgetMini.Disable();
            _GUIManager.HeroPortraitWidget.Disable();
            var skipTurnButton = GameManager.Instance.GUIManager.skipTurnButton;
            skipTurnButton.Interactable = false;
            skipTurnButton.InteractableForAI = true;
            _GUIManager.RecentCharactersWidget.Locked = true;
            DisableInteractionWithWorld();
            AIPlayerController.ProcessAI();
            if(Color == PlayerColor.Neutral)return;
            GameManager.Instance.MusicManager.PlayFractionMusic(_fraction, FractionMusicType.Turn);
            // _fogOfWarController.ShowFogOfWarForTerrain(ActiveTerrain);
        }
        
        public override void OnEnemyCastleSiegePlanningStage(CastleSiegeData siegeData)
        {
            //Расстановка ИИ для осады замка
            GameManager.Instance.MapScenarioHandler.OnSiegeNextPlayerPlanningTurn();
        }

        public override void OnSiegeStartFighting()
        {
            
        }

        public override void OnSiegeEnded()
        {
            
        }

        public override void OnCastleBeingSeigedPlanningStage(CastleSiegeData siegeData)
        {   
            //Растановка ИИ для защиты замка при осаде
            base.OnCastleBeingSeigedPlanningStage(siegeData);
            GameManager.Instance.MapScenarioHandler.OnSiegeNextPlayerPlanningTurn();
        }

        public override void OnPlayerEndedTurn()
        {
        }

        protected override void Update()
        {
        }
    }
}