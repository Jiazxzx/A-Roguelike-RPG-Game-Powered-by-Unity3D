using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//public enum Direction
//{
//    North, East, South, West, Northeast, Eastsouth, Southwest, Westnorth
//}

public enum TileType
{
    Dark, Floor, Wall, Corr,
}

public enum RoomType
{
    CorrNode, SmallRoom, BigRoom
}

public enum WallType
{
    Lu, U, Ru, L, R, Ld, D, Rd,
}

public class ClassBoardCreater : NetworkBehaviour, IEdgeInterface
{
    static public int roomScale = 15;          // 表示一个正方形grid区域边长对应多少个tiles边长
    public int columns = 150;           // tiles列数
    public int rows = 150;              // tiles行数
    public int columns_grid;            // grids列数
    public int rows_grid;               // grids行数
    private int depth = 0;              // 生成tiles时的z坐标
    private RoomCreater roomCreater;

    public IntRange[] range_roomNum = new IntRange[3];
    public IntRange range_smallRoomWidth;
    public IntRange range_smallRoomHeight;
    public IntRange range_bigRoomWidth;
    public IntRange range_bigRoomHeight;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] corrTiles;
    public GameObject[] inwallTiles;

    private bool[][] gridTaken;         // 表示对应位置的grid是否被房间占据
    private TileType[][] tiles;                               
    private ClassRoom[] rooms;                                
    private ClassCorridor[] corridors;                        
    private GameObject boardHolder;

    private int[] num_rooms;            // 目标各类房间数
    private int[] count_rooms;          // 已生成各类房间数
    private int totalnum_rooms;
    private int totalcount_rooms;

    public GameObject ePrefab2;
    public GameObject ePoint2;

    private Queue openRoom = new Queue();

    public override void OnStartServer()
    {
        boardHolder = new GameObject("ClassBoardHolder");
        roomCreater = GetComponent<RoomCreater>();

        InitArgument();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();
        CheckTilesValuesForFloor();

        InstantiateTiles();
        FurnishTheRooms();

        //move the whole block to the east
        boardHolder.transform.position = new Vector3(300, 0, 0);
        roomCreater.roomHolder.transform.position = new Vector3(300, 0, 0);

        Vector3 ePos = new Vector3(300 + rooms[0].xPos + 2, rooms[0].yPos + 2, 0);
        ePoint2 = Instantiate(ePrefab2, ePos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(ePoint2);
    }

    void SetupTilesArray()
    {
        // 初始化地形表
        tiles = new TileType[columns][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[rows];
        }       
    }
    
    void SetupGridsArray()
    {
        // 初始化gird占用表
        gridTaken = new bool[columns_grid][];

        for (int i = 0; i < gridTaken.Length; i++)
        {
            gridTaken[i] = new bool[rows_grid];
        }
    }

    void SetupRoomNumAndCount()
    {
        // 记录不同种类房间的目标生成数目
        num_rooms = new int[3];
        // 记录目标生成房间总数
        totalnum_rooms = 0;
        // 记录不同种类房间已生成数
        count_rooms = new int[num_rooms.Length];
        // 记录总生成房间数
        totalcount_rooms = 0;    
        
        for (int i = 0; i < num_rooms.Length; i++)
        {
            num_rooms[i] = range_roomNum[i].Random;
            totalnum_rooms += num_rooms[i];
            count_rooms[i] = 0;
        }
    }

    void InitArgument()
    {
        columns_grid = columns / roomScale;
        rows_grid = rows / roomScale;

        range_roomNum[(int)RoomType.CorrNode] = new IntRange(4, 5);
        range_roomNum[(int)RoomType.SmallRoom] = new IntRange(25, 30);
        range_roomNum[(int)RoomType.BigRoom] = new IntRange(2, 3);

        range_smallRoomWidth = new IntRange(Mathf.RoundToInt(roomScale * .75f), roomScale - 2);
        range_smallRoomHeight = new IntRange(Mathf.RoundToInt(roomScale * .75f), roomScale - 2);

        range_bigRoomWidth = new IntRange(Mathf.RoundToInt(roomScale * 1.5f), 2 * roomScale - 2);
        range_bigRoomHeight = new IntRange(Mathf.RoundToInt(roomScale * 1.5f), 2 * roomScale - 2);

    SetupTilesArray();
        SetupGridsArray();
        SetupRoomNumAndCount();
    }

    void CreateRoomsAndCorridors()
    {
        // 创建第一个初始房间 ////////////
        rooms = new ClassRoom[totalnum_rooms];
        corridors = new ClassCorridor[totalnum_rooms - 1];
        rooms[0] = new ClassRoom();
        rooms[0].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, columns_grid / 2, rows_grid / 2, (int)RoomType.SmallRoom);
        gridTaken[rooms[0].xPos_grid][rooms[0].yPos_grid] = true;
        count_rooms[(int)RoomType.SmallRoom]++;
        totalcount_rooms++;
        openRoom.Enqueue(rooms[0]);
        ///////////////////////////////

        /*  当拓展房间队列为空时，说明所有新生成的房间均无法继续拓展，
            此时需要回退查询之前的房间是否可以继续拓展，backTracer
            用于保存回退的房间号 */
        int count_backtrace = -1;

        while(totalcount_rooms < totalnum_rooms)
        {
            ClassRoom currentRoom;
            // 当拓展房间队列不为空时
            if(openRoom.Count > 0)
            {
                currentRoom = (ClassRoom)openRoom.Dequeue();
            }
            else
            {
                if (count_backtrace < 0)
                {
                    count_backtrace = totalcount_rooms - 1;
                }
                currentRoom = rooms[count_backtrace];
                count_backtrace--;
            }
            ExpandRoom(currentRoom);
        }
    }

    void ExpandRoom(ClassRoom currentRoom)
    {
        ArrayList dirLinkable = GetLinkableDir(currentRoom);
        // 如果当前房间周围可以生成新房间
        if(dirLinkable.Count > 0)
        {
            int num_newroom = Random.Range(1, dirLinkable.Count);
            
            while(num_newroom > 0 && totalcount_rooms < totalnum_rooms)
            {
                // 选择新房间方位
                int randDir = Random.Range(0, dirLinkable.Count);
                Direction dirTolink = (Direction)dirLinkable[randDir];
                dirLinkable.RemoveAt(randDir);
                // 选择新房间类型
                int randType = Random.Range(0, 3);
                while(count_rooms[randType] >= num_rooms[randType])
                {
                    randType = (randType + 1) % 3;
                }
                // 根据新房间方位和类型判断是否能建新房间
                if(currentRoom.type == RoomType.BigRoom)
                {
                    if (TryBuildNewRoom_ForBig(currentRoom, dirTolink, randType))
                    {
                        openRoom.Enqueue(rooms[totalcount_rooms]);
                        count_rooms[randType]++;
                        totalcount_rooms++;
                    }
                }
                else
                {
                    if (TryBuildNewRoom_ForRegular(currentRoom, dirTolink, randType))
                    {
                        openRoom.Enqueue(rooms[totalcount_rooms]);
                        count_rooms[randType]++;
                        totalcount_rooms++;
                    }
                }
                num_newroom--;
            }
        }
    }

    bool TryBuildNewRoom_ForBig(ClassRoom currentRoom, Direction dirTolink, int randType)
    {
        if(rooms[totalcount_rooms] == null)
        {
            rooms[totalcount_rooms] = new ClassRoom();
            corridors[totalcount_rooms - 1] = new ClassCorridor();
        }

        if((RoomType)randType == RoomType.BigRoom)
        {
            switch(dirTolink)
            {
                case Direction.North:
                    if(CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid + 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    break;
                case Direction.East:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid + 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid + 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid + 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    break;
                case Direction.South:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    break;
                case Direction.West:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    break;
                case Direction.Northeast:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid + 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid + 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid + 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid + 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    break;
                case Direction.Eastsouth:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid - 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid - 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid - 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 2, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    break;
                case Direction.Southwest:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    break;
                case Direction.Westnorth:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid + 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid + 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid + 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    break;
            }

            return false;
        }
        else
        {
            switch (dirTolink)
            {
                case Direction.North:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid + 2, randType);
                    gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid + 2] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                    break;
                case Direction.East:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid + 1, randType);
                    gridTaken[currentRoom.xPos_grid + 2][currentRoom.yPos_grid + 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                    break;
                case Direction.South:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 1, randType);
                    gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid - 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                    break;
                case Direction.West:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid, randType);
                    gridTaken[currentRoom.xPos_grid - 1][currentRoom.yPos_grid] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                    break;

                case Direction.Northeast:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid + 2, randType);
                    gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid + 2] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                    break;
                case Direction.Eastsouth:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid + 2, currentRoom.yPos_grid, randType);
                    gridTaken[currentRoom.xPos_grid + 2][currentRoom.yPos_grid] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                    break;
                case Direction.Southwest:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid - 1, randType);
                    gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid - 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                    break;
                case Direction.Westnorth:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 1, randType);
                    gridTaken[currentRoom.xPos_grid - 1][currentRoom.yPos_grid + 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                    break;
            }

            return true;
        }
    }

    bool TryBuildNewRoom_ForRegular(ClassRoom currentRoom, Direction dirTolink, int randType)
    {
        if (rooms[totalcount_rooms] == null)
        {
            rooms[totalcount_rooms] = new ClassRoom();
            corridors[totalcount_rooms - 1] = new ClassCorridor();
        }

        if ((RoomType)randType == RoomType.BigRoom)
        {
            switch (dirTolink)
            {
                case Direction.North:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid + 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid + 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid + 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                        return true;
                    }
                    break;
                case Direction.East:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid - 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid + 1, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                        return true;
                    }
                    break;
                case Direction.South:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 1, currentRoom.yPos_grid - 2);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                        return true;
                    }
                    break;
                case Direction.West:
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    if (CheckGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1))
                    {
                        rooms[totalcount_rooms].SetupRoom(range_bigRoomWidth, range_bigRoomHeight, currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1, randType);
                        SetGridForBigRoom(currentRoom.xPos_grid - 2, currentRoom.yPos_grid - 1);
                        corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                        return true;
                    }
                    break;
            }

            return false;
        }
        else
        {
            switch (dirTolink)
            {
                case Direction.North:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid + 1, randType);
                    gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid + 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.North);
                    break;
                case Direction.East:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid + 1, currentRoom.yPos_grid, randType);
                    gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.East);
                    break;
                case Direction.South:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid, currentRoom.yPos_grid - 1, randType);
                    gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid - 1] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.South);
                    break;
                case Direction.West:
                    rooms[totalcount_rooms].SetupRoom(range_smallRoomWidth, range_smallRoomHeight, currentRoom.xPos_grid - 1, currentRoom.yPos_grid, randType);
                    gridTaken[currentRoom.xPos_grid - 1][currentRoom.yPos_grid] = true;
                    corridors[totalcount_rooms - 1].SetupCorridor(currentRoom, rooms[totalcount_rooms], (int)Direction.West);
                    break;
            }

            return true;
        }
    }

    bool CheckGridForBigRoom(int xPos_grid, int yPos_grid)
    {
        if (xPos_grid < 0 || xPos_grid + 1 >= columns_grid || yPos_grid < 0 || yPos_grid + 1 >= rows_grid)
        {
            return false;
        }
        if (gridTaken[xPos_grid][yPos_grid] || gridTaken[xPos_grid][yPos_grid + 1] || gridTaken[xPos_grid + 1][yPos_grid] || gridTaken[xPos_grid + 1][yPos_grid + 1])
        {
            return false;
        }
        return true;
    }

    void SetGridForBigRoom(int xPos_grid, int yPos_grid)
    {
        gridTaken[xPos_grid][yPos_grid] = true;
        gridTaken[xPos_grid][yPos_grid + 1] = true;
        gridTaken[xPos_grid + 1][yPos_grid] = true;
        gridTaken[xPos_grid + 1][yPos_grid + 1] = true;
    }

    ArrayList GetLinkableDir(ClassRoom currentRoom)
    {
        ArrayList dirLinkable = new ArrayList();

        if (currentRoom.type == RoomType.BigRoom)
        {
            if(currentRoom.yPos_grid + 2 < rows_grid)
            {
                if (!gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid + 2])
                {
                    dirLinkable.Add(Direction.North);
                }
                if (!gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid + 2])
                {
                    dirLinkable.Add(Direction.Northeast);
                }
            }
            if (currentRoom.yPos_grid - 1 >= 0)
            {
                if (!gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid - 1])
                {
                    dirLinkable.Add(Direction.Southwest);
                }
                if (!gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid - 1])
                {
                    dirLinkable.Add(Direction.South);
                }
            }
            if (currentRoom.xPos_grid - 1 >= 0)
            {
                if (!gridTaken[currentRoom.xPos_grid- 1][currentRoom.yPos_grid])
                {
                    dirLinkable.Add(Direction.West);
                }
                if (!gridTaken[currentRoom.xPos_grid - 1][currentRoom.yPos_grid + 1])
                {
                    dirLinkable.Add(Direction.Westnorth);
                }
            }
            if (currentRoom.xPos_grid + 2 < columns_grid)
            {
                if (!gridTaken[currentRoom.xPos_grid + 2][currentRoom.yPos_grid])
                {
                    dirLinkable.Add(Direction.Eastsouth);
                }
                if (!gridTaken[currentRoom.xPos_grid + 2][currentRoom.yPos_grid + 1])
                {
                    dirLinkable.Add(Direction.East);
                }
            }
        }
        else
        {
            if (currentRoom.yPos_grid < rows_grid - 1)
            {
                if (!gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid + 1])
                {
                    dirLinkable.Add((Direction.North));
                }
            }
            if (currentRoom.xPos_grid < columns_grid - 1)
            {
                if(!gridTaken[currentRoom.xPos_grid + 1][currentRoom.yPos_grid])
                {
                    dirLinkable.Add((Direction.East));
                }
            }
            if (currentRoom.yPos_grid > 0)
            {
                if (!gridTaken[currentRoom.xPos_grid][currentRoom.yPos_grid - 1])
                {
                    dirLinkable.Add((Direction.South));
                }
            }
            if (currentRoom.xPos_grid > 0)
            {
                if (!gridTaken[currentRoom.xPos_grid - 1][currentRoom.yPos_grid])
                {
                    dirLinkable.Add((Direction.West));
                }
            }
        }

        return dirLinkable;
    }

    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            ClassRoom currentRoom = rooms[i];

            if(currentRoom.type == RoomType.CorrNode)
            {
                int xCoord = currentRoom.xPos;
                int yCoord = currentRoom.yPos;

                tiles[xCoord][yCoord] = TileType.Corr;

                tiles[xCoord - 1][yCoord + 1] = TileType.Wall;
                tiles[xCoord - 1][yCoord] = TileType.Wall;
                tiles[xCoord - 1][yCoord - 1] = TileType.Wall;
                tiles[xCoord + 1][yCoord + 1] = TileType.Wall;
                tiles[xCoord + 1][yCoord] = TileType.Wall;
                tiles[xCoord + 1][yCoord - 1] = TileType.Wall;
                tiles[xCoord][yCoord + 1] = TileType.Wall;
                tiles[xCoord][yCoord - 1] = TileType.Wall;
            }
            else
            {
                // ... and for each room go through it's width.
                for (int j = 0; j < currentRoom.roomWidth; j++)
                {
                    int xCoord = currentRoom.xPos + j;

                    // For each horizontal tile, go up vertically through the room's height.
                    for (int k = 0; k < currentRoom.roomHeight; k++)
                    {
                        int yCoord = currentRoom.yPos + k;

                        if (xCoord < 0 || xCoord >= columns || yCoord < 0 || yCoord >= rows)
                            Debug.Log("Error!");
                        // The coordinates in the jagged array are based on the room's position and it's width and height.
                        if (j == 0 || j == currentRoom.roomWidth - 1 || k == 0 || k == currentRoom.roomHeight - 1)
                            tiles[xCoord][yCoord] = TileType.Wall;
                        else
                            tiles[xCoord][yCoord] = TileType.Floor;
                    }
                }
            }
        }
    }

    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            ClassCorridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set the tile at these coordinates to Floor.
                //tiles[xCoord][yCoord] = TileType.Floor;

                for (int k = -1; k < 2; k++)
                {
                    switch (currentCorridor.direction)
                    {
                        case Direction.North:
                        case Direction.South:
                            if(k == 0)
                            {
                                tiles[xCoord + k][yCoord] = TileType.Corr;
                            }
                            else
                            {
                                tiles[xCoord + k][yCoord] = TileType.Wall;
                            }
                            break;
                        case Direction.East:
                        case Direction.West:
                            if (k == 0)
                            {
                                tiles[xCoord][yCoord + k] = TileType.Corr;
                            }
                            else
                            {
                                tiles[xCoord][yCoord + k] = TileType.Wall;
                            }
                            break;
                    }

                }
            }
        }
    }

    void CheckTilesValuesForFloor()
    {
        for (int i = 1; i < columns - 1; i++)
        {
            for (int j = 1; j < rows - 1; j++)
            {
                if (tiles[i][j] == TileType.Wall)
                {
                    if (!BesideOfTiles(i, j, TileType.Dark))
                    {
                        if (!((tiles[i + 1][j] == TileType.Wall && tiles[i - 1][j] == TileType.Wall) ||
                        (tiles[i][j + 1] == TileType.Wall && tiles[i][j - 1] == TileType.Wall)))
                        {
                            tiles[i][j] = TileType.Floor;
                        }
                        else
                            tiles[i][j] = TileType.Corr;
                    }
                }
            }
        }
    }

    bool BesideOfTiles(int i, int j, TileType type)
    {
        for (int m = -1; m < 2; m++)
        {
            for (int n = -1; n < 2; n++)
            {
                if (tiles[i + m][j + n] == type)
                    return true;
            }
        }

        return false;
    }

    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // ... and instantiate a floor tile for it.
                if(tiles[i][j] == TileType.Floor)
                {
                    InstantiateFromArray(floorTiles, i, j);
                }
                // If the tile type is Wall...
                if (tiles[i][j] == TileType.Wall)
                {
                    // ... instantiate a wall over the top.
                    InstantiateWall(i, j);
                }

                if (tiles[i][j] == TileType.Corr)
                {
                    InstantiateFromArray(corrTiles, i, j);
                }
            }
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;

        NetworkServer.Spawn(tileInstance);
    }


    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    for(int i = 0; i < columns_grid + 1; i++)
    //    {
    //        Gizmos.DrawLine(new Vector3(0, i * roomScale), new Vector3(columns, i * roomScale));
    //        Gizmos.DrawLine(new Vector3(i * roomScale, 0), new Vector3(i * roomScale, rows));
    //    }

    //}

    void InstantiateWall(float xCoord, float yCoord)
    {

        int xCross = 0;
        int yCross = 0;

        if (yCoord < rows - 1 &&
            (tiles[(int)xCoord][(int)yCoord + 1] == TileType.Floor || tiles[(int)xCoord][(int)yCoord + 1] == TileType.Corr))
            yCross++;//up
        if (xCoord < columns - 1 &&
            (tiles[(int)xCoord + 1][(int)yCoord] == TileType.Floor || tiles[(int)xCoord + 1][(int)yCoord] == TileType.Corr))
            xCross++;//right
        if (yCoord > 0 &&
            (tiles[(int)xCoord][(int)yCoord - 1] == TileType.Floor || tiles[(int)xCoord][(int)yCoord - 1] == TileType.Corr))
            yCross--;//down
        if (xCoord > 0 &&
            (tiles[(int)xCoord - 1][(int)yCoord] == TileType.Floor || tiles[(int)xCoord - 1][(int)yCoord] == TileType.Corr))
            xCross--;//left


        if (xCross == 0 && yCross == 1) //up
        {
            InstantiateWallForPosition(xCoord, yCoord, WallType.D);
            return;
        }
        if (xCross == 1 && yCross == 0)//right
        {
            InstantiateWallForPosition(xCoord, yCoord, WallType.L);
            return;
        }
        if (xCross == 0 && yCross == -1)//down
        {
            InstantiateWallForPosition(xCoord, yCoord, WallType.U);
            return;
        }
        if (xCross == -1 && yCross == 0)//left
        {
            InstantiateWallForPosition(xCoord, yCoord, WallType.R);
            return;
        }
        if (xCross == -1 && yCross == 1)//left & up
        {
            //if the left & up tile can be accessed.
            if (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr)
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Lu, true);
                return;
            }
        }
        if (xCross == 1 && yCross == 1)//right & up
        {
            //if the right & up tile can be accessed.
            if (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr)
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Ru, true);
                return;
            }
        }
        if (xCross == -1 && yCross == -1)//left & down
        {
            //if the right & up tile can be accessed.
            if (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr)
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Ld, true);
                return;
            }
        }
        if (xCross == 1 && yCross == -1)//right & down
        {
            //if the right & up tile can be accessed.
            if (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr)
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Rd, true);
                return;
            }
        }

        //xCross = yCross = 0:
        if (xCross == 0 && yCross == 0)
        {
            xCross = -1;
            yCross = 1;
            if (xCoord != 0 && yCoord != rows - 1 &&
                (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr))
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Rd);
                return;
            }
            xCross = 1;
            yCross = 1;
            if (xCoord != columns - 1 && yCoord != rows - 1 &&
                (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr))
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Ld);
                return;
            }
            xCross = -1;
            yCross = -1;
            if (xCoord != 0 && yCoord != 0 &&
                (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr))
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Ru);
                return;
            }

            xCross = 1;
            yCross = -1;
            if (xCoord != columns - 1 && yCoord != 0 &&
                (tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Floor || tiles[(int)xCoord + xCross][(int)yCoord + yCross] == TileType.Corr))
            {
                InstantiateWallForPosition(xCoord, yCoord, WallType.Lu);
                return;
            }

        }
    }

    void InstantiateWallForPosition(float xCoord, float yCoord, WallType wall, bool inwall = false)
    {
        Vector3 position = new Vector3(xCoord, yCoord, depth);
        //if (wall == WallType.L || wall == WallType.R)
        //    position += new Vector3(0f, 0f, -0.5f);
        GameObject tileInstance = null;
        if (inwall)
            tileInstance = Instantiate(inwallTiles[(int)wall], position, Quaternion.identity) as GameObject;
        else
            tileInstance = Instantiate(wallTiles[(int)wall], position, Quaternion.identity) as GameObject;
        tileInstance.transform.parent = boardHolder.transform;
        NetworkServer.Spawn(tileInstance);
    }

    void FurnishTheRooms()
    {
        int roomNumber = 0;
        foreach (var item in rooms)
        {
            if (item.type != RoomType.CorrNode)
            {
                roomCreater.FurnishRoom(new Vector2(item.xPos+1, item.yPos+1), item.roomWidth-2, item.roomHeight-2, roomNumber);
            }
            roomNumber++;
        }
    }

    public List<Vector2>[] GetRoomEdgeAndInterier(int roomNumber)
    {
        List<Vector2> up = new List<Vector2>();
        List<Vector2> down = new List<Vector2>();
        List<Vector2> left = new List<Vector2>();
        List<Vector2> right = new List<Vector2>();
        List<Vector2> inter = new List<Vector2>();

        List<Vector2> leftup = new List<Vector2>();
        List<Vector2> leftdown = new List<Vector2>();
        List<Vector2> rightup = new List<Vector2>();
        List<Vector2> rightdown = new List<Vector2>();
        List<Vector2> hallway = new List<Vector2>();
        List<Vector2> door = new List<Vector2>();
        ClassRoom temp = rooms[roomNumber];
        float width = temp.roomWidth - 2;
        float height = temp.roomHeight - 2;
        float xPos = temp.xPos + 1;
        float yPos = temp.yPos + 1;


        for (int i = 1; i < width-1; i++)
        {
            down.Add(new Vector2(xPos + i, yPos));
            up.Add(new Vector2(xPos + i, yPos + height-1));
        }
        for (int i = 1; i < height - 1; i++)
        {
            left.Add(new Vector2(xPos, yPos + i));
            right.Add(new Vector2(xPos + width - 1, yPos + i));
        }
        leftup.Add(new Vector2(xPos, yPos + height - 1));
        rightup.Add(new Vector2(xPos + width - 1, yPos + height - 1));
        rightdown.Add(new Vector2(xPos + width - 1, yPos));
        leftdown.Add(new Vector2(xPos, yPos));
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                inter.Add(new Vector2(xPos+i, yPos+j));
            }
        }

        foreach (var item in corridors)
        {
            switch (item.direction)
            {
                case Direction.East:
                    door.Add(new Vector2(item.startXPos, item.startYPos));
                    door.Add(new Vector2(item.EndPositionX, item.EndPositionY));
                    hallway.Add(new Vector2(item.EndPositionX + 1, item.EndPositionY));
                    hallway.Add(new Vector2(item.startXPos - 1, item.startYPos));
                    break;
                case Direction.West:
                    door.Add(new Vector2(item.startXPos, item.startYPos));
                    door.Add(new Vector2(item.EndPositionX, item.EndPositionY));
                    hallway.Add(new Vector2(item.EndPositionX + 1, item.EndPositionY));
                    hallway.Add(new Vector2(item.startXPos - 1, item.startYPos));
                    break;
                case Direction.North:
                    door.Add(new Vector2(item.startXPos, item.startYPos));
                    door.Add(new Vector2(item.EndPositionX, item.EndPositionY));
                    hallway.Add(new Vector2(item.startXPos, item.EndPositionY + 1));
                    hallway.Add(new Vector2(item.startXPos, item.startYPos - 1));
                    break;
                case Direction.South:
                    door.Add(new Vector2(item.startXPos, item.startYPos));
                    door.Add(new Vector2(item.EndPositionX, item.EndPositionY));
                    hallway.Add(new Vector2(item.startXPos, item.EndPositionY + 1));
                    hallway.Add(new Vector2(item.startXPos, item.startYPos - 1));
                    break;
            }
        }


        List<Vector2>[] result = new List<Vector2>[11];
        result[0] = up;
        result[1] = down;
        result[2] = left;
        result[3] = right;
        result[4] = leftup;
        result[5] = rightup;
        result[6] = leftdown;
        result[7] = rightdown;
        result[8] = inter;
        result[9] = hallway;
        result[10] = door;
        return result;

    }
}
 