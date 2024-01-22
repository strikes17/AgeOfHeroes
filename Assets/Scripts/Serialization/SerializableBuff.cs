using AgeOfHeroes.Spell;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableBuff
    {
        public bool IsPositive;
        public string InternalName;
        public int DurationLeft;

        public SerializableBuff()
        {
        }

        public SerializableBuff(Buff buff)
        {
            IsPositive = buff.IsNotDebuff();
            InternalName = buff.internalName;
            DurationLeft = buff.durationLeft;
        }
    }
}