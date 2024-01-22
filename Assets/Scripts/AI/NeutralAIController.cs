using System.Collections;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.AI
{
    public class NeutralAIController : BasicAIPlayerController
    {
        private MatchDifficulty _difficulty = MatchDifficulty.Easy;

        public NeutralAIController()
        {
            Debug.Log("Neutral AI Controller spawned!");
        }

        public override void ProcessAI()
        {
            Moroutine.Run(IEPollCharactersTurns());
        }

        public void SetDifficulty(MatchDifficulty difficulty)
        {
            _difficulty = difficulty;
        }

        protected override IEnumerator IEPollCharactersTurns()
        {
            yield return new WaitForEndOfFrame();
            OnAITurnEnded();
            yield return null;
        }
    }
}