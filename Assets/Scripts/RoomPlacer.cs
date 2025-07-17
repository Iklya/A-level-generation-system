using UnityEngine;
using System.Collections.Generic;

public class RoomPlacer
{
    private MatrixManager matrixManager;
    private GameObject roomPrefab;
    private Vector2 roomSize;

    public Sprite[] doorSprites;

    public RoomPlacer(MatrixManager matrixManager, GameObject roomPrefab, Vector2 roomSize, Sprite[] doorSprites)
    {
        this.matrixManager = matrixManager;
        this.roomPrefab = roomPrefab;
        this.roomSize = roomSize;
        this.doorSprites = doorSprites;
    }

    public void PlaceStartRoom()
    {
        int x = matrixManager.CenterX;
        int y = matrixManager.CenterY;

        matrixManager.SetCell(x, y, 1);
        GameObject startRoom = InstantiateRoom(x, y);

        PlaceRandomDoor(startRoom, x, y);
    }

    private GameObject InstantiateRoom(int x, int y)
    {
        Vector2 gamePosition = MatrixToGame(x, y);
        return Object.Instantiate(roomPrefab, gamePosition, Quaternion.identity);
    }

    private Vector3 MatrixToGame(int x, int y)
    {
        float gameX = (x - matrixManager.CenterX) * roomSize.x;
        float gameY = (y - matrixManager.CenterY) * roomSize.y;
        return new Vector2(gameX, gameY);
    }

    private void PlaceRandomDoor(GameObject room, int x, int y)
    {
        List<int> avaliableDoors = new List<int>();

        if (matrixManager.GetCell(x - 1, y) == -1) avaliableDoors.Add(0);
        if (matrixManager.GetCell(x + 1, y) == -1) avaliableDoors.Add(1);
        if (matrixManager.GetCell(x, y + 1) == -1) avaliableDoors.Add(2);
        if (matrixManager.GetCell(x, y - 1) == -1) avaliableDoors.Add(3);

        // в будущем проверка на недоступность размещения дверей (генерация пойдёт заново)

        int randDoor = avaliableDoors[Random.Range(0, avaliableDoors.Count)];

        switch(randDoor)
        {
            case 0:
                Transform wallsLeft = room.transform.Find("WallsLeft");
                Transform leftDoorPlacement = wallsLeft.transform.Find("doorPlacement");
                ReplaceDoorSprite(leftDoorPlacement, avaliableDoors[randDoor]);
                break;
            case 1:
                Transform wallsRight = room.transform.Find("WallsRight");
                Transform rightDoorPlacement = wallsRight.transform.Find("doorPlacement");
                ReplaceDoorSprite(rightDoorPlacement, avaliableDoors[randDoor]);
                break;
            case 2:
                Transform wallsUp = room.transform.Find("WallsUp");
                Transform upDoorPlacement = wallsUp.transform.Find("doorPlacement");
                ReplaceDoorSprite(upDoorPlacement, avaliableDoors[randDoor]);
                break;
            case 3:
                Transform wallsDown = room.transform.Find("WallsDown");
                Transform downDoorPlacement = wallsDown.transform.Find("doorPlacement");
                ReplaceDoorSprite(downDoorPlacement, avaliableDoors[randDoor]);
                break;
        }
    }

    private void ReplaceDoorSprite(Transform doorPlacement, int door)
    {
        SpriteRenderer spriteRenderer = doorPlacement.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorSprites[door];
    }
}
