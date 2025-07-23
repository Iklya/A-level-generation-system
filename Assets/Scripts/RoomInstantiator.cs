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

    public (GameObject, int) InstantiateMainRooms(List<RoomData> mainPath, int r)
    {
        GameObject roomObj = null;
        int curType = matrixManager.GetCell(mainPath[r].X, mainPath[r].Y);
        Debug.Log($"{curType} - изначально для {r + 1} комнаты был такой индекс матрицы");

        Vector2 gamePosition = matrixManager.MatrixToGame(mainPath[r].X, mainPath[r].Y, roomSize);

        if (curType <= 0)
        {
            curType = roomTypeSelector.GetRandomRoomType();
            matrixManager.SetCell(mainPath[r].X, mainPath[r].Y, curType);
        }

        roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);

        Debug.Log($"{curType} - выбранный тип комнаты для {r+1} комнаты основного пути");
        mainPath[r] = new RoomData(mainPath[r].X, mainPath[r].Y, roomObj, curType);

        return (roomObj, curType);
    }

    public void InstantiateExtraRooms(List<RoomData> extraPath, int r)
    {
        GameObject roomObj = null;
        int curType = matrixManager.GetCell(extraPath[r].X, extraPath[r].Y);

        Vector2 gamePosition = matrixManager.MatrixToGame(extraPath[r].X, extraPath[r].Y, roomSize);

        roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);

        extraPath[r] = new RoomData(extraPath[r].X, extraPath[r].Y, roomObj, curType);
    }
}
