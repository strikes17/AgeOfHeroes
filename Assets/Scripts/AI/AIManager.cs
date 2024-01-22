using System;
using System.IO;
using System.Linq;
using System.Text;
using AStar;
using UnityEngine;

namespace AgeOfHeroes.AI
{
    public class AIManager : MonoBehaviour
    {
        public BasicAIPlayerController GetAIPlayerController(MatchDifficulty matchDifficulty)
        {
            switch (matchDifficulty)
            {
                case MatchDifficulty.Easy: return new EasyAIPlayerController();
            }

            return new BasicAIPlayerController();
        }

        public NeutralAIController GetNeutralAIController()
        {
            return new NeutralAIController();
        }
    }

    public class AIDecision
    {
        public AICharacterDecisionType DecisionType;
        public int DecisionValue;
    }

    public enum AICharacterDecisionType
    {
        Move,
        Attack,
        CastSpell,
        Wait,
        Defense,
        Retreat,
        Scout
    }
}