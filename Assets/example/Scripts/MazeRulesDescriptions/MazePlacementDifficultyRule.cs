using System.Collections.Generic;
using UnityEngine;

namespace example.Scripts.MazeRulesDescriptions
{
    [System.Serializable]
    public class MazePlacementDifficultyRule
    {
        public int Difficulty;
        public Vector2Int MainPathMinMaxSteps;
        public List<MazeRoomsPlacementRule> RoomsPlacementRules;
    }
}