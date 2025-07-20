using UnityEngine;

public class RoomTypeSelector
{
    private RoomChance[] roomChances;

    public RoomTypeSelector(RoomChance[] roomChances)
    {
        this.roomChances = roomChances;
    }

    public int GetRandomRoomType()
    {
        float currChance = Random.Range(0f, 100f);
        float totalChance = 0f;

        foreach (var roomChance in roomChances)
        {
            totalChance += roomChance.chance;
            if (totalChance > currChance)
                return roomChance.roomType;
        }

        return -1;
    }
}
