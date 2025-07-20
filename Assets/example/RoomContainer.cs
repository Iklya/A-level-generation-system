using System.Collections.Generic;
using example;
using UnityEngine;

namespace example
{
    public class RoomContainer : MonoBehaviour
    {
        public Transform RoomTransform;
        public GameObject RoomGameObject;
        public Transform[] RoomAdditionalItemsPlaces;
        public Transform[] ConnectPoints;
        public RoomTypes RoomType;
        public SpriteRenderer[] DoorSprites;

        public void DoorEnable(int doorId)
        {
            DoorSprites[doorId].enabled = true;
        }
    }
}

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
        room.RoomTransform.position = 
            new Vector3(RoomData.RoomPosition.x * _mazeGenerationDescription.RoomOffsetValue, 
                RoomData.RoomPosition.y * _mazeGenerationDescription.RoomOffsetValue, 0);
        foreach (var currentConnection in _currentConnections)
        {
            room.DoorEnable((int)currentConnection.Value);
        }
        
    }
}

public class RoomGenerationData
{
    public List<ConnectionDirection> ConnectionDirections;
    public RoomTypes RoomType;
    public Vector2Int RoomPosition;
    public RoomContainer RoomContainer;
} 

public enum RoomTypes
{
    Empty = -1,
    Start = 1,
    Coridor = 2,
    Fight = 3,
    Rest = 4,
    End = 5,
    Quest = 6
}

