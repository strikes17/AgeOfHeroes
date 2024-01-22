namespace AgeOfHeroes
{
    public abstract class AbstractData
    {
        
    }
    public class CombatData : AbstractData
    {
        public ControllableCharacter offensiveCharacter, defensiveCharacter;
        public int totalDamage;
        public int killQuantity;
        public bool willRetilate;
        public bool performedOnRetilation;
    }
}