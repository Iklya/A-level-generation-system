using System.Collections.Generic;
using UnityEngine;

namespace example.Scripts.RoomScripts
{
    public class RoomGenerationData
    {
        public List<ConnectionDirection> ConnectionDirections;
        public RoomTypes RoomType;
        public Vector2Int RoomPosition;
        public RoomContainer RoomContainer;
    }
}