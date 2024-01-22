using System;
using System.Collections.Generic;
using System.IO;
using AgeOfHeroes.AI;
using AgeOfHeroes.Spell;
using Newtonsoft.Json;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AgeOfHeroes
{
    public enum TileDirection
    {
        TOP = 1,
        RIGHT_TOP = 2,
        RIGHT = 4,
        RIGHT_BOT = 8,
        BOT = 16,
        LEFT_BOT = 32,
        LEFT = 64,
        LEFT_TOP = 128,
        CENTER = 256
    }

    public enum TileCondition
    {
        Any = 0,
        Required = 1,
        Forbidden = 2
    }

    public enum TileState
    {
        Any = 0,
        Empty = 1,
        Filled = 2
    }

    public enum TileSide
    {
        NONE = 0,
        CENTER_TOP = 1,
        RIGHT_TOP,
        RIGHT_CENTER,
        RIGHT_BOT,
        CENTER_BOT,
        LEFT_BOT,
        LEFT_CENTER,
        LEFT_TOP,
        CENTER,
        LB_TR_MERGE,
        LT_BR_MERGE,
        TL_CORNER,
        TR_CORNER,
        BL_CORNER,
        BR_CORNER,
        SI_LE,
        SI_RE,
        SI_HC,
        SI_TE,
        SI_BE,
        SI_VC,
        SI_C,
        SI_TURN_TL,
        SI_TURN_TR,
        SI_TURN_BL,
        SI_TURN_BR
    }

    public enum MapCategory
    {
        Original,
        Custom,
        Downloaded,
        SavedGame
    }

    public enum MatchDifficulty
    {
        Easy,
        Normal,
        Hard,
        Insane
    }

    public enum MapSize
    {
        Mini,
        Small,
        Medium,
        Large,
        Huge,
        Extreme
    }

    public enum MatchType
    {
        Local,
        Network
    }

    public enum PlayerColor
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        Cyan,
        Magenta,
        Black,
        White,
        Neutral
    }

    public enum DeviceType
    {
        Editor,
        Android,
        Windows,
        IOS,
        Linux,
        Unknown
    }

    public enum ArtifactQuality
    {
        Common = 0,
        Magical,
        Rare,
        Reliquary
    }

    public static class GlobalVariables
    {
        public static Color
            manaTextColor = new Color(0.2f, 0.3f, 0.9f),
            healthPositiveTextColor = new Color(0.2f, 0.9f, 0.3f),
            healthNegativeTextColor = new Color(0.9f, 0.05f, 0.2f),
            experienceGainTextColor = new Color(0.9f, 0.4f, 0.1f);


        public static string GetString(string key)
        {
            if (Strings.TryGetValue(key, out string val))
            {
                return val;
            }

            return string.Empty;
        }

        public static void SetString(string key, string value)
        {
            if (Strings.ContainsKey(key))
            {
                Strings[key] = value;
                return;
            }

            Strings.Add(key, value);
        }

        private static Dictionary<string, string> Strings = new Dictionary<string, string>();
        public static readonly float DamageModifierCoeff = 1f;
        public static readonly float AttackModifierCoeff = 0.01f;
        public static readonly float DefenseModifierCoeff = 0.02f;
        public static readonly float AttackArmorRatioMaxValue = 3f;
        public static readonly int CastleWidthValue = 6;
        public static readonly int CastleHeightValue = 4;
        public static readonly int CastleVisionValue = 10;
        public static readonly int CastleMaxTiersCount = 5;

        // public static readonly int CastleRenderOrder = 6;
        // public static readonly int DecorativeRenderOrder = 36;
        // public static readonly int CorpseRenderOrder = 37;
        // public static readonly int TreasureRenderOrder = 40;
        // public static readonly int HugeCharacterRenderOrder = 45;
        // public static readonly int HeroRenderOrder = 56;
        // public static readonly int SmallCharacterRenderOrder = 55;

        public static readonly int CastleRenderOrder = 3;
        public static readonly int DecorativeRenderOrder = 6;
        public static readonly int CorpseRenderOrder = 5;
        public static readonly int TreasureRenderOrder = 5;
        public static readonly int HugeCharacterRenderOrder = 6;
        public static readonly int HeroRenderOrder = 6;
        public static readonly int SmallCharacterRenderOrder = 6;

        public static readonly int MaxShopRecruitmentsMultiplier = 4;

        public static readonly float StartingArmyCoeff = 0.6f;

        public static readonly int MaxArtifactsOnHero = 3;
        public static readonly float ShooterMeleeCombatPenalty = 0.75f;

        public static int ValuePercentCompare(int value, int maxValue, float percent)
        {
            var ratio = (float)value / (float)maxValue;
            int result = ratio < percent ? -1 : Mathf.Approximately(ratio, percent) ? 0 : 1;
            return result;
        }
        
        public static int PointsOfCondition(AIConditionImportance conditionImportance)
        {
            return (int)conditionImportance;
        }

        public static Dictionary<PlayerType, string> PlayerTypes = new Dictionary<PlayerType, string>()
        {
            { PlayerType.None, "-" },
            { PlayerType.Human, "P" },
            { PlayerType.AIEasy, "AI_1" },
            { PlayerType.AIMedium, "AI_2" },
            { PlayerType.AIHard, "AI_3" },
        };

        public static JsonSerializerSettings GetDefaultSerializationSettings()
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            return jsonSerializerSettings;
        }

        public static Dictionary<Fraction, string> FractionIcons = new Dictionary<Fraction, string>()
        {
            { Fraction.Human, "Human_Fraction" },
            { Fraction.Undead, "Undead_Fraction" },
            { Fraction.Inferno, "Inferno_Fraction" },
            { Fraction.None, "character_unavail" },
        };

        public static void LoadMapPreviewIcon(string previewRelativePath, int sizeX, int sizeY, MapCategory mapCategory,
            Action<Sprite> onLoadedSprite)
        {
            string fullPath = string.Empty;
            Texture2D texture2D = null;
            switch (mapCategory)
            {
                case MapCategory.Original:
                    fullPath = $"{GlobalStrings.ORIGINAL_MAPS_DIRECTORY}/{previewRelativePath}";
                    Moroutine.Run(WWWRequestSystem.Request.Texture2D(fullPath, (_texture2D) =>
                    {
                        texture2D = _texture2D;
                        texture2D.filterMode = FilterMode.Point;
                        var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                            new Vector2(0.5f, 0.0f), 1.0f);
                        onLoadedSprite.Invoke(sprite);
                    }));
                    break;
                case MapCategory.Custom:
                    fullPath = $"{GlobalStrings.USER_MAPS_DIRECTORY}/{previewRelativePath}";
                    var imageFile = File.ReadAllBytes(fullPath);
                    texture2D = new Texture2D(sizeX, sizeY);
                    texture2D.LoadImage(imageFile);
                    texture2D.filterMode = FilterMode.Point;
                    texture2D.Apply();
                    var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                        new Vector2(0.5f, 0.0f), 1.0f);
                    onLoadedSprite.Invoke(sprite);
                    break;
            }

            // Debug.Log(fullPath);
        }

        public static DeviceType CurrentDevice
        {
            get
            {
                var deviceType = DeviceType.Android;
#if UNITY_EDITOR
                deviceType = DeviceType.Editor;
#endif
#if UNITY_STANDALONE_WIN
                deviceType = DeviceType.Windows;
#endif
                return deviceType;
            }
        }

        public static MapSize CalculateMapSize(Vector2Int mapSizeValue)
        {
            var length = mapSizeValue.magnitude;
            switch (mapSizeValue.x)
            {
                case 10:
                    return MapSize.Mini;
                    break;
                case 20:
                    return MapSize.Small;
                    break;
                case 30:
                    return MapSize.Medium;
                    break;
                case 40:
                    return MapSize.Large;
                    break;
                case 50:
                    return MapSize.Huge;
                    break;
                case 60:
                    return MapSize.Extreme;
                    break;
            }

            return MapSize.Mini;
        }

        public static Vector2Int GetMapSizeVector2Int(MapSize mapSize)
        {
            switch (mapSize)
            {
                case MapSize.Mini: return new Vector2Int(10, 10);
                case MapSize.Small: return new Vector2Int(20, 20);
                case MapSize.Medium: return new Vector2Int(30, 30);
                case MapSize.Large: return new Vector2Int(40, 40);
                case MapSize.Huge: return new Vector2Int(50, 50);
                case MapSize.Extreme: return new Vector2Int(60, 60);
            }

            return Vector2Int.zero;
        }

        public static Dictionary<MapSize, Color> MapSizeColors = new Dictionary<MapSize, Color>()
        {
            { MapSize.Mini, Color.green },
            { MapSize.Small, Color.yellow },
            { MapSize.Medium, Color.blue },
            { MapSize.Large, Color.cyan },
            { MapSize.Huge, Color.red },
            { MapSize.Extreme, Color.magenta },
        };

        public static Dictionary<ArtifactQuality, Color> ArtifactQualityColors =
            new Dictionary<ArtifactQuality, Color>()
            {
                { ArtifactQuality.Common, Color.white },
                { ArtifactQuality.Magical, Color.magenta },
                { ArtifactQuality.Rare, Color.blue },
                { ArtifactQuality.Reliquary, Color.yellow }
            };

        public static Dictionary<TerrainTileMaterialName, Color> TerrainColors =
            new Dictionary<TerrainTileMaterialName, Color>()
            {
                { TerrainTileMaterialName.Grass, Color.green },
                { TerrainTileMaterialName.Dirt, new Color(0.5f, 0f, 0f) },
                { TerrainTileMaterialName.Wood, new Color(0.81f, 0.53f, 0.25f) },
                { TerrainTileMaterialName.Sand, new Color(0.65f, 0.59f, 0.47f) },
                { TerrainTileMaterialName.WaterShallow, Color.cyan },
                { TerrainTileMaterialName.WaterDeep, Color.blue },
                { TerrainTileMaterialName.Rock, Color.gray },
                { TerrainTileMaterialName.Snow, new Color(0.8f, 0.8f, 0.8f) },
            };

        public static Dictionary<PlayerColor, Color> playerColors = new Dictionary<PlayerColor, Color>()
        {
            { PlayerColor.Red, new Color(0.96f, 0.18f, 0.18f) },
            { PlayerColor.Blue, Color.blue },
            { PlayerColor.Green, new Color(0.49f, 0.76f, 0.41f) },
            { PlayerColor.Yellow, new Color(0.807f, 0.807f, 0f) },
            { PlayerColor.Magenta, new Color(0.67f, 0.34f, 0.65f) },
            { PlayerColor.Neutral, new Color(0.2f, 0.2f, 0.2f) }
        };

        public static Dictionary<int, Color> PlayerCountColors = new Dictionary<int, Color>()
        {
            { 0, Color.black },
            { 1, Color.red },
            { 2, Color.blue },
            { 3, new Color(0.49f, 0.76f, 0.41f) },
            { 4, new Color(0.807f, 0.807f, 0f) },
            { 5, new Color(0.67f, 0.34f, 0.65f) }
        };

        public static Dictionary<PlayerColor, string> playerBanners = new Dictionary<PlayerColor, string>()
        {
            { PlayerColor.Red, "players_banner_red" },
            { PlayerColor.Blue, "players_banner_blue" },
            { PlayerColor.Green, "players_banner_green" },
            { PlayerColor.Yellow, "players_banner_yellow" },
            { PlayerColor.Neutral, "players_banner_neutral" }
        };

        public static Dictionary<HeroModifieableStat, string> statIcons = new Dictionary<HeroModifieableStat, string>()
        {
            { HeroModifieableStat.Attack, "attack_stat" },
            { HeroModifieableStat.Defense, "defense_stat" },
            { HeroModifieableStat.Damage, "damage_stat" },
            { HeroModifieableStat.Health, "health_stat" },
            { HeroModifieableStat.Mana, "mana_stat" },
            { HeroModifieableStat.MovementSpeed, "movespeed_stat" },
            { HeroModifieableStat.HealthRegen, "health_stat" },
            { HeroModifieableStat.Vision, "vision_stat" }
        };

        public static Dictionary<HeroModifieableStat, string> statNames = new Dictionary<HeroModifieableStat, string>()
        {
            { HeroModifieableStat.Attack, "Attack" },
            { HeroModifieableStat.Defense, "Defense" },
            { HeroModifieableStat.Damage, "Damage" },
            { HeroModifieableStat.Health, "Health" },
            { HeroModifieableStat.Mana, "Mana" },
            { HeroModifieableStat.MovementSpeed, "Movement Points" },
            { HeroModifieableStat.HealthRegen, "Health Regen" },
            { HeroModifieableStat.Vision, "Vision Radius" }
        };

        public static Dictionary<ModifierOperation, string> operationNames = new Dictionary<ModifierOperation, string>()
        {
            { ModifierOperation.Change, "+" },
            { ModifierOperation.Multiply, "x" },
        };

        public static Vector2Int[] CrossNeighbours = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.left, Vector2Int.down, Vector2Int.right
        };

        public static Vector2Int[] QuadNeighbours = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.up + Vector2Int.right, Vector2Int.right, Vector2Int.down + Vector2Int.right,
            Vector2Int.down, Vector2Int.down + Vector2Int.left, Vector2Int.left, Vector2Int.up + Vector2Int.left
        };

        public static TileDirection TileDirection_Any()
        {
            var directions = Enum.GetValues(typeof(TileDirection));
            int direction = 0;
            foreach (int dir in directions)
            {
                direction |= dir;
            }

            return (TileDirection)direction;
        }

        public static TileDirection TileDirection_AnyExcept(TileDirection except)
        {
            var directions = Enum.GetValues(typeof(TileDirection));
            int direction = 0;
            foreach (int dir in directions)
            {
                if (dir == (int)except)
                    continue;
                direction |= dir;
            }

            return (TileDirection)direction;
        }

        public class TileRuleCompound
        {
            public Dictionary<TileDirection, (TileCondition, TileState)> Rules;
        }

        public static TileRuleCompound NewTRC(Dictionary<TileDirection, (TileCondition, TileState)> contents)
        {
            return new TileRuleCompound()
            {
                Rules = contents
            };
        }

        public static Dictionary<TileRuleCompound, TileSide> TileRules = new Dictionary<TileRuleCompound, TileSide>()
        {
            {
                NewTRC(new Dictionary<TileDirection, (TileCondition, TileState)>()
                {
                    { TileDirection_Any(), (TileCondition.Required, TileState.Empty) }
                }),
                TileSide.SI_C
            },
            {
                NewTRC(new Dictionary<TileDirection, (TileCondition, TileState)>()
                {
                    { TileDirection_AnyExcept(TileDirection.BOT), (TileCondition.Required, TileState.Empty) }
                }),
                TileSide.SI_TE
            },
            {
                NewTRC(new Dictionary<TileDirection, (TileCondition, TileState)>()
                {
                    { TileDirection_AnyExcept(TileDirection.TOP), (TileCondition.Required, TileState.Empty) }
                }),
                TileSide.SI_BE
            },
            {
                NewTRC(new Dictionary<TileDirection, (TileCondition, TileState)>()
                {
                    { TileDirection_AnyExcept(TileDirection.RIGHT), (TileCondition.Required, TileState.Empty) }
                }),
                TileSide.SI_LE
            },
            {
                NewTRC(new Dictionary<TileDirection, (TileCondition, TileState)>()
                {
                    { TileDirection_AnyExcept(TileDirection.LEFT), (TileCondition.Required, TileState.Empty) }
                }),
                TileSide.SI_RE
            }
        };

        public static Vector3 NowhereVector3()
        {
            return new Vector3(-100f, -100f, 0f);
        }

        public static int GetSuperLayerMask()
        {
            int character = 1 << LayerMask.NameToLayer("Character");
            int treasure = 1 << LayerMask.NameToLayer("Treasure");
            int corpse = 1 << LayerMask.NameToLayer("Corpse");
            int structure = 1 << LayerMask.NameToLayer("Building");
            int ac = 1 << LayerMask.NameToLayer("ActionCell");
            int mask = character | treasure | corpse | structure | ac;
            return mask;
        }

        public static long GetCharactersDefaultPersona(MagicSpellAllowedTarget additional)
        {
            MagicSpellAllowedTarget mask = MagicSpellAllowedTarget.Alive | MagicSpellAllowedTarget.Hero |
                                           MagicSpellAllowedTarget.Mechanism | MagicSpellAllowedTarget.Summon |
                                           MagicSpellAllowedTarget.Undead | additional;
            return (long)mask;
        }

        public static void __SpawnMarker(Vector2Int position, Color color, bool useDefaultColor = true,
            float lifeTime = 1f)
        {
#if UNITY_EDITOR
            var marker = GameObject.Instantiate(ResourcesBase.GetPrefab("_MARKER"), Vector3.zero, Quaternion.identity)
                .GetComponent<MapEditorSprite>();
            if (!useDefaultColor)
            {
                marker.GetComponent<SpriteRenderer>().color = color;
            }

            marker.transform.position = new Vector3(position.x, position.y);
            marker.CreateWithLifeTime(lifeTime);
#endif
        }

        public static AttackActionCell AttackActionCell = new AttackActionCell();
        public static MoveActionCell MoveActionCell = new MoveActionCell();
        public static CastSpellActionCell CastSpellActionCell = new CastSpellActionCell();
        public static CollectActionCell CollectActionCell = new CollectActionCell();
        public static DwellingInteractActionCell DwellingInteractActionCell = new DwellingInteractActionCell();
        public static DropArtifactActionCell DropArtifactActionCell = new DropArtifactActionCell();

        public static EditCharactersPositionActionCell EditCharactersPositionActionCell =
            new EditCharactersPositionActionCell();

        public static SiegeActionCell SiegeActionCell = new SiegeActionCell();
        public static SiegeApplyCharacterCell SiegeApplyCharacterCell = new SiegeApplyCharacterCell();

        public static float playerAnimationSpeedGlobalMultipler = 8f;
        public static float aiAnimationSpeedGlobalMultipler = 2f;
        public static float heroGainExperiencePenalty = 0.5f;
        public static int heroGainExperienceRange = 5;
        public static float globalAmbienceVolume = 0.7f;
        public static int hotbarMagicSpellsMaxCount = 3;
    }
}