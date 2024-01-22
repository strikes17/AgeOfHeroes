namespace AgeOfHeroes.Spell
{
    public class FloatModifier : AbstractModifier
    {
        public FloatModifier(float value)
        {
            _id = instanceIdIndex++;
            this.value = value;
        }

        public float value;
    }
}