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

    public (GameObject, RoomType) InstantiateMainRooms(List<RoomData> mainPath, int r)
    {
        int curTypeInt = matrixManager.GetCell(mainPath[r].X, mainPath[r].Y);
        RoomType curType = (RoomType)curTypeInt;

        Vector2 pos = matrixManager.MatrixToGame(mainPath[r].X, mainPath[r].Y, roomSize);

        if (curTypeInt <= 0)
        {
            curType = (RoomType)roomTypeSelector.GetRandomRoomType();
            matrixManager.SetCell(mainPath[r].X, mainPath[r].Y, (int)curType);
        }

        GameObject roomObj = Object.Instantiate(roomPrefabs[(int)curType - 1], pos, Quaternion.identity);
        mainPath[r] = new RoomData(mainPath[r].X, mainPath[r].Y, roomObj, curType);

        return (roomObj, curType);
    }


    public void InstantiateExtraRooms(List<RoomData> extraPath, int r)
    {
        GameObject roomObj = null;
        RoomType curType = (RoomType)matrixManager.GetCell(extraPath[r].X, extraPath[r].Y);

        Vector2 gamePosition = matrixManager.MatrixToGame(extraPath[r].X, extraPath[r].Y, roomSize);

        roomObj = Object.Instantiate(roomPrefabs[(int)curType - 1], gamePosition, Quaternion.identity);

        extraPath[r] = new RoomData(extraPath[r].X, extraPath[r].Y, roomObj, curType);
    }
}
