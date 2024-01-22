namespace AgeOfHeroes.AI
{
    public enum AIConditionImportance
    {
        NotImportant = 0,
        MereImportant = 1,
        MostImportant = 4
    }

    public class AIAttackTargetConditions
    {
        public int targetHasRetilation,
            targetIsMelee,
            targetIsShooter,
            targetIsWizard,
            targetIsTheHighestTier,
            targetMaxOutputDamage,
            targetMinRetilationDamage,
            targetIsTheLeastTier,
            targetIsClosest,
            targetIsNeutral,
            targetIsHero,
            targetIsHumanPlayer,
            targetIsAIPlayer,
            targetSecuresGoldMine,
            targetSecuresTreasure,
            targetSecuresArtifact;
    }

    public class AIPlayerMoveConditions
    {
        public AIConditionImportance shouldBuiltShop, shouldBuiltSpecial, shouldSkip, shouldBuyCharacter;
    }
}