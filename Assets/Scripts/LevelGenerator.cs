using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(2, 100)]
    public int N;
    public GameObject[] roomPrefabs;
    public Sprite[] doorSprites; // Пусть: 0 - left, 1 - right, 2 - up, 3 - down

    private Vector2 roomSize;

    private MatrixManager matrixManager;
    private RoomPlacer roomPlacer;
    private RoomSizeCalculator roomSizeCalculator;

    public RoomChance[] roomChances;
    public CoridorDoorChance[] coridorDoorChances;

    void Start()
    {
        roomSizeCalculator = new RoomSizeCalculator(roomPrefabs[1]);
        roomSize = roomSizeCalculator.RoomSizeCalculation(roomPrefabs[1]);

        matrixManager = new MatrixManager(N);
        roomPlacer = new RoomPlacer(matrixManager, roomPrefabs, roomSize, doorSprites, N, roomChances);

        roomPlacer.GenerateRooms();
    }

    private void OnValidate()
    {
        float roomTypeTotalChance = 0f;
        float coridorDoorTotalChance = 0f;

        foreach (var roomChance in roomChances)
        {
            roomTypeTotalChance += roomChance.chance;
        }

        if (roomTypeTotalChance > 100f)
        {
            Debug.LogError($"Вероятность генерации {roomTypeTotalChance} превышает 100%.");
        }
        
        if (roomTypeTotalChance < 100f)
        {
            Debug.LogError($"Вероятность генерации {roomTypeTotalChance} меньше 100%");
        }

        foreach (var coridorDoorChance in coridorDoorChances)
        {
            coridorDoorTotalChance += coridorDoorChance.chance;
        }

        if (coridorDoorTotalChance > 100f)
        {
            Debug.LogError($"Вероятность генерации {coridorDoorTotalChance} превышает 100%.");
        }

        if (coridorDoorTotalChance < 100f)
        {
            Debug.LogError($"Вероятность генерации {coridorDoorTotalChance} меньше 100%");
        }
    }
}
