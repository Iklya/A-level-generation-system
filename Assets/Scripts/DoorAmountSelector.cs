using UnityEngine;

public class DoorAmountSelector
{
    private CoridorDoorChance[] coridorDoorChances;

    public DoorAmountSelector(CoridorDoorChance[] coridorDoorChances)
    {
        this.coridorDoorChances = coridorDoorChances;
    }
    
    public int GetCoridorDoorsAmount()
    {
        float currChance = Random.Range(0f, 99.9f);
        float totalChance = 0f;

        foreach (var doorChance in coridorDoorChances)
        {
            totalChance += doorChance.chance;
            if (currChance <= totalChance)
                return doorChance.doorAmount;
        }

        return 2;
    }
}
