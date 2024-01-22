namespace AgeOfHeroes
{
    public class NecVeilOfDarknessSpecialBuilding : SpecialBuilding
    {
        public int Radius;

        public override void OnBuilt(Castle castle)
        {
            base.OnBuilt(castle);
            _targetCastle.Player.TurnRecieved += (player) => UpdateOnRound(castle);
        }

        public override void SpecialAction(Castle castle)
        {
            base.SpecialAction(castle);
        }

        private void UpdateOnRound(Castle castle)
        {
            var allPlayers = GameManager.Instance.MapScenarioHandler.AllPlayers;
            allPlayers.Remove(castle.Player);
            allPlayers.ForEach(player =>
            {
                player.fogOfWarController.UpdateVisionAtAreaForPlayer(castle.Player,_targetCastle.Position,
                    GameManager.Instance.terrainManager.CurrentPlayableMap.WorldTerrain, 10,
                    true);
            });
        }
    }
}