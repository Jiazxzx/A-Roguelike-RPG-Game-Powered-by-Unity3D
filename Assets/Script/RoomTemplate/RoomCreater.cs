using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoomCreater : NetworkBehaviour {

    public float irregularEdgeItem = 0.1f;
    public float irregularInterItem = 0.1f;
    public String roomString = "Unknown";
    public GameObject roomHolder;


    public GameObject[] Item11List;

    public GameObject[] Chest11List;

    public GameObject[] Barrier11List;
    public GameObject[] Barrier22List;
    public GameObject[] Barrier31List;
    public GameObject[] Barrier71List;
    public GameObject[] Barrier15List;

    public GameObject[] MonsterList;

    public GameObject[] Terrain11List;
    public GameObject[] Terrain22List;
    public GameObject[] Terrain33List;

    public GameObject[] LUList;
    public GameObject[] LDList;
    public GameObject[] RUList;
    public GameObject[] RDList;
    public GameObject[] UpList;
    public GameObject[] DownList;
    public GameObject[] LeftList;
    public GameObject[] RightList;

    List<List<ItemAndPos>> list11;

    void Awake () {
        roomHolder = new GameObject(roomString + "RoomHolder");

        //initiate list11x11
        R11x11 r11x11 = new R11x11();
        r11x11.InitList();
        list11 = r11x11.ItemList;
        //Debug.Log(Item11List[Item11List.Length]);
        //TODO: initiate other list
    }


    public void FurnishRoom(Vector2 pos, int width, int height,int roomNumber)
    {
        bool fit = false;
        List<ItemAndPos> list = new List<ItemAndPos>();
        if (!fit && width==11 && height==11)
        {
            list = RandomRoom(list11);
            fit = true;
        }

        if (fit)
            InstantiateRoomByList(list, pos, roomNumber);
        else
            InstantiateRandomly(roomNumber);
    }


    List<ItemAndPos> RandomRoom(List<List<ItemAndPos>> list)
    {
        int randomIndex = UnityEngine.Random.Range(0, list.Count);

        return list[randomIndex];
    }

    private void InstantiateRoomByList(List<ItemAndPos> list, Vector2 pos, int roomNumber)
    {
        var hallway = GetComponent<IEdgeInterface>().GetRoomEdgeAndInterier(roomNumber)[9];
        GameObject tempHolder = new GameObject(roomString + roomNumber);
        foreach (var item in list)
        {
            Vector2 itemPos = pos + new Vector2(item.x, item.y);

            bool coverHallway = false;
            foreach (var hallwayPos in hallway)
                if (item.ifCover(pos,hallwayPos))
                    coverHallway = true;
            if (coverHallway) continue;

            switch (item.type)
            {
                /////Barrier//////
                case ItemType.Barrier11:
                    InstantiateFromArray(Barrier11List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Barrier22:
                    InstantiateFromArray(Barrier22List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Barrier31:
                    InstantiateFromArray(Barrier31List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Barrier71:
                    InstantiateFromArray(Barrier71List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Barrier15:
                    InstantiateFromArray(Barrier15List, itemPos.x, itemPos.y, tempHolder);
                    break;
                /////Item//////
                case ItemType.Item11:
                    InstantiateFromArray(Item11List, itemPos.x, itemPos.y, tempHolder);
                    break;
                /////Monster//////
                case ItemType.Monster:
                    InstantiateFromArray(MonsterList, itemPos.x, itemPos.y, tempHolder);
                    break;
                /////Chest//////
                case ItemType.Chest11:
                    InstantiateFromArray(Chest11List, itemPos.x, itemPos.y, tempHolder);
                    break;
                /////Terrain//////
                case ItemType.Terrain11:
                    InstantiateFromArray(Terrain11List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Terrain22:
                    InstantiateFromArray(Terrain22List, itemPos.x, itemPos.y, tempHolder);
                    break;
                case ItemType.Terrain33:
                    InstantiateFromArray(Terrain33List, itemPos.x, itemPos.y, tempHolder);
                    break;

                default:
                    break;
            }
        }
        
        tempHolder.transform.parent = roomHolder.transform;
    }


    private void InstantiateRandomly(int roomNumber)
    {
        GameObject tempHolder = new GameObject(roomString + roomNumber);
        var allList = GetComponent<IEdgeInterface>().GetRoomEdgeAndInterier(roomNumber);
        if (allList == null) return;
        var up = allList[0];
        var down = allList[1];
        var left = allList[2];
        var right = allList[3];
        var leftup = allList[4];
        var rightup = allList[5];
        var leftdown = allList[6];
        var rightdown = allList[7];
        var inter = allList[8];
        var hallway = allList[9];
        var door = allList[10];

        
        foreach (var item in up)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(UpList, item.x, item.y, tempHolder);
        }

        foreach (var item in down)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(DownList, item.x, item.y, tempHolder);
        }

        foreach (var item in right)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(RightList, item.x, item.y, tempHolder);
        }

        foreach (var item in left)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(LeftList, item.x, item.y, tempHolder);
        }


        foreach (var item in leftup)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(LUList, item.x, item.y, tempHolder);
        }

        foreach (var item in rightdown)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(RDList, item.x, item.y, tempHolder);
        }

        foreach (var item in leftdown)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(LDList, item.x, item.y, tempHolder);
        }

        foreach (var item in rightup)
        {
            if (InHallway(item, hallway)) continue;
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < irregularEdgeItem)
                InstantiateFromArray(RUList, item.x, item.y, tempHolder);
        }
        foreach (var item in inter)
        {
            float rand = UnityEngine.Random.Range(0f, 1.0f);//TODO: 细分各种类型
            if (rand < irregularInterItem)
                InstantiateFromArray(Item11List, item.x, item.y, tempHolder);
        }


        tempHolder.transform.parent = roomHolder.transform;
    }

    bool InHallway(Vector2 item,List<Vector2> hallway)
    {

        bool coverHallway = false;
        foreach (var hallwayPos in hallway)
            if (item.Equals(hallwayPos))
            {
                coverHallway = true;
                break;
            }
        return coverHallway;
    }


    private void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord,GameObject roomNumber)
    {
        if (prefabs.Length == 0) return;

        // Create a random index for the array.
        int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0.0001f* yCoord);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = roomNumber.transform;

        NetworkServer.Spawn(tileInstance);
    }
}
