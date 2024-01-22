using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public class AIGuardMarker : AbstractMarker
    {
        private SpriteRenderer _spriteRenderer;

        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0);
        }
        
        public int Tier, Quantity, Level;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
        }

        public override void LoadFromSerializable(SerializableEntity serializableEntity)
        {
            var serializableMarker = serializableEntity as SerializableMarker;
            Tier = serializableMarker.Tier;
            Quantity = serializableMarker.Quantity;
            Level = serializableMarker.Level;
            Position = new Vector2Int(serializableMarker.positionX, serializableMarker.positionY);
        }
    }
}