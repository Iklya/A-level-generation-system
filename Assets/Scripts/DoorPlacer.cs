using UnityEngine;

public class DoorPlacer
{
    private Sprite[] doorSprites;

    public DoorPlacer(Sprite[] doorSprites)
    {
        this.doorSprites = doorSprites;
    }

    public void PlaceDoor(GameObject room, Direction dir)
    {
        Transform doorPlacement = null;

        switch (dir)
        {
            case Direction.Left:
                doorPlacement = room.transform.Find("WallsLeft/doorPlacement");
                break;
            case Direction.Right:
                doorPlacement = room.transform.Find("WallsRight/doorPlacement");
                break;
            case Direction.Up:
                doorPlacement = room.transform.Find("WallsUp/doorPlacement");
                break;
            case Direction.Down:
                doorPlacement = room.transform.Find("WallsDown/doorPlacement");
                break;
        }

        SpriteRenderer spriteRenderer = doorPlacement.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorSprites[(int)dir];
    }
}
