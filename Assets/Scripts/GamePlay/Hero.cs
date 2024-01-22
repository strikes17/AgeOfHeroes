using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;

namespace AgeOfHeroes
{
    [Icon("Assets/Editor/hero.png")]
    [RequireComponent(typeof(HeroEditorAddon))]
    public class Hero : ControllableCharacter
    {
        public override int DefenseValue
        {
            get
            {
                float defenseMultiplier = 0f;
                float defenseSummarizer = 0f;
                foreach (var mod in defenseModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                        defenseMultiplier += mod.value * baseDefenseValue;
                    else if (mod.Operation == (int)ModifierOperation.Change)
                        defenseSummarizer += mod.value;
                }

                float artifactsDefenseMultiplier = 0f;
                float artifactsDefenseSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Defense)
                        artifactsDefenseSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Defense)
                        artifactsDefenseMultiplier += modifier.Value.value * baseDefenseValue;
                }

                return Mathf.RoundToInt(baseDefenseValue + artifactsDefenseMultiplier + defenseMultiplier +
                                        defenseSummarizer + artifactsDefenseSummarizer);
            }
        }

        public override int AttackValue
        {
            get
            {
                float attackMultiplier = 0f;
                float attackSummarizer = 0f;
                foreach (var mod in attackModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                        attackMultiplier += mod.value * baseAttackValue;
                    else if (mod.Operation == (int)ModifierOperation.Change)
                        attackSummarizer += mod.value;
                }

                float artifactsAttackMultiplier = 0f;
                float artifactsAttackSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Attack)
                        artifactsAttackSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Attack)
                        artifactsAttackMultiplier += modifier.Value.value * baseAttackValue;
                }

                return Mathf.RoundToInt(baseAttackValue + attackMultiplier + artifactsAttackMultiplier +
                                        artifactsAttackSummarizer + attackSummarizer);
            }
        }

        public override int DamageValue
        {
            get
            {
                float damageMultiplier = 0f;
                float damageSummarizer = 0f;
                foreach (var mod in damageModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                        damageMultiplier += mod.value * baseDamageValue;
                    else if (mod.Operation == (int)ModifierOperation.Change)
                        damageSummarizer += mod.value;
                }

                float artifactsDamageMultiplier = 0f;
                float artifactsDamageSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Damage)
                        artifactsDamageSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Damage)
                        artifactsDamageMultiplier += modifier.Value.value * baseDamageValue;
                }

                float damage = baseDamageValue + artifactsDamageSummarizer + damageSummarizer +
                               artifactsDamageMultiplier + damageMultiplier;
                int calculatedDamage = Mathf.FloorToInt(damage);
                calculatedDamage = calculatedDamage <= 0 ? 1 : calculatedDamage;
                return calculatedDamage;
            }
        }

        public override int VisionValue
        {
            get
            {
                float visionMult = 0f;
                float visionSumm = 0f;
                foreach (var mod in visionModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                    {
                        visionMult += mod.value * baseVisionRadius;
                    }
                    else if (mod.Operation == (int)ModifierOperation.Change)
                    {
                        visionSumm += mod.value;
                    }
                }

                float artifactsDamageMultiplier = 0f;
                float artifactsDamageSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Vision)
                        artifactsDamageSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Vision)
                        artifactsDamageMultiplier += modifier.Value.value * baseDamageValue;
                }

                float vision = baseVisionRadius + visionMult + visionSumm + artifactsDamageSummarizer +
                               artifactsDamageMultiplier;
                return Mathf.RoundToInt(vision);
            }
        }

        public override int HealthValue
        {
            get
            {
                float healthMult = 0f;
                float healthSumm = 0f;
                foreach (var mod in healthModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                    {
                        healthMult += mod.value * baseHealth;
                    }
                    else if (mod.Operation == (int)ModifierOperation.Change)
                    {
                        healthSumm += mod.value;
                    }
                }

                float artifactsDamageMultiplier = 0f;
                float artifactsDamageSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Health)
                        artifactsDamageSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.Health)
                        artifactsDamageMultiplier += modifier.Value.value * baseDamageValue;
                }

                float health = baseHealth + healthMult + healthSumm + artifactsDamageMultiplier +
                               artifactsDamageSummarizer;
                return Mathf.RoundToInt(health);
            }
        }

        public override int MaxMovementValue
        {
            get
            {
                float moveMult = 0f;
                float moveSumm = 0f;
                foreach (var mod in moveSpeedModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                    {
                        moveMult += mod.value * startingMovementPoints;
                    }
                    else if (mod.Operation == (int)ModifierOperation.Change)
                    {
                        moveSumm += mod.value;
                    }
                }

                float artifactsMultiplier = 0f;
                float artifactsSummarizer = 0f;

                foreach (var modifier in _artifactsAdditionModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.MovementSpeed)
                        artifactsSummarizer += modifier.Value.value;
                }

                foreach (var modifier in _artifactsMultiplicationModifiers)
                {
                    if (modifier.Value.StatType == HeroModifieableStat.MovementSpeed)
                        artifactsMultiplier += modifier.Value.value * startingMovementPoints;
                }

                float health = startingMovementPoints + moveMult + moveSumm + artifactsMultiplier + artifactsSummarizer;
                return Mathf.RoundToInt(health);
            }
        }

        public HeroObject HeroObject
        {
            get { return (HeroObject)_controllableCharacterObject; }
            set { _controllableCharacterObject = value; }
        }

        public delegate void OnExperienceEventDelegate(Hero hero, int currentLevel, int experienceRecievedValue);

        protected void OnLeveledUp(Hero hero, int currentLevel, int experienceRecieved)
        {
            leveledUp?.Invoke(hero, currentLevel, experienceRecieved);
        }

        protected void OnExperienceRecieved(Hero hero, int currentLevel, int experienceRecievedValue)
        {
            experienceRecieved?.Invoke(hero, currentLevel, experienceRecievedValue);
        }

        public event OnExperienceEventDelegate LeveledUp
        {
            add => leveledUp += value;
            remove => leveledUp -= value;
        }

        public event OnExperienceEventDelegate ExperienceRecieved
        {
            add => experienceRecieved += value;
            remove => experienceRecieved -= value;
        }

        private event OnExperienceEventDelegate leveledUp;
        private event OnExperienceEventDelegate experienceRecieved;
        public Colored coloredFlag;
        public TMP_Text _tmpTextHealthLeft;
        public SpriteRenderer _quantityBackgroundSprite;


        public HeroInventoryManager inventoryManager => _inventoryManager;

        public HeroSkillTree SkillTree => _heroSkillTree;

        private Dictionary<int, ArtifactModifier> _artifactsAdditionModifiers = new Dictionary<int, ArtifactModifier>();

        private Dictionary<int, ArtifactModifier> _artifactsMultiplicationModifiers =
            new Dictionary<int, ArtifactModifier>();

        private HeroSkillTree _heroSkillTree;
        private Moroutine _updateSkillTreeMoroutine;

        // private IEnumerator IEUpdateSkillTree()
        // {
        //     while (isExistingInWorld)
        //     {
        //         SkillTree.Update();
        //         yield return null;
        //     }
        // }
        //
        // private void OnDestroy()
        // {
        //     isExistingInWorld = false;
        //     _updateSkillTreeMoroutine.Stop();
        // }

        private void OnArtifactAddedToHero(Artifact artifact)
        {
            var modifiers = artifact.Modifiers;
            foreach (var modifier in modifiers)
            {
                if (modifier.operation == ModifierOperation.Change)
                    _artifactsAdditionModifiers.TryAdd(modifier.GetHashCode(), modifier);
                else if (modifier.operation == ModifierOperation.Multiply)
                    _artifactsMultiplicationModifiers.TryAdd(modifier.GetHashCode(), modifier);
            }
        }

        private void OnArtifactRemovedFromHero(Artifact artifact)
        {
            var modifiers = artifact.Modifiers;
            foreach (var modifier in modifiers)
            {
                if (modifier.operation == ModifierOperation.Change)
                    _artifactsAdditionModifiers.Remove(modifier.GetHashCode());
                else if (modifier.operation == ModifierOperation.Multiply)
                    _artifactsMultiplicationModifiers.Remove(modifier.GetHashCode());

                Debug.Log(
                    $"{title} has removed #{modifier.GetHashCode()} {modifier.StatType.ToString()} with value of: {modifier.value}");
            }
        }

        public int CurrentExperience
        {
            get { return _currentExperience = _totalExperience - experiencePerLevel[_currentLevel - 1]; }
        }

        public int TotalExperience
        {
            get { return _totalExperience; }
            set
            {
                if (value > _totalExperience)
                {
                    FloatingText.Create(Position, 0.25f, 0.25f).MakeFloat(GlobalVariables.experienceGainTextColor,
                        value - _totalExperience);
                }

                _totalExperience = value > experiencePerLevel[MaxLevel - 1] ? experiencePerLevel[MaxLevel - 1] : value;
                for (int i = _currentLevel; i < experiencePerLevel.Count; i++)
                {
                    if (_totalExperience >= experiencePerLevel[i])
                    {
                        _currentLevel = i + 1;
                        OnLeveledUp(this, CurrentLevel, value);
                    }
                    else break;
                }
            }
        }

        public int ExperienceForNextLevel
        {
            get
            {
                if (_currentLevel == MaxLevel) return -1;
                return experiencePerLevel[_currentLevel] - experiencePerLevel[_currentLevel - 1];
            }
        }

        public float LeftExperienceRatio
        {
            get => (float)CurrentExperience / (float)ExperienceForNextLevel;
        }

        public int CurrentLevel => _currentLevel;

        public int MaxLevel => experiencePerLevel.Count;

        private int _currentExperience, _totalExperience;
        private int _currentLevel;
        private HeroInventoryManager _inventoryManager = new HeroInventoryManager();
        public List<int> experiencePerLevel = new List<int>();

        public override void Init()
        {
            base.Init();
            _spriteRenderer.sortingOrder = GlobalVariables.HeroRenderOrder;
            isFlying = HeroObject.isFlying;
            Vector3 v3Position = transform.position;
            Vector3Int v3IntPosition = new Vector3Int((int)v3Position.x, (int)v3Position.y, 0);
            _currentLevel = 1;
            experiencePerLevel = HeroObject.experiencePerLevel;
            for (int i = 0; i < HeroObject.persona.Count; i++)
            {
                persona |= (long)HeroObject.persona[i];
            }

            inventoryManager.ArtifactAdded -= OnArtifactAddedToHero;
            inventoryManager.ArtifactAdded += OnArtifactAddedToHero;
            inventoryManager.ArtifactRemoved -= OnArtifactRemovedFromHero;
            inventoryManager.ArtifactRemoved += OnArtifactRemovedFromHero;
            var skillTreeObject = ResourcesBase.GetHeroSkillTree(HeroObject.skillTreeObjectName);
            _heroSkillTree = new HeroSkillTree(skillTreeObject);
            LearnBaseSpecialization();
            LeveledUp += (hero, level, value) =>
            {
                SpriteAnimationPlayer.Instance.PlayAnimation(transform.position,
                    ResourcesBase.GetSpriteAnimation("ps_levelup_1"), 1);
                SkillTree.LevelPoints++;
            };
            UniqueId = GetInstanceID();
        }

        public override void LoadFromSerializable(SerializableCharacter serializableCharacter)
        {
            base.LoadFromSerializable(serializableCharacter);
            var serializableHero = (SerializableHero)serializableCharacter;
            TotalExperience = serializableHero.totalExperience;
            Count = 1;
            if (serializableHero.SerializableHeroSkillTree != null)
                SkillTree.LoadFromSerializable(this, serializableHero.SerializableHeroSkillTree);
            var equipedArtifacts = serializableHero.EquipedArtifacts;
            foreach (var equipedArtifact in equipedArtifacts)
            {
                var artifactObject = ResourcesBase.GetArtifactObject(equipedArtifact.objectName);
                List<ArtifactModifier> modifiers = new List<ArtifactModifier>();
                foreach (var modifier in artifactObject.Modifiers)
                {
                    modifiers.Add(modifier.Clone() as ArtifactModifier);
                }

                var artifact = new Artifact()
                {
                    ArtifactObject = artifactObject,
                    Modifiers = modifiers,
                };
                inventoryManager.AddArtifact(artifact);
            }
        }

        private void LearnBaseSpecialization()
        {
            var specSkill = _heroSkillTree.HeroSkills[1].Values.FirstOrDefault();
            LearntHeroSkillData learntHeroSkillData = new LearntHeroSkillData(this, specSkill, HeroSkillType.Spec, 1);
            specSkill.OnLearnt(learntHeroSkillData);
        }

        public override void GainExperience(int experienceValue)
        {
            TotalExperience += experienceValue;
        }

        private float upd = 0f;

        protected void Update()
        {
            base.Update();
            upd += Time.deltaTime;
            _heroSkillTree?.Update();
            // if(upd < 4f)return;
            // upd = 0f;
            // TotalExperience += 70;
        }

        public override void VisuallyHide()
        {
            base.VisuallyHide();
            _tmpTextHealthLeft.gameObject.SetActive(false);
            _quantityBackgroundSprite.gameObject.SetActive(false);
            coloredFlag.gameObject.SetActive(false);
        }

        public override void VisuallyShow()
        {
            base.VisuallyShow();
            _tmpTextHealthLeft.gameObject.SetActive(true);
            _quantityBackgroundSprite.gameObject.SetActive(true);
            coloredFlag.gameObject.SetActive(true);
        }

        public override void DeselectAndRemoveControll(PlayerColor playerColor)
        {
            base.DeselectAndRemoveControll(playerColor);
            GameManager.Instance.GUIManager.HeroPortraitWidget.Hide();
            GameManager.Instance.GUIManager.HeroPortraitWidget.ReferencedHero = null;
        }

        public override Sprite GetMainSprite()
        {
            return HeroObject.mainSprite;
        }

        public override int GetTier()
        {
            return 0;
        }

        public override void OnNewTurnBegin()
        {
            if (!gameObject.activeSelf)
                return;
            if (AttacksLeft == AttacksCountValue && _movementPointsLeft == MaxMovementValue && _state != State.Combat)
            {
                var val = HealthValue * 0.1f;
                ChangeHealthValue((int)val);
            }

            AttacksLeft = AttacksCountValue;
            _retilationsLeft = HeroObject.retilationsCount;
            countOfPerformedAttacks = 0;
            var m = (int)((float)maxMana * 0.05f);
            // Debug.Log(m);
            ManaLeft += m;
            base.OnNewTurnBegin();
        }

        public override void OnEndOfTurn()
        {
        }

        public override void ChangeHealthValue(int deltaValue, ControllableCharacter characterSource = null)
        {
            if (this == null)
                return;
            _healthLeft += deltaValue;
            _healthLeft = Mathf.Clamp(_healthLeft, 0, HealthValue);
            OnHealthChanged(deltaValue);

            if (HealthLeft <= 0)
            {
                OnQuantityChanged(-1);
                OnStackDemolished(this);
            }

            var color = deltaValue >= 0
                ? GlobalVariables.healthPositiveTextColor
                : GlobalVariables.healthNegativeTextColor;
            FloatingText.Create(Position, 0.2f, 0.2f).MakeFloat(color, deltaValue);
        }

        public override void OnStackDemolished(ControllableCharacter demolishedCharacter,
            ControllableCharacter characterSource = null)
        {
            var castle = Player.controlledCastles.FirstOrDefault();
            if (castle != null)
            {
                isExistingInWorld = true;
                castle.AddGarnisonCharacterStack(this);
                _retilationsLeft = 0;
                _healthLeft = 1;
                _movementPointsLeft = 0;
                manaLeft = 0;
                Position = Vector2Int.zero;
                CreateBuff(ResourcesBase.GetBuff("revive_tired")).Apply();
                CreateBuff(ResourcesBase.GetBuff("revive_tired_attacks")).Apply();
            }
            else
            {
                isExistingInWorld = false;
                ClearUp(this);
                DestroyStack();
            }
            base.OnStackDemolished(demolishedCharacter, characterSource);
            Player.fogOfWarController.UpdateVisionForCharacterOnDeath(this);
        }

        public override IEnumerator IECollectTreasure(AbstractCollectable collectable)
        {
            collectable.ShowDialogue(this);
            yield return null;
        }


        public override void SelectAndGainControll(PlayerColor playerColor)
        {
            base.SelectAndGainControll(playerColor);
            GameManager.Instance.GUIManager.HeroPortraitWidget.Show();
            GameManager.Instance.GUIManager.HeroPortraitWidget.ReferencedHero = this as Hero;
            var castles = _activeTerrain.TerrainNavigator.NavigationMap.GetAllObjectsOfType<Castle>(Position, 3,
                LayerMask.NameToLayer("Building"));
            if (castles.Count == 0) return;
            for (int i = 0; i < castles.Count; i++)
            {
                var castle = castles[i];
                if (castle.PlayerOwnerColor != playerOwnerColor)
                {
                    Player.actionCellPool.CreateActionCellAtPosition(castle.Position, GlobalVariables.SiegeActionCell);
                }
            }
        }

        public override void CreateTreasureCollectGrid(List<Vector2Int> neighbours, Vector3 position)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbourTilePosition = neighbours[i];
                var vector2Int = new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y);
                var neighbourCharacter =
                    Physics2D.OverlapPoint(vector2Int,
                        selfLayerMask);
                if (neighbourCharacter != null)
                {
                    continue;
                }
                Vector2Int realPosition = vector2Int;
                Player.actionCellPool.CreateActionCellAtPosition(realPosition, GlobalVariables.CollectActionCell);
            }
        }

        public override void CreateDwellingInteractGrid(List<Vector2Int> neighbours, Vector3 position)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbourTilePosition = neighbours[i];
                var neighbourCharacter =
                    Physics2D.OverlapPoint(new Vector2(neighbourTilePosition.x, neighbourTilePosition.y),
                        selfLayerMask);
                if (neighbourCharacter != null)
                {
                    continue;
                }

                Player.actionCellPool.CreateActionCellAtPosition(neighbourTilePosition,
                    GlobalVariables.DwellingInteractActionCell);
            }
        }

        public override IEnumerator IEDwellInteract(DwellBuildingBehaviour dwellBuildingBehaviour)
        {
            dwellBuildingBehaviour.Capture(Player);
            yield return null;
        }

        public void CreateDropArtifactGrid()
        {
            Player.actionCellPool.ResetAll();
            var neighbours = GlobalVariables.CrossNeighbours;
            for (int i = 0; i < neighbours.Length; i++)
            {
                var neighbourTilePosition = Position + neighbours[i];
                NavigationMap navigationMap = PlayableTerrain.TerrainNavigator.NavigationMap;
                bool hasArtifactInCell =
                    navigationMap.GetObjectOfType<ArtifactBehaviour>(neighbourTilePosition,
                        LayerMask.NameToLayer("Treasure")) != null;
                bool isCellBlocked = navigationMap.IfCellBlocked(neighbourTilePosition.x, neighbourTilePosition.y);
                if (isCellBlocked || hasArtifactInCell)
                {
                    continue;
                }


                Vector2Int realPosition = new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y);
                Player.actionCellPool.CreateActionCellAtPosition(realPosition, GlobalVariables.DropArtifactActionCell);
            }
        }
    }
}