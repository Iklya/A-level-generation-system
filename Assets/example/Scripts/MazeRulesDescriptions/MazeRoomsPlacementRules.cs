using UnityEngine;

namespace example.Scripts.MazeRulesDescriptions
{
    [CreateAssetMenu(fileName = "MazeRoomsPlacementRules", menuName = "Scriptable Objects/MazeRoomsPlacementRules")]
    public class MazeRoomsPlacementRules : ScriptableObject
    {
        public MazePlacementDifficultyRule[] MazeDifficultyRules;
    }
}