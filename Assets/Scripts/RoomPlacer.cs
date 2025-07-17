using UnityEngine;

public class RoomPlacer
{
    private MatrixManager matrixManager;
    private GameObject roomPrefab;
    private Vector2 roomSize;

    public RoomPlacer(MatrixManager gridManager, GameObject roomPrefab, Vector2 roomSize)
    {
        this.matrixManager = gridManager;
        this.roomPrefab = roomPrefab;
        this.roomSize = roomSize;
    }

    public void PlaceStartRoom()
    {
        int x = matrixManager.CenterX;
        int y = matrixManager.CenterY;

        matrixManager.SetCell(x, y, 1);
        InstantiateRoom(x, y);
    }

    private void InstantiateRoom(int x, int y)
    {
        Vector2 gamePosition = MatrixToGame(x, y);
        Object.Instantiate(roomPrefab, gamePosition, Quaternion.identity);
    }

    private Vector3 MatrixToGame(int x, int y)
    {
        float gameX = (x - matrixManager.CenterX) * roomSize.x;
        float gameY = (y - matrixManager.CenterY) * roomSize.y;
        return new Vector2(gameX, gameY);
    }
}
