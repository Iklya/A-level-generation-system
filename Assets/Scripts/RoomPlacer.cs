using UnityEngine;
using System.Collections.Generic;
using example;


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

    // ���� - ������ �������� + ����������� ���. ����, �������� - ������ ������ ��� ����
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
    public RoomContainer StartRoom;

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
                Debug.Log($"new PATH");
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
    
    private void GenerateGameMainRooms()
    {
        for (int r = 0; r < N; r++)
        {
            roomInstantiator.InstantiateMainRooms(mainPath, r);

            mainPath[r].room.transform.SetParent(GameObject.Find("MainPathRooms").transform);

            if (r > 0)
            {
                // ����� ������ ����� ����� ������� � ���������� ��������
                int direction = directionManager.GetDirection(
                    (mainPath[r - 1].x, mainPath[r - 1].y),
                    (mainPath[r].x, mainPath[r].y)
                );

                doorPlacer.PlaceDoor(mainPath[r - 1].room, direction);
                doorPlacer.PlaceDoor(mainPath[r].room, directionManager.GetOppositeDirection(direction));
            }

            if (mainPath[r].roomType == 3)
            {
                int curDoorAmount = doorAmountSelector.GetCoridorDoorsAmount();
                if (curDoorAmount > 2)
                {
                    // ��������� ������� ����� ���. ����� ��������� �� �������� ��������

                    List<int> availableDirections = directionManager.SelectNextDirections(matrixManager);
                    curDoorAmount = Mathf.Min(curDoorAmount - 2, availableDirections.Count);
                    Debug.Log($"{curDoorAmount} - �������� ������ ��� ���������� � ������� ��������");

                    if (curDoorAmount > 0)
                    {
                        int S = N - (r + 1);

                        GenerateMatrixExtraRooms(extraPaths, r, S, curDoorAmount);
                        GenerateGameExtraRooms(extraPaths, r);
                    }
                }
            }
        }

        Debug.Log($"���������: {GameObject.Find("MainPathRooms").transform.childCount} ������ ��������� ����");
    }

    private void GenerateMatrixExtraRooms(Dictionary<(int, int), List<(int x, int y, GameObject room, int roomType)>> extraPaths, int coridorIdx, int S, int curDoorAmount)
    {
        for (int i = 0; i < curDoorAmount; i++)
        {
            matrixManager.CurrentX = mainPath[coridorIdx].x;
            matrixManager.CurrentY = mainPath[coridorIdx].y;

            int k = Random.Range(1, S + 1);

            Debug.Log($"{coridorIdx} - ������ �������� ��� ���� � 3+ �������, ����� ���. ���� - {k}");

            List<(int x, int y, GameObject room, int roomType)> curPath = new List<(int x, int y, GameObject room, int roomType)>();

            int curDirection = -1;

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

                if (j == 0)
                    curDirection = nextDirection;

                matrixManager.MoveToNextCell(nextDirection);

                int curRoomType = isQuestRoom ? 6 : roomTypeSelector.GetRandomRoomType();
                matrixManager.SetCell(matrixManager.CurrentX, matrixManager.CurrentY, curRoomType);
                curPath.Add((matrixManager.CurrentX, matrixManager.CurrentY, null, curRoomType));

                if (isQuestRoom)
                    break;
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

            // ����� ��� ���������� (����� � ��������� ������� ����� ����, �� ���� �� �����)
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
