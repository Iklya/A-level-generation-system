using System.Collections.Generic;
using UnityEngine;

public class DirectionManager
{
    public Direction GetDirection((int x, int y) fromRoom, (int x, int y) toRoom)
    {
        if (toRoom.x < fromRoom.x)
            return Direction.Left;
        if (toRoom.x > fromRoom.x)
            return Direction.Right;
        if (toRoom.y > fromRoom.y)
            return Direction.Up;
        return Direction.Down;
    }

    public Direction GetOppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Left: 
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            default: return Direction.Up;
        }
    }

    public List<Direction> SelectNextDirections(MatrixManager matrixManager)
    {
        var avaliableDoors = new List<Direction>();

        if (matrixManager.GetCell(matrixManager.CurrentX - 1, matrixManager.CurrentY) == (int)RoomType.Empty) avaliableDoors.Add(Direction.Left);
        if (matrixManager.GetCell(matrixManager.CurrentX + 1, matrixManager.CurrentY) == (int)RoomType.Empty) avaliableDoors.Add(Direction.Right);
        if (matrixManager.GetCell(matrixManager.CurrentX, matrixManager.CurrentY + 1) == (int)RoomType.Empty) avaliableDoors.Add(Direction.Up);
        if (matrixManager.GetCell(matrixManager.CurrentX, matrixManager.CurrentY - 1) == (int)RoomType.Empty) avaliableDoors.Add(Direction.Down);

        return avaliableDoors;
    }
}


