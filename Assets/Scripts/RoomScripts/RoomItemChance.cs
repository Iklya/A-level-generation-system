using UnityEngine;

[System.Serializable]
public class RoomItemChance
{
    public RoomType AllowedRoomType;
    public ItemType ItemType;
    [Range(0, 100)]
    public float SpawnChance;
}
