using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.AI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class MapScenarioHandler : MonoBehaviour
    {
        public bool showDebugGUI;
        public PlayerColor turnOfPlayerId;
        public int siegeTurnOfPlayerId;
        public Dictionary<PlayerColor, Player> players = new Dictionary<PlayerColor, Player>();
        public List<Player> playersInSiege = new List<Player>();
        private GameObject playerPrefab, playerBotPrefab;
        private int turnsCountThisDay = 0, siegeRoundCounter = 0, totalSiegeRoundsForNewDay = 4;
        private MatchInfo _matchInfo;
        private int _playersLeftStanding;

        public bool IsSiegeStarted => playersInSiege.Count > 0;

        public int TotalPlayersCountWithNeutral
        {
            get => _totalPlayersCount + 1;
        }

        private int _totalPlayersCount, _firstHumanPlayerIndex = -1;

        public Player CurrentPlayer => players[turnOfPlayerId];

        private Player _latestTurnHumanPlayer;

        public delegate void OnGameEventDelegate();

        public Player NextPlayer
        {
            get
            {
                if ((int)turnOfPlayerId > _matchInfo.totalPlayerCount)
                    return players[0];
                else
                    return players[turnOfPlayerId + 1];
            }
        }

        public List<Player> AllPlayers
        {
            get => players.Values.ToList();
        }

        public List<Player> AllPlayersExcludeNeutral
        {
            get
            {
                List<Player> p = new List<Player>();
                var keys = players.Keys;
                foreach (var key in keys)
                {
                    var player = players[key];
                    if (player.Color == PlayerColor.Neutral) continue;
                    p.Add(player);
                }

                return p;
            }
        }

        public Player LatestTurnHumanPlayer
        {
            get
            {
                _latestTurnHumanPlayer = _latestTurnHumanPlayer == null
                    ? players.Values.Where(x => x.isHuman).FirstOrDefault() : _latestTurnHumanPlayer;
                return _latestTurnHumanPlayer;
            }
        }

        public PlayerColor TurnOfPlayerId
        {
            get => turnOfPlayerId;
            set
            {
                turnOfPlayerId = value;
                GameManager.Instance.GUIManager.OnSwitchTurnUpdateGUI(turnOfPlayerId);
                GameManager.Instance.GUIResourcesPanel.UpdateGUI(CurrentPlayer);
                GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel.Hide();
                CurrentPlayer.ContinueTurn();
                // Debug.Log(CurrentPlayer.Color);
            }
        }

        private void OnNewDayBegin()
        {
            foreach (var player in players)
            {
                player.Value.OnNewDayForPlayer();
            }

            newDayBegin?.Invoke();
        }

        private void OnNewSiegeRoundBegin()
        {
            foreach (var player in players)
            {
                player.Value.OnNewSiegeRoundForPlayer();
            }

            newSiegeRoundBegin?.Invoke();
        }

        public event OnGameEventDelegate NewDayBegin
        {
            add => newDayBegin += value;
            remove => newDayBegin -= value;
        }

        public event OnGameEventDelegate NewSiegeRoundBegin
        {
            add => newSiegeRoundBegin += value;
            remove => newSiegeRoundBegin -= value;
        }

        private event OnGameEventDelegate newDayBegin, newSiegeRoundBegin;
        private event CastleSiegeData.OnSiegeEventDelegate siegeBegin;

        public void OnSiegeBegin(CastleSiegeData castlesiegedata)
        {
            siegeBegin?.Invoke(castlesiegedata);
        }

        public event CastleSiegeData.OnSiegeEventDelegate SiegeBegin
        {
            add => siegeBegin += value;
            remove => siegeBegin -= value;
        }

        public event OnGameEventDelegate GameStarted
        {
            add => gameStarted += value;
            remove => gameStarted -= value;
        }

        private event OnGameEventDelegate gameStarted;

        public void SpawnPlayers()
        {
            _matchInfo = GameObject.FindObjectOfType<MatchInfo>();
            playerPrefab = ResourcesBase.LoadPrefab("Player");
            playerBotPrefab = ResourcesBase.LoadPrefab("Player Bot");
            _totalPlayersCount = (int)_matchInfo.totalPlayerCount;
            _playersLeftStanding = _totalPlayersCount; //excluding neutral
            // Debug.Log($"Total players: {_matchInfo.totalPlayerCount}");
            for (int i = 0; i < _totalPlayersCount; i++)
            {
                PlayerColor playerColor = (PlayerColor)i;
                bool isPlayerHuman = _matchInfo.isPlayerHuman[playerColor] == PlayerType.Human;
                // Debug.Log($"Spawning Player {i}");
                Player playerInstance = null;
                if (!isPlayerHuman)
                {
                    playerInstance = GameObject.Instantiate(playerBotPrefab, Vector3.zero, Quaternion.identity)
                        .GetComponent<AIPlayer>();
                }
                else
                {
                    _firstHumanPlayerIndex = _firstHumanPlayerIndex == -1 ? i : _firstHumanPlayerIndex;
                    playerInstance = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                        .GetComponent<Player>();
                    playerInstance.GoldChanged += (int value) =>
                        GameManager.Instance.GUIResourcesPanel.UpdateGUI(playerInstance);
                    playerInstance.GemsChanged += (int value) =>
                        GameManager.Instance.GUIResourcesPanel.UpdateGUI(playerInstance);
                    playerInstance.TurnRecieved += GameManager.Instance.GUIResourcesPanel.UpdateGUI;
                }

                var aiManager = GameManager.Instance.aiManager;
                var playerAIController = aiManager.GetAIPlayerController(_matchInfo.Difficulty);
                playerAIController.Init(playerInstance);
                playerInstance.CharacterSpawned += (chararacter) =>
                {
                    AICharacterController aiCharacterController = null;
                    if (chararacter is Hero)
                        aiCharacterController = chararacter.gameObject.AddComponent<AIHeroController>();
                    else
                        aiCharacterController = chararacter.gameObject.AddComponent<AICharacterController>();
                };
                playerInstance.SetAIController(playerAIController);
                playerInstance.name = $"Player-{((PlayerColor)i).ToString()}";
                playerInstance.realColor = GlobalVariables.playerColors[(PlayerColor)i];
                playerInstance.Color = playerColor;
                playerInstance.isHuman = isPlayerHuman;
                playerInstance.Fraction = _matchInfo.playersFractions[playerColor];
                players.Add((PlayerColor)i, playerInstance);
                playerInstance.Init();
                playerInstance.Banished += CheckWinCondition;
            }

            GameManager.Instance.MainCamera.Setup(AllPlayers);
            GameManager.Instance.GUIManager.skipTurnButton.AddListener(SkipPlayerTurn);
            SpawnNeutralPlayer();
        }

        private void CheckWinCondition(Player player)
        {
            _playersLeftStanding--;
            if (_playersLeftStanding == 1)
            {
                var pvals = players.Values;
                foreach (var p in pvals)
                {
                    if (p.IsBanished || p.Color == PlayerColor.Neutral) continue;
                    GameManager.Instance.GUIManager.GameResultWindow.SetPlayerWinState(p.Color);
                }
            }
        }


        private void SpawnNeutralPlayer()
        {
            PlayerColor neutralColor = PlayerColor.Neutral;
            var playerInstance = GameObject.Instantiate(playerBotPrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<AIPlayer>();
            playerInstance.name = $"Player-NEUTRAL";
            playerInstance.realColor = GlobalVariables.playerColors[neutralColor];
            playerInstance.Color = neutralColor;
            playerInstance.isHuman = false;
            playerInstance.Init();
            var aiManager = GameManager.Instance.aiManager;
            var playerAIController = aiManager.GetNeutralAIController();
            playerAIController.Init(playerInstance);
            playerInstance.SetAIController(playerAIController);
            playerAIController.SetDifficulty(_matchInfo.Difficulty);
            players.Add(neutralColor, playerInstance);
        }

        public void ChangeCastleOwner(Castle castle, Player newOwner)
        {
            var oldOwner = castle.Player;
            castle.Selected -= oldOwner.OnPlayerSelectedCastle;
            castle.Deselected -= oldOwner.OnPlayerDeselectedCastle;
            oldOwner.controlledCastles.Remove(castle);

            castle.Selected += newOwner.OnPlayerSelectedCastle;
            castle.Deselected += newOwner.OnPlayerDeselectedCastle;
            newOwner.controlledCastles.Add(castle);

            castle.PlayerOwnerColor = newOwner.Color;
            castle.UpdateFogOnWorldTerrain(GameManager.Instance.WorldTerrain);

            castle.BanishCastleGarnison();
            newOwner.fogOfWarController.ClearDeepFogOfWarEverywhereReserved(castle.CastleTerrain, newOwner);
            if (oldOwner.controlledCastles.Count <= 0)
            {
                oldOwner.Banish();
            }
        }

        public void LoadGameInitialization(PlayerColor playerTurn)
        {
            turnOfPlayerId = playerTurn;
            var player = players[turnOfPlayerId];
            GUIDialogueFactory.canvasRoot = GameObject.FindWithTag("CanvasMain").transform;
            gameStarted?.Invoke();
            GameManager.Instance.GUIManager.OnSwitchTurnUpdateGUI(turnOfPlayerId);
            GameManager.Instance.GUIResourcesPanel.UpdateGUI(player);
            GameManager.Instance.GUIManager.HeroSkillTreeButton.Init();
            GameManager.Instance.GUIManager.HeroPortraitWidget.ReferencedHero = player.MainHero;
            GameManager.Instance.GUIManager.HeroPortraitWidget.Show();
            GameManager.Instance.GUIManager.skipTurnButton.Interactable = true;
            GameManager.Instance.MusicManager.PlayFractionMusic(player.Fraction, FractionMusicType.Main);
            player.EnableInteractionWithWorld();
            player.fogOfWarController.ShowFogOfWarForTerrain(player.ActiveTerrain);
            player.controlledCharacters.ForEach(x =>
            {
                x.VisuallyShow();
                x.Player.fogOfWarController.UpdateVisionForCharacter(x);
            });
            player.UpdateVisionForEveryEnemy();
            __Clouds();
        }

        public void NewGameInitialization()
        {
            GUIDialogueFactory.canvasRoot = GameObject.FindWithTag("CanvasMain").transform;
            turnOfPlayerId = (PlayerColor)_firstHumanPlayerIndex;
            gameStarted?.Invoke();
            OnNewDayBegin();
            GivePlayerTurn(players[turnOfPlayerId]);
            // GameManager.Instance.GUIManager.OnSwitchTurnUpdateGUI(turnOfPlayerId);
            // GameManager.Instance.GUIManager.HeroSkillTreeButton.Init();
            __Clouds();
            GameManager.Instance.GUIManager.HeroSkillTreeButton.Init();
        }

        private void __Clouds()
        {
            var size = GameManager.Instance.WorldTerrain.Size;
            for (int i = 0; i < size.y / 3; i++)
            {
                Vector2 offset = new Vector2(-(Random.Range(3, 15)), i * 3);
                float speed = Random.Range(0.25f, 1f);
                CloudDecoration.Create(offset)
                    .Float(Vector2.right, GameManager.Instance.WorldTerrain.Size, speed);
            }
        }

        public void GivePlayerTurn(Player player)
        {
            TurnOfPlayerId = player.Color;
            player.OnPlayerRecievedTurn();
        }


        public void SkipPlayerTurn()
        {
            turnsCountThisDay++;
            if (turnsCountThisDay >= TotalPlayersCountWithNeutral)
            {
                turnsCountThisDay = 0;
                OnNewDayBegin();
            }

            var player = players[turnOfPlayerId];
            player.OnPlayerEndedTurn();
            player.SelectedCastle?.ExitCastle();
            if (player.isHuman) _latestTurnHumanPlayer = CurrentPlayer;
            turnOfPlayerId++;
            if ((int)turnOfPlayerId >= TotalPlayersCountWithNeutral)
                turnOfPlayerId = 0;

            if ((int)turnOfPlayerId >= _matchInfo.totalPlayerCount)
                turnOfPlayerId = PlayerColor.Neutral;

            player = players[turnOfPlayerId];
            if (player.IsBanished)
            {
                SkipPlayerTurn();
                return;
            }
            // Debug.Log($"Default Next Turn {siegeTurnOfPlayerId}");

            GivePlayerTurn(player);
        }

        public void SetupSiege(Player attackingPlayer, Player defendingPlayer)
        {
            playersInSiege.Clear();
            siegeRoundCounter = 0;
            siegeTurnOfPlayerId = 0;
            playersInSiege.Add(attackingPlayer);
            playersInSiege.Add(defendingPlayer);
        }

        public void SkipPlayerTurnOnSiege()
        {
            siegeRoundCounter++;
            if (siegeRoundCounter >= totalSiegeRoundsForNewDay)
            {
                siegeRoundCounter = 0;
                OnNewDayBegin();
                Debug.Log("New day in siege!!");
            }
            else if (siegeRoundCounter % 2 == 0)
            {
                OnNewSiegeRoundBegin();
                Debug.Log("New siege round!");
            }
            Debug.Log($"On Siege Next Turn {siegeTurnOfPlayerId}");

            var player = playersInSiege[siegeTurnOfPlayerId];
            player.OnPlayerEndedTurn();
            siegeTurnOfPlayerId++;
            if ((int)siegeTurnOfPlayerId >= 2)
                siegeTurnOfPlayerId = 0;

            player = playersInSiege[siegeTurnOfPlayerId];
            GivePlayerTurn(player);
        }

        public void OnSiegeNextPlayerPlanningTurn()
        {
            siegeTurnOfPlayerId++;
            if ((int)siegeTurnOfPlayerId >= 2)
            {
                playersInSiege.ForEach(x => x.SetActiveContext(PlayerContext.Default));
                var guiManager = GameManager.Instance.GUIManager;
                guiManager.skipTurnButton.RemoveAllListeners();
                guiManager.skipTurnButton.AddListener(SkipPlayerTurnOnSiege);
                siegeTurnOfPlayerId = 1;
                SkipPlayerTurnOnSiege();
                foreach (var p in playersInSiege)
                {
                    p.OnSiegeStartFighting();
                }
                return;
            }

            Debug.Log($"On Siege Planning Turn {siegeTurnOfPlayerId}");
            var player = playersInSiege[siegeTurnOfPlayerId];
            playersInSiege[siegeTurnOfPlayerId - 1].DisableInteractionWithWorld();
            GameManager.Instance.GUIManager.OnSwitchTurnUpdateGUI(player.Color);
            GameManager.Instance.GUIResourcesPanel.UpdateGUI(player);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUIStyle gUIStyle = new(GUI.skin.label)
            {
                fontSize = 28
            };
            gUIStyle.normal.textColor = UnityEngine.Color.white;
            if (turnOfPlayerId == PlayerColor.Neutral)
            {
                GUILayout.EndVertical();
                return;
            }

            // GUILayout.Label($"Context: {players[turnOfPlayerId].activePlayerContext}", gUIStyle);
            GUILayout.EndVertical();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                int opcounter = 0;
                foreach (var player in AllPlayersExcludeNeutral)
                {
                    if (player != CurrentPlayer)
                    {
                        for (int i = 0; i < player.controlledCharacters.Count; i++)
                        {
                            opcounter++;
                            var character = player.controlledCharacters[i];
                            character.Banish();
                            if (!character.isExistingInWorld) i--;
                            if (opcounter >= 1000)
                            {
                                Debug.Log("Overflow!");
                                break;
                            }
                        }
                    }
                }
                // var bluePlayer = GameManager.Instance.MapScenarioHandler.players[PlayerColor.Blue];
                // bluePlayer.Banish();
            }
        }
    }
}