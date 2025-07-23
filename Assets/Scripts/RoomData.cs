using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public int X;
    public int Y;
    public GameObject Room;
    public RoomType RoomType;

    public List<Transform> ItemPlaces = new List<Transform>();

    public RoomData(int x, int y, GameObject room, RoomType roomType)
    {
        X = x;
        Y = y;
        Room = room;
        RoomType = roomType;
    }

    public void UpdateItemPlaces()
    {
        ItemPlaces.Clear();
        if (Room == null) return;

        Transform itemPlacesParent = Room.transform.Find("RoomItemPlaces");
        if (itemPlacesParent != null)
        {
            foreach (Transform child in itemPlacesParent)
                ItemPlaces.Add(child);
        }
    }
}
