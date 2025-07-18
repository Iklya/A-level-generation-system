using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(2, 100)]
    public int N;
    public GameObject[] roomPrefabs;
    public Sprite[] doorSprites; // ѕусть: 0 - left, 1 - right, 2 - up, 3 - down

    private Vector2 roomSize;

    private MatrixManager matrixManager;
    private RoomPlacer roomPlacer;
    private RoomSizeCalculator roomSizeCalculator;

    public RoomChance[] roomChances;

    void Start()
    {
        roomSizeCalculator = new RoomSizeCalculator(roomPrefabs[1]);
        roomSize = roomSizeCalculator.RoomSizeCalculation(roomPrefabs[1]);

        matrixManager = new MatrixManager(N);
        roomPlacer = new RoomPlacer(matrixManager, roomPrefabs, roomSize, doorSprites, N, roomChances);

        roomPlacer.GenerateRooms();

        // тут задам тип конечной комнате
    }

    private void OnValidate()
    {
        float totalChance = 0f;
        foreach (var roomChance in roomChances)
        {
            totalChance += roomChance.chance;
        }

        if (totalChance > 100f)
        {
            Debug.LogError("¬еро€тность генерации типов комнат превышает 100%.");
        }
        
        if (totalChance < 100f)
        {
            Debug.LogError("¬еро€тность генерации типов комнат меньше 100%");
        }
    }
}
