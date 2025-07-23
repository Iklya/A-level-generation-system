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

    private List<RoomData> mainPath = new List<RoomData>();

    // ключ - индекс коридора, значение - список из направления доп. пути + списка комнат доп пути
    private Dictionary<int, List<(Direction Direction, List<RoomData> Path)>> extraPaths = new();

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
        matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, (int)RoomType.Start);
        mainPath.Add(new RoomData(matrixManager.CurrentX, matrixManager.CurrentY, null, RoomType.Start));

        for (int i = 1; i < N; i++)
        {
            bool isLastRoom = (i == N - 1);
            List<Direction> nextDirections = directionManager.SelectNextDirections(matrixManager);

            if (nextDirections.Count == 0)
            {
                Debug.Log($"new PATH");
                mainPath.Clear();
                matrixManager.Reset();
                GenerateMatrixMainRooms();
                return;
            }

            Direction nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];
            matrixManager.MoveToNextCell(nextDirection);
            RoomType curRoomType = isLastRoom ? RoomType.End : RoomType.Reserve;
            matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, (int)curRoomType);
            mainPath.Add(new RoomData(matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));
        }
    }

    private void GenerateGameMainRooms()
    {
        for (int r = 0; r < N; r++)
        {
            (GameObject room, RoomType curType) = roomInstantiator.InstantiateMainRooms(mainPath, r);

            mainPath[r] = new RoomData(mainPath[r].X, mainPath[r].Y, room, curType);

            mainPath[r].Room.transform.SetParent(GameObject.Find("MainPathRooms").transform);

            if (r > 0)
            {
                // Сразу ставим дверь между текущей и предыдущей комнатой
                Direction direction = directionManager.GetDirection(
                    (mainPath[r - 1].X, mainPath[r - 1].Y),
                    (mainPath[r].X, mainPath[r].Y)
                );

                ConnectRooms(mainPath[r - 1].Room, mainPath[r].Room, direction);
            }

            if (mainPath[r].RoomType == RoomType.Corridor)
            {
                int curDoorAmount = doorAmountSelector.GetCoridorDoorsAmount();
                if (curDoorAmount > 2)
                {
                    Debug.Log($"{curDoorAmount} - доступно дверей для размещения в текущем коридоре");

                    int S = N - (r + 1);

                    GenerateMatrixExtraRooms(r, S, curDoorAmount);
                    GenerateGameExtraRooms(r);
                }
            }
        }

        Debug.Log($"Создалось: {GameObject.Find("MainPathRooms").transform.childCount} комнат основного пути");
    }

    private void GenerateMatrixExtraRooms(int corridorIdx, int S, int curDoorAmount)
    {
        if (!extraPaths.ContainsKey(corridorIdx))
            extraPaths[corridorIdx] = new List<(Direction, List<RoomData>)>();

        for (int i = 0; i < curDoorAmount; i++)
        {
            matrixManager.CurrentX = mainPath[corridorIdx].X;
            matrixManager.CurrentY = mainPath[corridorIdx].Y;

            int k = Random.Range(1, S + 1);

            Debug.Log($"{corridorIdx} - индекс коридора осн пути с 3+ дверьми, длина доп. пути - {k}");

            List<RoomData> curPath = new List<RoomData>();

            List<Direction> availableDirections = directionManager.SelectNextDirections(matrixManager);
            if (availableDirections.Count == 0)
            {
                Debug.LogWarning($"No available directions for main room #{corridorIdx + 1}'s extra path");
                continue;
            }

            Direction curDirection = availableDirections[Random.Range(0, availableDirections.Count)];

            for (int j = 0; j < k; j++) 
            {
                bool isQuestRoom = (j == k - 1);
                List<Direction> nextDirections = directionManager.SelectNextDirections(matrixManager);

                if (nextDirections.Count == 0)
                {
                    matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, 6);
                    if (curPath.Count > 0)
                        curPath[curPath.Count - 1] = new RoomData(matrixManager.CurrentX, matrixManager.CurrentY, null, RoomType.Quest);
                    else
                        curPath.Add(new RoomData(matrixManager.CurrentX, matrixManager.CurrentY, null, RoomType.Quest));
                    break;
                }

                Direction nextDirection = nextDirections[Random.Range(0, nextDirections.Count)];
                matrixManager.MoveToNextCell(nextDirection);

                RoomType curRoomType = isQuestRoom ? RoomType.Quest : (RoomType)roomTypeSelector.GetRandomRoomType();
                matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, (int)curRoomType);

                curPath.Add(new RoomData(matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));

                if (isQuestRoom)
                    break;
            }

            extraPaths[corridorIdx].Add((curDirection, curPath));
        }
    }

    private void GenerateGameExtraRooms(int corridorIdx)
    {
        if (!extraPaths.ContainsKey(corridorIdx))
            return;

        GameObject previousRoom = mainPath[corridorIdx].Room;

        foreach (var extra in extraPaths[corridorIdx])
        {
            Direction previousDirection = extra.Direction;

            List<RoomData> extraPath = extra.Path;

            // задаём имя контейнеру (можно в отдельную функцию потом тоже, но есть ли смысл)
            string parentName = $"ExtraPath_{previousDirection}";
            GameObject containerObj = new GameObject(parentName);
            containerObj.transform.SetParent(previousRoom.transform);

            for (int r = 0; r < extraPath.Count; r++)
            {
                roomInstantiator.InstantiateExtraRooms(extraPath, r);
                extraPath[r].Room.transform.SetParent(containerObj.transform);
            }

            ConnectRooms(previousRoom, extraPath[0].Room, previousDirection);

            for (int i = 0; i < extraPath.Count - 1; i++)
            {
                GameObject currRoom = extraPath[i].Room;
                GameObject nextRoom = extraPath[i + 1].Room;

                Direction direction = directionManager.GetDirection((extraPath[i].X, extraPath[i].Y),
                                                              (extraPath[i + 1].X, extraPath[i + 1].Y));

                ConnectRooms(currRoom, nextRoom, direction);
            }
        }
    }

    private void ConnectRooms(GameObject roomA, GameObject roomB, Direction direction)
    {
        doorPlacer.PlaceDoor(roomA, direction);
        doorPlacer.PlaceDoor(roomB, directionManager.GetOppositeDirection(direction));
    }
    public void Clear()
    {
        mainPath.Clear();
        extraPaths.Clear();
    }
}
