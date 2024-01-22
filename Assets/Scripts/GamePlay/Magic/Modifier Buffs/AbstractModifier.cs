namespace AgeOfHeroes.Spell
{
    public abstract class AbstractModifier
    {
        public int Operation;
        public int id => _id;

        protected static int instanceIdIndex;
        protected int _id;
    }
}