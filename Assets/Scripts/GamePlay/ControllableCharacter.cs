using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    [RequireComponent(typeof(BaseGridSnapResolver))]
    public class ControllableCharacter : AbstractMagicSpellTarget, IClickTarget
    {
        public enum State
        {
            Combat,
            Moving,
            Awaiting
        }

        public Dictionary<PlayableTerrain, List<Vector3Int>> OpenedFogDictionary =
            new Dictionary<PlayableTerrain, List<Vector3Int>>();

        protected GameManager _gameManager = GameManager.Instance;

        // protected List<GameObject> _actionCells = new List<GameObject>();
        protected SpriteVisualEffector _spriteVisualEffector = new SpriteVisualEffector();
        public bool isShooter;
        public int startingMovementPoints;
        public int attackRange;
        public int startingMana;
        public string title, description;
        public int startingShoots;
        public int baseVisionRadius;
        public float growthPerDay;
        protected int _movementPointsLeft;
        protected int _shootsLeft;
        protected int _retilationsLeft, _rangedRetilationsLeft, _attacksLeft;
        [SerializeField] protected ControllableCharacterObject _controllableCharacterObject;
        public PlayerColor playerOwnerColor;
        public SpriteRenderer _spriteRenderer;
        protected bool _isSelected;
        public bool isFlying;
        public bool isExistingInWorld;
        private CharacterAmbienceAudioSource _characterAmbience;
        private AudioSource _ambientAudioSource;

        public int UniqueId;

        public int baseDefenseValue,
            baseAttackValue,
            baseDamageValue,
            baseHealth,
            baseAttacksCount;

        public int HealthLeft
        {
            get => _healthLeft;
            set => _healthLeft = Mathf.Clamp(value, 0, HealthValue);
        }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                _spawnedCount = _spawnedCount == 0 ? _count : _spawnedCount;
            }
        }

        protected int _count, _healthLeft, _spawnedCount;
        protected SpriteAnimationUnit _idleSpriteAnimation;
        protected PlayableTerrain _activeTerrain;

        public int FightingPoints
        {
            get { return (int)(Mathf.Pow(3, GetTier()) * Count); }
        }

        protected bool _hasAmbienceClips;
        public State LastState => _lastState;
        public State CurrentState => _state;

        protected State _state;
        protected State _lastState;

        public delegate void OnStateDelegate(State state);

        public event OnStateDelegate StateChanged
        {
            add => stateChanged += value;
            remove => stateChanged -= value;
        }

        protected event OnStateDelegate stateChanged;

        public bool FlipX
        {
            get => _spriteRenderer.flipX;
            set
            {
                _spriteRenderer.flipX = value;
                _idleSpriteAnimation?.FlipOffset(_spriteRenderer.flipX);
            }
        }

        public void ChangeTerrainResetWorldPosition(Vector2Int newPosition, PlayableTerrain playableTerrain)
        {
            _activeTerrain.TerrainNavigator.NavigationMap.SetCellFree(Position.x, Position.y);
            _previousPosition = newPosition;
            _currentPosition = newPosition;
            transform.position = new Vector3(newPosition.x, newPosition.y);
            PlaceOnTerrain(playableTerrain);
            _activeTerrain.TerrainNavigator.NavigationMap.BlockCell(newPosition.x, newPosition.y);
        }

        public void ResetWorldPosition(Vector2Int newPosition)
        {
            _previousPosition = newPosition;
            _currentPosition = newPosition;
            transform.position = new Vector3(newPosition.x, newPosition.y);
            _activeTerrain.TerrainNavigator.NavigationMap.BlockCell(newPosition.x, newPosition.y);
        }

        public void OnStateChanged(State state)
        {
            stateChanged?.Invoke(state);
        }

        public void SetState(State state)
        {
            if (_state == state)
                return;
            _lastState = _state;
            _state = state;
            OnStateChanged(_state);
        }

        public virtual int DefenseValue
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

                return Mathf.RoundToInt(baseDefenseValue + defenseMultiplier + defenseSummarizer);
            }
        }

        public virtual int AttackValue
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

                return Mathf.RoundToInt(baseAttackValue + attackMultiplier + attackSummarizer);
            }
        }

        public virtual int DamageValue
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

                float damage = baseDamageValue + damageMultiplier + damageSummarizer;
                int calculatedDamage = Mathf.RoundToInt(damage);
                calculatedDamage = calculatedDamage <= 0 ? 1 : calculatedDamage;
                return calculatedDamage;
            }
        }

        public virtual int VisionValue
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

                float vision = baseVisionRadius + visionMult + visionSumm;
                return Mathf.RoundToInt(vision);
            }
        }

        public virtual int HealthValue
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

                float health = baseHealth + healthMult + healthSumm;
                return Mathf.RoundToInt(health);
            }
        }

        public virtual int MaxMovementValue
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

                float movementPoints = startingMovementPoints + moveMult + moveSumm;
                return Mathf.RoundToInt(movementPoints);
            }
        }

        public virtual int AttacksCountValue
        {
            get
            {
                float multiplier = 0f;
                float summarizer = 0f;
                foreach (var mod in attacksCountModifiers)
                {
                    if (mod.Operation == (int)ModifierOperation.Multiply)
                        multiplier += mod.value * baseAttacksCount;
                    else if (mod.Operation == (int)ModifierOperation.Change)
                        summarizer += mod.value;
                }

                return Mathf.RoundToInt(baseAttacksCount + multiplier + summarizer);
            }
        }

        public int HealthDifference
        {
            get => HealthValue - baseHealth;
        }

        public int VisionDifference
        {
            get => VisionValue - baseVisionRadius;
        }

        public int AttackDifference
        {
            get => AttackValue - baseAttackValue;
        }

        public int DamageDifference
        {
            get => DamageValue - baseDamageValue;
        }

        public int DefenseDifference
        {
            get => DefenseValue - baseDefenseValue;
        }

        public int MaxMovementDifference
        {
            get => MaxMovementValue - startingMovementPoints;
        }

        public Fraction fraction;
        public List<SpecialAbility> specialAbilities = new List<SpecialAbility>();
        public Stack<SpecialAbility> offensivePreAttackCombatAbilitiesStack;
        public Stack<SpecialAbility> offensivePostAttackCombatAbilitiesStack;
        public Stack<SpecialAbility> offensivePreRetilationCombatAbilitiesStack;
        public Stack<SpecialAbility> offensivePostRetilationCombatAbilitiesStack;

        public Stack<SpecialAbility> defensiveCombatAbilitiesStack;

        public List<FloatModifier> damageModifiers = new List<FloatModifier>();
        public List<FloatModifier> attackModifiers = new List<FloatModifier>();
        public List<FloatModifier> defenseModifiers = new List<FloatModifier>();
        public List<FloatModifier> spellPowerModifiers = new List<FloatModifier>();
        public List<IntegerModifier> manaModifiers = new List<IntegerModifier>();
        public List<FloatModifier> moveSpeedModifiers = new List<FloatModifier>();
        public List<FloatModifier> healthModifiers = new List<FloatModifier>();
        public List<FloatModifier> visionModifiers = new List<FloatModifier>();
        public List<FloatModifier> attacksCountModifiers = new List<FloatModifier>();
        public bool preventiveRetilation;
        public SpellBook spellBook;
        public bool isWizard;
        public int countOfPerformedAttacks = 0, maxCountOfPerformedAttacks = 0;
        public bool interactionLocked;
        private bool _insideGarnison;

        public void PutInGarnison()
        {
            _activeTerrain.TerrainNavigator.NavigationMap.SetCellFree(Position.x, Position.y);
            Hide();
            _insideGarnison = true;
        }

        public void RemoveFromGarnison()
        {
            Show();
            _insideGarnison = false;
        }

        public void Show()
        {
            if (_insideGarnison) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void VisuallyHide()
        {
            // Debug.Log($"Hide {name}");
            _spriteRenderer.enabled = false;
        }

        public virtual void VisuallyShow()
        {
            // Debug.Log($"Show {name}");
            _spriteRenderer.enabled = true;
        }

        public bool IsVisibleForPlayer(Player player)
        {
            bool isInsideTheFog = player.fogOfWarController.IsCharacterInsideFogOfWar(this);
            return !isInsideTheFog;
        }

        public Player Player
        {
            get => GameManager.Instance.MapScenarioHandler.players[playerOwnerColor];
        }

        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set
            {
                _previousPosition = Position;
                transform.position = new Vector3(value.x, value.y, 0f);
                _currentPosition = Position;
                OnPositionChanged(this);
            }
        }


        public Vector2Int PreviousPosition
        {
            get => _previousPosition;
        }

        protected Vector2Int _previousPosition, _currentPosition;

        protected int manaLeft, maxMana;
        protected int selfLayerMask;
        protected int experienceValue;

        public delegate void OnIntegerChanged(int deltaValue);

        public delegate void OnCombatEventDelegate(CombatData combatData);

        public delegate void OnSpellCombatEventDelegate(MagicSpellCombatData magicSpellCombatData);

        public delegate void OnCharacterEventDelegate(ControllableCharacter _character);

        public delegate void OnCharactersEventDelegate(ControllableCharacter target,
            ControllableCharacter source);


        private event OnCharacterEventDelegate positionChanged;
        private event OnCharacterEventDelegate movementPointsExpired;
        private event OnCharacterEventDelegate attacksExpired;

        private event OnSpellCombatEventDelegate spellCasted, spellRecieved;
        private event OnCombatEventDelegate attackRecieved, attackPerformed;

        public ControllableCharacterObject BaseCharacterObject
        {
            get => _controllableCharacterObject;
            set => _controllableCharacterObject = value;
        }

        public int ManaLeft
        {
            get { return manaLeft; }
            set
            {
                var manaBefore = manaLeft;
                manaLeft = value > maxMana ? maxMana : value < 0 ? 0 : value;
                var manaDelta = manaLeft - manaBefore;
                if (manaDelta == 0) return;
                var color = GlobalVariables.manaTextColor;
                FloatingText.Create(Position, 0.2f, 0.2f).MakeFloat(color, manaDelta);
            }
        }

        public int ShootsLeft
        {
            get { return _shootsLeft; }
            set { _shootsLeft = value; }
        }

        public void OnAttackPerformed(CombatData combatData)
        {
            attackPerformed?.Invoke(combatData);
        }

        public void OnAttackRecieved(CombatData combatData)
        {
            attackRecieved?.Invoke(combatData);
        }

        public event OnCombatEventDelegate AttackPerformed
        {
            add => attackPerformed += value;
            remove => attackPerformed -= value;
        }

        public event OnCombatEventDelegate AttackRecieved
        {
            add => attackRecieved += value;
            remove => attackRecieved -= value;
        }

        public event OnSpellCombatEventDelegate MagicSpellRecieved
        {
            add => spellRecieved += value;
            remove => spellRecieved -= value;
        }

        public event OnSpellCombatEventDelegate SpellCasted
        {
            add => spellCasted += value;
            remove => spellCasted -= value;
        }

        public event OnCharacterEventDelegate PositionChanged
        {
            add => positionChanged += value;
            remove => positionChanged -= value;
        }

        public event OnCharacterEventDelegate MovementPointsExpired
        {
            add => movementPointsExpired += value;
            remove => movementPointsExpired -= value;
        }

        public event OnCharacterEventDelegate AttacksExpired
        {
            add => attacksExpired += value;
            remove => attacksExpired -= value;
        }

        public int MovementPointsLeft
        {
            get => _movementPointsLeft;
            set
            {
                _movementPointsLeft = value;
                if (_movementPointsLeft <= 0)
                    OnMovementPointsExpired();
            }
        }

        public int AttacksLeft
        {
            get => _attacksLeft;
            set
            {
                _attacksLeft = value;
                if (_attacksLeft <= 0)
                    OnAttacksExpired();
            }
        }

        public int RetilationsLeft
        {
            get => _retilationsLeft;
            set { _retilationsLeft = value; }
        }

        public int RangedRetilationsLeft
        {
            get => _rangedRetilationsLeft;
            set { _rangedRetilationsLeft = value; }
        }

        private void OnMovementPointsExpired()
        {
            movementPointsExpired?.Invoke(this);
            if (_movementPointsLeft <= 0 && _attacksLeft == 0)
                OnTurnExpired();
        }

        private void OnAttacksExpired()
        {
            attacksExpired?.Invoke(this);
            if (_movementPointsLeft <= 0 && _attacksLeft == 0)
                OnTurnExpired();
        }

        public void OnTurnExpired()
        {
            _spriteVisualEffector.CreatePlayerMovementPointsEffect(_spriteRenderer);
        }


        public event OnCharacterEventDelegate Selected
        {
            add => selected += value;
            remove => selected -= value;
        }

        public event OnCharacterEventDelegate Deselected
        {
            add => deselected += value;
            remove => deselected -= value;
        }

        public event OnCharactersEventDelegate StackBanished
        {
            add => stackBanished += value;
            remove => stackBanished -= value;
        }

        public event OnCharactersEventDelegate StackDemolished
        {
            add => stackDemolished += value;
            remove => stackDemolished -= value;
        }

        public event OnCharacterEventDelegate OneCharacterDied
        {
            add => oneCharacterDied += value;
            remove => oneCharacterDied -= value;
        }

        private event OnCharacterEventDelegate selected;
        private event OnCharacterEventDelegate deselected;
        private event OnCharactersEventDelegate stackDemolished, stackBanished;

        private event OnCharacterEventDelegate oneCharacterDied;

        public event OnIntegerChanged HealthIncreased
        {
            add => healthIncreased += value;
            remove => healthIncreased -= value;
        }

        public event OnIntegerChanged HealthDecreased
        {
            add => healthDecreased += value;
            remove => healthDecreased -= value;
        }

        public event OnIntegerChanged QuantityChanged
        {
            add => quantityChanged += value;
            remove => quantityChanged -= value;
        }

        private event OnIntegerChanged quantityChanged;
        private event OnIntegerChanged healthIncreased;
        private event OnIntegerChanged healthDecreased;

        public void OnHealthChanged(int deltaValue)
        {
            if (deltaValue < 0)
                healthDecreased?.Invoke(deltaValue);
            else if (deltaValue > 0)
                healthIncreased?.Invoke(deltaValue);
        }

        public void OnQuantityChanged(int delta)
        {
            quantityChanged?.Invoke(delta);
        }

        private List<SpecialAbility> _combatAbilities = new List<SpecialAbility>();

        public void ReserveTerrainFog(PlayableTerrain playableTerrain)
        {
            OpenedFogDictionary.TryAdd(playableTerrain, new List<Vector3Int>());
        }

        public void Banish()
        {
            stackBanished?.Invoke(this, this);
            Moroutine.Run(IEDestroyBody());
        }

        public virtual void Init()
        {
            selfLayerMask = 1 << LayerMask.NameToLayer("Character");
            _gameManager = GameManager.Instance;
            title = _controllableCharacterObject.title;
            description = _controllableCharacterObject.description;
            _currentPosition = _previousPosition = Position;
            baseVisionRadius = _controllableCharacterObject.visionRadius;
            InitDataFromObject();
            SetState(State.Awaiting);
            experienceValue = _controllableCharacterObject.experienceValue;

            HealthDecreased += ReactionToHealthChanged;


            var ambienceClips = _controllableCharacterObject.idleAudioClips.ToList();
            _hasAmbienceClips = ambienceClips.Count > 0;
            if (_hasAmbienceClips)
            {
                var audioBase = ResourcesBase.GetPrefab("AudioSource");
                _ambientAudioSource = GameObject.Instantiate(audioBase, Vector3.zero, Quaternion.identity, transform)
                    .GetComponent<AudioSource>();
                _ambientAudioSource.transform.localPosition = Vector3.zero;
                _ambientAudioSource.maxDistance = 13;
                _ambientAudioSource.rolloffMode = AudioRolloffMode.Linear;
                _characterAmbience = new CharacterAmbienceAudioSource(_ambientAudioSource, ambienceClips);
            }

            var idleSpriteAnimation = _controllableCharacterObject.IdleSpriteAnimation;
            if (idleSpriteAnimation != null)
            {
                var offset = idleSpriteAnimation.Offset;
                _idleSpriteAnimation = SpriteAnimationPlayer.Instance.PlayAnimation(transform.position,
                    idleSpriteAnimation, -1, offset.x, offset.y);
                _idleSpriteAnimation.transform.SetParent(transform);
            }


            UniqueId = GetInstanceID();
        }

        public bool IsInsideTheCastle
        {
            get { return _gameManager.terrainManager.CurrentPlayableMap.IsCastleTerrain(_activeTerrain); }
        }

        private void OnPositionChanged(ControllableCharacter character)
        {
            _activeTerrain.TerrainNavigator.NavigationMap.UpdateNavigationsMapForCharacter(this);
            Player.fogOfWarController.UpdateVisionForCharacter(this);
            positionChanged?.Invoke(character);

            if (Player.isHuman && GameManager.Instance.MapScenarioHandler.CurrentPlayer == Player)
            {
                Player.UpdateVisionForEveryEnemy();
                return;
            }

            CheckVisibility();
        }

        public void CheckVisibility()
        {
            var allPlayer = GameManager.Instance.MapScenarioHandler.AllPlayers;
            foreach (var player in allPlayer)
            {
                if (GameManager.Instance.MapScenarioHandler.LatestTurnHumanPlayer != player) continue;
                bool isVisibleForPlayer = IsVisibleForPlayer(player);
                if (isVisibleForPlayer) VisuallyShow();
                else VisuallyHide();
            }
        }

        public void PlaceOnTerrain(PlayableTerrain playableTerrain)
        {
            if (_activeTerrain == playableTerrain) return;
            if (_activeTerrain != null)
            {
                if (_activeTerrain.SpawnedCharacters.Contains(this))
                {
                    _activeTerrain.SpawnedCharacters.Remove(this);
                }
            }

            _activeTerrain = playableTerrain;
            if (!_activeTerrain.SpawnedCharacters.Contains(this))
                _activeTerrain.SpawnedCharacters.Add(this);
            if (Player.ActiveTerrain != _activeTerrain)
                gameObject.SetActive(false);
            if (gameObject.activeSelf)
                Player.fogOfWarController.UpdateVisionForCharacter(this);
        }

        protected virtual void Update()
        {
            if (_hasAmbienceClips)
                _characterAmbience.Update();
        }

        public virtual void LoadFromSerializable(SerializableCharacter serializableCharacter)
        {
            UniqueId = serializableCharacter.UniqueId == 0 ? UniqueId : serializableCharacter.UniqueId;
            Count = _spawnedCount = serializableCharacter.quantity;
            fraction = serializableCharacter.Fraction;
            playerOwnerColor = serializableCharacter.PlayerOwnerColor;
            FlipX = serializableCharacter.flipX;
            _healthLeft = serializableCharacter.healthLeft;
            _retilationsLeft = serializableCharacter.retilationsLeft;
            _attacksLeft = serializableCharacter.attacksLeft;
            manaLeft = serializableCharacter.manaLeft;
            _shootsLeft = serializableCharacter.shootsLeft;
            _movementPointsLeft = serializableCharacter.movePointsLeft;
            ResetWorldPosition(new Vector2Int(serializableCharacter.positionX, serializableCharacter.positionY));
            var serializableSpellBook = serializableCharacter.SerializableSpellBook;
            if (serializableSpellBook != null)
            {
                spellBook = new SpellBook(Player);
                spellBook.LoadFromSerializable(serializableSpellBook);
            }

            var serializablePositiveBuffs = serializableCharacter.appliedPositiveBuffs;
            foreach (var serializedBuff in serializablePositiveBuffs)
            {
                var buffObject = ResourcesBase.GetBuff(serializedBuff.InternalName);
                var buff = CreateBuff(buffObject);
                buff.durationLeft = serializedBuff.DurationLeft;
            }

            var serializableNegativeBuffs = serializableCharacter.appliedNegativeBuffs;
            foreach (var serializedBuff in serializableNegativeBuffs)
            {
                var buffObject = ResourcesBase.GetBuff(serializedBuff.InternalName);
                var buff = CreateBuff(buffObject);
                buff.durationLeft = serializedBuff.DurationLeft;
            }

            if (!serializableCharacter.isActive)
                Hide();
            // if(serializableCharacter.isVisuallyHidden)
            //     VisuallyHide();
        }

        protected virtual void InitDataFromObject()
        {
            GameManager.Instance.MapScenarioHandler.NewDayBegin += UpdateBuffsOnTurn;
            fraction = _controllableCharacterObject.Fraction;
            isFlying = _controllableCharacterObject.isFlying;
            _movementPointsLeft = startingMovementPoints = _controllableCharacterObject.startingMovementPoints;
            attackRange = _controllableCharacterObject.attackRange;
            isShooter = _controllableCharacterObject.isShooter;
            _retilationsLeft = _controllableCharacterObject.retilationsCount;
            baseAttacksCount = AttacksLeft = _controllableCharacterObject.attacksCount;
            baseDamageValue = _controllableCharacterObject.damageValue;
            baseAttackValue = _controllableCharacterObject.attackValue;
            baseDefenseValue = _controllableCharacterObject.defenseValue;
            _healthLeft = baseHealth = _controllableCharacterObject.startingHealth;
            maxMana = startingMana = manaLeft = _controllableCharacterObject.startingMana;
            name = $"{_controllableCharacterObject.title}_{playerOwnerColor}";
            isExistingInWorld = true;
            for (int i = 0; i < _controllableCharacterObject.persona.Count; i++)
            {
                persona |= (long)_controllableCharacterObject.persona[i];
            }


            isWizard = _controllableCharacterObject.isWizard;
            maxCountOfPerformedAttacks = _controllableCharacterObject.attacksCount;
            var combatAbilityNames = _controllableCharacterObject._combatAbilityNames;
            foreach (var combatAbilityName in combatAbilityNames)
            {
                var combatAbilityClone = ResourcesBase.GetCombatAbility(combatAbilityName)?.Clone() as SpecialAbility;
                if (combatAbilityClone == null) continue;
                combatAbilityClone.Init(this);
                specialAbilities.Add(combatAbilityClone);
            }

            if (_controllableCharacterObject.isWizard)
            {
                if (!string.IsNullOrEmpty(_controllableCharacterObject.spellBookInternalName))
                {
                    var spellBookObject =
                        ResourcesBase.GetSpellBook(_controllableCharacterObject.spellBookInternalName);
                    spellBook = new SpellBook(spellBookObject, Player);
                }
            }
        }

        public void OnNewDayBegin()
        {
        }

        protected virtual bool PrepareForAttack(CombatData combatData)
        {
            var targetChar = combatData.defensiveCharacter;
            if (countOfPerformedAttacks >= maxCountOfPerformedAttacks)
                return false;
            if (this == null || targetChar == null)
                return false;
            targetChar.SetupDefensiveCombatAbilities();
            SetupOffensiveCombatAbilities();
            _movementPointsLeft = 0;
            AttacksLeft--;
            countOfPerformedAttacks++;
            DeselectAndRemoveControll(playerOwnerColor);
            return true;
        }

        protected virtual IEnumerator IEAttackAudioEffects(ControllableCharacter targetChar)
        {
            yield return new WaitForSeconds(0.2f);
            if (this == null) yield break;
            var vaac = _controllableCharacterObject.voiceAttackAudioClips;
            var waac = _controllableCharacterObject.weaponAttackAudioClips;
            if (vaac.Length > 0)
                _gameManager.SoundManager.Play3DAudioClip(vaac[Random.Range(0, vaac.Length)], transform.position,
                    false);
            yield return new WaitForSeconds(0.5f);
            if (this == null) yield break;
            if (waac.Length > 0)
                _gameManager.SoundManager.Play3DAudioClip(waac[Random.Range(0, waac.Length)], transform.position,
                    false);
        }

        public virtual void ChangeHealthValue(int deltaValue, ControllableCharacter characterSource = null)
        {
        }

        public virtual IEnumerator IECombatAttackProcess(CombatData combatData)
        {
            bool isReadyToAttack = PrepareForAttack(combatData);
            if (!isReadyToAttack)
                yield break;
            interactionLocked = true;
            Player.isInputLocked = true;

            var targetChar = combatData.defensiveCharacter;
            SetState(State.Combat);
            targetChar.SetState(State.Combat);
            bool isBotAttackingHuman = targetChar.Player.isHuman && !Player.isHuman;
            var latestTurnHumanPlayer = GameManager.Instance.MapScenarioHandler.LatestTurnHumanPlayer;
            bool deepFog = false, wasHidden = false;
            var attackPosition = new Vector3Int(targetChar.Position.x, targetChar.Position.y);
            if (isBotAttackingHuman)
            {
                if (latestTurnHumanPlayer.fogOfWarController.GetCellTypeAtPosition(attackPosition,
                        targetChar.PlayableTerrain) == FogOfWarCellType.Deep)
                {
                    latestTurnHumanPlayer.fogOfWarController.SetFogOfWarCellTypeSingle(attackPosition
                        , targetChar.PlayableTerrain,
                        FogOfWarCellType.Low, true);
                    deepFog = true;
                }

                if (!targetChar.IsVisibleForPlayer(latestTurnHumanPlayer))
                {
                    wasHidden = true;
                    targetChar.VisuallyShow();
                }
            }

            yield return Moroutine
                .Run(targetChar.ApplyCombatAbilitiesStack(targetChar.defensiveCombatAbilitiesStack, combatData))
                .WaitForComplete();
            yield return Moroutine.Run(ApplyCombatAbilitiesStack(offensivePreAttackCombatAbilitiesStack, combatData))
                .WaitForComplete();
            yield return Moroutine.Run(IEAttackProcess(combatData)).WaitForComplete();

            if (deepFog)
            {
                latestTurnHumanPlayer.fogOfWarController.SetFogOfWarCellTypeSingle(attackPosition
                    , _activeTerrain,
                    FogOfWarCellType.Deep, true);
            }

            if (targetChar == null || this == null)
            {
                Stack<SpecialAbility> postAttacksAlwaysExecute = new Stack<SpecialAbility>();
                var tempList = offensivePostAttackCombatAbilitiesStack.Where(x => x.executeAlways).ToList();
                tempList.ForEach(x => postAttacksAlwaysExecute.Push(x));
                yield return Moroutine.Run(ApplyCombatAbilitiesStack(postAttacksAlwaysExecute, combatData))
                    .WaitForComplete();
                interactionLocked = false;
                Player.isInputLocked = false;
                yield break;
            }

            if (wasHidden)
                targetChar.VisuallyHide();

            yield return Moroutine.Run(ApplyCombatAbilitiesStack(offensivePostAttackCombatAbilitiesStack, combatData))
                .WaitForComplete();
            if (targetChar == null || this == null)
            {
                interactionLocked = false;
                Player.isInputLocked = false;
                yield break;
            }


            if (targetChar.RetilationsLeft > 0 && Distance(targetChar) <= targetChar.attackRange)
            {
                yield return Moroutine.Run(targetChar.IERetilationAttackProcess(this)).WaitForComplete();
            }

            interactionLocked = false;
            Player.isInputLocked = false;
        }

        public virtual IEnumerator IERetilationAttackProcess(ControllableCharacter targetChar)
        {
            var combatData = PrepareForRetilation(targetChar);
            if (combatData == null)
                yield break;
            yield return Moroutine
                .Run(ApplyCombatAbilitiesStack(offensivePreAttackCombatAbilitiesStack, combatData))
                .WaitForComplete();

            yield return Moroutine.Run(IEAttackProcess(combatData)).WaitForComplete();
            if (targetChar == null)
                yield break;

            yield return Moroutine.Run(ApplyCombatAbilitiesStack(offensivePostAttackCombatAbilitiesStack,
                    new CombatData()
                        { defensiveCharacter = targetChar, offensiveCharacter = this, performedOnRetilation = true }))
                .WaitForComplete();
        }

        protected virtual CombatData PrepareForRetilation(ControllableCharacter targetChar)
        {
            if (this == null || targetChar == null || _retilationsLeft <= 0 || !targetChar.IsVisibleForPlayer(Player))
                return null;
            var retilationUnitPosition = V3Int(transform.position);
            var attackerUnitPosition = V3Int(targetChar.transform.position);
            var distance = Vector3Int.Distance(retilationUnitPosition, attackerUnitPosition);
            if (distance > attackRange)
                return null;

            CombatData combatData = new CombatData()
            {
                defensiveCharacter = targetChar,
                offensiveCharacter = this,
                performedOnRetilation = true,
            };
            var calculatedDamage = CalculateIncomingDamage(combatData);
            combatData.totalDamage = calculatedDamage;
            var killQuantity = CalculateQuantityChangeInStack(combatData);
            combatData.killQuantity = killQuantity;
            bool willRetilate = targetChar.RetilationsLeft > 0;
            combatData.willRetilate = willRetilate;

            _retilationsLeft--;

            SetupOffensiveCombatAbilities();

            return combatData;
        }

        public virtual IEnumerator IEAttackProcess(CombatData combatData)
        {
            var targetChar = combatData.defensiveCharacter;
            if (this == null || targetChar == null)
                yield break;
            RotateTowards(targetChar.Position);

            yield return Moroutine.Run(IEAttackAudioEffects(targetChar)).WaitForComplete();

            yield return Moroutine.Run(IEHitEffect(combatData)).WaitForComplete();
        }

        public virtual IEnumerator IEPreventiveRetilationAttackProcess(ControllableCharacter targetChar)
        {
            yield return null;
        }

        public virtual IEnumerator IEDestroyBody()
        {
            isExistingInWorld = false;
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }

        public virtual IEnumerator IECollectTreasure(AbstractCollectable collectable)
        {
            yield return null;
        }

        public virtual IEnumerator IEDwellInteract(DwellBuildingBehaviour dwellBuildingBehaviour)
        {
            yield return null;
        }

        protected virtual void OnMagicSpellCasted(MagicSpellCombatData magicSpellCombatData)
        {
            spellCasted?.Invoke(magicSpellCombatData);
        }

        public virtual int CalculateQuantityChangeInStack(CombatData combatData)
        {
            var totalDamage = combatData.totalDamage;
            var offensive = combatData.offensiveCharacter;
            var defensive = combatData.defensiveCharacter;
            int countInStack = defensive.Count;

            int totalHealth = defensive.baseHealth * (defensive.Count - 1) + defensive.HealthLeft;
            int health = totalHealth - totalDamage;
            float ratio = (float)health / defensive.baseHealth;
            int ostatok = health % defensive.baseHealth;
            int countLeft = Mathf.CeilToInt(ratio);

            return countInStack - countLeft;
        }

        public virtual int CalculateIncomingDamage(CombatData combatData)
        {
            var attackingChar = combatData.offensiveCharacter;
            var defendingChar = combatData.defensiveCharacter;

            float penalty = CalculateDamagePenalty(combatData);

            float attackArmorRatio = (float)attackingChar.AttackValue / defendingChar.DefenseValue;
            if (attackArmorRatio > GlobalVariables.AttackArmorRatioMaxValue)
                attackArmorRatio = GlobalVariables.AttackArmorRatioMaxValue;

            float fDamage = attackingChar.DamageValue * attackArmorRatio;
            fDamage -= fDamage * penalty;
            float calculatedDamage = fDamage * attackingChar.Count;
            calculatedDamage = calculatedDamage <= 0 ? 1 : calculatedDamage;
            return Mathf.RoundToInt(calculatedDamage);
        }

        public virtual float CalculateDamagePenalty(CombatData combatData)
        {
            float penalty = 0f;
            var attackingChar = combatData.offensiveCharacter;
            var defendingChar = combatData.defensiveCharacter;
            if (!attackingChar.isShooter)
                return penalty;
            var crossNeighbours = GlobalVariables.CrossNeighbours;
            var neighbours =
                attackingChar.CheckNeighbourCellsForCharacters(attackingChar.transform.position,
                    crossNeighbours.ToArray(), (long)MagicSpellAllowedTarget.Enemy);
            var noPenaltyCombatAbility = attackingChar.GetSpecialCombatAbility<ShooterNoPenaltyAbility>();
            var shooterMeleeCombatPenalty = GlobalVariables.ShooterMeleeCombatPenalty;
            if (neighbours.Count > 0)
            {
                penalty = shooterMeleeCombatPenalty;
                if (noPenaltyCombatAbility != null)
                    penalty = 0f;
                return penalty;
            }

            int movementPointsLeft = attackingChar.MovementPointsLeft;
            int movementPointsMax = attackingChar.MaxMovementValue;
            float ratio = (float)movementPointsLeft / (float)movementPointsMax;
            penalty = 1 - ratio;
            penalty = Mathf.Clamp(penalty, 0f, shooterMeleeCombatPenalty);
            // Debug.Log(penalty);
            return penalty;
        }

        public SpecialAbility GetSpecialCombatAbility<T>() where T : SpecialAbility
        {
            if (specialAbilities == null)
            {
                return null;
            }

            foreach (var combatAbility in specialAbilities)
            {
                var t = combatAbility is T;
                if (t) return combatAbility;
            }

            return null;
        }

        public virtual void ReactionToHealthChanged(int deltaValue)
        {
            AudioClip[] audioClipsRef = null;
            if (Count <= 0)
            {
                audioClipsRef = _controllableCharacterObject.deathAudioClips;
            }
            else
            {
                audioClipsRef = _controllableCharacterObject.painAudioClips;
            }

            if (audioClipsRef == null)
                return;

            if (audioClipsRef.Length > 0)
                _gameManager.SoundManager.Play3DAudioClip(audioClipsRef[Random.Range(0, audioClipsRef.Length)],
                    transform.position, false);
        }

        protected void RemoveBuff(Buff buff)
        {
            if (buff.IsNotDebuff())
                RemovePositiveExpiredBuff(buff);
            else
                RemoveNegativeExpiredBuff(buff);
        }

        public void DestroyBuff(Buff buff)
        {
            buff?.OnExpired(buff);
        }

        public void RemoveAllBuffs()
        {
            for (int i = 0; i < AllAppliedBuffs.Count; i++)
            {
                var buff = AllAppliedBuffs[i];
                RemoveBuff(buff);
            }
        }

        public void DestroyBuff(string internalName, bool isNotDebuff)
        {
            List<Buff> buffs = null;
            if (!isNotDebuff)
                buffs = AppliedNegativeBuffs;
            else buffs = AppliedPositiveBuffs;
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].internalName == internalName)
                {
                    DestroyBuff(buffs[i]);
                    return;
                }
            }
        }

        protected void RemovePositiveExpiredBuff(Buff buff)
        {
            buff.Expired -= RemovePositiveExpiredBuff;
            AppliedPositiveBuffs.Remove(buff);
            // Debug.Log($"Removing positive buff {buff.internalName}");
        }

        protected void RemoveNegativeExpiredBuff(Buff buff)
        {
            buff.Expired -= RemoveNegativeExpiredBuff;
            AppliedNegativeBuffs.Remove(buff);
            // Debug.Log($"Removing negative buff {buff.GetHashCode()}");
        }

        public void UpdateBuffsOnTurn()
        {
            if(!isExistingInWorld)return;
            if (!gameObject.activeSelf) return;
            Moroutine.Run(UpdateBuffsState());
        }

        public Buff CreateBuff(Buff buff)
        {
            Buff buffClone = (Buff)buff.Clone();
            MagicSpellCombatData magicSpellCombatData = new MagicSpellCombatData();
            magicSpellCombatData.source = this;
            magicSpellCombatData.target = this;
            buffClone.Init(magicSpellCombatData);
            bool canBeUsed = buffClone.CheckConditions(magicSpellCombatData);
            if (!canBeUsed)
                return null;
            if (buff.IsNotDebuff())
            {
                AppliedPositiveBuffs.Add(buffClone);
                buffClone.Expired += RemovePositiveExpiredBuff;
            }
            else
            {
                AppliedNegativeBuffs.Add(buffClone);
                buffClone.Expired += RemoveNegativeExpiredBuff;
            }

            return buffClone;
        }

        public Buff CreateBuff(Buff buff, MagicSpellCombatData magicSpellCombatData)
        {
            Buff buffClone = (Buff)buff.Clone();
            buffClone.Init(magicSpellCombatData);
            bool canBeUsed = buffClone.CheckConditions(magicSpellCombatData);
            if (!canBeUsed)
                return null;
            if (buff.IsNotDebuff())
            {
                AppliedPositiveBuffs.Add(buffClone);
                buffClone.Expired += RemovePositiveExpiredBuff;
            }
            else
            {
                AppliedNegativeBuffs.Add(buffClone);
                buffClone.Expired += RemoveNegativeExpiredBuff;
            }

            return buffClone;
        }

        public override IEnumerator SelfCastMagicSpell(MagicSpellCombatData magicSpellCombatData)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log($"Spell: {magicSpellCombatData.magicSpell.title} self casted on {magicSpellCombatData.source}");

            List<Buff> appliedPositiveBuffsOnCasterClone = new List<Buff>();
            List<Buff> appliedNegativeBuffsOnCasterClone = new List<Buff>();
            var sourcePositiveBuffs = magicSpellCombatData.magicSpell.appliedPositiveBuffsOnCaster;
            var sourceNegativeBuffs = magicSpellCombatData.magicSpell.appliedNegativeBuffsOnCaster;
            foreach (var sourceBuff in sourcePositiveBuffs)
            {
                var buffInstance = CreateBuff(sourceBuff, magicSpellCombatData);
                buffInstance.Apply();
            }

            foreach (var sourceBuff in sourceNegativeBuffs)
            {
                var buffInstance = CreateBuff(sourceBuff, magicSpellCombatData);
                buffInstance.Apply();
            }
            magicSpellCombatData.source.Character.OnMagicSpellCasted(magicSpellCombatData);
        }

        public override IEnumerator RecieveOffsensiveMagicSpell(MagicSpellCombatData magicSpellCombatData)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log(
                $"Offensive Spell: {magicSpellCombatData.magicSpell.title} used by: {magicSpellCombatData.source.name} to target: {magicSpellCombatData.target.name}");

            var sourceNegativeBuffs = magicSpellCombatData.magicSpell.appliedNegativeBuffsOnTarget;
            if (sourceNegativeBuffs == null)
                yield break;
            foreach (var sourceBuff in sourceNegativeBuffs)
            {
                var buffInstance = CreateBuff(sourceBuff, magicSpellCombatData);
                buffInstance.Apply();
            }
            magicSpellCombatData.source.Character.OnMagicSpellCasted(magicSpellCombatData);
        }

        public override IEnumerator RecieveFriendlyMagicSpell(MagicSpellCombatData magicSpellCombatData)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log(
                $"Helping Spell: {magicSpellCombatData.magicSpell.title} used by: {magicSpellCombatData.source.name} to target: {magicSpellCombatData.target.name}");
            var sourcePositiveBuffs = magicSpellCombatData.magicSpell.appliedPositiveBuffsOnTarget;
            if (sourcePositiveBuffs == null)
                yield break;
            foreach (var sourceBuff in sourcePositiveBuffs)
            {
                var buffInstance = CreateBuff(sourceBuff, magicSpellCombatData);
                buffInstance.Apply();
            }
            magicSpellCombatData.source.Character.OnMagicSpellCasted(magicSpellCombatData);
        }

        public long TargetTypeMask
        {
            get => persona;
            set { }
        }

        public PlayableTerrain PlayableTerrain
        {
            get => _activeTerrain;
            set => _activeTerrain = value;
        }

        public bool InsideGarnison => _insideGarnison;

        public int SpawnedCount => _spawnedCount;

        public virtual Sprite GetMainSprite()
        {
            return null;
        }

        public virtual int GetTier()
        {
            return 1;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void OnStackDemolished(ControllableCharacter demolishedCharacter,
            ControllableCharacter characterSource = null)
        {
            stackDemolished?.Invoke(demolishedCharacter, characterSource);
        }

        public void DestroyStack()
        {
            stackDemolished?.Invoke(this, null);
            gameObject.SetActive(false);
            Player.fogOfWarController.UpdateVisionForCharacterOnDeath(this);
            Moroutine.Run(IEDestroyBody());
        }

        protected void MassiveCheer()
        {
            var charactersAround =
                _activeTerrain.TerrainNavigator.NavigationMap.GetAllObjectsOfType<ControllableCharacter>(Position, 6,
                    LayerMask.NameToLayer("Character"));
            for (int i = 0; i < charactersAround.Count; i++)
            {
                var character = charactersAround[i];
                if (character.IsAnAllyTo(this)) continue;
                float chance = Random.Range(0f, 1f);
                if (chance < 0.5f) continue;
                character.PlayCheerSound();
            }
        }

        public virtual void PlayCheerSound()
        {
            var cheerClips = _controllableCharacterObject.cheerAudioClips;
            bool hasCheerSounds = cheerClips != null;
            if (hasCheerSounds && cheerClips.Length > 0)
            {
                var cheerClip = cheerClips[Random.Range(0, cheerClips.Length)];
                _gameManager.SoundManager.Play3DAudioClip(cheerClip, transform.position, false);
            }
        }

        public virtual void GainExperience(int experienceValue)
        {
        }

        public virtual void ClearUp(ControllableCharacter characterStack)
        {
            GameManager.Instance.MapScenarioHandler.NewDayBegin -= UpdateBuffsOnTurn;
            GameManager.Instance.MapScenarioHandler.NewSiegeRoundBegin -= UpdateBuffsOnTurn;
            StopAllCoroutines();
            AppliedNegativeBuffs.ForEach(x => { x.Expired -= RemoveNegativeExpiredBuff; });
            AppliedPositiveBuffs.ForEach(x => { x.Expired -= RemovePositiveExpiredBuff; });
            AppliedNegativeBuffs.Clear();
            AppliedPositiveBuffs.Clear();
            HealthDecreased -= ReactionToHealthChanged;
            
        }

        public IEnumerator ApplyCombatAbilitiesStack(Stack<SpecialAbility> combatAbilitiesStack, CombatData combatData)
        {
            int count = combatAbilitiesStack.Count;
            for (int i = 0; i < count; i++)
            {
                var combatAbility = combatAbilitiesStack.Pop();
                if (combatAbility == null)
                    continue;
                if (combatData.performedOnRetilation)
                {
                    if (combatAbility.performedOnRetilation)
                        yield return Moroutine.Run(combatAbility.Use(combatData)).WaitForComplete();
                    else
                    {
                        Debug.Log("Not perfomed on retilation!!");
                        yield break;
                    }
                }
                else
                {
                    yield return Moroutine.Run(combatAbility.Use(combatData)).WaitForComplete();
                }
            }
        }

        public void SetupOffensiveCombatAbilities()
        {
            offensivePreAttackCombatAbilitiesStack = new Stack<SpecialAbility>();
            offensivePostAttackCombatAbilitiesStack = new Stack<SpecialAbility>();
            offensivePreRetilationCombatAbilitiesStack = new Stack<SpecialAbility>();
            offensivePostRetilationCombatAbilitiesStack = new Stack<SpecialAbility>();
            foreach (var ca in specialAbilities)
            {
                if (ca.combatAbilityType == CombatAbilityType.Offensive)
                {
                    if (ca.combatAbilityOrder == CombatAbilityOrder.PreAttack)
                        offensivePreAttackCombatAbilitiesStack.Push(ca);
                    else if (ca.combatAbilityOrder == CombatAbilityOrder.PostAttack)
                        offensivePostAttackCombatAbilitiesStack.Push(ca);
                    else if (ca.combatAbilityOrder == CombatAbilityOrder.PreRetilation)
                        offensivePreRetilationCombatAbilitiesStack.Push(ca);
                    else if (ca.combatAbilityOrder == CombatAbilityOrder.PostRetilation)
                        offensivePostRetilationCombatAbilitiesStack.Push(ca);
                }
            }
        }

        public float Distance(ControllableCharacter other)
        {
            var retilationUnitPosition = V3Int(transform.position);
            var attackerUnitPosition = V3Int(other.transform.position);
            var distance = Vector3Int.Distance(retilationUnitPosition, attackerUnitPosition);
            return distance;
        }

        public void SetupDefensiveCombatAbilities()
        {
            defensiveCombatAbilitiesStack = new Stack<SpecialAbility>();
            foreach (var ca in specialAbilities)
            {
                if (ca.combatAbilityType == CombatAbilityType.Defensive)
                    defensiveCombatAbilitiesStack.Push(ca);
            }
        }


        public List<ControllableCharacter> GetNeighboursInSplashAttackRange(ControllableCharacter target)
        {
            List<ControllableCharacter> resultNeighbours = new List<ControllableCharacter>();
            var targetPosition = new Vector2Int(Mathf.CeilToInt(target.transform.position.x),
                Mathf.CeilToInt(target.transform.position.y));
            var attackerPosition = Position;
            var attackDirection = targetPosition - attackerPosition;
            attackDirection.x = Math.Clamp(attackDirection.x, -1, 1);
            attackDirection.y = Math.Clamp(attackDirection.y, -1, 1);
            bool angledAttack = attackDirection.x != 0 && attackDirection.y != 0;
            Vector2Int firstNPos = Vector2Int.zero;
            Vector2Int secondNPos = Vector2Int.zero;
            if (!angledAttack)
            {
                firstNPos = new Vector2Int(targetPosition.x - attackDirection.y,
                    targetPosition.y - attackDirection.x);
                secondNPos = new Vector2Int(targetPosition.x + attackDirection.y,
                    targetPosition.y + attackDirection.x);
            }
            else
            {
                bool differentSigns = (attackDirection.x >= 0 && attackDirection.y <= 0) ||
                                      (attackDirection.x <= 0 && attackDirection.y >= 0);
                if (differentSigns)
                {
                    firstNPos = new Vector2Int(targetPosition.x + attackDirection.y,
                        targetPosition.y);
                    secondNPos = new Vector2Int(targetPosition.x,
                        targetPosition.y + attackDirection.x);
                }
                else
                {
                    firstNPos = new Vector2Int(targetPosition.x - attackDirection.y,
                        targetPosition.y);
                    secondNPos = new Vector2Int(targetPosition.x,
                        targetPosition.y - attackDirection.x);
                }
            }

            var firstNeighbour =
                _activeTerrain.TerrainNavigator.NavigationMap.GetRelativeCharacterAtPosition(this, firstNPos,
                    (long)MagicSpellAllowedTarget.Enemy);
            var secondNeighbour =
                _activeTerrain.TerrainNavigator.NavigationMap.GetRelativeCharacterAtPosition(this, secondNPos,
                    (long)MagicSpellAllowedTarget.Enemy);
            if (firstNeighbour != null)
                resultNeighbours.Add(firstNeighbour.GetComponent<ControllableCharacter>());
            if (secondNeighbour != null)
                resultNeighbours.Add(secondNeighbour.GetComponent<ControllableCharacter>());
            return resultNeighbours;
        }


        public List<ControllableCharacter> CheckNeighbourCellsForCharacters(Vector3 position, Vector2Int[] directions,
            long mask)
        {
            Vector3Int v3Int = V3Int(position);
            List<ControllableCharacter> resultNeighbours = new List<ControllableCharacter>();
            for (int i = 0; i < directions.Length; i++)
            {
                var neighbourTilePosition = v3Int + new Vector3Int(directions[i].x, directions[i].y);

                var neighbourCharacter =
                    _activeTerrain.TerrainNavigator.NavigationMap.GetRelativeCharacterAtPosition(this,
                        new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y), mask);
                if (neighbourCharacter != null)
                {
                    resultNeighbours.Add(neighbourCharacter);
                }
            }

            return resultNeighbours;
        }

        public List<ControllableCharacter> CheckNeighbourCellsForCharactersAndActionCells(Vector3 position,
            bool lookForAlly = false)
        {
            Vector3Int v3Int = V3Int(position);
            int acLayerMask = 1 << LayerMask.NameToLayer("ActionCell");
            Vector3Int[] positions = new[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
            List<ControllableCharacter> resultNeighbours = new List<ControllableCharacter>();
            for (int i = 0; i < positions.Length; i++)
            {
                var neighbourTilePosition = v3Int + positions[i];

                var neighbourCharacter =
                    Physics2D.OverlapPoint(new Vector2(neighbourTilePosition.x, neighbourTilePosition.y),
                        selfLayerMask);
                if (neighbourCharacter != null)
                {
                    var neighbourControllableCharacterComponent =
                        neighbourCharacter.gameObject.GetComponent<ControllableCharacter>();
                    bool isAlly = playerOwnerColor == neighbourControllableCharacterComponent.playerOwnerColor;
                    if (isAlly && !lookForAlly)
                        continue;
                    var overlapForActionCell =
                        Physics2D.OverlapPoint(new Vector2(neighbourTilePosition.x, neighbourTilePosition.y),
                            acLayerMask);
                    if (overlapForActionCell == null)
                    {
                        resultNeighbours.Add(neighbourControllableCharacterComponent);
                    }
                }
            }

            return resultNeighbours;
        }
        
        public List<Vector2Int> CheckNeighbourCells(Vector3 position, string layerMaskName)
        {
            Vector3Int v3Int = V3Int(position);
            int acLayerMask = 1 << LayerMask.NameToLayer("ActionCell");
            int buildingLayerMask = 1 << LayerMask.NameToLayer(layerMaskName);
            Vector3Int[] positions = new[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
            List<Vector2Int> resultPositions = new List<Vector2Int>();
            for (int i = 0; i < positions.Length; i++)
            {
                var neighbourTilePosition = v3Int + positions[i];
                var v2 = new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y);
                var neighbourTreasure = Physics2D.OverlapPoint(v2, buildingLayerMask);
                if (neighbourTreasure != null)
                {
                    var overlapForActionCell = Physics2D.OverlapPoint(v2, acLayerMask);
                    if (overlapForActionCell == null)
                    {
                        resultPositions.Add(v2);
                    }
                }
            }

            return resultPositions;
        }

        public List<Vector2Int> CheckNeighbourCellsForTreasures(Vector3 position)
        {
            Vector3Int v3Int = V3Int(position);
            int acLayerMask = 1 << LayerMask.NameToLayer("ActionCell");
            int treasureLayerMask = 1 << LayerMask.NameToLayer("Treasure");
            Vector3Int[] positions = new[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
            List<Vector2Int> resultPositions = new List<Vector2Int>();
            for (int i = 0; i < positions.Length; i++)
            {
                var neighbourTilePosition = v3Int + positions[i];
                var v2 = new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y);
                var neighbourTreasure = Physics2D.OverlapPoint(v2, treasureLayerMask);
                if (neighbourTreasure != null)
                {
                    var overlapForActionCell = Physics2D.OverlapPoint(v2, acLayerMask);
                    if (overlapForActionCell == null)
                    {
                        resultPositions.Add(v2);
                    }
                }
            }

            return resultPositions;
        }

        public void CreateMeleeAttackGrid(List<ControllableCharacter> controllableCharacters, Vector3 position)
        {
            if (_attacksLeft <= 0)
                return;
            Player.actionCellPool.CreateActionCellsOnCharacters(controllableCharacters,
                GlobalVariables.AttackActionCell);
        }

        public virtual void CreateTreasureCollectGrid(List<Vector2Int> neighbours, Vector3 position)
        {
        }

        public virtual void CreateDwellingInteractGrid(List<Vector2Int> neighbours, Vector3 position)
        {
        }

        protected void CreateMovementGrid()
        {
            int waveStep = 0;
            var position = transform.position;
            List<Vector3Int> _edgeTiles = new List<Vector3Int>();
            Vector3Int startingTilePosition = new Vector3Int((int)(position.x), (int)(position.y));
            Vector3Int[] positions =
            {
                new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, 0, 0)
            };
            Player.actionCellPool.ResetAll();

            var neighboursForAttack = CheckNeighbourCellsForCharactersAndActionCells(position);
            if (neighboursForAttack.Count > 0)
            {
                CreateMeleeAttackGrid(neighboursForAttack, position);
            }

            var neighboursForCollect = CheckNeighbourCellsForTreasures(position);
            if (neighboursForCollect.Count > 0)
                CreateTreasureCollectGrid(neighboursForCollect, position);

            var dwellingsAround =
                _activeTerrain.TerrainNavigator.NavigationMap.GetAllObjectsOfType<DwellBuildingBehaviour>(Position, 1,
                    LayerMask.NameToLayer("Building"));
            List<Vector2Int> dwellingsPosition = new List<Vector2Int>();
            dwellingsAround.ForEach(x =>
            {
                if (!IsAnAllyTo(x.Player))
                    dwellingsPosition.Add(x.Position);
            });
            if (dwellingsAround.Count > 0)
                CreateDwellingInteractGrid(dwellingsPosition, position);

            _edgeTiles.Add(startingTilePosition);
            var activeTerrain = Player.ActiveTerrain;

            int acLayerMask = 1 << LayerMask.NameToLayer("ActionCell");
            while (waveStep < MovementPointsLeft)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector3Int neighbourTilePosition = edgeTilePosition + positions[i];

                        var nextLowObstacleTile = activeTerrain.ObstaclesLowTilemap.GetTile(neighbourTilePosition);
                        var nextTreeTile = activeTerrain.TreesTilemap.GetTile(neighbourTilePosition);
                        var nextWaterTile = activeTerrain.WaterTilemap.GetTile(neighbourTilePosition);
                        var nextGroundTile = activeTerrain.GroundTilemap.GetTile(neighbourTilePosition);
                        var navigationMap = _activeTerrain.TerrainNavigator.NavigationMap;
                        if (neighbourTilePosition == startingTilePosition)
                            continue;
                        if (_edgeTiles.Contains(neighbourTilePosition))
                            continue;
                        if (isFlying)
                        {
                            _edgeTiles.Add(neighbourTilePosition);
                        }

                        if (navigationMap.IfCellBlocked(neighbourTilePosition.x, neighbourTilePosition.y))
                        {
                            continue;
                        }

                        if (nextGroundTile == null)
                            continue;
                        if (nextWaterTile != null)
                        {
                            if (nextGroundTile == null && !isFlying)
                                continue;
                        }

                        if (nextLowObstacleTile != null)
                            continue;

                        if (nextTreeTile != null)
                            continue;
                        var overlapForActionCell =
                            Physics2D.OverlapPoint(new Vector2(neighbourTilePosition.x, neighbourTilePosition.y),
                                acLayerMask);
                        if (overlapForActionCell != null)
                        {
                            continue;
                        }

                        var neighbourCharacter =
                            Physics2D.OverlapPoint(new Vector2(neighbourTilePosition.x, neighbourTilePosition.y),
                                selfLayerMask);
                        if (neighbourCharacter != null)
                        {
                            continue;
                        }

                        Player.actionCellPool.CreateActionCellAtPosition(
                            new Vector2Int(neighbourTilePosition.x, neighbourTilePosition.y),
                            GlobalVariables.MoveActionCell);
                        if (!isFlying)
                        {
                            _edgeTiles.Add(neighbourTilePosition);
                        }
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                waveStep++;
            }
        }

        public List<Corpse> GetAllCorpsesInRange(int range, long mask)
        {
            int waveStep = 0;
            var position = transform.position;
            List<Corpse> corpses = new List<Corpse>();
            List<Vector2Int> _edgeTiles = new List<Vector2Int>();
            Vector2Int startingTilePosition = new Vector2Int((int)(position.x), (int)(position.y));
            Vector2Int[] positions =
            {
                new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0)
            };
            _edgeTiles.Add(startingTilePosition);
            int corpseLayerMask = 1 << LayerMask.NameToLayer("Corpse");
            while (waveStep < range)
            {
                int edgeCount = _edgeTiles.Count;
                for (int j = 0; j < edgeCount; j++)
                {
                    var edgeTilePosition = _edgeTiles[j];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        Vector2Int ntpos = edgeTilePosition + positions[i];
                        Vector3Int nt3Pos = new Vector3Int(ntpos.x, ntpos.y, 0);

                        if (ntpos == startingTilePosition)
                            continue;

                        if (_edgeTiles.Contains(ntpos))
                            continue;

                        var overlapForCorpse =
                            Physics2D.OverlapPoint(ntpos,
                                corpseLayerMask);

                        var neighbourCharacter =
                            Physics2D.OverlapPoint(ntpos,
                                selfLayerMask);

                        if (neighbourCharacter == null && overlapForCorpse != null)
                        {
                            var corpse = overlapForCorpse.GetComponent<Corpse>();
                            bool isHero = (corpse.persona & (long)MagicSpellAllowedTarget.Hero) != 0;
                            bool affectsHeroes = (mask & (long)MagicSpellAllowedTarget.Hero) != 0;
                            if (isHero)
                            {
                                if (!affectsHeroes) continue;
                            }

                            if ((corpse.persona & mask) != 0)
                                corpses.Add(corpse);
                        }

                        _edgeTiles.Add(ntpos);
                    }
                }

                _edgeTiles = _edgeTiles.Distinct().ToList();
                waveStep++;
            }

            return corpses;
        }

        public virtual bool IsAnAllyTo(ControllableCharacter character)
        {
            return playerOwnerColor == character.playerOwnerColor;
        }

        public virtual bool IsAnAllyTo(Player player)
        {
            return playerOwnerColor == player.Color;
        }

        public List<ControllableCharacter> GetAllCharactersInRange(int range, long mask, bool onlyVisibles)
        {
            bool collectAllies = ((mask & (long)MagicSpellAllowedTarget.Ally) != 0);
            bool collectEnemies = ((mask & (long)MagicSpellAllowedTarget.Enemy) != 0);
            var players = _gameManager.MapScenarioHandler.players;
            List<ControllableCharacter> allCharacters = new List<ControllableCharacter>();
            foreach (var playerColor in players.Keys)
            {
                if ((playerColor == playerOwnerColor && collectAllies) ||
                    (playerColor != playerOwnerColor && collectEnemies))
                    allCharacters.AddRange(players[playerColor].controlledCharacters);
            }

            List<ControllableCharacter> suitableCharacters = new List<ControllableCharacter>();

            foreach (var character in allCharacters)
            {
                if (!character.gameObject.activeSelf || !character.isExistingInWorld)
                    continue;
                bool maskMatch = (mask & character.persona) != 0;
                if (!maskMatch && ((mask & (long)MagicSpellAllowedTarget.Any) == 0))
                {
                    continue;
                }

                var v3pos = V3Int(transform.position);
                var enemyV3Pos = V3Int(character.transform.position);
                var distance = Mathf.CeilToInt(Vector3Int.Distance(v3pos, enemyV3Pos));
                bool isVisible = character.IsVisibleForPlayer(Player);
                bool isInRange = distance <= range;
                bool isHero = (character.persona & (long)MagicSpellAllowedTarget.Hero) != 0;
                bool affectsHeroes = (mask & (long)MagicSpellAllowedTarget.Hero) != 0;
                if (isHero)
                {
                    if (!affectsHeroes) continue;
                }

                if (onlyVisibles)
                {
                    if (isVisible && isInRange)
                        suitableCharacters.Add(character);
                }
                else
                {
                    if (isInRange)
                        suitableCharacters.Add(character);
                }
            }

            return suitableCharacters;
        }

        protected void CreateShootingGrid()
        {
            if (_attacksLeft <= 0)
                return;
            List<ControllableCharacter> allEnemies =
                GetAllCharactersInRange(attackRange,
                    GlobalVariables.GetCharactersDefaultPersona(MagicSpellAllowedTarget.Enemy), true);
            foreach (var enemy in allEnemies)
            {
                var direction = enemy.transform.position - transform.position;
                var raycast = Physics2D.Raycast(transform.position, direction, attackRange);
                if (raycast.collider != null)
                {
                }
            }

            Player.actionCellPool.CreateActionCellsOnCharacters(allEnemies, GlobalVariables.AttackActionCell);
        }

        public virtual void OnNewTurnBegin()
        {
            if (!gameObject.activeSelf)
                return;
            foreach (var specialAbility in specialAbilities)
            {
                if (specialAbility.combatAbilityType != CombatAbilityType.Passive) continue;
                CombatData combatData = new CombatData() { offensiveCharacter = this };
                Moroutine.Run(specialAbility.Use(combatData));
            }

            UpdateSpellCooldowns();
            MovementPointsLeft = MaxMovementValue;
            // Player.fogOfWarController.UpdateVisionForCharacter(this);
            if (!Player.isHuman) return;
            interactionLocked = false;
            VisuallyShow();
            _spriteVisualEffector.RemovePlayerMovementPointsEffect(_spriteRenderer);
        }

        private void UpdateSpellCooldowns()
        {
            if (isWizard)
            {
                if (spellBook != null)
                    spellBook.LearntMagicSpells.Values.ToList().ForEach(x =>
                    {
                        if (x.Cooldown > 0)
                            x.Cooldown--;
                    });
            }
        }

        public virtual void OnEndOfTurn()
        {
        }

        public virtual IEnumerator OnClicked(Player player)
        {
            var playerColor = player.Color;
            yield return new WaitForEndOfFrame();
            if (player != Player)
            {
                if (IsVisibleForPlayer(player))
                    SelectWithoutControll(playerColor);
                yield break;
            }

            if (_isSelected)
            {
                DeselectAndRemoveControll(playerColor);
            }
            else
            {
                SelectAndGainControll(playerColor);
            }
        }


        public static Vector3Int V3Int(Vector3 vector3)
        {
            Vector3Int v3IntPosition = new Vector3Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), 0);
            return v3IntPosition;
        }

        public IEnumerator MoveToPosition(Vector2Int targetPosition)
        {
            Player.isInputLocked = true;
            SetState(State.Moving);
            DeselectAndRemoveControll(playerOwnerColor);
            Vector2Int startPosition = Position;
            Vector2Int nextPosition =
                new Vector2Int(Mathf.RoundToInt(targetPosition.x), Mathf.RoundToInt(targetPosition.y));
            GetComponent<BaseGridSnapResolver>().isEnabled = false;
            _previousPosition = Position;
            interactionLocked = true;
            if (!isFlying)
            {
                var navigationMap = _activeTerrain.TerrainNavigator.NavigationMap;
                var path = navigationMap.FindPath(GetHashCode(), _previousPosition, nextPosition);

                if (path.Count <= 0)
                    yield break;
                path.RemoveAt(0);
                // foreach (var pathNode in path)
                // {
                //     GlobalVariables.__SpawnMarker(pathNode, Color.blue, false, 3f);
                // }

                foreach (var pathNode in path)
                {
                    RotateTowards(pathNode);
                    yield return Moroutine.Run(IEMoveProcess(pathNode)).WaitForComplete();
                    _previousPosition = _currentPosition;
                    _currentPosition = Position;
                    if (_currentPosition.x != _previousPosition.x || _currentPosition.y != _previousPosition.y)
                    {
                        OnPositionChanged(this);
                    }
                }
            }
            else
            {
                RotateTowards(targetPosition);
                yield return Moroutine.Run(IEMoveProcess(targetPosition)).WaitForComplete();
                Vector2Int distance = targetPosition - startPosition;
                MovementPointsLeft -= (Mathf.Abs((int)distance.x) + Math.Abs((int)distance.y));
                OnPositionChanged(this);
            }

            GetComponent<BaseGridSnapResolver>().isEnabled = true;

            Position = targetPosition;

            Player.isInputLocked = false;
            interactionLocked = false;
        }

        private IEnumerator IEMoveProcess(Vector2Int targetPositionInt)
        {
            var targetPosition = new Vector3(targetPositionInt.x, targetPositionInt.y, 0f);
            float animationSpeed = 3f * GlobalVariables.playerAnimationSpeedGlobalMultipler;
            while (Vector3.Distance(transform.position, targetPosition) - 0.1 > 0f)
            {
                Vector3 pos = transform.position;
                pos = Vector3.MoveTowards(pos, targetPosition, animationSpeed * Time.deltaTime);
                // Debug.Log($"pos: {pos} | tp {targetPosition}");
                transform.position = pos;
                yield return null;
            }

            MovementPointsLeft--;
        }

        public virtual IEnumerator IERecieveDamage(CombatData combatData)
        {
            var calculatedDamage = combatData.totalDamage;
            ChangeHealthValue(-calculatedDamage, combatData.offensiveCharacter);
            yield return null;
        }

        public void RotateTowards(Vector2Int position)
        {
            if (Position.x <= position.x)
            {
                FlipX = true;
            }
            else
            {
                FlipX = false;
            }
        }

        public void SelectWithoutControll(PlayerColor playerColor)
        {
            Debug.Log($"{title} terrain: {PlayableTerrain.Name}");

            if (_activeTerrain != Player.ActiveTerrain) return;
            // _spriteVisualEffector.CreatePlayerSelectionEffect(_spriteRenderer, playerOwnerColor);
            GameManager.Instance.GUIManager.UpdateInfoWindowState(this);
        }

        public void DeselectWithoutControll(PlayerColor playerColor)
        {
            _spriteVisualEffector.RemovePlayerSelectionEffect(_spriteRenderer);
        }

        public virtual void SelectAndGainControll(PlayerColor playerColor)
        {
            if (!gameObject.activeSelf) return;
            if (interactionLocked) return;
            if (_activeTerrain != Player.ActiveTerrain) return;
            selected?.Invoke(this);
            Player.OnSelectedAnyCharacter(this);
            _spriteVisualEffector.CreatePlayerSelectionEffect(_spriteRenderer, playerOwnerColor);
            _isSelected = true;
            CreateMovementGrid();
            if (isShooter || attackRange > 1)
                CreateShootingGrid();
        }

        public virtual void DeselectAndRemoveControll(PlayerColor playerColor)
        {
            if (!_isSelected) return;
            deselected?.Invoke(this);
            Player.OnDeselectedAnyCharacter(this);
            _spriteVisualEffector.RemovePlayerSelectionEffect(_spriteRenderer);
            _isSelected = false;
            Player.actionCellPool.ResetAll();
        }

        protected virtual IEnumerable IEHitEffect(CombatData combatData)
        {
            var targetChar = combatData.defensiveCharacter;
            if (targetChar == null) yield break;
            SpriteRenderer targetSpriteRenderer = targetChar.GetComponent<SpriteRenderer>();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            float targetValue = 1f, currentValue = 0f;
            int directionNum = Position.x < targetChar.Position.x ? 1 : 2;
            var bloodEffect = ResourcesBase.GetSpriteAnimation($"ps_blood_{directionNum}");
            SpriteAnimationPlayer.Instance.PlayAnimation(targetChar.transform.position, bloodEffect, 1);
            while (currentValue < targetValue)
            {
                if (targetSpriteRenderer == null)
                    yield break;
                currentValue = Mathf.MoveTowards(currentValue, targetValue, 3f * Time.deltaTime);
                targetSpriteRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_HitEffectBlend", currentValue);
                targetSpriteRenderer.SetPropertyBlock(mpb);
                yield return null;
            }

            if (targetSpriteRenderer == null)
                yield break;
            OnAttackPerformed(combatData);
            targetChar.OnAttackRecieved(combatData);
            var recieveDamageMoroutine = Moroutine.Run(targetChar.IERecieveDamage(combatData));
            currentValue = 1f;
            targetValue = 0f;
            while (currentValue > targetValue)
            {
                if (targetSpriteRenderer == null)
                    yield break;
                currentValue = Mathf.MoveTowards(currentValue, targetValue, 3f * Time.deltaTime);
                targetSpriteRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_HitEffectBlend", currentValue);
                targetSpriteRenderer.SetPropertyBlock(mpb);
                yield return null;
            }

            yield return null;
        }
    }
}