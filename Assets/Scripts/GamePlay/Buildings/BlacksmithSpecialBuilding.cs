namespace AgeOfHeroes
{
    public class BlacksmithSpecialBuilding : SpecialBuilding
    {
        public int attackValue, defenseValue;

        public override void OnBuilt(Castle castle)
        {
            base.OnBuilt(castle);
            var armorBuff = ResourcesBase.GetBuff("blacksmith_armor5");
            var attackBuff = ResourcesBase.GetBuff("blacksmith_attack10");
            var targetPlayer = _targetCastle.Player;
            targetPlayer.controlledCharacters.ForEach(x =>
            {
                x.CreateBuff(armorBuff).Apply();
                x.CreateBuff(attackBuff).Apply();
            });
        }
    }
}