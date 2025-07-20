using System.Collections.Generic;
using UnityEngine;

public class DirectionManager
{
    public int GetDirection((int x, int y) fromRoom, (int x, int y) toRoom)
    {
        if (toRoom.x < fromRoom.x)
            return 0;
        if (toRoom.x > fromRoom.x)
            return 1;
        if (toRoom.y > fromRoom.y)
            return 2;
        return 3;
    }

    public int GetOppositeDirection(int dir)
    {
        if (dir == 0)
            return 1;
        if (dir == 1)
            return 0;
        if (dir == 2)
            return 3;
        return 2;
    }

    public List<int> SelectNextDirections(MatrixManager matrixManager)
    {
        List<int> avaliableDoors = new List<int>();

        if (matrixManager.GetCell(matrixManager.CurrentX - 1, matrixManager.CurrentY) == -1) avaliableDoors.Add(0);
        if (matrixManager.GetCell(matrixManager.CurrentX + 1, matrixManager.CurrentY) == -1) avaliableDoors.Add(1);
        if (matrixManager.GetCell(matrixManager.CurrentX, matrixManager.CurrentY + 1) == -1) avaliableDoors.Add(2);
        if (matrixManager.GetCell(matrixManager.CurrentX, matrixManager.CurrentY - 1) == -1) avaliableDoors.Add(3);

        return avaliableDoors;
    }
}


