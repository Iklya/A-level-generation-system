using System.Collections.Generic;
using UnityEngine;

namespace example
{
    public class RoomHandler
    {
        public RoomGenerationData RoomData;
        public MazeGenerationDescription _mazeGenerationDescription;

        private int _currentConnectionCount;
        private Dictionary<RoomContainer, ConnectionDirection> _currentConnections;

        public void AddConnection(RoomContainer roomContainer, ConnectionDirection direction)
        {
            RoomData.ConnectionDirections.Add(direction);
            _currentConnections.Add(roomContainer, direction);
        }

        public void SetConnectionCount(int count)
        {
            _currentConnectionCount = count;
        }

        public bool CheckConnectionCount(int count)
        {
            return _currentConnections.Count == _currentConnectionCount;
        }

        public void InstantiateRoom()
        {
            var room = Object.Instantiate(RoomData.RoomContainer);
            InitializeRoom(room);
        }

        public void InitializeRoom(RoomContainer roomContainer)
        {
            roomContainer.RoomTransform.position = 
                new Vector3(RoomData.RoomPosition.x * _mazeGenerationDescription.RoomOffsetValue, 
                    RoomData.RoomPosition.y * _mazeGenerationDescription.RoomOffsetValue, 0);
            foreach (var currentConnection in _currentConnections)
            {
                roomContainer.DoorEnable((int)currentConnection.Value);
            }
        }
    }
}