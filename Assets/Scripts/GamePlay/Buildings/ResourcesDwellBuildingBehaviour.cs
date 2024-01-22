namespace AgeOfHeroes
{
    public class ResourcesDwellBuildingBehaviour : DwellBuildingBehaviour
    {
        public override void Capture(Player player)
        {
            base.Capture(player);
            Building.OnOwnerChanged(player);
        }
    }
}