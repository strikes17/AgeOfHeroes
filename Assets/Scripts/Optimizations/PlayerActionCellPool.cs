using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class PlayerActionCellPool : MonoBehaviour
    {
        private List<ActionCell> _actionCellsPool = new List<ActionCell>();
        private ActionCell _actionCellPrefab;

        private void Awake()
        {
            _actionCellPrefab = ResourcesBase.GetPrefab("_AC").GetComponent<ActionCell>();
        }
        
        public void CreateActionCellsOnCharacters(List<ControllableCharacter> controllableCharacters,
            AbstractActionCell actionCellVariant)
        {
            foreach (var character in controllableCharacters)
            {
                CreateActionCellAtPosition(character.transform.position, actionCellVariant);
            }
        }
        
        public void CreateActionCellAtPosition(Vector2 position, AbstractActionCell actionCellVariant)
        {
            var actionCellInstance =
                GetOrCreate(actionCellVariant, position);
            actionCellInstance.name = $"{actionCellVariant.GetType()}";
        }
        
        public void CreateActionCellsAtPositions(List<Vector2Int> positions, AbstractActionCell actionCellVariant)
        {
            ResetAll();
            foreach (var position in positions)
            {
                CreateActionCellAtPosition(position, actionCellVariant);
            }
        }


        protected ActionCell GetOrCreate(AbstractActionCell actionCellVariant, Vector3 position)
        {
            int count = _actionCellsPool.Count;
            for (int i = 0; i < count; i++)
            {
                var actionCell = _actionCellsPool[i];
                if (!actionCell.Active)
                {
                    actionCell.Variant = actionCellVariant;
                    actionCell.transform.position = position;
                    actionCell.Active = true;
                    return actionCell;
                }
            }

            var instance = GameObject.Instantiate(_actionCellPrefab, position, Quaternion.identity, transform);
            instance.Variant = actionCellVariant;
            instance.Active = true;
            _actionCellsPool.Add(instance);
            return instance;
        }

        public void ResetAll()
        {
            _actionCellsPool.ForEach(x => { x.Active = false; });
        }
    }
}