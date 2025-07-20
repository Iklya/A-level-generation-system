using System.Collections.Generic;
using UnityEngine;

public class RoomInstantiator
{
    private MatrixManager matrixManager;
    private GameObject[] roomPrefabs;
    private Vector2 roomSize;
    private RoomTypeSelector roomTypeSelector;

    public RoomInstantiator(MatrixManager matrixManager, GameObject[] roomPrefabs, Vector2 roomSize, RoomTypeSelector roomTypeSelector)
    {
        this.matrixManager = matrixManager;
        this.roomPrefabs = roomPrefabs;
        this.roomSize = roomSize;
        this.roomTypeSelector = roomTypeSelector;
    }

    public (GameObject, int) InstantiateMainRooms(List<(int x, int y, GameObject room, int roomType)> mainPath, int r)
    {
        GameObject roomObj = null;
        int curType = matrixManager.GetCell(mainPath[r].x, mainPath[r].y);
        Debug.Log($"{curType} - изначально для {r + 1} комнаты был такой индекс матрицы");

        Vector2 gamePosition = matrixManager.MatrixToGame(mainPath[r].x, mainPath[r].y, roomSize);

        if (curType <= 0)
        {
            curType = roomTypeSelector.GetRandomRoomType();
            matrixManager.SetCell(mainPath[r].x, mainPath[r].y, curType);
        }

        roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);

        Debug.Log($"{curType} - выбранный тип комнаты для {r+1} комнаты основного пути");
        mainPath[r] = (mainPath[r].x, mainPath[r].y, roomObj, curType);

        return (roomObj, curType);
    }

    public void InstantiateExtraRooms(List<(int x, int y, GameObject room, int roomType)> extraPath, int r)
    {
        GameObject roomObj = null;
        int curType = matrixManager.GetCell(extraPath[r].x, extraPath[r].y);

        Vector2 gamePosition = matrixManager.MatrixToGame(extraPath[r].x, extraPath[r].y, roomSize);

        roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);

        extraPath[r] = (extraPath[r].x, extraPath[r].y, roomObj, curType);
    }
}
