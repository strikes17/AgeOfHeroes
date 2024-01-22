using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using AgeOfHeroes.Spell;
using AStar;
using AStar.Heuristics;
using AStar.Options;
using Redcode.Moroutines;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace AgeOfHeroes.AI
{
    public class AICharacterController : MonoBehaviour
    {
        protected ControllableCharacter _character;
        protected List<AIDecision> _allAIDecisionsThisRound = new List<AIDecision>();
        protected int characterLayerMask;
        protected AIManager _aiManager;
        protected List<GameObject> visualPath = new List<GameObject>();
        protected int hashCode;
        protected bool _isResting;

        public ControllableCharacter Character => _character;

        public bool IsResting
        {
            get => _isResting;
            set => _isResting = value;
        }

        protected virtual void Awake()
        {
            _aiManager = GameManager.Instance.aiManager;
            characterLayerMask = 1 << LayerMask.NameToLayer("Character");
            _character = GetComponent<ControllableCharacter>();
            hashCode = GetHashCode();
        }

        public void Clear()
        {
            for (int i = 0; i < visualPath.Count; i++)
            {
                Destroy(visualPath[i]);
            }

            visualPath.Clear();
        }

        public IEnumerator IEOrderToMove(Vector2Int targetPosition, int steps = 1000)
        {
            // Debug.Log($"Order to move for {_character.title} - {_character.GetHashCode()}");
            GameManager.Instance.MainCamera.SetFollowTo(_character.transform);
            var start = _character.Position;
            var end = targetPosition;
            NavigationMap navigationMap = _character.PlayableTerrain.TerrainNavigator.NavigationMap;
            var path = navigationMap.FindPath(hashCode, start, end);
            if (path.Count <= 0)
                yield break;
            path.RemoveAt(path.Count - 1);
            if (path.Count <= 0)
                yield break;
            path.RemoveAt(0);
            int movementCost = path.Count;
            int movementPointsLeft = _character.MovementPointsLeft;
            if (movementPointsLeft > 0 && steps > 0)
            {
                // Debug.Log($"{_character.title} mpl: {movementPointsLeft}");
                if (movementCost > 0)
                {
                    if (movementCost > movementPointsLeft)
                    {
                        int targetIndex = Mathf.Min(steps, movementPointsLeft) - 1;
                        // Debug.Log($"s: min({steps}|{movementPointsLeft}): {_character.title} movecost: {movementCost}");
                        // Debug.Log(targetIndex);
                        end = new Vector2Int(path[targetIndex].x, path[targetIndex].y);
                    }
                    else
                    {
                        int targetIndex = Mathf.Min(steps, movementCost) - 1;
                        // Debug.Log(targetIndex);
                        end = new Vector2Int(path[targetIndex].x, path[targetIndex].y);
                    }

                    yield return Moroutine.Run(_character.MoveToPosition(end)).WaitForComplete();
                }
            }
        }

        public Dictionary<ControllableCharacter, int> FindCandidates(List<ControllableCharacter> characters,
            AIAttackTargetConditions conditions)
        {
            List<int> charactersPoints = new List<int>();
            int highestTier = -1, highestOutputDamage = -1, leastRetilationDamage = -1, leastTier = -1;
            float closestDistance = Mathf.Infinity;
            for (int i = 0; i < characters.Count; i++)
            {
                charactersPoints.Add(0);

                var enemy = characters[i];
                int tier = enemy.GetTier();
                var charPos = _character.transform.position;
                var enemyPos = enemy.transform.position;
                float distance = Vector2.Distance(charPos, enemyPos);
                // Debug.Log($"distance to {enemy.title} = {distance}");
                var outputDamage = enemy.CalculateIncomingDamage(new CombatData()
                {
                    defensiveCharacter = enemy,
                    offensiveCharacter = _character
                });

                var retilationDamage = enemy.CalculateIncomingDamage(new CombatData()
                {
                    defensiveCharacter = _character,
                    offensiveCharacter = enemy
                });
                if (tier >= highestTier)
                    highestTier = tier;
                if (tier <= leastTier)
                    leastTier = tier;
                if (outputDamage >= highestOutputDamage)
                    highestOutputDamage = outputDamage;
                if (retilationDamage >= leastRetilationDamage)
                    leastRetilationDamage = retilationDamage;
                if (retilationDamage >= leastRetilationDamage)
                    leastRetilationDamage = retilationDamage;
                if (distance <= closestDistance)
                    closestDistance = distance;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                var enemy = characters[i];
                int tier = enemy.GetTier();
                var charPos = _character.transform.position;
                var enemyPos = enemy.transform.position;
                float distance = Vector2.Distance(charPos, enemyPos);
                var outputDamage = enemy.CalculateIncomingDamage(new CombatData()
                {
                    defensiveCharacter = enemy,
                    offensiveCharacter = _character
                });

                var retilationDamage = enemy.CalculateIncomingDamage(new CombatData()
                {
                    defensiveCharacter = _character,
                    offensiveCharacter = enemy
                });
                var humanPlayerPoints = (int)conditions.targetIsHumanPlayer * (Convert.ToInt16(enemy.Player.isHuman));
                var aiPlayerPoints = (int)conditions.targetIsAIPlayer *
                                     (Convert.ToInt16(!enemy.Player.isHuman &&
                                                      enemy.playerOwnerColor != PlayerColor.Neutral));
                var heroPoints = (int)conditions.targetIsHero * (Convert.ToInt16(enemy is Hero));
                var meleePoints = (int)conditions.targetIsMelee * (Convert.ToInt16(!enemy.isShooter));
                var shooterPoints = (int)conditions.targetIsShooter * (Convert.ToInt16(enemy.isShooter));
                var wizardPoints = (int)conditions.targetIsWizard * (Convert.ToInt16(enemy.isWizard));
                var noRetilationPoints =
                    (int)conditions.targetHasRetilation * (Convert.ToInt16(enemy.RetilationsLeft <= 0));
                var highestTierPoints = (int)conditions.targetIsTheHighestTier * (Convert.ToInt16(tier == highestTier));
                var highestOutputDamagePoints = (int)conditions.targetMaxOutputDamage *
                                                (Convert.ToInt16(outputDamage == highestOutputDamage));
                var leastTierPoints = (int)conditions.targetIsTheLeastTier * (Convert.ToInt16(tier == leastTier));
                var leastRetilationDamagePoints = (int)conditions.targetMinRetilationDamage *
                                                  (Convert.ToInt16(retilationDamage == leastRetilationDamage));
                var closestDistancePoints = (int)conditions.targetIsClosest *
                                            (Convert.ToInt16(distance <= closestDistance + 0.1f));

                int summaryPoints = meleePoints + shooterPoints + wizardPoints + noRetilationPoints + highestTierPoints
                                    + highestOutputDamagePoints + leastTierPoints + leastRetilationDamagePoints +
                                    closestDistancePoints + heroPoints + aiPlayerPoints + humanPlayerPoints;
                // Debug.Log(
                //     $"{_character.title}looks at {enemy.title} - points: {summaryPoints}; distance: {distance}; closest: {closestDistance}");
                charactersPoints[i] += summaryPoints;
            }

            Dictionary<ControllableCharacter, int> candidates = new Dictionary<ControllableCharacter, int>();

            int averagePoints = 0;
            foreach (var cp in charactersPoints)
            {
                if (cp > 0)
                    averagePoints += cp;
            }

            int count = Mathf.Clamp(charactersPoints.Count, 1, charactersPoints.Count);
            averagePoints /= count;

            for (int i = 0; i < charactersPoints.Count; i++)
            {
                // Debug.Log($"Points of {characters[i].title} :{charactersPoints[i]}");
                if (charactersPoints[i] >= averagePoints)
                {
                    candidates.Add(characters[i], charactersPoints[i]);
                }
            }

            return candidates;
        }

        public IEnumerator FindTheCandidatesForAttack(AIAttackTargetConditions conditions)
        {
            var enemies = ScanAreaForEnemies(500);
            var candidates = FindCandidates(enemies, conditions);
            if (candidates.Count <= 0)
                yield break;
            ControllableCharacter target = null;
            int maximumPoints = 0;
            foreach (var candidate in candidates.Keys)
            {
                var points = candidates[candidate];
                if (points < maximumPoints) continue;
                if (candidate.GetTier() - 3 > Character.GetTier()) continue;
                maximumPoints = points;
                target = candidate;
            }

            if (target == null) yield break;

            float distance = 1000f;
            bool isShooter = _character.isShooter;
            if (isShooter)
            {
                List<ControllableCharacter> availableCandidates = new List<ControllableCharacter>();
                GameManager.Instance.MainCamera.SetFollowTo(_character.transform);
                foreach (var candidate in candidates.Keys)
                {
                    distance = Vector2.Distance(_character.Position, candidate.Position);
                    if (distance <= _character.attackRange)
                    {
                        yield return Moroutine.Run(IEAttackTarget(candidate)).WaitForComplete();
                        break;
                    }
                }

                yield return Moroutine.Run(IEOrderRangedAttack(target)).WaitForComplete();
            }
            else
                yield return Moroutine.Run(IEOrderToMeleeAttack(target)).WaitForComplete();
        }

        public virtual IEnumerator IEOrderRangedAttack(ControllableCharacter target)
        {
            if (target == null || _character == null) yield break;
            GameManager.Instance.MainCamera.SetFollowTo(_character.transform);
            int desiredSteps =
                Mathf.CeilToInt((target.Position - _character.Position).magnitude - _character.attackRange);
            yield return Moroutine.Run(IEOrderToMove(target.Position, desiredSteps)).WaitForComplete();
            yield return Moroutine.Run(IEAttackTarget(target)).WaitForComplete();
        }

        public virtual IEnumerator IEAttackTarget(ControllableCharacter target)
        {
            if (target == null) yield break;
            var distance = Mathf.CeilToInt(Vector2.Distance(_character.Position, target.Position));
            if (distance > _character.attackRange)
            {
                yield break;
            }

            CombatData combatData = new CombatData()
            {
                defensiveCharacter = target,
                offensiveCharacter = _character,
                performedOnRetilation = false,
            };
            combatData.totalDamage = target.CalculateIncomingDamage(combatData);
            combatData.killQuantity = target.CalculateQuantityChangeInStack(combatData);
            yield return Moroutine.Run(_character.IECombatAttackProcess(combatData)).WaitForComplete();
        }

        public IEnumerator IEOrderToMeleeAttack(ControllableCharacter target)
        {
            if (target == null) yield break;
            yield return Moroutine.Run(IEOrderToMove(target.Position)).WaitForComplete();
            // var neighbours = _character.CheckNeighbourCellsForCharactersAndActionCells(_character.transform.position);
            var neighbours = _character.PlayableTerrain.TerrainNavigator.NavigationMap
                .GetAllCharactersInRange(_character.Position, _character.Player, _character.attackRange,
                    (long)MagicSpellAllowedTarget.Enemy, true);
            if (neighbours.Count > 0)
            {
                ControllableCharacter targetOfAttack = null;
                bool isNeighbourTarget = neighbours.Contains(target);
                if (isNeighbourTarget)
                    targetOfAttack = target;
                else
                {
                    targetOfAttack = neighbours[Random.Range(0, neighbours.Count - 1)];
                }

                yield return Moroutine.Run(IEAttackTarget(targetOfAttack)).WaitForComplete();
            }
        }

        public void ShowVisualPath(ControllableCharacter currentTarget)
        {
            var start = _character.Position;
            var end = currentTarget.Position;

            NavigationMap navigationMap = _character.PlayableTerrain.TerrainNavigator.NavigationMap;
            var path = navigationMap.FindPath(hashCode, start, end);

            var visualPrefab = ResourcesBase.GetPrefab("_NAV");
            path.ForEach(x =>
            {
                var visualInstance =
                    GameObject.Instantiate(visualPrefab, new Vector3(x.x, x.y, 0f), Quaternion.identity);
                visualPath.Add(visualInstance);
            });
        }

        public List<ControllableCharacter> ScanAreaForEnemies(int radius)
        {
            // int radius = _character.MovementPointsLeft;
            var results = Physics2D.OverlapCircleAll(transform.position, radius, characterLayerMask);
            List<ControllableCharacter> enemies = new List<ControllableCharacter>();
            foreach (var r in results)
            {
                var character = r.GetComponent<ControllableCharacter>();
                if (character != null)
                {
                    if (!character.gameObject.activeSelf) continue;
                    bool sameMap = _character.PlayableTerrain.GetHashCode() == character.PlayableTerrain.GetHashCode();
                    if (!sameMap)
                        continue;
                    if (_character.playerOwnerColor == character.playerOwnerColor)
                        continue;
                    if (character.InsideGarnison) continue;
                    // if (character.GetComponent<Hero>() != null)
                    //     continue;
                    // Debug.Log($"{_character.title} has found {character.title} enemy!");
                    enemies.Add(character);
                }
            }

            return enemies;
        }
    }
}