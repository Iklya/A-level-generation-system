using UnityEngine;
using System;

[Serializable]
public class CoridorDoorChance
{
    public int doorAmount;

    [Range(0f, 100f)]
    public float chance;
}
