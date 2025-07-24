using UnityEngine;

public class RoomSizeCalculator
{
    GameObject roomPrefab;

    public RoomSizeCalculator(GameObject roomPrefab)
    {
        this.roomPrefab = roomPrefab;
    }

    public Vector2 RoomSizeCalculation(GameObject roomPrefab)
    {
        Transform wallsUp = roomPrefab.transform.Find("WallsUp");
        Transform wallUpPrefab = wallsUp.transform.Find("wallUpPrefab");

        Transform wallsDown = roomPrefab.transform.Find("WallsDown");
        Transform wallDownPrefab = wallsDown.transform.Find("wallDownPrefab");

        Transform wallsLeft = roomPrefab.transform.Find("WallsLeft");
        Transform wallLeftPrefab = wallsLeft.transform.Find("wallLeftPrefab");

        Transform wallsRight = roomPrefab.transform.Find("WallsRight");
        Transform wallRightPrefab = wallsRight.transform.Find("wallRightPrefab");


        float width = Mathf.Abs(wallLeftPrefab.position.x - wallRightPrefab.position.x) + 0.470f;
        float height = Mathf.Abs(wallUpPrefab.position.y - wallDownPrefab.position.y) + 0.470f;

        Debug.Log($"Размер комнаты: {width}x{height}");

        return new Vector2(width, height);
    }
}
