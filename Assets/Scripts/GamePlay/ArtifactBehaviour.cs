using System;
using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using AgeOfHeroes.Spell;
using UnityEngine;
using UnityEngine.Serialization;

namespace AgeOfHeroes
{
    public class ArtifactBehaviour : AbstractCollectable
    {
        protected ArtifactObject _artifactObject;
        public PlayableTerrain PlayableTerrain;

        public ArtifactObject artifactObject => _artifactObject;

        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0f);
        }

        public void LoadFromSerializable(SerializableArtifact serializableArtifact)
        {
            Position = new Vector2Int(serializableArtifact.positionX, serializableArtifact.positionY);
            UniqueId = serializableArtifact.UniqueId == 0 ? UniqueId : serializableArtifact.UniqueId;
        }

        public override void ShowDialogue(Hero heroCollector)
        {
            var dialogue = GameManager.Instance.GUIManager.collectArtifactDialogue;
            if (dialogue == null)
            {
                dialogue = GUIDialogueFactory.CreateCollectArtifactDialogue();
            }

            if (heroCollector.inventoryManager.HasFreeSlots())
            {
                dialogue.HasFreeSlots();
                dialogue.okButton.onClick.AddListener((() =>
                {
                    OnCollected(heroCollector);
                    dialogue.Hide();
                }));
            }
            else
            {
                dialogue.AlertOfMaxSlots();
            }

            dialogue.Set(_artifactObject);
            dialogue.Show();
        }

        public override void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = GlobalVariables.TreasureRenderOrder;
            _spriteRenderer.sprite = _artifactObject.Icon;
            UniqueId = gameObject.GetInstanceID();
            var modifiers = _artifactObject.Modifiers;
            foreach (var m in modifiers)
            {
                int points = 0;
                var mValue = (int)m.value;
                var mOperation = m.operation;
                int multiplier = mOperation == ModifierOperation.Change ? 1 : 10;
                int sign = mValue > 0 ? 1 : -1;
                points = mValue * multiplier * sign;
                overallValue += points;
            }
        }

        public void Set(ArtifactObject artifactObject)
        {
            _artifactObject = artifactObject;
        }

        public override void OnAICollected(Hero hero)
        {
            OnCollected(hero);
        }

        public override void OnCollected(Hero heroCollector)
        {
            base.OnCollected(heroCollector);
            List<ArtifactModifier> modifiers = new List<ArtifactModifier>();
            foreach (var modifier in _artifactObject.Modifiers)
            {
                modifiers.Add(modifier.Clone() as ArtifactModifier);
            }
            
            var artifact = new Artifact()
            {
                ArtifactObject = _artifactObject,
                Modifiers = modifiers,
            };
            
            heroCollector.inventoryManager.AddArtifact(artifact);
            
            gameObject.SetActive(false);
            PlayableTerrain.SpawnedArtifacts.Remove(this);
            Destroy(gameObject);
            Debug.Log($"{heroCollector.title} collected {artifact.ArtifactObject.name}");
        }
    }
}