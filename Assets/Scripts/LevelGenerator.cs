using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(2, 100)]
    public int N;
    public GameObject roomPrefab; // потом заменю на GameObject[] типов комнат
    public Sprite[] doorSprites; // Пусть: 0 - left, 1 - right, 2 - up, 3 - down

    private Vector2 roomSize;

    private MatrixManager matrixManager;
    private RoomPlacer roomPlacer;
    private RoomSizeCalculator roomSizeCalculator;

    void Start()
    {
        roomSizeCalculator = new RoomSizeCalculator(roomPrefab);
        roomSize = roomSizeCalculator.RoomSizeCalculation(roomPrefab);

        matrixManager = new MatrixManager(N);

        roomPlacer = new RoomPlacer(matrixManager, roomPrefab, roomSize, doorSprites);

        roomPlacer.PlaceStartRoom();

        // тут размещу остальные комнаты по порядку
        // тут задам тип конечной комнате
    }
}
