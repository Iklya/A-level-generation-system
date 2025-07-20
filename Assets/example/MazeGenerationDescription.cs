using UnityEngine;

namespace example
{
    [CreateAssetMenu(menuName = "Create MazeGenerationDescription", fileName = "MazeGenerationDescription", order = 0)]
    public class MazeGenerationDescription : ScriptableObject
    {
        public float RoomOffsetValue;
    }
}
