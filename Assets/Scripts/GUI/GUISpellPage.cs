using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class GUISpellPage : MonoBehaviour
    {
        [HideInInspector] public List<GUISpellSlot> spellSlots = new List<GUISpellSlot>();

        public delegate void OnSpellDelegate(MagicSpell magicSpell);

        public event OnSpellDelegate SpellSelected
        {
            add => spellSelected += value;
            remove => spellSelected -= value;
        }

        private event OnSpellDelegate spellSelected;

        public void PlaceSpell(MagicSpell magicSpell)
        {
            var spellSlotPrefab = ResourcesBase.GetPrefab("GUI/SpellSlot");
            var spellSlotInstance = GameObject.Instantiate(spellSlotPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<GUISpellSlot>();
            spellSlotInstance.MagicSpell = magicSpell;
            spellSlotInstance.button.onClick.AddListener(() => OnSpellSelected(magicSpell));
            spellSlots.Add(spellSlotInstance);
        }


        public void OnSpellSelected(MagicSpell magicSpell)
        {
            spellSelected.Invoke(magicSpell);
        }

        public void Clear()
        {
            for (int i = 0; i < spellSlots.Count; i++)
                Destroy(spellSlots[i].gameObject);
            spellSlots.Clear();
        }
    }
}