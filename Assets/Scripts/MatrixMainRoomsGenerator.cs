using System.Collections.Generic;
using UnityEngine;

public class MatrixMainRoomsGenerator
{
    private int N;
    private MatrixManager matrixManager;
    private List<(int x, int y, GameObject room, int roomType)> mainPath;
    private DirectionManager directionManager;

    public MatrixMainRoomsGenerator(int n, MatrixManager matrixManager, List<(int x, int y, GameObject room, int roomType)> mainPath, DirectionManager directionManager)
    {
        N = n;
        this.matrixManager = matrixManager;
        this.mainPath = mainPath;
        this.directionManager = directionManager;
    }

    public void GenerateMatrixMainRooms()
    {
        matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 1);
        mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, 1));

        for (int i = 1; i < N; i++)
        {
            bool isLastRoom = (i == N - 1);
            List<int> nextDirections = directionManager.SelectNextDirections(matrixManager);

            if (nextDirections.Count == 0)
            {
                mainPath.Clear();

                matrixManager.Reset();

                GenerateMatrixMainRooms();
                return;
            }

            int nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];
            matrixManager.MoveToNextCell(nextDirection);
            int curRoomType = isLastRoom ? 2 : 0;
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
            mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));
        }
    }
}
