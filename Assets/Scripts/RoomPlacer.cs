using UnityEngine;
using System.Collections.Generic;


public class RoomPlacer
{
    private MatrixManager matrixManager;
    private DirectionManager directionManager;
    private DoorPlacer doorPlacer;
    private RoomTypeSelector roomTypeSelector;
    private RoomInstantiator roomInstantiator;
    private DoorAmountSelector doorAmountSelector;

    private GameObject[] roomPrefabs;
    private Vector2 roomSize;
    private Sprite[] doorSprites;
    private int N;
    private RoomChance[] roomChances;
    public CoridorDoorChance[] coridorDoorChances;

    private List<(int x, int y, GameObject room, int roomType)> mainPath = new List<(int x, int y, GameObject room, int roomType)>();

    // ключ - индекс коридора + направление доп. пути, значение - список комнат доп пути
    private Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>> extraPaths = new Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>>();

    public RoomPlacer(MatrixManager matrixManager, GameObject[] roomPrefabs, Vector2 roomSize,
                      Sprite[] doorSprites, int N, RoomChance[] roomChances, CoridorDoorChance[] coridorDoorChances,
                      RoomTypeSelector roomTypeSelector, RoomInstantiator roomInstantiator, DoorAmountSelector doorAmountSelector)
    {
        this.matrixManager = matrixManager;
        doorPlacer = new DoorPlacer(doorSprites);
        this.roomTypeSelector = roomTypeSelector;
        this.roomInstantiator = roomInstantiator;
        directionManager = new DirectionManager();
        this.roomPrefabs = roomPrefabs;
        this.roomSize = roomSize;
        this.doorSprites = doorSprites;
        this.N = N;
        this.roomChances = roomChances;
        this.coridorDoorChances = coridorDoorChances;
        this.doorAmountSelector = doorAmountSelector;
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
            List<int> nextDirections = directionManager.SelectNextDirections(matrixManager);

            if (nextDirections.Count == 0)
            {
                mainPath.Clear();

                matrixManager = new MatrixManager(N);

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

    private void GenerateGameMainRooms()
    {
        for (int r = 0; r < N; r++)
        {
            roomInstantiator.InstantiateMainRooms(mainPath, r);

            mainPath[r].room.transform.SetParent(GameObject.Find("MainPathRooms").transform);

            if (mainPath[r].roomType == 3)
            {
                int curDoorAmount = doorAmountSelector.GetCoridorDoorsAmount();
                if (curDoorAmount > 2)
                {
                    // проверяем сколько можно доп. путей построить от текущего коридора

                    List<int> availableDirections = directionManager.SelectNextDirections(matrixManager);
                    
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

            int direction = directionManager.GetDirection((mainPath[i].x, mainPath[i].y), (mainPath[i + 1].x, mainPath[i + 1].y));

            doorPlacer.PlaceDoor(currRoom, direction);
            doorPlacer.PlaceDoor(nextRoom, directionManager.GetOppositeDirection(direction));
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

            matrixManager.MoveToNextCell(curDirection);

            int curRoomType = (k == 0) ? 6 : roomTypeSelector.GetRandomRoomType();
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
            curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));

            for (int j = 0; j < k; j++) 
            {
                bool isQuestRoom = (j == k - 1);
                List<int> nextDirections = directionManager.SelectNextDirections(matrixManager);

                if (nextDirections.Count == 0)
                {
                    matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 6);
                    curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, 6));
                    break;
                }

                int nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];

                matrixManager.MoveToNextCell(nextDirection);
                curRoomType = isQuestRoom ? 6 : roomTypeSelector.GetRandomRoomType();
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

            // задаём имя контейнеру (можно в отдельную функцию потом тоже, но есть ли смысл)
            string parentName = $"ExtraPath_{prevDirection}";
            GameObject containerObj = new GameObject(parentName);
            containerObj.transform.SetParent(prevRoom.transform);

            for (int r = 0; r < extraPath.Count; r++)
            {
                roomInstantiator.InstantiateExtraRooms(extraPath, r);

                extraPath[r].room.transform.SetParent(containerObj.transform);
            }

            doorPlacer.PlaceDoor(prevRoom, prevDirection);
            doorPlacer.PlaceDoor(extraPath[0].room, directionManager.GetOppositeDirection(prevDirection));

            for (int i = 0; i < kv.Value.Count - 1; i++)
            {
                GameObject currRoom = extraPath[i].room;
                GameObject nextRoom = extraPath[i + 1].room;

                int direction = directionManager.GetDirection((extraPath[i].x, extraPath[i].y), (extraPath[i + 1].x, extraPath[i + 1].y));

                doorPlacer.PlaceDoor(currRoom, direction);
                doorPlacer.PlaceDoor(nextRoom, directionManager.GetOppositeDirection(direction));
            }
        }
    }


}
