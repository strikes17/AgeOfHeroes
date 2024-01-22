using System;
using System.Collections;
using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class DwellBuildingBehaviour : MonoBehaviour, IClickTarget
    {
        protected DwellBuilding _dwellBuilding;
        protected SpriteRenderer _spriteRenderer;
        protected Player _player;
        private List<Colored> coloreds = new List<Colored>();
        public DwellBuilding Building => _dwellBuilding;
        
        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0);
        }

        public Player Player
        {
            set
            {
                _player = value;
                foreach (var colored in coloreds)
                {
                    colored.PlayerOwnerColor = _player.Color;
                }
            }
            get => _player;
        }

        public virtual void Capture(Player player)
        {
            Player = player;
        }
        
        public IEnumerator OnClicked(Player playerColor)
        {
            yield return null;
        }
        
        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public virtual void LoadFromSerializable(SerializableDwelling serializableDwelling)
        {
            name = $"{serializableDwelling.objectName}";
            _dwellBuilding = (ResourcesBase.GetBuilding(serializableDwelling.objectName) as DwellBuilding).Clone() as DwellBuilding;
            Position = new Vector2Int(serializableDwelling.positionX, serializableDwelling.positionY);
            _dwellBuilding.Position = Position;
            var coloredNames = _dwellBuilding.coloredsNames;
            foreach (var coloredName in coloredNames)
            {
                var coloredPrefab = ResourcesBase.GetPrefab($"Coloreds/{coloredName}");
                var coloredInstance = GameObject
                    .Instantiate(coloredPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Colored>();
                coloredInstance.transform.localPosition = Vector3.zero;
                coloreds.Add(coloredInstance);
            }
            _spriteRenderer.sprite = _dwellBuilding.GetIcon();
        }
    }
}