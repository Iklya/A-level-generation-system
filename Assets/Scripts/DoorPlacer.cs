using UnityEngine;

public class DoorPlacer
{
    private Sprite[] doorSprites;

    public DoorPlacer(Sprite[] doorSprites)
    {
        this.doorSprites = doorSprites;
    }

    public void PlaceDoor(GameObject room, int dir)
    {
        Transform doorPlacement = null;

        switch (dir)
        {
            case 0:
                doorPlacement = room.transform.Find("WallsLeft/doorPlacement");
                break;
            case 1:
                doorPlacement = room.transform.Find("WallsRight/doorPlacement");
                break;
            case 2:
                doorPlacement = room.transform.Find("WallsUp/doorPlacement");
                break;
            case 3:
                doorPlacement = room.transform.Find("WallsDown/doorPlacement");
                break;
        }

        SpriteRenderer spriteRenderer = doorPlacement.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorSprites[dir];
    }
}
