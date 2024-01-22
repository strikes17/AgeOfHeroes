using System.Collections;
using System.Linq;
using AgeOfHeroes.Spell;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.AI
{
    public class EasyAIPlayerController : BasicAIPlayerController
    {
        public EasyAIPlayerController()
        {
            Debug.Log("Easy AI Controller spawned!");
        }

        public override void Init(Player player)
        {
            base.Init(player);
            _difficulty = MatchDifficulty.Easy;
        }

        protected override IEnumerator IEPlayerBuildDecision()
        {
            var players = GameManager.Instance.MapScenarioHandler.AllPlayersExcludeNeutral;
            int maxFightPoints = 1;
            int selfFightingPoints = _player.FightingPoints;
            foreach (var player in players)
            {
                if (player == _player) continue;
                if (player.FightingPoints > maxFightPoints)
                    maxFightPoints = player.FightingPoints;
            }

            float globalFpRatio = selfFightingPoints / maxFightPoints;

            foreach (var castle in _controlledCastles)
            {
                var gold = _player.Gold;
                var gems = _player.Gems;
                var invadingCharacters = _player.ActiveTerrain.TerrainNavigator.NavigationMap.GetAllCharactersInRange(
                    castle.Position,
                    _player, 10, (long)MagicSpellAllowedTarget.Enemy, true);
                int invadersFightingPoints = 1, defendersFightingPoints = 1;
                float shouldBuyMoreCharacters =
                    _player.controlledCharacters.Where(x => x is Character).Count() < 3 ? 1f : 0.1f;
                bool enemyHeroesAround = false;
                for (int i = 0; i < invadingCharacters.Count; i++)
                {
                    var x = invadingCharacters[i];
                    if (x.playerOwnerColor == PlayerColor.Neutral) continue;
                    if (x is Hero) enemyHeroesAround = true;
                    invadersFightingPoints += x.FightingPoints;
                }

                var defendingCharacters = _player.ActiveTerrain.TerrainNavigator.NavigationMap.GetAllCharactersInRange(
                    castle.Position, _player, 10, (long)MagicSpellAllowedTarget.Ally, true);
                for (int i = 0; i < defendingCharacters.Count; i++)
                {
                    var x = defendingCharacters[i];
                    defendersFightingPoints += x.FightingPoints;
                }

                defendersFightingPoints += castle.FightingPoints;
                Debug.Log($"{_player.Color} castle has fp: {defendersFightingPoints}");
                int invadersFpRatio = defendersFightingPoints / invadersFightingPoints;

                if (invadersFpRatio <= 0.5f)
                {
                    Debug.Log($"{_player.Color} raising army! ec before: {EconomicsGold}");
                    _economicsGold = EconomicsGold - (int)(EconomicsGold * 0.25f);
                    Debug.Log($"{_player.Color} raising army! ec after: {EconomicsGold}");
                    shouldBuyMoreCharacters += 1f;
                }
                else
                {
                    var g = (int)(castle.IncomeValues.gold * Random.Range(0.15f, 0.5f));
                    _economicsGold = EconomicsGold + g;
                }

                var specialBuildings = castle.SpecialBuildings.Values;
                foreach (var specialBuilding in specialBuildings)
                {
                }

                var tierUpgradeBuildings = castle.SpecialBuildings.Values.Where(x =>
                    x is CastleTierSpecialBuilding && !x.IsBuilt && !x.IsRestricted).ToList();
                if (tierUpgradeBuildings.Count > 0)
                    foreach (var tierUpgradeBuilding in tierUpgradeBuildings)
                    {
                        var goldCost = tierUpgradeBuilding.goldCost;
                        Debug.Log(
                            $"{_player.Color}Looking at {tierUpgradeBuilding.title}|cost{goldCost}| ec: {EconomicsGold}");
                        bool enoughGold = EconomicsGold >= goldCost;
                        bool enougGems = gems >= tierUpgradeBuilding.gemsCost;
                        if (enoughGold && enougGems)
                        {
                            castle.BuildSpecialBuilding(tierUpgradeBuilding);
                            _economicsGold = EconomicsGold - goldCost;
                            gold -= goldCost;
                        }
                    }
                else
                {
                    _economicsGold = 0;
                    shouldBuyMoreCharacters += 2f;
                }

                var shopBuildings = castle.ShopBuildings.Values.OrderBy(x => x.tier);
                foreach (var building in shopBuildings)
                {
                    // Debug.Log($"{_player.Color} - {building.title}");
                    if (building.IsBuilt)
                    {
                        int selfPoints = _player.FightingPoints;
                        foreach (var player in players)
                        {
                            if (player == _player) continue;
                            if (globalFpRatio <= 0.5f)
                            {
                                shouldBuyMoreCharacters += 0.35f;
                                break;
                            }
                        }

                        yield return Moroutine
                            .Run(IETryToPurchaseCharacter(castle, building, shouldBuyMoreCharacters))
                            .WaitForComplete();
                    }
                }

                for (int i = 1; i <= 6; i++)
                {
                    var buildingOfThisTier = castle.CheckIfTierAlreadyWasBuilt(i);
                    // Debug.Log(buildingOfThisTier.title);
                    bool enoughGold = false;
                    if (buildingOfThisTier != null)
                    {
                        enoughGold = gold - EconomicsGold >= buildingOfThisTier.upgradeGoldCost;
                        var isElite = buildingOfThisTier.Level > 1;
                        if (!isElite && enoughGold && gems >= buildingOfThisTier.upgradeGemsCost)
                        {
                            castle.UpgradeCharacterShop(buildingOfThisTier);
                            _player.Gold -= buildingOfThisTier.upgradeGoldCost;
                            _player.Gems -= buildingOfThisTier.upgradeGemsCost;
                        }
                    }
                    else
                    {
                        var building = DecideWhatShopBuildingToBuild(castle, i);
                        enoughGold = gold - EconomicsGold >= building.goldCost;
                        if (enoughGold && gems >= building.gemsCost)
                        {
                            castle.BuildCharacterShop(building);
                            _player.Gold -= building.goldCost;
                            _player.Gems -= building.gemsCost;
                            break;
                        }
                    }
                }

                yield return null;
            }
        }

        protected override IEnumerator IEPollSiegeTurns()
        {
            // yield return Moroutine.Run(IEPlayerBuildDecision()).WaitForComplete();
            // yield return Moroutine.Run(IEHeroesTurn()).WaitForComplete();
            for (int i = 0; i < _controlledCharacters.Count; i++)
            {
                var character = _controlledCharacters[i];
                if (character.InsideGarnison || character.PlayableTerrain != _player.ActiveTerrain) continue;
                var characterAI = character.GetComponent<AICharacterController>();
                if(characterAI.IsResting)continue;
                yield return Moroutine
                    .Run(characterAI.FindTheCandidatesForAttack(CreateAIAttackTargetConditions(character)))
                    .WaitForComplete();
                characterAI.Clear();
            }

            yield return new WaitForEndOfFrame();
            OnAITurnEnded();
        }

        protected override IEnumerator IEPollCharactersTurns()
        {
            // Debug.Log($"Polling {_player.Color} characters!");
            yield return Moroutine.Run(IEPlayerBuildDecision()).WaitForComplete();
            yield return Moroutine.Run(IEHeroesTurn()).WaitForComplete();
            int countBefore = 0, countAfter = 0, exceptionCounter = 0;
            for (int i = 0; i < _controlledCharacters.Count; i++)
            {
                countBefore = _controlledCharacters.Count;
                var character = _controlledCharacters[i];
                if(!character.isExistingInWorld)continue;
                if (character.InsideGarnison || character.PlayableTerrain != _player.ActiveTerrain) continue;
                var characterAI = character.GetComponent<AICharacterController>();
                if(characterAI.IsResting)continue;
                // Debug.Log($"Polling {character.title} - {character.GetHashCode()}");
                yield return Moroutine
                    .Run(characterAI.FindTheCandidatesForAttack(CreateAIAttackTargetConditions(character)))
                    .WaitForComplete();
                // Debug.Log($"Finished Polling {character.title} - {character.GetHashCode()}");
                characterAI.Clear();
                countAfter = _controlledCharacters.Count;
                if (countBefore != countAfter)
                {
                    exceptionCounter++;
                    if (exceptionCounter > 100)
                    {
                        Debug.Log($"Exception overflow!");
                        continue;
                    }
                    i--;
                }
                // Debug.Log($"before{countBefore} | after{countAfter}");
            }

            yield return new WaitForEndOfFrame();
            OnAITurnEnded();
            Debug.Log($"Finished Polling {_player.Color} characters!");
        }
    }
}