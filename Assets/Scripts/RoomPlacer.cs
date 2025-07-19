using UnityEngine;
using System.Collections.Generic;


public class RoomPlacer
{
    private MatrixManager matrixManager;
    private GameObject[] roomPrefabs;
    private Vector2 roomSize;
    private Sprite[] doorSprites;
    private int N;
    private RoomChance[] roomChances;
    public CoridorDoorChance[] coridorDoorChances;

    private List<(int x, int y, GameObject room, int roomType)> mainPath = new List<(int x, int y, GameObject room, int roomType)>();

    // ключ - индекс коридора, значение - список комнат доп пути
    private Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>> extraPaths = new Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>>();

    public RoomPlacer(MatrixManager matrixManager, GameObject[] roomPrefabs, Vector2 roomSize, Sprite[] doorSprites, int N, RoomChance[] roomChances, CoridorDoorChance[] coridorDoorChances)
    {
        this.matrixManager = matrixManager;
        this.roomPrefabs = roomPrefabs;
        this.roomSize = roomSize;
        this.doorSprites = doorSprites;
        this.N = N;
        this.roomChances = roomChances;
        this.coridorDoorChances = coridorDoorChances;
    }

    public void GenerateRooms()
    {
        GenerateMatrixMainRooms();
        GenerateGameMainRooms();
    }

