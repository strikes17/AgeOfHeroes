namespace AgeOfHeroes.Spell
{
    public class IntegerModifier : AbstractModifier
    {
        public IntegerModifier(int value)
        {
            _id = instanceIdIndex++;
            this.value = value;
        }

        public int value;
    }
}