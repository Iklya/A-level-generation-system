using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int N;
    public GameObject roomPrefab;

    private Vector2 roomSize;
    private MatrixManager matrixManager;
    private RoomPlacer roomPlacer;

    void Start()
    {
        roomSize = CalculateRoomSize(roomPrefab);

        matrixManager = new MatrixManager(N);

        roomPlacer = new RoomPlacer(matrixManager, roomPrefab, roomSize);

        roomPlacer.PlaceStartRoom();
        // тут размещу остальные комнаты по пор€дку
        // тут задам тип конечной комнате
    }

    private Vector2 CalculateRoomSize(GameObject roomPrefab)
    {
        Transform wallsUp = roomPrefab.transform.Find("WallsUp");
        Transform wallUpPrefab = wallsUp.transform.Find("wallUpPrefab");

        Transform wallsDown = roomPrefab.transform.Find("WallsDown");
        Transform wallDownPrefab = wallsDown.transform.Find("wallDownPrefab");

        Transform wallsLeft = roomPrefab.transform.Find("WallsLeft");
        Transform wallLeftPrefab = wallsLeft.transform.Find("wallLeftPrefab");

        Transform wallsRight = roomPrefab.transform.Find("WallsRight");
        Transform wallRightPrefab = wallsRight.transform.Find("wallRightPrefab");


        float width = Mathf.Abs(wallLeftPrefab.position.x - wallRightPrefab.position.x);
        float height = Mathf.Abs(wallUpPrefab.position.y - wallDownPrefab.position.y);

        Debug.Log($"–азмер комнаты: {width}x{height}");

        return new Vector2(width, height);
    }
}
