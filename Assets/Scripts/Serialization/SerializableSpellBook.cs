using System.Collections.Generic;
using AgeOfHeroes.Spell;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableSpellBook
    {
        public Dictionary<int, SerializableMagicSpellData> MagicSpellDatas = new Dictionary<int, SerializableMagicSpellData>();
        public Dictionary<int, int> HotbarSpellsData = new Dictionary<int, int>();

        public SerializableSpellBook(){}
        
        public SerializableSpellBook(SpellBook spellBook)
        {
            var magicSpells = spellBook.LearntMagicSpells.Values;
            foreach (var spell in magicSpells)
            {
                SerializableMagicSpellData data = new SerializableMagicSpellData()
                {
                    Cooldown = spell.Cooldown,
                    internalName =  spell.internalName
                };
                MagicSpellDatas.Add(spell.GetHashCode(), data);
            }

            var hotbarMagicSpells = spellBook.HotbarMagicSpells;
            for (int i = 0; i < hotbarMagicSpells.Count; i++)
            {
                var hotbarMagicSpell = hotbarMagicSpells[i];
                if (hotbarMagicSpell == null)
                {
                    HotbarSpellsData[i] = -1;
                    continue;
                }

                HotbarSpellsData[i] = hotbarMagicSpell.GetHashCode();
            }
        }
    }
}