    private void GenerateMatrixMainRooms()
    {
        matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 1);
        mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, 1));

        for (int i = 1; i < N; i++)
        {
            bool isLastRoom = (i == N - 1);
            List<int> nextDirections = SelectNextDirections(matrixManager.CurrentX, matrixManager.CurrentY);

            if (nextDirections.Count == 0)
            {
                mainPath.Clear();

                matrixManager = new MatrixManager(N);

                GenerateMatrixMainRooms();
                return;
            }

            int nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];
            MoveToNextCell(nextDirection);
            int curRoomType = isLastRoom ? 2 : 0;
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
            mainPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));
        }
    }

    private void GenerateGameMainRooms()
    {
        for (int r = 0; r < N; r++)
        {
            InstantiateMainRooms(mainPath, r);

            mainPath[r].room.transform.SetParent(GameObject.Find("MainPathRooms").transform);

            if (mainPath[r].roomType == 3)
            {
                int curDoorAmount = GetCoridorDoorsAmount();
                if (curDoorAmount > 2)
                {
                    // проверяем сколько можно доп. путей построить от текущего коридора

                    List<int> availableDirections = SelectNextDirections(mainPath[r].x, mainPath[r].y);
                    
                    curDoorAmount = Mathf.Min(curDoorAmount - 2, availableDirections.Count);

                    if (curDoorAmount > 0)
                    {
                        int S = N - (r + 1);

                        GenerateMatrixExtraRooms(extraPaths, r, S, curDoorAmount, availableDirections);
                        GenerateGameExtraRooms(extraPaths, r);
                    }
                }
            }
        }


        for (int i = 0; i < N - 1; i++)
        {
            GameObject currRoom = mainPath[i].room;
            GameObject nextRoom = mainPath[i + 1].room;

            int direction = GetDirection((mainPath[i].x, mainPath[i].y), (mainPath[i + 1].x, mainPath[i + 1].y));

            PlaceDoor(currRoom, direction);
            PlaceDoor(nextRoom, GetOppositeDirection(direction));
        }

        Debug.Log($"Создалось: {GameObject.Find("MainPathRooms").transform.childCount} комнат основного пути");
    }

    private void GenerateMatrixExtraRooms(Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>> extraPaths, int coridorIdx, int S, int curDoorAmount, List<int> availableDirections)
    {
        for (int i = 0; i < curDoorAmount; i++)
        {
            matrixManager.CurrentX = mainPath[coridorIdx].x;
            matrixManager.CurrentY = mainPath[coridorIdx].y;

            int curDirection = availableDirections[i];

            int k = Random.Range(1, S + 1) - 1; // -1, ибо первую комнату доп. пути расположим ещё до цикла

            Debug.Log($"{coridorIdx} - индекс коридора осн пути с 3+ дверьми, длина доп. пути - {k + 1}");

            List<(int x, int y, GameObject room, int roomType)> curPath = new List<(int x, int y, GameObject room, int roomType)>();

            MoveToNextCell(curDirection);

            int curRoomType = (k == 0) ? 6 : GetRandomRoomType();
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
            curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));

            for (int j = 0; j < k; j++) 
            {
                bool isQuestRoom = (j == k - 1);
                List<int> nextDirections = SelectNextDirections(matrixManager.CurrentX, matrixManager.CurrentY);

                if (nextDirections.Count == 0)
                {
                    matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 6);
                    curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, 6));
                    break;
                }

                int nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];

                MoveToNextCell(nextDirection);
                curRoomType = isQuestRoom ? 6 : GetRandomRoomType();
                matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
                curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));
            }

            extraPaths[(coridorIdx, curDirection)] = curPath;
        }
    }

    private void GenerateGameExtraRooms(Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>> extraPaths, int coridorIdx)
    {
        foreach (var kv in extraPaths)
        {
            if (kv.Key.Item1 != coridorIdx)
                continue;

            int prevDirection = kv.Key.Item2;
            GameObject prevRoom = mainPath[coridorIdx].room;

            List<(int x, int y, GameObject room, int roomType)> extraPath = kv.Value;

            // задаём имя контейнеру (можно в отдельную функцию потом тоже)
            string parentName = $"ExtraPath_{prevDirection}";
            GameObject containerObj = new GameObject(parentName);
            containerObj.transform.SetParent(prevRoom.transform);

            // инстанцирование комнат
            for (int r = 0; r < extraPath.Count; r++)
            {
                InstantiateExtraRooms(extraPath, r);

                extraPath[r].room.transform.SetParent(containerObj.transform);
            }

            PlaceDoor(prevRoom, prevDirection);
            PlaceDoor(extraPath[0].room, GetOppositeDirection(prevDirection));

            // наполнение дверьми
            for (int i = 0; i < kv.Value.Count - 1; i++)
            {
                GameObject currRoom = extraPath[i].room;
                GameObject nextRoom = extraPath[i + 1].room;

                int direction = GetDirection((extraPath[i].x, extraPath[i].y), (extraPath[i + 1].x, extraPath[i + 1].y));

                PlaceDoor(currRoom, direction);
                PlaceDoor(nextRoom, GetOppositeDirection(direction));
            }
        }
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

    private List<int> SelectNextDirections(int curX, int curY)
    {
        List<int> avaliableDoors = new List<int>();

        if (matrixManager.GetCell(curX - 1, curY) == -1) avaliableDoors.Add(0);
        if (matrixManager.GetCell(curX + 1, curY) == -1) avaliableDoors.Add(1);
        if (matrixManager.GetCell(curX, curY + 1) == -1) avaliableDoors.Add(2);
        if (matrixManager.GetCell(curX, curY - 1) == -1) avaliableDoors.Add(3);

        return avaliableDoors;
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

    private int GetRandomRoomType()
    {
        float currChance = Random.Range(0f, 100f);
        
        float totalChance = 0f;

        foreach (var roomChance in roomChances)
        {
            totalChance += roomChance.chance;
            if (totalChance > currChance)
                return roomChance.roomType;
        }

        return -1;
    }

    private int GetCoridorDoorsAmount()
    {
        float currChance = Random.Range(0f, 100f);
        float totalChance = 0f;

        foreach (var doorChance in coridorDoorChances)
        {
            totalChance += doorChance.chance;
            if (currChance <= totalChance)
                return doorChance.doorAmount;
        }

        return 2;
    }

    private void InstantiateMainRooms(List<(int x, int y, GameObject room, int roomType)> mainPath, int r)
    {
        GameObject roomObj = null;
        int curType = 0;

        Vector2 gamePosition = matrixManager.MatrixToGame(mainPath[r].x, mainPath[r].y, roomSize);


        switch (matrixManager.GetCell(mainPath[r].x, mainPath[r].y))
        {
            case 1:
                curType = 1;
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
            case 2:
                curType = 2;
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
            default:
                curType = GetRandomRoomType();
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
        }

        mainPath[r] = (mainPath[r].x, mainPath[r].y, roomObj, curType);
        matrixManager.SetCell(mainPath[r].x, mainPath[r].y, curType);
    }

    private void InstantiateExtraRooms(List<(int x, int y, GameObject room, int roomType)> extraPath, int r)
    {
        GameObject roomObj = null;
        int curType = matrixManager.GetCell(extraPath[r].x, extraPath[r].y);

        Vector2 gamePosition = matrixManager.MatrixToGame(extraPath[r].x, extraPath[r].y, roomSize);

        roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);

        extraPath[r] = (extraPath[r].x, extraPath[r].y, roomObj, curType);
    }
}
