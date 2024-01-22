using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.Spell;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.AI
{
    public class AIHeroController : AICharacterController
    {
        private Hero _hero;
        private AbstractCollectable _collectableTarget;
        private ControllableCharacter _targetEnemy;
        private Castle _castleTarget, _neareastAllyCastle;
        private DwellBuildingBehaviour _dwellTarget;
        private List<ControllableCharacter> _heroSquad = new List<ControllableCharacter>();
        private List<AbstractCollectable> _ignoredCollectables = new();
        private List<DwellBuildingBehaviour> _ignoredDwellings = new();

        protected override void Awake()
        {
            base.Awake();
            _hero = _character as Hero;
        }

        public IEnumerator IEPollHeroTurn()
        {
            _isResting = false;
            yield return Moroutine.Run(IETryToCapture()).WaitForComplete();
        }

        public IEnumerator IETryToSiege()
        {
            if (_isResting) yield break;
        }

        public IEnumerator IEHeroFlee()
        {
            _neareastAllyCastle = _hero.Player.controlledCastles.FirstOrDefault();
            _isResting = true;
            if (_neareastAllyCastle != null)
                yield return Moroutine.Run(IEOrderToMove(_neareastAllyCastle.Position)).WaitForComplete();
        }

        public IEnumerator IETryToCapture()
        {
            int heroCheckIterations = 0, maxHeroCheckIterations = 8;
           
            int dwellSecurityPoints = 0, treasureSecurityPoints = 0;
            bool shouldAttackSecurity = false;
            var distanceToDwelling = float.MaxValue;
            var distanceToCollectable = float.MaxValue;
            int squadPoints = 0;
            while (_hero.MovementPointsLeft > 0 && heroCheckIterations < maxHeroCheckIterations)
            {
                var treasures = _hero.PlayableTerrain.SpawnedTreasures;
                var arts = _hero.PlayableTerrain.SpawnedArtifacts;
                var dwells = _hero.PlayableTerrain.SpawnedDwellings;
                var collectables = new List<AbstractCollectable>();
                heroCheckIterations++;
                // Debug.Log($"Hero {_hero.MovementPointsLeft}");
                if (_collectableTarget == null)
                {
                    collectables.AddRange(treasures);
                    collectables.AddRange(arts);
                    var pair = FindClosestTreasure(collectables);
                    _collectableTarget = pair.Item1;
                }

                if (_collectableTarget != null)
                {
                    var pathToTreasure = _hero.PlayableTerrain.TerrainNavigator.NavigationMap.FindPath(
                        _hero.GetHashCode(),
                        _hero.Position, _collectableTarget.Position);
                    if (pathToTreasure.Count == 0)
                    {
                        squadPoints = GetSquadPoints();
                        var security = GetSecurityAroundTreasure(_collectableTarget);
                        if (security == null)
                        {
                            _ignoredCollectables.Add(_collectableTarget);
                            yield break;
                        }

                        treasureSecurityPoints = security.FightingPoints;
                        // if (squadPoints < securityTargetPoints)
                        // {
                        //     bool newSquadAssembled = TryMakeSquadAndSetTarget(_targetEnemy, 10);
                        //     shouldAttackSecurity = newSquadAssembled;
                        // }

                        // if (!shouldAttackSecurity) yield break;
                        // yield return Moroutine.Run(IEOrderToMeleeAttack(_targetEnemy)).WaitForComplete();
                        // yield return Moroutine.Run(IEOrderSquadToAttackSecurity()).WaitForComplete();
                    }

                    distanceToCollectable = Vector2.Distance(_collectableTarget.Position, _hero.Position);
                    Debug.Log($"Distance to {_collectableTarget.name}: {distanceToCollectable}");
                }

                if (_dwellTarget == null)
                {
                    var pair = FindClosestDwelling(dwells);
                    _dwellTarget = pair.Item1;
                    // Debug.Log(_dwellTarget);
                }

                if (_dwellTarget != null)
                {
                    var pathToDwelling = _hero.PlayableTerrain.TerrainNavigator.NavigationMap.FindPath(
                        _hero.GetHashCode(),
                        _hero.Position, _dwellTarget.Position);
                    if (pathToDwelling.Count == 0)
                    {
                        squadPoints = GetSquadPoints();
                        var security = GetSecurityAroundDwelling(_dwellTarget);
                        if (security == null)
                        {
                            _ignoredDwellings.Add(_dwellTarget);
                            yield break;
                        }
                    }

                    distanceToDwelling = Vector2.Distance(_dwellTarget.Position, _hero.Position);
                    // Debug.Log($"Distance to {_dwellTarget.name}: {distanceToDwelling}");
                }

                if (!_dwellTarget && !_collectableTarget) yield break;

                bool isDwellingTarget = false;
                var dwellingCloser = distanceToDwelling <= distanceToCollectable;
                var dwellingEasier = dwellSecurityPoints <= treasureSecurityPoints;
                if (dwellingCloser && dwellingEasier)
                    isDwellingTarget = true;

                if (isDwellingTarget)
                {
                    if (!_dwellTarget) yield break;
                    _targetEnemy = GetSecurityAroundDwelling(_dwellTarget);
                }
                else
                {
                    if (!_collectableTarget) yield break;
                    _targetEnemy = GetSecurityAroundTreasure(_collectableTarget);
                }


                if (_targetEnemy != null)
                {
                    Debug.Log($"has enemy");
                    bool newSquadAssembled = TryMakeSquadAndSetTarget(_targetEnemy, 10);
                    yield return Moroutine.Run(IEOrderToMeleeAttack(_targetEnemy)).WaitForComplete();
                    yield return Moroutine.Run(IEOrderSquadToAttackSecurity()).WaitForComplete();
                    yield break;
                }

                // Debug.Log($"isDwell Target {isDwellingTarget}");
                if (isDwellingTarget)
                    yield return Moroutine.Run(IEOrderToCaptureDwell(_dwellTarget)).WaitForComplete();
                else
                    yield return Moroutine.Run(IEOrderToCollectResource(_collectableTarget)).WaitForComplete();


                yield return null;
            }
        }

        public ControllableCharacter TryMakeSquadAndSetTarget(ControllableCharacter targetSecurity, int range,
            bool useExternalForcesIfNeeded = false)
        {
            _heroSquad.Clear();
            var targetFightingPoints = targetSecurity.FightingPoints;
            var alliesAroundHero = _hero.GetAllCharactersInRange(range,
                (long)MagicSpellAllowedTarget.Ally | (long)MagicSpellAllowedTarget.Any,
                false);
            int squadFightingPoints = 0;
            foreach (var ally in alliesAroundHero)
            {
                // Debug.Log($"ally {ally.title} - {ally.GetHashCode()}");
                if (squadFightingPoints < targetFightingPoints)
                {
                    squadFightingPoints += ally.FightingPoints;
                    _heroSquad.Add(ally);
                    ally.StackDemolished -= (target, source) => _heroSquad.Remove(ally);
                    ally.StackDemolished += (target, source) => _heroSquad.Remove(ally);
                    GlobalVariables.__SpawnMarker(ally.Position, Color.red);
                }
                else
                {
                    return targetSecurity;
                }
            }

            return null;
        }

        private int GetSquadPoints()
        {
            int points = 0;
            foreach (var ally in _heroSquad)
            {
                points += ally.FightingPoints;
            }

            return points;
        }

        private IEnumerator IEOrderSquadToAttackSecurity()
        {
            for (int i = 0; i < _heroSquad.Count; i++)
            {
                var character = _heroSquad[i];
                var aiController = character.GetComponent<AICharacterController>();
                bool isShooter = character.isShooter;
                if (isShooter)
                    yield return Moroutine.Run(aiController.IEOrderRangedAttack(_targetEnemy)).WaitForComplete();
                else
                    yield return Moroutine.Run(aiController.IEOrderToMeleeAttack(_targetEnemy)).WaitForComplete();
            }
        }

        public IEnumerator IEOrderToCaptureDwell(DwellBuildingBehaviour dwellBuilding)
        {
            yield return Moroutine.Run(IEOrderToMove(dwellBuilding.Position)).WaitForComplete();
            var dwellsAround = _character.CheckNeighbourCells(_character.transform.position, "Building");
            if (dwellsAround.Count > 0)
            {
                bool isDwellOurTarget = dwellsAround.Contains(dwellBuilding.Position);
                if (isDwellOurTarget)
                {
                    if(dwellBuilding.Player == _hero.Player)yield break;
                    dwellBuilding.Capture(_hero.Player);
                    _dwellTarget = null;
                    // Debug.Log("CAPTURED");
                }
            }
        }

        public IEnumerator IEOrderToCollectResource(AbstractCollectable abstractCollectable)
        {
            yield return Moroutine.Run(IEOrderToMove(abstractCollectable.Position)).WaitForComplete();
            var treasuresAround = _character.CheckNeighbourCellsForTreasures(_character.transform.position);
            if (treasuresAround.Count > 0)
            {
                bool isTreasureOurTarget = treasuresAround.Contains(abstractCollectable.Position);
                if (isTreasureOurTarget)
                {
                    abstractCollectable.OnAICollected(_hero);
                    _collectableTarget = null;
                }
            }
        }

        public ControllableCharacter GetSecurityAroundTreasure(AbstractCollectable abstractCollectable)
        {
            var securitiesAroundTreasure =
                _hero.PlayableTerrain.TerrainNavigator.NavigationMap.GetAllCharactersInRange(
                    abstractCollectable.Position,
                    _hero.Player, 1, (long)MagicSpellAllowedTarget.Enemy, false);
            var targetSecurity = securitiesAroundTreasure.FirstOrDefault();
            return targetSecurity;
        }

        public ControllableCharacter GetSecurityAroundDwelling(DwellBuildingBehaviour dwellBuildingBehaviour)
        {
            var securitiesAroundTreasure =
                _hero.PlayableTerrain.TerrainNavigator.NavigationMap.GetAllCharactersInRange(
                    dwellBuildingBehaviour.Position,
                    _hero.Player, 1, (long)MagicSpellAllowedTarget.Enemy, false);
            var targetSecurity = securitiesAroundTreasure.FirstOrDefault();
            return targetSecurity;
        }

        public (AbstractCollectable, ControllableCharacter) FindClosestTreasure(List<AbstractCollectable> collectables)
        {
            int leastFightingPoints = int.MaxValue;
            // for (int i = 0; i < collectables.Count; i++)
            // {
            //     var str = collectables[i] ? collectables[i].GetInstanceID().ToString() : "null";
            //     Debug.Log($"{str}: {collectables[i] != null}");
            // }
            List<AbstractCollectable> orderedCollectablesByDistance = new List<AbstractCollectable>();
            orderedCollectablesByDistance = collectables.Where(x => x is not null).ToList();
            orderedCollectablesByDistance =
                orderedCollectablesByDistance.Where(x => !_ignoredCollectables.Contains(x)).ToList();
            orderedCollectablesByDistance = orderedCollectablesByDistance
                .OrderBy(x => Vector2Int.Distance(_hero.Position, x.Position)).ToList();
            // Debug.Log($"Collectables count around {orderedCollectablesByDistance.Count}");
            bool squadAssembled = false;
            for (int i = 0; i < orderedCollectablesByDistance.Count; i++)
            {
                var collectable = orderedCollectablesByDistance[i];
                var collectablePosition = collectable.transform.position;
                // Debug.Log(
                //     $"collectable {collectable.name} has distance from hero: {Vector2.Distance(_hero.Position, collectablePosition)}");
                var targetSecurity = GetSecurityAroundTreasure(collectable);
                if (!targetSecurity)
                {
                    return (collectable, targetSecurity);
                }

                var securityFightingPoints = targetSecurity.FightingPoints;
                Debug.Log($"{targetSecurity.title} securing {collectable.name} has {securityFightingPoints} points");
                squadAssembled = TryMakeSquadAndSetTarget(targetSecurity, 10);
                if (squadAssembled)
                {
                    Debug.Log($"Squad assembled fp: {GetSquadPoints()}");
                    return (collectable, targetSecurity);
                }
                else
                {
                    Debug.Log("No purpose!");
                }
            }

            return (null, null);
        }

        public (DwellBuildingBehaviour, ControllableCharacter) FindClosestDwelling(
            List<DwellBuildingBehaviour> dwellBuildings)
        {
            int leastFightingPoints = int.MaxValue;
            List<DwellBuildingBehaviour> orderedDwellingsByBuildings = new List<DwellBuildingBehaviour>();
            orderedDwellingsByBuildings = dwellBuildings.Where(x => !_ignoredDwellings.Contains(x)).ToList();

            orderedDwellingsByBuildings = orderedDwellingsByBuildings
                .OrderBy(x => Vector2Int.Distance(_hero.Position, x.Position)).ToList();
            // Debug.Log($"Collectables count around {orderedCollectablesByDistance.Count}");
            bool squadAssembled = false;
            for (int i = 0; i < orderedDwellingsByBuildings.Count; i++)
            {
                var dwelling = orderedDwellingsByBuildings[i];
                if(dwelling.Player == _hero.Player)continue;
                var collectablePosition = dwelling.Position;
                // Debug.Log(
                //     $"collectable {collectable.name} has distance from hero: {Vector2.Distance(_hero.Position, collectablePosition)}");
                var targetSecurity = GetSecurityAroundDwelling(dwelling);
                if (targetSecurity == null)
                {
                    return (dwelling, targetSecurity);
                }

                var securityFightingPoints = targetSecurity.FightingPoints;
                Debug.Log($"{targetSecurity.title} securing {dwelling.name} has {securityFightingPoints} points");
                squadAssembled = TryMakeSquadAndSetTarget(targetSecurity, 10);
                if (squadAssembled)
                {
                    Debug.Log($"Squad assembled fp: {GetSquadPoints()}");
                    return (dwelling, targetSecurity);
                }
                else
                {
                    Debug.Log("No purpose!");
                }
            }

            return (null, null);
        }
    }
}