using example.Scripts.MazeGeneratorScripts;
using example.Scripts.MazeRulesDescriptions;
using UnityEngine;

namespace example.Scripts
{
    public class DummyGameController : MonoBehaviour
    {
        public MazeRoomsPlacementRules MazeGeneratorRules;
        public int CurrentDifficulty;
        
        private MazeGeneratorController _mazeGeneratorController;
        
        private void Start()
        {
            _mazeGeneratorController = new MazeGeneratorController();
        }

        [ContextMenu("Generate")]
        private void GenerateMaze()
        {
            _mazeGeneratorController.SetCurrentMazeRules(MazeGeneratorRules.MazeDifficultyRules[CurrentDifficulty]);
            _mazeGeneratorController.GenerateMaze();
        }
    }
}
