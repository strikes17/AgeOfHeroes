using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using Mono.Cecil;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using File = System.IO.File;
using Object = System.Object;

public static class ResourcesBase
{
    private static Dictionary<string, GameObject> _loadedPrefabs = new Dictionary<string, GameObject>();
    private static Dictionary<string, MagicSpellObject> _loadedMagicSpells = new Dictionary<string, MagicSpellObject>();
    private static Dictionary<string, SpellBookObject> _loadedSpellBooks = new Dictionary<string, SpellBookObject>();
    private static Dictionary<string, Buff> _loadedBuffs = new Dictionary<string, Buff>();
    private static Dictionary<string, Sprite> _loadedSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, BuffSpecialEffectObject> _loadedBuffSpecialObject = new Dictionary<string, BuffSpecialEffectObject>();
    private static Dictionary<string, AbstractBuilding> _loadedBuildings = new Dictionary<string, AbstractBuilding>();
    private static Dictionary<string, CastleInfo> _loadedCastleInfos = new Dictionary<string, CastleInfo>();
    private static Dictionary<string, CastleObject> _loadedCastleObjects = new Dictionary<string, CastleObject>();
    private static Dictionary<string, TerrainTileObject> _loadedTerrainTileObjects = new Dictionary<string, TerrainTileObject>();
    private static Dictionary<string, HeroObject> _loadedHeroes = new Dictionary<string, HeroObject>();
    private static Dictionary<string, TreasureObject> _loadedTreasureObjects = new Dictionary<string, TreasureObject>();
    private static Dictionary<string, ArtifactObject> _loadedArtifactObjects = new Dictionary<string, ArtifactObject>();
    private static Dictionary<string, SpecialAbility> _loadedCombatAbilities = new Dictionary<string, SpecialAbility>();
    private static Dictionary<string, SpriteAnimationSequenceObject> _loadedSpriteAnimations = new Dictionary<string, SpriteAnimationSequenceObject>();
    private static Dictionary<string, HeroSkillTreeObject> _loadedSkillTreeObjects = new Dictionary<string, HeroSkillTreeObject>();
    private static Dictionary<string, HeroSkill> _loadedHeroSkills = new Dictionary<string, HeroSkill>();
    private static Dictionary<string, Tile> _loadedTiles = new Dictionary<string, Tile>();
    private static Dictionary<string, AudioClip> _loadedAudioClips = new Dictionary<string, AudioClip>();
    private static Dictionary<Fraction, FractionObject> _loadedFractions = new Dictionary<Fraction, FractionObject>();

    private static Dictionary<Fraction, Dictionary<string, CharacterObject>> _loadedCharacters = new Dictionary<Fraction, Dictionary<string, CharacterObject>>()
    {
        {Fraction.Human, new Dictionary<string, CharacterObject>()},
        {Fraction.Undead, new Dictionary<string, CharacterObject>()},
        {Fraction.Inferno, new Dictionary<string, CharacterObject>()},
        {Fraction.Mages, new Dictionary<string, CharacterObject>()},
    };

    public static Sprite GetDefaultCharacterSprite()
    {
        return GetSprite("character_unavail");
    }

    public static Sprite GetDefaultArtifactSprite()
    {
        return GetSprite("artifact_default");
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefab == null)
        {
            Debug.LogError($"{prefabName} Prefab is null!");
        }

