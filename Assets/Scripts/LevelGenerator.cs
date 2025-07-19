using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Range(2, 50)]
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
        roomPlacer = new RoomPlacer(matrixManager, roomPrefabs, roomSize, doorSprites, N, roomChances, coridorDoorChances);

        roomPlacer.GenerateRooms();
    }

    private void OnValidate()
    {
        float roomTypeTotalChance = 0f, coridorDoorTotalChance = 0f;

        foreach (var roomChance in roomChances)
            roomTypeTotalChance += roomChance.chance;

        CheckProbability(roomTypeTotalChance);

        foreach (var coridorDoorChance in coridorDoorChances)
            coridorDoorTotalChance += coridorDoorChance.chance;

        CheckProbability(roomTypeTotalChance);
    }

    private void CheckProbability(float ch)
    {
        if (ch > 100f)
            Debug.LogError($"Вероятность генерации превышает 100%! ({ch})");

        if (ch < 100f)
            Debug.LogError($"Вероятность генерации меньше 100%! ({ch})");
    }
}
