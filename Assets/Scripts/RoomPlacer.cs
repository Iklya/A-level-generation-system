using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RoomPlacer
{
    private MatrixManager matrixManager;
    private GameObject roomPrefab;
    private Vector2 roomSize;
    public Sprite[] doorSprites;
    private int N;

    private List<(int x, int y, GameObject room)> mainPath = new List<(int x, int y, GameObject room)>();

    public RoomPlacer(MatrixManager matrixManager, GameObject roomPrefab, Vector2 roomSize, Sprite[] doorSprites, int N)
    {
        this.matrixManager = matrixManager;
        this.roomPrefab = roomPrefab;
        this.roomSize = roomSize;
        this.doorSprites = doorSprites;
        this.N = N;
    }

    public void GenerateRooms()
    {
        GenerateMatrixMainRooms();
        GenerateGameMainRooms();
    }

    public void GenerateMatrixMainRooms()
    {
        matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 1);
        mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null));

        for (int i = 1; i < N; i++)
        {
            bool isLastRoom = (i == N - 1);
            int nextDirection = ChooseNextDirection(matrixManager.CurrentX, matrixManager.CurrentY);

            if (nextDirection == 404)
            {
                mainPath.Clear();

                for (int x = 0; x < matrixManager.MatrixSize; x++)
                    for (int y = 0; y < matrixManager.MatrixSize; y++)
                        matrixManager.Matrix[x, y] = -1;

                matrixManager.CurrentX = matrixManager.CenterX;
                matrixManager.CurrentY = matrixManager.CenterY;

                GenerateMatrixMainRooms();
                return;
            }

            MoveToNextCell(nextDirection);
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, isLastRoom ? 2 : 0);
            mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null));
        }
    }

    public void GenerateGameMainRooms()
    {
        for (int r = 0; r < mainPath.Count; r++)
        {
            GameObject room = InstantiateRoom(mainPath[r].x, mainPath[r].y);

            room.transform.SetParent(GameObject.Find("MainPathRooms").transform);

            mainPath[r] = (mainPath[r].x, mainPath[r].y, room);
        }


        for (int i = 0; i < mainPath.Count - 1; i++)
        {
            GameObject currRoom = mainPath[i].room;
            GameObject nextRoom = mainPath[i + 1].room;

            int direction = GetDirection((mainPath[i].x, mainPath[i].y), (mainPath[i + 1].x, mainPath[i + 1].y));

            PlaceDoor(currRoom, direction);
            PlaceDoor(nextRoom, GetOppositeDirection(direction));
            
        }

        Debug.Log($"Создалось: {GameObject.Find("MainPathRooms").transform.childCount} комнат");
    }

    private int GetDirection((int x, int y) fromRoom, (int x, int y)  toRoom)
    {
        if (toRoom.x < fromRoom.x)
            return 0;
        if (toRoom.x > fromRoom.x)
            return 1;
        if (toRoom.y > fromRoom.y)
            return 2;
        return 3;
    }

    private int GetOppositeDirection(int dir)
    {
        if (dir == 0)
            return 1;
        if (dir == 1)
            return 0;
        if (dir == 2)
            return 3;
        return 2;
    }

    private int ChooseNextDirection(int curX, int curY)
    {
        List<int> avaliableDoors = new List<int>();

        if (matrixManager.GetCell(curX - 1, curY) == -1) avaliableDoors.Add(0);
        if (matrixManager.GetCell(curX + 1, curY) == -1) avaliableDoors.Add(1);
        if (matrixManager.GetCell(curX, curY + 1) == -1) avaliableDoors.Add(2);
        if (matrixManager.GetCell(curX, curY - 1) == -1) avaliableDoors.Add(3);

        if (avaliableDoors.Count == 0)
            return 404;

        return avaliableDoors[Random.Range(0, avaliableDoors.Count)];
    }

    private void PlaceDoor(GameObject room, int dir)
    {
        Transform doorPlacement = null;

        switch (dir)
        {
            case 0:
                Transform wallsLeft = room.transform.Find("WallsLeft");
                doorPlacement = wallsLeft.transform.Find("doorPlacement");
                break;
            case 1:
                Transform wallsRight = room.transform.Find("WallsRight");
                doorPlacement = wallsRight.transform.Find("doorPlacement");
                break;
            case 2:
                Transform wallsUp = room.transform.Find("WallsUp");
                doorPlacement = wallsUp.transform.Find("doorPlacement");
                break;
            case 3:
                Transform wallsDown = room.transform.Find("WallsDown");
                doorPlacement = wallsDown.transform.Find("doorPlacement");
                break;
        }

        ReplaceDoorSprite(doorPlacement, dir);
    }

    private void MoveToNextCell(int dir)
    {
        switch (dir)
        {
            case 0:
                matrixManager.CurrentX -= 1;
                break;
            case 1:
                matrixManager.CurrentX += 1;
                break;
            case 2:
                matrixManager.CurrentY += 1;
                break;
            case 3:
                matrixManager.CurrentY -= 1;
                break;
        }
    }

    private void ReplaceDoorSprite(Transform doorPlacement, int door)
    {
        SpriteRenderer spriteRenderer = doorPlacement.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorSprites[door];
    }
    private GameObject InstantiateRoom(int x, int y)
    {
        Vector2 gamePosition = MatrixToGame(x, y);
        return Object.Instantiate(roomPrefab, gamePosition, Quaternion.identity);
    }

    private Vector2 MatrixToGame(int x, int y)
    {
        float gameX = (x - matrixManager.CenterX) * roomSize.x;
        float gameY = (y - matrixManager.CenterY) * roomSize.y;
        return new Vector2(gameX, gameY);
    }

}