        return prefab;
    }

    private static T DeserializeAsset<T>(string assetPath)
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
        jsonSerializerSettings.Formatting = Formatting.Indented;
        var jsonContents = Resources.Load<TextAsset>(assetPath);
        T asset = default;
        try
        {
            asset = JsonConvert.DeserializeObject<T>(jsonContents.ToString(), jsonSerializerSettings);
        }
        catch (Exception e)
        {
            Debug.Log($"Failed for {assetPath} of {typeof(T)}");
        }
        return asset;
    }

    public static Tile GetTile(string tileName, Action<Tile> onLoaded = null)
    {
        foreach (var ms in _loadedTiles)
        {
            if (ms.Key == tileName)
                return ms.Value;
        }

        Tile tile = Resources.Load<Tile>($"Tiles/{tileName}");
        if (tile == null)
        {
            Debug.LogError($"{tileName} tile is null!");
            return null;
        }

        _loadedTiles.TryAdd(tileName, tile);
        return tile;
    }

    public static AudioClip GetAudioClip(string name, AudioClipType audioClipType)
    {
        foreach (var ms in _loadedAudioClips)
        {
            if (ms.Key == name)
                return ms.Value;
        }

        string directory = audioClipType == AudioClipType.Sound ? "Sounds" : "Music";
        AudioClip audioClip = Resources.Load<AudioClip>($"{directory}/{name}");
        if (audioClip == null)
        {
            Debug.LogError($"{name} audioclip is null!");
            return null;
        }

        _loadedAudioClips.TryAdd(name, audioClip);
        return audioClip;
    }

    public static SpriteAnimationSequenceObject GetSpriteAnimation(string animationObjectName, Action<SpriteAnimationSequenceObject> onLoaded = null)
    {
        foreach (var ms in _loadedSpriteAnimations)
        {
            if (ms.Key == animationObjectName)
                return ms.Value;
        }

        SpriteAnimationSequenceObject animationObject = Resources.Load<SpriteAnimationSequenceObject>($"Particles/Animation/{animationObjectName}");
        if (animationObject == null)
        {
            Debug.LogError($"{animationObjectName} animationObjectName is null!");
            return null;
        }

        _loadedSpriteAnimations.TryAdd(animationObjectName, animationObject);
        return animationObject;
    }

    public static HeroSkill GetHeroSkill(string heroSkillName, Action<HeroSkill> onLoaded = null)
    {
        foreach (var ms in _loadedHeroSkills)
        {
            if (ms.Key == heroSkillName)
                return ms.Value;
        }

        var heroSkill = DeserializeAsset<HeroSkill>($"SkillTrees/Skills/{heroSkillName}");

        if (heroSkill == null)
        {
            Debug.LogError($"{heroSkillName} hero skill is null!");
            return null;
        }

        _loadedHeroSkills.TryAdd(heroSkillName, heroSkill);
        return heroSkill;
    }

    public static HeroSkillTreeObject GetHeroSkillTree(string heroSkillTreeName, Action<HeroSkillTreeObject> onLoaded = null)
    {
        foreach (var ms in _loadedSkillTreeObjects)
        {
            if (ms.Key == heroSkillTreeName)
                return ms.Value;
        }

        var skillTreeObject = DeserializeAsset<HeroSkillTreeObject>($"SkillTrees/{heroSkillTreeName}");

        if (skillTreeObject == null)
        {
            Debug.LogError($"{heroSkillTreeName} skill tree is null!");
            return null;
        }

        _loadedSkillTreeObjects.TryAdd(heroSkillTreeName, skillTreeObject);
        return skillTreeObject;
    }

    public static SpecialAbility GetCombatAbility(string combatAbilityName, Action<SpecialAbility> onLoaded = null)
    {
        foreach (var ms in _loadedCombatAbilities)
        {
            if (ms.Key == combatAbilityName)
                return ms.Value;
        }

        var combatAbility = DeserializeAsset<SpecialAbility>($"CombatAbilities/{combatAbilityName}");

        if (combatAbility == null)
        {
            Debug.LogError($"{combatAbilityName} combat ability is null!");
            return null;
        }

        _loadedCombatAbilities.TryAdd(combatAbilityName, combatAbility);
        return combatAbility;
    }

    public static CastleInfo GetCastleInfo(string castleInfoName, Action<CastleInfo> onLoaded = null)
    {
        foreach (var ms in _loadedCastleInfos)
        {
            if (ms.Key == castleInfoName)
                return ms.Value;
        }

        var castleInfo = DeserializeAsset<CastleInfo>($"Castles/{castleInfoName}");

        if (castleInfo == null)
        {
            Debug.LogError($"{castleInfoName} castle is null!");
            return null;
        }

        _loadedCastleInfos.TryAdd(castleInfoName, castleInfo);
        return castleInfo;
    }

    public static AbstractBuilding GetBuilding(string buildingName, Action<CharacterObject> onLoaded = null)
    {
        foreach (var ms in _loadedBuildings)
        {
            if (ms.Key == buildingName)
                return ms.Value;
        }

        var building = DeserializeAsset<AbstractBuilding>($"Buildings/{buildingName}");

        if (building == null)
        {
            Debug.LogError($"{buildingName} building is null!");
            return null;
        }

        _loadedBuildings.TryAdd(buildingName, building);
        return building;
    }

    public static FractionObject GetFractionObject(Fraction fraction)
    {
        if(_loadedFractions.TryGetValue(fraction, out FractionObject cachedFractionObject)){
            return cachedFractionObject;
        }
        FractionObject fractionObject = Resources.Load<FractionObject>($"Fractions/{fraction}");
        if (fractionObject == null)
        {
            Debug.LogError($"{fraction} is null!");
            return null;
        }

        _loadedFractions.TryAdd(fraction, fractionObject);
        return fractionObject;
    }

    public static CharacterObject GetCharacterObject(string characterName, Fraction fraction, Action<CharacterObject> onLoaded = null)
    {
        foreach (var ms in _loadedCharacters[fraction])
        {
            if (ms.Key == characterName)
                return ms.Value;
        }

        CharacterObject characterObject = Resources.Load<CharacterObject>($"Characters/{fraction.ToString()}/{characterName}");
        if (characterObject == null)
        {
            Debug.LogError($"{characterName} character is null!");
            return null;
        }

        _loadedCharacters[fraction].TryAdd(characterName, characterObject);
        return characterObject;
    }

    public static TreasureObject GetTreasureObject(string treasureObjectName)
    {
        foreach (var ms in _loadedTreasureObjects)
        {
            if (ms.Key == treasureObjectName)
                return ms.Value;
        }

        var treasureObject = Resources.Load<TreasureObject>($"Treasures/{treasureObjectName}");
        if (treasureObject == null)
        {
            Debug.LogError($"{treasureObjectName} treasure object is null!");
            return null;
        }

        _loadedTreasureObjects.TryAdd(treasureObjectName, treasureObject);
        return treasureObject;
    }

    public static ArtifactObject GetArtifactObject(string artifactObjectName)
    {
        foreach (var ms in _loadedArtifactObjects)
        {
            if (ms.Key == artifactObjectName)
                return ms.Value;
        }

        var artifactObject = Resources.Load<ArtifactObject>($"Artifacts/{artifactObjectName}");
        if (artifactObject == null)
        {
            Debug.LogError($"{artifactObjectName} treasure object is null!");
            return null;
        }

        _loadedArtifactObjects.TryAdd(artifactObjectName, artifactObject);
        return artifactObject;
    }

    public static TerrainTileObject GetTerrainTileObject(string terrainTileObjectName)
    {
        foreach (var ms in _loadedTerrainTileObjects)
        {
            if (ms.Key == terrainTileObjectName)
                return ms.Value;
        }

        TerrainTileObject terrainTileObject = Resources.Load<TerrainTileObject>($"Terrain/{terrainTileObjectName}");
        if (terrainTileObject == null)
        {
            Debug.LogError($"{terrainTileObjectName} terrain tile object is null!");
            return null;
        }

        _loadedTerrainTileObjects.TryAdd(terrainTileObjectName, terrainTileObject);
        return terrainTileObject;
    }

    public static HeroObject GetHeroObject(string heroObjectName, Fraction fraction)
    {
        foreach (var ms in _loadedHeroes)
        {
            if (ms.Key == heroObjectName)
                return ms.Value;
        }

        HeroObject heroObject = Resources.Load<HeroObject>($"Heroes/{fraction.ToString()}/{heroObjectName}");
        if (heroObject == null)
        {
            Debug.LogError($"{heroObjectName} hero object is null!");
            return null;
        }

        _loadedHeroes.TryAdd(heroObjectName, heroObject);
        return heroObject;
    }

    public static CastleObject GetCastleObject(string castleObjectName)
    {
        foreach (var ms in _loadedCastleObjects)
        {
            if (ms.Key == castleObjectName)
                return ms.Value;
        }

        CastleObject castleObject = Resources.Load<CastleObject>($"CastleObjects/{castleObjectName}");
        if (castleObject == null)
        {
            Debug.LogError($"{castleObjectName} castle object is null!");
            return null;
        }

        _loadedCastleObjects.TryAdd(castleObjectName, castleObject);
        return castleObject;
    }

    public static CharacterObject GetCharacterObject(string characterName, Action<CharacterObject> onLoaded = null)
    {
        Fraction foundInFraction = Fraction.None;

        foreach (var fraction in _loadedCharacters)
        {
            foreach (var character in _loadedCharacters[fraction.Key])
            {
                if (character.Key == characterName)
                {
                    foundInFraction = fraction.Key;
                    return character.Value;
                }
            }
        }

        if (foundInFraction == Fraction.None)
        {
            Debug.LogError($"Fraction is undefined!");
            return null;
        }

        CharacterObject characterObject = Resources.Load<CharacterObject>($"Characters/{foundInFraction.ToString()}/{characterName}");
        if (characterObject == null)
        {
            Debug.LogError($"{characterName} character is null!");
            return null;
        }

        _loadedCharacters[foundInFraction].TryAdd(characterName, characterObject);
        return characterObject;
    }

    public static BuffSpecialEffectObject GetBuffSpecialEffect(string buffSpecialEffectObjectName, Action<BuffSpecialEffectObject> onLoaded = null)
    {
        foreach (var ms in _loadedBuffSpecialObject)
        {
            if (ms.Key == buffSpecialEffectObjectName)
                return ms.Value;
        }

        BuffSpecialEffectObject buffSpecialEffectObject = Resources.Load<BuffSpecialEffectObject>($"Buffs/Special Effects/{buffSpecialEffectObjectName}");
        if (buffSpecialEffectObject == null)
        {
            Debug.LogWarning($"{buffSpecialEffectObjectName} buff special effect is null!");
            return null;
        }

        _loadedBuffSpecialObject.TryAdd(buffSpecialEffectObjectName, buffSpecialEffectObject);
        return buffSpecialEffectObject;
    }

    public static Sprite GetPlayerBanner(string bannerName)
    {
        string fullBannerSprite = $"players_banner+{bannerName}";
        var sprite = GetSprite(fullBannerSprite);
        return sprite;
    }

    public static Sprite GetSprite(string spriteName)
    {
        foreach (var ms in _loadedSprites)
        {
            if (ms.Key == spriteName)
                return ms.Value;
        }

        var sprite = Resources.Load<Sprite>($"Sprites/{spriteName}");
        if (sprite == null)
        {
            if (string.IsNullOrEmpty(spriteName))
                return null;
            var path = spriteName.Split('+');
            var spritesSubAssets = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            foreach (var spriteSubAsset in spritesSubAssets)
            {
                if (spriteSubAsset.name == path[1])
                {
                    _loadedSprites.TryAdd(spriteName, spriteSubAsset);
                    return spriteSubAsset;
                }
            }

            Debug.LogError($"{spriteName} Sprite is null!");
        }

        _loadedSprites.TryAdd(spriteName, sprite);

        return sprite;
    }

    public static Buff GetBuff(string internalBuffName, Action<Buff> onLoaded = null)
    {
        foreach (var ms in _loadedBuffs)
        {
            if (ms.Key == internalBuffName)
                return ms.Value;
        }

        var buff = DeserializeAsset<Buff>($"Buffs/{internalBuffName}");
        if (buff == null)
        {
            Debug.LogError($"{internalBuffName} buff is null!");
        }

        _loadedBuffs.TryAdd(internalBuffName, buff);
        return buff;
    }

    public static MagicSpellObject GetMagicSpell(string internalMagicSpellName, Action<MagicSpellObject> onLoaded = null)
    {
        foreach (var ms in _loadedMagicSpells)
        {
            if (ms.Key == internalMagicSpellName)
                return ms.Value;
        }

        var magicSpell = DeserializeAsset<MagicSpellObject>($"Magic Spells/{internalMagicSpellName}");
        if (magicSpell == null)
        {
            Debug.LogError($"{internalMagicSpellName} Spell book object is null!");
        }

        _loadedMagicSpells.TryAdd(internalMagicSpellName, magicSpell);
        return magicSpell;
    }

    public static SpellBookObject GetSpellBook(string internalSpellBookName, Action<SpellBookObject> onLoaded = null)
    {
        foreach (var ms in _loadedSpellBooks)
        {
            if (ms.Key == internalSpellBookName)
                return ms.Value;
        }

        var spellBookObject = DeserializeAsset<SpellBookObject>($"Spell Books/{internalSpellBookName}");

        if (spellBookObject == null)
        {
            Debug.LogError($"{internalSpellBookName} Spell book object is null!");
        }

        _loadedSpellBooks.TryAdd(internalSpellBookName, spellBookObject);
        return spellBookObject;
    }

    public static void LoadSoundAsync(string prefabName, Action<Object> loaded = null)
    {
        string path = $"Sounds/{prefabName}";
        var loadAsync = Resources.LoadAsync<AudioClip>(path);
        loadAsync.completed += (operation =>
        {
            var loadedPrefab = loadAsync.asset as AudioClip;
            if (loadedPrefab == null)
            {
                Debug.LogError($"{prefabName} Sound is null!");
                return;
            }

            loaded?.Invoke(loadAsync.asset);
        });
    }

    public static void LoadBuffAsync(string buffInternalName, Action<string> loaded = null)
    {
        string path = $"Buffs/{buffInternalName}";
        var loadAsync = Resources.LoadAsync<TextAsset>(path);
        loadAsync.completed += (operation =>
        {
            var buffAsset = loadAsync.asset as TextAsset;
            if (buffAsset == null)
            {
                Debug.LogError($"{buffAsset} Buff is null!");
                return;
            }

            loaded?.Invoke(loadAsync.asset.ToString());
        });
    }

    public static void LoadPrefabAsync(string prefabName, Action<Object> loaded = null, bool cacheSoundInPrefabDataBase = true)
    {
        var loadAsync = Resources.LoadAsync<GameObject>($"Prefabs/{prefabName}");
        loadAsync.completed += (operation =>
        {
            var loadedPrefab = loadAsync.asset as GameObject;
            if (loadedPrefab == null)
            {
                Debug.LogError($"{prefabName} Prefab is null!");
                return;
            }

            if (cacheSoundInPrefabDataBase)
                _loadedPrefabs.TryAdd(string.IsNullOrEmpty(loadedPrefab.name) ? prefabName : loadedPrefab.name, loadedPrefab);
            loaded?.Invoke(loadAsync.asset);
        });
    }

    public static GameObject GetPrefab(string prefabName)
    {
        GameObject prefab = null;
        _loadedPrefabs.TryGetValue(prefabName, out prefab);
        if (prefab == null)
        {
            return LoadPrefab(prefabName);
        }

        return prefab;
    }
}