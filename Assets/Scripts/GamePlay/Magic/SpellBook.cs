using System;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SpellBook
    {
        public Player Player => _player;

        public Dictionary<string, MagicSpell> LearntMagicSpells => _learntMagicSpells;

        public List<MagicSpell> LearntMagicSpellsList =>
            _learntMagicSpells.Values.OrderBy(x => x.BaseManaCost).ToList();

        public Dictionary<int, MagicSpell> HotbarMagicSpells => _hotbarMagicSpells;

        public delegate void OnMagicSpellEventDelegate(MagicSpell magicSpell);

        public event OnMagicSpellEventDelegate SpellLearnt
        {
            add => spellLearnt += value;
            remove => spellLearnt -= value;
        }
        protected event OnMagicSpellEventDelegate spellLearnt;


        protected virtual void OnSpellLearnt(MagicSpell magicSpell)
        {
            spellLearnt?.Invoke(magicSpell);
        }
        protected Player _player;
        protected Dictionary<string, MagicSpell> _learntMagicSpells = new Dictionary<string, MagicSpell>();
        protected Dictionary<int, MagicSpell> _hotbarMagicSpells = new Dictionary<int, MagicSpell>();

        public SpellBook(SpellBookObject spellBookObject, Player player)
        {
            _player = player;
            foreach (var msName in spellBookObject.spells)
            {
                AddSpell(msName);
            }
        }

        public SpellBook(Player player)
        {
            _player = player;
        }

        public void SetHotbarMagicSpell(int hotbarSlotId, MagicSpell magicSpell)
        {
            if (hotbarSlotId >= GlobalVariables.hotbarMagicSpellsMaxCount) return;
            var keys = _hotbarMagicSpells.Keys;
            foreach (var mKey in keys)
            {
                var mValue = _hotbarMagicSpells[mKey];
                if (mValue == magicSpell)
                {
                    _hotbarMagicSpells[mKey] = null;
                    break;
                }
            }
            
            _hotbarMagicSpells[hotbarSlotId] = magicSpell;
        }

        public void RemoveSpell(string spellName)
        {
            _learntMagicSpells.Remove(spellName);
            int hotbarSlotId = -1;
            hotbarSlotId = _hotbarMagicSpells.Keys.Where(key => _hotbarMagicSpells[key].internalName == spellName)
                .FirstOrDefault();
            Debug.Log(hotbarSlotId);
            if (hotbarSlotId != -1)
                _hotbarMagicSpells.Remove(hotbarSlotId);
        }

        public MagicSpell AddSpell(string spellName)
        {
            var magicSpellObject = ResourcesBase.GetMagicSpell(spellName).Clone();
            var spellTargetingType = magicSpellObject.GetType();
            MagicSpell magicSpell = null;
            if (spellTargetingType == typeof(SingleTargetedMagicSpellObject))
            {
                magicSpell = new SingleTargetedMagicSpell((SingleTargetedMagicSpellObject)magicSpellObject);
            }
            else if (spellTargetingType == typeof(AreaTargetedMagicSpellObject))
            {
                magicSpell = new AreaTargetedMagicSpell((AreaTargetedMagicSpellObject)magicSpellObject);
            }

            // Debug.Log($"Trying to learn {spellName}");
            if (!_learntMagicSpells.ContainsKey(spellName))
            {
                // Debug.Log($"Doesn't contain it's ok, adding {spellName}");
                _learntMagicSpells.Add(spellName, magicSpell);
                OnSpellLearnt(magicSpell);
            }
            else
            {
                // Debug.Log($"Already has {spellName}");
            }
            return magicSpell;
        }

        public void PrepareSpell(MagicSpellObject magicSpellObject)
        {
        }

        public void LoadFromSerializable(SerializableSpellBook serializableSpellBook)
        {
            var magicSpellDatas = serializableSpellBook.MagicSpellDatas;
            var values = magicSpellDatas.Values;
            foreach (var spellData in values)
            {
                var magicSpell = AddSpell(spellData.internalName);
                magicSpell.Cooldown = spellData.Cooldown;
            }

            var hotbarSpellsData = serializableSpellBook.HotbarSpellsData;
            var keys = hotbarSpellsData.Keys;
            foreach (var hotbarSlotId in keys)
            {
                var spellId = hotbarSpellsData[hotbarSlotId];
                if(spellId == -1)continue;
                var spellName = magicSpellDatas[spellId].internalName;
                SetHotbarMagicSpell(hotbarSlotId, _learntMagicSpells[spellName]);
            }
        }
    }
}