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

    public void InstantiateMainRooms(List<(int x, int y, GameObject room, int roomType)> mainPath, int r)
    {
        GameObject roomObj = null;
        int curType = 0;

        Vector2 gamePosition = matrixManager.MatrixToGame(mainPath[r].x, mainPath[r].y, roomSize);


        switch (matrixManager.GetCell(mainPath[r].x, mainPath[r].y))
        {
            case 1:
                curType = 1;
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
            case 2:
                curType = 2;
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
            default:
                curType = roomTypeSelector.GetRandomRoomType();
                roomObj = Object.Instantiate(roomPrefabs[curType - 1], gamePosition, Quaternion.identity);
                break;
        }

        mainPath[r] = (mainPath[r].x, mainPath[r].y, roomObj, curType);
        matrixManager.SetCell(mainPath[r].x, mainPath[r].y, curType);
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
