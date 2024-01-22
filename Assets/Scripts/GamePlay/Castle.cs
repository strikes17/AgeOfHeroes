using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public enum SiegeResult
    {
        AttackerWin,
        AttackerLost,
        AttackerFlee,
        DefenderFlee
    }

    [SelectionBase]
    public class Castle : MonoBehaviour, IClickTarget
    {
        public string customName;

        public int TotalBuildingsCount
        {
            get => _totalBuildingCount;
        }

        public int FightingPoints
        {
            get
            {
                int points = 0;
                for (int i = 0; i < _garnisonCharacters.Count; i++)
                    points += _garnisonCharacters[i].FightingPoints;
                for (int i = 0; i < _guardsCharacters.Count; i++)
                    points += _guardsCharacters[i].FightingPoints;
                return points;
            }
        }

        private int _totalBuildingCount = 0;

        public PlayerColor PlayerOwnerColor
        {
            set
            {
                _playerColor = value;
                _coloreds.ForEach(x => x.PlayerOwnerColor = _playerColor);
            }
            get { return _playerColor; }
        }

        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0);
        }

        public Player Player
        {
            get => GameManager.Instance.MapScenarioHandler.players[_playerColor];
        }

        public string castleMapName;
        public SpriteRenderer _spriteRenderer;
        public CastleObject _castleObject;
        public List<Colored> _coloreds;
        public Transform spawnPoint;
        public bool hasBuiltThisRound;
        public int UniqueId;
        private PlayerColor _playerColor;
        private List<ControllableCharacter> _garnisonCharacters = new List<ControllableCharacter>();
        private List<ControllableCharacter> _guardsCharacters = new List<ControllableCharacter>();
        private PlayableTerrain _castleTerrain;
        private float gemsValue;

        public delegate void OnSiegeEventDelegate(CastleSiegeData castleSiegeData);

        public event OnSiegeEventDelegate SiegeBegin
        {
            add => siegeBegin += value;
            remove => siegeBegin -= value;
        }

        public event OnSiegeEventDelegate SiegeEnd
        {
            add => siegeEnd += value;
            remove => siegeEnd -= value;
        }


        protected event OnSiegeEventDelegate siegeBegin, siegeEnd;


        public void AddGarnisonCharacterStack(ControllableCharacter controllableCharacter)
        {
            var garnisonPanel = GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel;
            controllableCharacter.PutInGarnison();
            controllableCharacter.StackBanished += RemoveBanishedCharacterFromGarnison;
            _garnisonCharacters.Add(controllableCharacter);
            garnisonPanel.UpdateGarnisonForPlayerCastle(this);
        }

        public void BanishCastleGarnison()
        {
            // _garnisonCharacters.ForEach(character => character.Banish());
            for (int i = 0; i < _garnisonCharacters.Count; i++)
            {
                var character = _garnisonCharacters[i];
                character.Banish();
                if (!character.isExistingInWorld) i--;
            }
        }

        private void RemoveBanishedCharacterFromGarnison(ControllableCharacter controllableCharacter,
            ControllableCharacter empty)
        {
            RemoveFromGarnisonCharacterStack(controllableCharacter);
        }

        public void RemoveFromGarnisonCharacterStack(ControllableCharacter controllableCharacter)
        {
            var garnisonPanel = GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel;
            controllableCharacter.RemoveFromGarnison();
            controllableCharacter.StackBanished -= RemoveBanishedCharacterFromGarnison;
            garnisonPanel.RemoveCharacterSlot(controllableCharacter);
            _garnisonCharacters.Remove(controllableCharacter);
            garnisonPanel.UpdateGarnisonForPlayerCastle(this);
        }

        public void EnterCastle()
        {
            if (Player.ActiveTerrain == _castleTerrain)
                return;
            GameManager.Instance.MusicManager.PlayFractionMusic(Player.Fraction, FractionMusicType.Castle);
            Player.actionCellPool.ResetAll();
            var guiManager = GameManager.Instance.GUIManager;
            var _guiLeftSidebar = guiManager.LeftSidebar;
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.RemoveAllListeners();
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.AddListener(ExitCastle);
            _guiLeftSidebar.GUICharacterShopMenu.UpdateShopGUI(this);
            _guiLeftSidebar.GUISpecialBuildsMenu.UpdateShopGUI(this);
            _guiLeftSidebar.ShowForCastleInternal();
            guiManager.RecentCharactersWidget.Locked = true;
            Player.ActiveTerrain = _castleTerrain;
            _guiLeftSidebar.GUICastleGarnisonPanel.UpdateGarnisonForPlayerCastle(this);
        }

        public void ExitCastle()
        {
            if (Player.ActiveTerrain != _castleTerrain)
                return;
            GameManager.Instance.MusicManager.PlayFractionMusic(Player.Fraction, FractionMusicType.Main);
            Player.actionCellPool.ResetAll();
            var guiManager = GameManager.Instance.GUIManager;
            var _guiLeftSidebar = guiManager.LeftSidebar;
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.RemoveAllListeners();
            _guiLeftSidebar.GUICastleMenu.castleMenuButton.button.onClick.AddListener(EnterCastle);
            _guiLeftSidebar.GUICharacterShopMenu.UpdateShopGUI(this);
            _guiLeftSidebar.GUISpecialBuildsMenu.UpdateShopGUI(this);
            _guiLeftSidebar.Hide();
            guiManager.RecentCharactersWidget.Locked = false;
            Player.ActiveTerrain = GameManager.Instance.WorldTerrain;
            _guiLeftSidebar.GUICastleGarnisonPanel.UpdateGarnisonForPlayerCastle(this);
        }

        public void CastleSiegeSetup(CastleSiegeData castleSiegeData)
        {
            GameManager.Instance.MusicManager.PlayFractionMusic(castleSiegeData.attackingPlayerData.Player.Fraction,
                FractionMusicType.Siege, MusicImportance.NotInterruptable);
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var guiManager = GameManager.Instance.GUIManager;

            var skipTurnButton = guiManager.skipTurnButton;
            skipTurnButton.RemoveAllListeners();
            skipTurnButton.AddListener(mapScenarioHandler.OnSiegeNextPlayerPlanningTurn);
            skipTurnButton.AddListener(() => OnSiegeSetupDefenders(castleSiegeData));
            OnSiegeSetupAttacker(castleSiegeData);
            guiManager.EscapeSiegeButton.RemoveAllListeners();
            guiManager.EscapeSiegeButton.AddListener(() =>
            {
                var scenarioHandler = GameManager.Instance.MapScenarioHandler;
                var fleeingPlayer = scenarioHandler.playersInSiege[scenarioHandler.siegeTurnOfPlayerId];
                castleSiegeData.loserPlayer = fleeingPlayer;
                castleSiegeData.SiegeResult = castleSiegeData.attackingPlayerData.Player == fleeingPlayer
                    ? SiegeResult.AttackerFlee
                    : SiegeResult.DefenderFlee;
                OnSiegeEnded(castleSiegeData);
            });
        }

        private void OnSiegeSetupAttacker(CastleSiegeData castleSiegeData)
        {
            var attackingPlayer = castleSiegeData.attackingPlayerData.Player;
            Player.SelectedCastle = null;
            Player.ActiveTerrain = _castleTerrain;

            attackingPlayer.SelectedCastle = null;
            attackingPlayer.ActiveTerrain = _castleTerrain;

            var toolsTilemap = _castleTerrain.ToolTilemap;
            var terrainTiles = _castleTerrain.Tool.TerrainTiles;
            var siegeTile = ResourcesBase.GetTile("siege_attacker");
            foreach (var toolTile in terrainTiles.Keys)
            {
                var position = new Vector3Int(toolTile.x, toolTile.y, 0);
                var tileAtPosition = toolsTilemap.GetTile(position);
                if (tileAtPosition != siegeTile)
                    continue;
                attackingPlayer.fogOfWarController.ClearCellForPlayer(position, _castleTerrain, attackingPlayer);
            }

            attackingPlayer.OnEnemyCastleSiegePlanningStage(castleSiegeData);
            // attackingPlayer.SiegeEnd -= OnSiegeEnded;
            // attackingPlayer.SiegeEnd += OnSiegeEnded;
        }

        private void OnSiegeSetupDefenders(CastleSiegeData castleSiegeData)
        {
            var guiManager = GameManager.Instance.GUIManager;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var defendingPlayer = castleSiegeData.defendingPlayerData.Player;

            var skipTurnButton = guiManager.skipTurnButton;
            skipTurnButton.RemoveAllListeners();
            skipTurnButton.AddListener(() => TryNextPlayerPlanningTurn(castleSiegeData));
            defendingPlayer.OnCastleBeingSeigedPlanningStage(castleSiegeData);
            // defendingPlayer.SiegeEnd -= OnSiegeEnded;
            // defendingPlayer.SiegeEnd += OnSiegeEnded;
        }

        private void TryNextPlayerPlanningTurn(CastleSiegeData siegeData)
        {
            var allDefendingCharacters = siegeData.defendingPlayerData.allCharacters;
            var appliedDefendingCharacters = siegeData.defendingPlayerData.appliedCharacters;
            if (allDefendingCharacters.Count != appliedDefendingCharacters.Count)
            {
                var dialogueWindow = GUIDialogueFactory.CreateCommonDialogueWindow();
                dialogueWindow.Title = "Warning!";
                dialogueWindow.Description =
                    "Not all of characters were placed for defense! Are you willing to continue?";
                dialogueWindow.Applied = () =>
                {
                    for (int i = 0; i < allDefendingCharacters.Count; i++)
                    {
                        var defendingCharacter = allDefendingCharacters[i];
                        if (defendingCharacter == null) continue;
                        if (!appliedDefendingCharacters.Contains(defendingCharacter))
                        {
                            // defendingCharacter.Banish();
                        }
                    }

                    NextPlayerPlanningTurn();
                };
                return;
            }

            NextPlayerPlanningTurn();
        }

        private void NextPlayerPlanningTurn()
        {
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            mapScenarioHandler.OnSiegeNextPlayerPlanningTurn();
        }

        public void OnSiegeEnded(CastleSiegeData castleSiegeData)
        {
            var guiManager = GameManager.Instance.GUIManager;
            var skipTurnButton = guiManager.skipTurnButton;
            skipTurnButton.Interactable = false;
            skipTurnButton.InteractableForAI = false;
            var siegeResultDialogue = guiManager.SiegeResultDialogue;
            siegeResultDialogue.Init(castleSiegeData);
            siegeResultDialogue.Show();
            siegeResultDialogue.OkButton.onClick.AddListener((() => ReturnPlayersToWorld(castleSiegeData)));
        }

        private void ReturnPlayersToWorld(CastleSiegeData castleSiegeData)
        {
            var guiManager = GameManager.Instance.GUIManager;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var attackingPlayerData = castleSiegeData.attackingPlayerData;
            var attackingPlayer = attackingPlayerData.Player;
            var worldTerrain = GameManager.Instance.WorldTerrain;
            var defendingPlayer = castleSiegeData.defendingPlayerData.Player;
            attackingPlayer.OnSiegeEnded();
            defendingPlayer.OnSiegeEnded();
            // mapScenarioHandler.GivePlayerTurn(attackingPlayer);
            var skipTurnButton = guiManager.skipTurnButton;
            skipTurnButton.Interactable = true;
            skipTurnButton.InteractableForAI = true;
            skipTurnButton.RemoveAllListeners();
            skipTurnButton.AddListener(mapScenarioHandler.SkipPlayerTurn);
            guiManager.EscapeSiegeButton.Hide();

            SiegeResult siegeResult = castleSiegeData.SiegeResult;

            Debug.Log($"{siegeResult}");
            switch (siegeResult)
            {
                case SiegeResult.AttackerWin:
                    OnSiegeResultAttackerWin(castleSiegeData);
                    break;
                case SiegeResult.AttackerFlee:
                    OnSiegeResultAttackerFlee(castleSiegeData);
                    break;
                case SiegeResult.AttackerLost:
                    OnSiegeResultAttackerLost(castleSiegeData);
                    break;
                case SiegeResult.DefenderFlee:
                    OnSiegeResultAttackerWin(castleSiegeData);
                    break;
            }

            Debug.Log("siege ended!");
            attackingPlayer.OnSiegeEnded();
            defendingPlayer.OnSiegeEnded();
            attackingPlayer.ActiveTerrain = worldTerrain;
            defendingPlayer.ActiveTerrain = worldTerrain;
            var musicManager = GameManager.Instance.MusicManager;
            musicManager.ResetImportance();
            musicManager.PlayFractionMusic(attackingPlayer.Fraction, FractionMusicType.Main,
                MusicImportance.Default);
            mapScenarioHandler.TurnOfPlayerId = attackingPlayer.Color;
            siegeEnd?.Invoke(castleSiegeData);
        }

        private void OnSiegeResultAttackerWin(CastleSiegeData castleSiegeData)
        {
            var worldTerrain = GameManager.Instance.WorldTerrain;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var attackingPlayerData = castleSiegeData.attackingPlayerData;
            var attackingPlayer = attackingPlayerData.Player;
            var attackingCharacters = attackingPlayerData.allCharacters;
            foreach (var attackingCharacter in attackingCharacters)
            {
                if (attackingCharacter == null) continue;
                attackingCharacter.ChangeTerrainResetWorldPosition(
                    attackingPlayerData.charsPositionBeforeSiege[attackingCharacter], worldTerrain);
            }

            mapScenarioHandler.ChangeCastleOwner(this, attackingPlayer);
        }

        private void OnSiegeResultAttackerFlee(CastleSiegeData castleSiegeData)
        {
            var worldTerrain = GameManager.Instance.WorldTerrain;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var attackingPlayerData = castleSiegeData.attackingPlayerData;
            var attackingPlayer = attackingPlayerData.Player;
            var attackingCharacters = attackingPlayerData.allCharacters;
            int bcounter = 0;
            for (int i = 0; i < attackingCharacters.Count; i++)
            {
                bcounter++;
                if (bcounter > 1000) break;
                var attackingCharacter = attackingCharacters[i];
                if (attackingCharacter == null) continue;
                Debug.Log($"fleeing {attackingCharacter.name}");
                if (attackingCharacter is Hero)
                {
                    var controlledCastle = attackingPlayer.controlledCastles[0];
                    attackingCharacter.ChangeTerrainResetWorldPosition(controlledCastle.Position, worldTerrain);
                    attackingCharacter.ChangeHealthValue(-100000);
                    i--;
                    attackingCharacters.Remove(attackingCharacter);
                    continue;
                }

                attackingCharacter.Banish();
                if (!attackingCharacter.isExistingInWorld)
                    i--;
                attackingCharacters.Remove(attackingCharacter);
            }
        }

        private void OnSiegeResultAttackerLost(CastleSiegeData castleSiegeData)
        {
            var worldTerrain = GameManager.Instance.WorldTerrain;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var attackingPlayerData = castleSiegeData.attackingPlayerData;
            var attackingPlayer = attackingPlayerData.Player;
        }


        public CastleTierIncomeValues IncomeValues
        {
            get => tiersIncomeValues[_tier];
        }

        public Dictionary<int, CastleTierIncomeValues> tiersIncomeValues;

        public int Tier
        {
            set
            {
                _tier = value > _castleObject.maxTiers ? _castleObject.maxTiers : value;
                _spriteRenderer.sprite = _castleObject.tierSprite[_tier];
                for (int i = 0; i < Player.controlledCharacters.Count; i++)
                {
                    var controlledCharacter = Player.controlledCharacters[i];
                    if (controlledCharacter == null) continue;
                    if (controlledCharacter.GetType() == typeof(Character))
                    {
                        var character = (Character)controlledCharacter;
                        var baseGrowthPerDay = character.CharacterObject.baseGrowthPerDay;
                        character.growthPerDay = baseGrowthPerDay + baseGrowthPerDay * IncomeValues.growth;
                    }
                }
            }
            get => _tier;
        }

        private int _tier;
        private CastleInfo _castleInfo;

        public List<AbstractBuilding> AllBuildingsList
        {
            get
            {
                if (_allBuildingsList.Count <= 0)
                {
                    _allBuildingsList.AddRange(SpecialBuildings.Values);
                    _allBuildingsList.AddRange(ShopBuildings.Values);
                }

                return _allBuildingsList;
            }
        }

        public List<ControllableCharacter> garnisonCharacters => _garnisonCharacters;

        public PlayableTerrain CastleTerrain
        {
            get => _castleTerrain;
            set => _castleTerrain = value;
        }

        public float GemsValue => gemsValue;

        public List<ControllableCharacter> GuardsCharacters => _guardsCharacters;

        private List<AbstractBuilding> _allBuildingsList = new List<AbstractBuilding>();
        public Dictionary<string, SpecialBuilding> SpecialBuildings = new Dictionary<string, SpecialBuilding>();

        public Dictionary<string, CharacterShopBuilding>
            ShopBuildings = new Dictionary<string, CharacterShopBuilding>();

        public delegate void OnCastleEventDelegate(Castle castle);

        public event OnCastleEventDelegate Selected
        {
            add => selected += value;
            remove => selected -= value;
        }

        public event OnCastleEventDelegate Deselected
        {
            add => deselected += value;
            remove => deselected -= value;
        }

        private event OnCastleEventDelegate selected;
        private event OnCastleEventDelegate deselected;


        public void LoadGarnison(SerializableCastle serializableCastle)
        {
            var _garnisonIds = serializableCastle.GarnisonCharactersIds;
            var allCharacters = Player.controlledCharacters;
            (from character in allCharacters
                where _garnisonIds.Contains(character.UniqueId)
                select character).ToList().ForEach(x =>
            {
                Debug.Log(x.title);
                AddGarnisonCharacterStack(x);
            });
        }

        public void LoadFromSerializable(SerializableCastle serializableCastle)
        {
            int indexer = 0, shopIndexer = 0;
            var terrainManager = GameManager.Instance.terrainManager;
            PlayerOwnerColor = serializableCastle.PlayerOwnerColor;
            name = $"Castle_of_{PlayerOwnerColor}";
            castleMapName = serializableCastle.castleMapName;
            _castleObject = ResourcesBase.GetCastleObject(serializableCastle.objectName);
            _castleInfo = ResourcesBase.GetCastleInfo(serializableCastle.castleInfoName);
            UniqueId = serializableCastle.UniqueId == 0 ? UniqueId : serializableCastle.UniqueId;
            Position = new Vector2Int(serializableCastle.positionX, serializableCastle.positionY);
            gemsValue = serializableCastle.gemsValue;
            _castleInfo.Buildings.ForEach(x =>
            {
                var buildingSource = ResourcesBase.GetBuilding(x);
                var building = buildingSource.Clone() as AbstractBuilding;
                var isBuilt = _castleInfo.IsBuildingBuilt[indexer];
                building.PlayerOwner = Player;
                building.IsRestricted = _castleInfo.IsBuildingRestricted[indexer];
                if (building.GetType() == typeof(CharacterShopBuilding))
                {
                    var shopBuilding = (CharacterShopBuilding)building;
                    shopBuilding.basicCharacterForm =
                        ResourcesBase.GetCharacterObject(shopBuilding.basicCharacterName, shopBuilding.fraction);
                    shopBuilding.eliteCharacterForm =
                        ResourcesBase.GetCharacterObject(shopBuilding.eliteCharacterName, shopBuilding.fraction);
                    shopBuilding.Level = _castleInfo.BuildingLevel[shopIndexer];
                    ShopBuildings.Add(shopBuilding.internalName, shopBuilding);
                    shopIndexer++;
                    shopBuilding.Init();
                    if (isBuilt)
                        BuildCharacterShop(shopBuilding);
                }
                else
                {
                    var specialBuilding = (SpecialBuilding)building;
                    SpecialBuildings.TryAdd(specialBuilding.internalName, specialBuilding);
                    if (isBuilt)
                        BuildSpecialBuilding(specialBuilding);
                }

                building.Init();
                _allBuildingsList.Add(building);
                indexer++;
            });
            foreach (var specialBuilding in SpecialBuildings.Values)
            {
                if (specialBuilding is not CastleTierSpecialBuilding) continue;
                var tierBuilding = specialBuilding as CastleTierSpecialBuilding;
                tiersIncomeValues[tierBuilding.tier] =
                    new CastleTierIncomeValues()
                    {
                        gems = tierBuilding.gemsIncome,
                        gold = tierBuilding.goldIncome,
                        growth = tierBuilding.growthMultiplier
                    };
            }

            foreach (var pair in serializableCastle.Buildings)
            {
                var serializableBuilding = pair.Value;
                var name = pair.Key;
                var level = serializableBuilding.Level;

                if (SpecialBuildings.ContainsKey(name))
                {
                    var specialBuilding = SpecialBuildings[name];
                    if (level > 0 && !specialBuilding.IsBuilt)
                        BuildSpecialBuilding(specialBuilding);
                }
                else if (ShopBuildings.ContainsKey(name))
                {
                    var shopBuilding = ShopBuildings[name];
                    shopBuilding.Level = level;
                    if (level > 0 && !shopBuilding.IsBuilt)
                        BuildCharacterShop(shopBuilding);
                    shopBuilding.RecruitmentsAvailable = serializableBuilding.RecruitmentsAvailable;
                }
            }
        }

        public void Init()
        {
            var leftSidebar = GameManager.Instance.GUIManager.LeftSidebar;
            leftSidebar.Closed += () =>
            {
                leftSidebar.GUICastleGarnisonPanel.SetMode(GUICastleGarnisonEditWidget.Mode.Info);
                leftSidebar.GUICastleGarnisonPanel.Hide();
                Deselect();
            };
            tiersIncomeValues = new Dictionary<int, CastleTierIncomeValues>()
            {
                { 0, new CastleTierIncomeValues() { } },
                { 1, new CastleTierIncomeValues() { } },
                { 2, new CastleTierIncomeValues() { } },
                { 3, new CastleTierIncomeValues() { } },
                { 4, new CastleTierIncomeValues() { } },
            };
            foreach (var specialBuilding in SpecialBuildings.Values)
            {
                if (specialBuilding is not CastleTierSpecialBuilding) continue;
                var tierBuilding = specialBuilding as CastleTierSpecialBuilding;
                tiersIncomeValues.Add(tierBuilding.tier,
                    new CastleTierIncomeValues()
                    {
                        gems = tierBuilding.gemsIncome,
                        gold = tierBuilding.goldIncome,
                        growth = tierBuilding.growthMultiplier
                    });
            }

            UniqueId = GetInstanceID();
            _spriteRenderer.sortingOrder = GlobalVariables.CastleRenderOrder;
        }

        public void UpdateFogOnWorldTerrain(PlayableTerrain worldTerrain)
        {
            // Debug.Log($"Castle Fog for {Player.Color}");
            Player.fogOfWarController.UpdateVisionAtAreaForPlayer(Player, Position,
                worldTerrain,
                GlobalVariables.CastleVisionValue, true);
        }

        private void OnBuildingBuilt(AbstractBuilding building)
        {
            _totalBuildingCount++;
        }

        public CharacterShopBuilding CheckIfTierAlreadyWasBuilt(int tier)
        {
            foreach (var shopBuilding in ShopBuildings)
            {
                CharacterShopBuilding value = shopBuilding.Value;
                bool built = value.tier == tier && value.IsBuilt;
                if (built) return value;
            }

            return null;
        }

        public void UpdateGuards(Dictionary<int, CastleTierSpecialBuilding.CastleGuardInfo> guards)
        {
            var spawnManager = GameManager.Instance.SpawnManager;
            for (int i = 0; i < _guardsCharacters.Count; i++)
            {
                var g = _guardsCharacters[i];
                Debug.Log($"Banishing {g.title}");
                // if (g.InsideGarnison) RemoveFromGarnisonCharacterStack(g);
                g.Banish();
            }

            _guardsCharacters.Clear();
            for (int i = 1; i <= 6; i++)
            {
                var info = guards[i];
                var points = info.FightingPoints;
                var isElite = info.IsElite;
                var targetShop = ShopBuildings.Values.Where(x => x.basicCharacterForm.tier == i).FirstOrDefault();
                var form = isElite ? targetShop.eliteCharacterForm : targetShop.basicCharacterForm;
                int count = Mathf.FloorToInt((float)points / Mathf.Pow(3, form.tier));
                if (count <= 0) continue;
                int iterations = Mathf.CeilToInt((float)count / form.fullStackCount);
                for (int j = 0; j < iterations; j++)
                {
                    var c = Mathf.RoundToInt((float)count / iterations);
                    var spawnedGuard = spawnManager.SpawnCharacter(form.name, form.Fraction, PlayerOwnerColor,
                        Vector3Int.zero, _castleTerrain, c);
                    spawnedGuard.StackDemolished += (target, source) => _guardsCharacters.Remove(target);
                    _guardsCharacters.Add(spawnedGuard);
                    // AddGarnisonCharacterStack(spawnedGuard);
                }
            }

            CastleSiegeData castleSiegeData = new CastleSiegeData()
            {
                attackingPlayerData = new PlayerSiegeData() { Player = this.Player },
                defendingPlayerData = new PlayerSiegeData() { Player = this.Player },
                siegedCastle = this, SiegedCastleTerrain = _castleTerrain
            };
            Player.SetupSiegeGuards(castleSiegeData);
            GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel.UpdateGarnisonForPlayerCastle(this);
        }

        public void BuildCharacterShop(CharacterShopBuilding building)
        {
            building.OnBuilt(this);
            building.RecruitmentsAvailable = building.basicCharacterForm.fullStackCount;
            OnBuildingBuilt(building);
            // Debug.Log(building.title);
        }

        public void BuildSpecialBuilding(SpecialBuilding specialBuilding)
        {
            specialBuilding.OnBuilt(this);
            specialBuilding.SpecialAction(this);
            OnBuildingBuilt(specialBuilding);
            // Debug.Log($"{specialBuilding.title} was built!");
        }

        public void UpgradeCharacterShop(CharacterShopBuilding building)
        {
            building.Level++;
        }

        public void OnNewTurnBegin()
        {
            hasBuiltThisRound = false;
        }

        public void OnNewDayBegin()
        {
            Player.Gold += IncomeValues.gold;
            gemsValue += IncomeValues.gems;
            if (gemsValue >= 1f)
            {
                Player.Gems++;
                gemsValue = 0f;
            }

            foreach (var shopBuilding in ShopBuildings.Values)
            {
                if (!shopBuilding.IsBuilt)
                    continue;
                var baseGrowthPerDay = shopBuilding.basicCharacterForm.baseGrowthPerDay;
                shopBuilding.RecruitmentsAvailable += baseGrowthPerDay + baseGrowthPerDay * IncomeValues.growth;
            }
        }

        public void Select()
        {
            selected?.Invoke(this);
        }

        public void Deselect()
        {
            deselected?.Invoke(this);
        }

        public IEnumerator OnClicked(Player player)
        {
            var playerColor = player.Color;
            if (PlayerOwnerColor != playerColor)
                yield break;
            if (player.SelectedCastle)
            {
                Deselect();
            }
            else
            {
                Select();
            }

            yield return null;
        }
    }
}