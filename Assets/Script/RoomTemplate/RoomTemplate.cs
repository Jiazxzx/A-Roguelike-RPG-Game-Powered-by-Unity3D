using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplate{

    public List<List<ItemAndPos>> ItemList = new List<List<ItemAndPos>>();

}

public enum ItemType
{
    Item11, Item21, Item22,
    Barrier11, Barrier21, Barrier22,Barrier31, Barrier71, Barrier15,
    Monster,
    Chest11,
    Terrain11, Terrain22, Terrain33,
}

public class ItemAndPos
{
    public float x;
    public float y;
    public ItemType type;

    public ItemAndPos(ItemType t,float xx,float yy)
    {
        type = t;
        x = xx;
        y = yy;
    }
    public Vector2 RightUpPoint()
    {
        switch(type)
        {
            case ItemType.Monster:
            case ItemType.Item11:
            case ItemType.Barrier11:
            case ItemType.Chest11:
                return new Vector2(x, y);
            case ItemType.Item21:
            case ItemType.Barrier21:
                return new Vector2(x+1, y);
            case ItemType.Item22:
            case ItemType.Barrier22:
                return new Vector2(x + 1, y+1);
            case ItemType.Barrier31:
                return new Vector2(x + 2, y);
            case ItemType.Barrier71:
                return new Vector2(x + 6, y);
            case ItemType.Barrier15:
                return new Vector2(x , y+ 4);
            default:
                Debug.Log("no exsist");
                return new Vector2(-1,-1);

        }
    }
    public bool ifCover(Vector2 offset,Vector2 pos)
    {
        Vector2 rightup = RightUpPoint();
        if (pos.x > offset.x+rightup.x || pos.y > offset.y+rightup.y || pos.x < offset.x + x || pos.y < offset.y + y)
            return false;
        else
            return true;
    }

}
