using System;
using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class SpawnManager : AbstractManager
    {
        [SerializeField] private Transform charactersRoot;
        [SerializeField] private Transform castlesRoot;
        [SerializeField] private Transform treasuresRoot;

        [SerializeField]
        private GameObject _castlePrefab, _heroPrefab, _treasurePrefab, _artifactPrefab, _dwellingPrefab;

        private void Start()
        {
            OnLoaded();
        }

        public ControllableCharacter SpawnCharacter(string name, Fraction fraction, PlayerColor playerOwnerId,
            Vector3Int position, PlayableTerrain targetTerrain, int customAmountInStack = -1)
        {
            var prefab = ResourcesBase.GetPrefab("_CHAR");
            var characterObject = ResourcesBase.GetCharacterObject(name, fraction);
            var newUnit = GameObject.Instantiate(prefab, position, Quaternion.identity, charactersRoot);
            var character = newUnit.GetComponent<Character>();
            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            var castleTerrains = playableMap.CastlesTerrains;
            if (castleTerrains.Count > 0)
                foreach (var castleTerrain in castleTerrains.Values)
                {
                    character.ReserveTerrainFog(castleTerrain);
                }

            character.ReserveTerrainFog(playableMap.WorldTerrain);
            character.CharacterObject = characterObject;
            character.playerOwnerColor = playerOwnerId;
            character.Init();
            character.PlaceOnTerrain(targetTerrain);
            newUnit.name = characterObject.title;
            var players = GameManager.Instance.MapScenarioHandler.players;
            if (customAmountInStack == -1)
            {
                character.Count = character.CharacterObject.fullStackCount;
            }
            else
            {
                character.Count = customAmountInStack;
            }

            var player = players[playerOwnerId];
            player.OnCharacterSpawned(character);
            return character;
        }

        public ControllableCharacter SpawnCharacter(SerializableCharacter serializableCharacter,
            PlayableTerrain playableTerrain)
        {
            var playerOwnerColor = serializableCharacter.PlayerOwnerColor;
            var prefab = ResourcesBase.GetPrefab("_CHAR");
            var character = GameObject.Instantiate(prefab,
                    new Vector3(serializableCharacter.positionX, serializableCharacter.positionY, 0f),
                    Quaternion.identity, charactersRoot)
                .GetComponent<Character>();
            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            
            var castleTerrains = playableMap.CastlesTerrains;
            if (castleTerrains.Count > 0)
                foreach (var castleTerrain in castleTerrains.Values)
                {
                    character.ReserveTerrainFog(castleTerrain);
                }

            character.ReserveTerrainFog(playableMap.WorldTerrain);
            character.CharacterObject =
                ResourcesBase.GetCharacterObject(serializableCharacter.objectName, serializableCharacter.Fraction);
            character.playerOwnerColor = playerOwnerColor;
            character.Init();
            character.PlaceOnTerrain(playableTerrain);
            character.LoadFromSerializable(serializableCharacter);

            var players = GameManager.Instance.MapScenarioHandler.players;
            var player = players[playerOwnerColor];
            player.OnCharacterSpawned(character);
            return character;
        }

        public Hero SpawnHero(SerializableHero serializableHero, PlayableTerrain playableTerrain)
        {
            var playerOwnerColor = serializableHero.PlayerOwnerColor;
            var heroInstance = GameObject.Instantiate(_heroPrefab,
                    new Vector3(serializableHero.positionX, serializableHero.positionY, 0f), Quaternion.identity,
                    castlesRoot)
                .GetComponent<Hero>();
            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            var castleTerrains = playableMap.CastlesTerrains;
            if (castleTerrains.Count > 0)
                foreach (var castleTerrain in castleTerrains.Values)
                {
                    heroInstance.ReserveTerrainFog(castleTerrain);
                }

            heroInstance.ReserveTerrainFog(playableMap.WorldTerrain);
            heroInstance.playerOwnerColor = playerOwnerColor;
            heroInstance.HeroObject =
                ResourcesBase.GetHeroObject(serializableHero.objectName, serializableHero.Fraction);
            heroInstance.Init();
            heroInstance.PlaceOnTerrain(playableTerrain);
            heroInstance.LoadFromSerializable(serializableHero);
            var players = GameManager.Instance.MapScenarioHandler.players;
            var player = players[playerOwnerColor];
            player.OnCharacterSpawned(heroInstance);
            return heroInstance;
        }

        public ControllableCharacter SpawnCharacter(CharacterObject characterObject, Fraction fraction,
            PlayerColor playerOwnerId, Vector2Int position, PlayableTerrain targetTerrain, int customAmountInStack = -1)
        {
            var prefab = ResourcesBase.GetPrefab("_CHAR");
            var newUnit = GameObject.Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity, charactersRoot);
            var character = newUnit.GetComponent<Character>();
            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            
            var castleTerrains = playableMap.CastlesTerrains;
            if (castleTerrains.Count > 0)
                foreach (var castleTerrain in castleTerrains.Values)
                {
                    character.ReserveTerrainFog(castleTerrain);
                }
            character.ReserveTerrainFog(playableMap.WorldTerrain);
            character.CharacterObject = characterObject;
            character.playerOwnerColor = playerOwnerId;
            character.Init();
            character.PlaceOnTerrain(targetTerrain);
            newUnit.name = characterObject.title;
            var players = GameManager.Instance.MapScenarioHandler.players;
            if (customAmountInStack == -1)
            {
                character.Count = character.CharacterObject.fullStackCount;
            }
            else
            {
                character.Count = customAmountInStack;
            }

            var player = players[playerOwnerId];
            player.OnCharacterSpawned(character);
            return character;
        }

        public ControllableCharacter SpawnCorpse(ControllableCharacter controllableCharacter)
        {
            var corpsePrefab = ResourcesBase.GetPrefab("_CORPSE");
            var corpseComponent = GameObject.Instantiate(corpsePrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<Corpse>();
            corpseComponent.BaseCharacterObject = controllableCharacter.BaseCharacterObject;
            corpseComponent.Init(controllableCharacter.persona);
            corpseComponent.Count = controllableCharacter.SpawnedCount;
            corpseComponent.playerOwnerColor = controllableCharacter.playerOwnerColor;
            corpseComponent.PlayableTerrain = controllableCharacter.PlayableTerrain;
            corpseComponent.PlayableTerrain.SpawnedCorpses.Add(corpseComponent);
            corpseComponent.ResetWorldPosition(controllableCharacter.Position);
            corpseComponent.PlayableTerrain.TerrainNavigator.NavigationMap.SetCellFree(controllableCharacter.Position.x,
                controllableCharacter.Position.y);
            controllableCharacter.Player.OnCorpseSpawned(corpseComponent);
            return corpseComponent;
        }

        public Hero SpawnHero(HeroObject heroObject, PlayerColor playerColor, Vector3Int position,
            PlayableTerrain targetTerrain)
        {
            var heroInstance = GameObject.Instantiate(_heroPrefab, position, Quaternion.identity, castlesRoot)
                .GetComponent<Hero>();
            var playableMap = GameManager.Instance.terrainManager.CurrentPlayableMap;
            var castleTerrains = playableMap.CastlesTerrains;
            if (castleTerrains.Count > 0)
                foreach (var castleTerrain in castleTerrains.Values)
                {
                    heroInstance.ReserveTerrainFog(castleTerrain);
                }

            heroInstance.ReserveTerrainFog(playableMap.WorldTerrain);
            heroInstance.playerOwnerColor = playerColor;
            heroInstance.HeroObject = heroObject;
            heroInstance.Init();
            heroInstance.PlaceOnTerrain(targetTerrain);
            var players = GameManager.Instance.MapScenarioHandler.players;
            var player = players[playerColor];
            player.OnCharacterSpawned(heroInstance);
            return heroInstance;
        }

        public Castle SpawnCastle(SerializableCastle serializableCastle, PlayableTerrain playableTerrain)
        {
            var castleInstance = GameObject.Instantiate(_castlePrefab,
                    new Vector3(serializableCastle.positionX, serializableCastle.positionY, 0f), Quaternion.identity,
                    castlesRoot)
                .GetComponent<Castle>();
            castleInstance.Init();
            var players = GameManager.Instance.MapScenarioHandler.players;
            var player = players[serializableCastle.PlayerOwnerColor];
            player.OnCastleSpawned(castleInstance);
            playableTerrain.SpawnedCastles.Add(castleInstance);
            return castleInstance;
        }

        // public Castle SpawnCastle(CastleObject castleObject, CastleInfo castleInfo, PlayerColor playerColor,
        //     Vector3Int position, string castleMapName, PlayableTerrain targetTerrain)
        // {
        //     var castleInstance = GameObject.Instantiate(_castlePrefab, position, Quaternion.identity, castlesRoot)
        //         .GetComponent<Castle>();
        //     castleInstance.PlayerOwnerColor = playerColor;
        //     castleInstance._castleObject = castleObject;
        //     castleInstance.castleMapName = castleMapName;
        //     castleInstance.Init(castleInfo);
        //     var players = GameManager.Instance.MapScenarioHandler.players;
        //     var player = players[playerColor];
        //     player.OnCastleSpawned(castleInstance);
        //     targetTerrain.SpawnedCastles.Add(castleInstance);
        //     return castleInstance;
        // }

        public void SpawnMarker(SerializableMarker serializableMarker, PlayableTerrain playableTerrain)
        {
            var markerPrefab = ResourcesBase.GetPrefab("AI Guard Marker");
            var marker = GameObject
                .Instantiate(markerPrefab, Vector3.zero, Quaternion.identity, treasuresRoot)
                .GetComponent<AbstractMarker>();
            marker.name = $"ai_marker_{marker.GetType()}";
            marker.LoadFromSerializable(serializableMarker);
            playableTerrain.SpawnedMarkers.Add(marker);
        }

        public void SpawnTreasure(SerializableTreasure serializableTreasure, PlayableTerrain playableTerrain)
        {
            var treasureInstance = GameObject
                .Instantiate(_treasurePrefab, Vector3.zero, Quaternion.identity, treasuresRoot)
                .GetComponent<AbstractTreasure>();
            treasureInstance.Set(ResourcesBase.GetTreasureObject(serializableTreasure.objectName));
            treasureInstance.Init();
            treasureInstance.LoadFromSerializable(serializableTreasure);
            playableTerrain.SpawnedTreasures.Add(treasureInstance);
            treasureInstance.Collected += collector =>
            {
                playableTerrain.SpawnedTreasures.Remove(treasureInstance);
                // Debug.Log($"TREASURE REMOVAL {treasureInstance.GetInstanceID()}");
            };
        }
        
        public void SpawnDwelling(SerializableDwelling serializableDwelling, PlayableTerrain playableTerrain)
        {
            var instance = GameObject
                .Instantiate(_dwellingPrefab, Vector3.zero, Quaternion.identity, treasuresRoot);
            var building = ResourcesBase.GetBuilding(serializableDwelling.objectName);
            var type = building.GetType();
            DwellBuildingBehaviour dwellBuilding = null;
            if (type == typeof(ResourceIncomeDwell))
            {
                dwellBuilding = instance.AddComponent<ResourcesDwellBuildingBehaviour>();
            }
            else if (type == typeof(CharacterIncomeDwell))
            {
                dwellBuilding = instance.AddComponent<CharactersDwellBuildingBehaviour>();
            }
            dwellBuilding.LoadFromSerializable(serializableDwelling);
            var player = GameManager.Instance.MapScenarioHandler.players[serializableDwelling.PlayerColor];
            dwellBuilding.Capture(player);
            playableTerrain.SpawnedDwellings.Add(dwellBuilding);
        }

        public void SpawnTreasure(TreasureObject treasureObject, Vector3Int position, PlayableTerrain playableTerrain)
        {
            var treasureInstance = GameObject.Instantiate(_treasurePrefab, position, Quaternion.identity, treasuresRoot)
                .GetComponent<AbstractTreasure>();
            treasureInstance.Set(treasureObject);
            treasureInstance.Init();
            playableTerrain.SpawnedTreasures.Add(treasureInstance);
            treasureInstance.Collected += collector =>
            {
                playableTerrain.SpawnedTreasures.Remove(treasureInstance);
                Debug.Log("TREASURE REMOVAL");

            };
        }

        public ArtifactBehaviour SpawnArtifact(SerializableArtifact serializableArtifact, PlayableTerrain playableTerrain)
        {
            var artifactBehaviour = GameObject
                .Instantiate(_artifactPrefab, Vector3.zero, Quaternion.identity, treasuresRoot)
                .GetComponent<ArtifactBehaviour>();
            artifactBehaviour.Set(ResourcesBase.GetArtifactObject(serializableArtifact.objectName));
            artifactBehaviour.Init();
            artifactBehaviour.PlayableTerrain = playableTerrain;
            artifactBehaviour.LoadFromSerializable(serializableArtifact);
            playableTerrain.SpawnedArtifacts.Add(artifactBehaviour);
            artifactBehaviour.Collected += collector =>
            {
                playableTerrain.SpawnedArtifacts.Remove(artifactBehaviour);
            };
            return artifactBehaviour;
        }

        public ArtifactBehaviour SpawnArtifact(ArtifactObject artifactObject, Vector3Int position, PlayableTerrain playableTerrain)
        {
            var artifactBehaviour = GameObject
                .Instantiate(_artifactPrefab, position, Quaternion.identity, treasuresRoot)
                .GetComponent<ArtifactBehaviour>();
            artifactBehaviour.Set(artifactObject);
            artifactBehaviour.Init();
            artifactBehaviour.PlayableTerrain = playableTerrain;
            playableTerrain.SpawnedArtifacts.Add(artifactBehaviour);
            artifactBehaviour.Collected += collector =>
            {
                playableTerrain.SpawnedArtifacts.Remove(artifactBehaviour);
            };
            return artifactBehaviour;
        }

        private void DespawnTreasure(AbstractCollectable collectable)
        {
            Moroutine.Run(collectable.IEDestroy());
        }
    }
}