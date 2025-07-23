using UnityEngine;

public class RoomData
{
    public int X;
    public int Y;
    public GameObject Room;
    public int RoomType;

    public RoomData(int x, int y, GameObject room, int roomType)
    {
        X = x;
        Y = y;
        Room = room;
        RoomType = roomType;
    }
}
