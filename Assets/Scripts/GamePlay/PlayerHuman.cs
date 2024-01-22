using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgeOfHeroes
{
    public class PlayerHuman : Player
    {
        public override void Init()
        {
            base.Init();
            GameManager.Instance.MapScenarioHandler.GameStarted += OnPlayerNewGameStarted;
            _GUIManager.CancelSpellButtonWidget.Button1.onClick.AddListener(() =>
                SetActiveContext(PlayerContext.Default));
            ContextChanged += OnUsingSpellContext;
            ContextChanged += OnDefaultContext;
            ContextChanged += OnPlacingCharactersContext;
            ContextChanged += OnPlanningSiegeContext;
        }

        public override void OnPlayerRecievedTurn()
        {
            base.OnPlayerRecievedTurn();
        }

        public override void ContinueTurn()
        {
            var worldTerrainSize = GameManager.Instance.WorldTerrain.Size;
            Vector3 newCameraPosition = new Vector3(worldTerrainSize.x / 2, worldTerrainSize.y / 2, 0f);
            if (MainHero != null)
                newCameraPosition = MainHero.transform.position;
            else
            {
                var anyCharacter = controlledCharacters.FirstOrDefault();
                if (anyCharacter != null)
                    newCameraPosition = anyCharacter.transform.position;
            }

            GameManager.Instance.MainCamera.MoveToPosition(newCameraPosition);
            _GUIManager.PauseMenuWindow.Locked = false;
            _GUIManager.RecentCharactersWidget.Locked = false;
            GameManager.Instance.GUIManager.HeroPortraitWidget.ReferencedHero = MainHero;
            GameManager.Instance.GUIManager.HeroPortraitWidget.Show();
            GameManager.Instance.GUIManager.skipTurnButton.Interactable = true;
            EnableInteractionWithWorld();
            UpdateVisionForEveryEnemy();
            _fogOfWarController.ShowFogOfWarForTerrain(ActiveTerrain);
            UpdateVisionForAllCharacters();
            GameManager.Instance.MusicManager.PlayFractionMusic(_fraction, FractionMusicType.Main);
            if (AIControlls)
                AIPlayerController.ProcessAI();
        }

        public override void OnEnemyCastleSiegePlanningStage(CastleSiegeData siegeData)
        {
            base.OnEnemyCastleSiegePlanningStage(siegeData);
            GameManager.Instance.GUIManager.SwitchAIButton.Interactable = false;
            var siegePanel = GameManager.Instance.GUIManager.SiegePanel;

            siegePanel.SetupSiegePanel(siegeData.attackingPlayerData);
        }

        public override void OnSiegeStartFighting()
        {
            var guiManager = GameManager.Instance.GUIManager;
            guiManager.SwitchAIButton.Interactable = true;
        }

        public override void OnSiegeEnded()
        {
            var guiManager = GameManager.Instance.GUIManager;
        }

        public override void OnCastleBeingSeigedPlanningStage(CastleSiegeData siegeData)
        {
            GameManager.Instance.MapScenarioHandler.GivePlayerTurn(this);
            GameManager.Instance.GUIManager.SwitchAIButton.Interactable = false;
            base.OnCastleBeingSeigedPlanningStage(siegeData);
            var siegePanel = GameManager.Instance.GUIManager.SiegePanel;

            siegePanel.SetupSiegePanel(siegeData.defendingPlayerData);
        }


        public override void OnPlayerEndedTurn()
        {
            base.OnPlayerEndedTurn();
            _guiSpellBook.Close(true);
            actionCellPool.ResetAll();
            SetActiveContext(PlayerContext.Default);
            isInteractionWithWorldEnabled = false;
            if (selectedCharacter != null)
                selectedCharacter.DeselectAndRemoveControll(Color);
            selectedCharacter = null;
            GameManager.Instance.GUIManager.skipTurnButton.Interactable = false;
            _GUIManager.LeftSidebar.Hide();
        }
    }
}