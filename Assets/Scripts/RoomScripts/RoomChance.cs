using UnityEngine;
using System;

[Serializable]
public class RoomChance
{
    public int roomType;

    [Range(0, 100)]
    public int chance;
}
