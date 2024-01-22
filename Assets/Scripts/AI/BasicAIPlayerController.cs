using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.AI
{
    public class BasicAIPlayerController
    {
        protected int _economicsGold;
        protected MatchDifficulty _difficulty;
        protected Player _player;
        protected List<ControllableCharacter> _controlledCharacters = new List<ControllableCharacter>();
        protected List<Castle> _controlledCastles = new List<Castle>();

        protected Dictionary<PlayerColor, ControllableCharacter> _revealedEnemyCharacters =
            new Dictionary<PlayerColor, ControllableCharacter>();

        public delegate void OnAITurnDelegate(PlayerColor playerColor);

        protected event OnAITurnDelegate turnEnded;

        public event OnAITurnDelegate TurnEnded
        {
            add => turnEnded += value;
            remove => turnEnded -= value;
        }

        public int EconomicsGold => _economicsGold;

        protected virtual void OnAITurnEnded()
        {
            // Debug.Log($"{_player.Color} ai ended turn");
            GameManager.Instance.MainCamera.StopFollowing();
            turnEnded?.Invoke(_player.Color);
        }
        
        public void LoadFromSerializable(SerializableAIPlayerData serializableAIPlayerData)
        {
            _economicsGold = serializableAIPlayerData.EconomicsGold;
        }

        public virtual AIAttackTargetConditions CreateAIAttackTargetConditions(
            ControllableCharacter controllableCharacter)
        {
            bool chaseShooter = controllableCharacter.isFlying || controllableCharacter.MaxMovementValue >= 8;
            bool chaseWizard = controllableCharacter.isFlying || controllableCharacter.MaxMovementValue >= 8;
            var aiAttackTargetConditions = new AIAttackTargetConditions()
            {
                targetHasRetilation = 4,
                targetIsMelee = 1,
                targetIsShooter = chaseShooter ? 8 : 1,
                targetIsWizard = chaseWizard ? 4 : 1,
                targetIsTheHighestTier = 1,
                targetIsTheLeastTier = 1,
                targetMaxOutputDamage = 1,
                targetMinRetilationDamage = 1,
                targetIsClosest = 1,
                targetIsHero = 1,
                targetIsNeutral = 0,
                targetIsHumanPlayer = 4,
                targetIsAIPlayer = 4
            };
            return aiAttackTargetConditions;
        }

        public virtual void Init(Player player)
        {
            _player = player;
            _controlledCharacters = _player.controlledCharacters;
            // player.CharacterSpawned += character => _controlledCharacters.Add(character);
            // player.CharacterDestroyed += character => _controlledCharacters.Remove(character);
            _controlledCastles = _player.controlledCastles;
        }

        public virtual CharacterShopBuilding DecideWhatShopBuildingToBuild(Castle castle, int tier)
        {
            var shops = castle.ShopBuildings.Values;
            List<CharacterShopBuilding> shopBuildingsThisTier = new List<CharacterShopBuilding>();
            foreach (var shop in shops)
            {
                if (shop.tier != tier) continue;
                shopBuildingsThisTier.Add(shop);
            }

            var rnd = Random.Range(0, shopBuildingsThisTier.Count);
            return shopBuildingsThisTier[rnd];
        }

        protected virtual bool ShouldCharacterFlee(ControllableCharacter controllableCharacter)
        {
            bool isHero = controllableCharacter is Hero;
            bool shoudFlee = false;
            if (isHero)
            {
                shoudFlee = (GlobalVariables.ValuePercentCompare(controllableCharacter.HealthLeft,
                                 controllableCharacter.HealthValue, 0.5f) ==
                             -1);
            }
            else
            {
            }

            return shoudFlee;
        }

        protected virtual IEnumerator IEHeroesTurn()
        {
            var heroes = _controlledCharacters.Where(x => x is Hero && x.isExistingInWorld).ToList();
            for (int i = 0; i < heroes.Count; i++)
            {
                var hero = heroes[i];
                var spellBook = hero.spellBook;
                var aiHeroController = hero.GetComponent<AIHeroController>();
                bool heroInsideGarnison = hero.InsideGarnison;
                if (heroInsideGarnison)
                {
                    var castle = _player.controlledCastles.Where(x => x.garnisonCharacters.Contains(hero))
                        .FirstOrDefault();
                    var position = castle.spawnPoint.position;
                    var v2 = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
                    var freePositions =
                        _player.ActiveTerrain.TerrainNavigator.NavigationMap.GetFreeCellsInRange(v2, 3);
                    if (freePositions.Count > 0)
                    {
                        var garnison = GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel;
                        if (hero != null)
                        {
                            garnison.RemoveCharacterSlot(hero);
                            garnison.SelectedGarnisonCharacter = null;
                            hero.PlaceOnTerrain(_player.ActiveTerrain);
                            hero.gameObject.SetActive(true);
                            castle.RemoveFromGarnisonCharacterStack(hero);
                            hero.Position = freePositions.FirstOrDefault();
                        }
                    }
                }

                bool shouldFlee = ShouldCharacterFlee(hero);
                if (shouldFlee)
                {
                    yield return Moroutine.Run(aiHeroController.IEHeroFlee()).WaitForComplete();
                }
                else
                {
                    yield return Moroutine.Run(aiHeroController.IEPollHeroTurn()).WaitForComplete();
                }
            }

            yield return null;
        }

        protected virtual IEnumerator IETryToPurchaseCharacter(Castle castle, CharacterShopBuilding shop,
            float purchaseValue)
        {
            if (shop.RecruitmentsAvailable == 0) yield break;
            var players = GameManager.Instance.MapScenarioHandler.AllPlayersExcludeNeutral;
            int maxFightPoints = 0;
            foreach (var player in players)
            {
                if (player.FightingPoints > maxFightPoints)
                    maxFightPoints = player.FightingPoints;
            }

            var pointsPurchaseTarget = maxFightPoints * purchaseValue;
            int pointsPurchaseCurrent = 0;
            bool dealSuccess = false;
            var gold = _player.Gold;
            var gems = _player.Gems;
            var character = shop.Level > 1 ? shop.eliteCharacterForm : shop.basicCharacterForm;
            var dealQuantity = Mathf.RoundToInt(Mathf.Min(shop.RecruitmentsAvailable, character.fullStackCount));
            if (dealQuantity == 0) yield break;
            var shopTier = shop.tier;
            if (shopTier <= 3 && dealQuantity <= Mathf.RoundToInt(character.baseGrowthPerDay)) yield break;
            int currentGoldPriceValue = character.goldPrice * Mathf.RoundToInt(dealQuantity);
            int currentGemsPriceValue = character.gemsPrice * Mathf.RoundToInt(dealQuantity);
            bool enoughGold = gold - _economicsGold >= currentGoldPriceValue;
            bool enoughGems = gems >= currentGemsPriceValue;
            // Debug.Log($"Buying {character.title} q: {dealQuantity}");
            int dealPercentLimit = 6 - (shopTier > 3 ? shopTier - 2 : 0);
            int minPurchaseCount = Mathf.RoundToInt(dealPercentLimit / 10f * character.fullStackCount);
            if (!enoughGold && dealQuantity > 1)
            {
                for (int i = 1; i <= 10 - dealPercentLimit; i++)
                {
                    dealQuantity -= Mathf.RoundToInt(dealQuantity * 0.1f);
                    currentGoldPriceValue = character.goldPrice * Mathf.RoundToInt(dealQuantity);
                    currentGemsPriceValue = character.gemsPrice * Mathf.RoundToInt(dealQuantity);
                    // Debug.Log($"Trying deal {dealQuantity} | gp: {currentGoldPriceValue}");
                    enoughGold = gold - _economicsGold >= currentGoldPriceValue;
                    enoughGems = gems >= currentGemsPriceValue;
                    if (dealQuantity == 0) break;
                    if (enoughGold && enoughGems)
                    {
                        dealSuccess = true;
                        break;
                    }
                }

                if (!dealSuccess) yield break;
            }
            else if (enoughGold)
            {
                dealSuccess = dealQuantity >= minPurchaseCount;
            }

            if (!dealSuccess) yield break;
            for (int i = Mathf.RoundToInt(dealQuantity); i >= minPurchaseCount; i--)
            {
                var pointsForDeal = (int)(Mathf.Pow(3, character.tier) * i);
                if (pointsForDeal < pointsPurchaseTarget) break;
                dealQuantity = i;
            }

            if (dealQuantity == 0) yield break;
            var position = castle.spawnPoint.position;
            var v2 = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            var freePositions =
                _player.ActiveTerrain.TerrainNavigator.NavigationMap.GetFreeCellsInRange(v2, 3);
            if (freePositions.Count <= 0) yield break;
            var spawnManager = GameManager.Instance.SpawnManager;
            var turnOfPlayerId = GameManager.Instance.MapScenarioHandler.TurnOfPlayerId;

            _player.Gold -= currentGoldPriceValue;
            _player.Gems -= currentGemsPriceValue;
            shop.RecruitmentsAvailable -= dealQuantity;

            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            var targetTerrain = playableMap.WorldTerrain;

            var firstFreePosition = freePositions.FirstOrDefault();
            var targetPosition = new Vector3Int(firstFreePosition.x, firstFreePosition.y, 0);
            var spawnedCharacter = spawnManager.SpawnCharacter(character.name, shop.fraction,
                turnOfPlayerId, targetPosition, targetTerrain, (int)dealQuantity);
            spawnedCharacter.CheckVisibility();
        }

        public virtual void ProcessAI()
        {
            bool siegeStarted = GameManager.Instance.MapScenarioHandler.IsSiegeStarted;
            bool playerInSiege = GameManager.Instance.MapScenarioHandler.playersInSiege.Contains(_player);
            if (siegeStarted)
            {
                if(playerInSiege)
                    Moroutine.Run(IEPollSiegeTurns());
                return;
            }
            Moroutine.Run(IEPollCharactersTurns());
        }

        protected virtual IEnumerator IEPlayerBuildDecision()
        {
            yield return null;
        }

        protected virtual IEnumerator IEPollSiegeTurns()
        {
            yield return null;
        }

        protected virtual IEnumerator IEPollCharactersTurns()
        {
            yield return null;
        }
    }
}