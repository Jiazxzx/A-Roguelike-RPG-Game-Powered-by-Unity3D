using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LabBoardCreator : NetworkBehaviour, IEdgeInterface
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Dark, Floor,Wall,Corr,
    }
    public enum WallType
    {
        Lu,U,Ru,L,R,Ld,D,Rd,
    }

    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.
    public GameObject[] floorTiles;                           // An array of floor tile prefabs.
    public GameObject[] CorridorTiles;                            // An array of wall tile prefabs.
    public GameObject[] wallTiles;
    public GameObject[] inwallTiles;
    public GameObject[] outerWallTiles;                       // An array of outer wall tile prefabs.
    public GameObject player;

    public GameObject sPrefab;
    public GameObject ePrefab;
    public GameObject sPrefab2;
    public GameObject itemPrefab1;
    public GameObject itemPrefab2;
    public GameObject itemPrefab3;
    private GameObject sPoint;
    private GameObject ePoint;
    private GameObject sPoint2;

    GameObject play;
    Vector3 testOffset = new Vector3(0, 0, -10);

    private int depth = 0;
    public List<Vector2>[] comRoom;
    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.
   
    private Vector2[] roomSize;
    private RoomCreater roomCreater;

    public override void OnStartServer()
    {
        //init
        //depth = rows + 1;

        ////initiate the list
        initComRoom();

        //// Create the board holder.
        boardHolder = new GameObject("LabBoardHolder");

        ////initiate roomCreater
        roomCreater = GetComponent<RoomCreater>();

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();
        SetWall();

        CleanRedundantCorr();
        CleanRedundantRoom();


        InstantiateTiles();
        InstantiateOuterWalls();

        FurnishTheRooms();

        Debug.Log(comRoom[0][0].x);
        Debug.Log(comRoom[0][0].y);
        Debug.Log(IfRectRoom(comRoom[0][0].x, comRoom[0][0].y));
    }


    private void Update()
    {
        //this.gameObject.transform.position = new Vector3( play.transform.position.x, play.transform.position.y, testOffset.z);
        //boardHolder.transform.position = new Vector3(0,0,0f+ play.transform.position.z);
    }

    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
        }
    }


    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }

            if (i == rooms.Length / 2)
            {
                Vector3 sPos = new Vector3(-3, 0, 0);
                sPoint = Instantiate(sPrefab, sPos, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(sPoint);

                Vector3 ePos = new Vector3(rooms[i].xPos + 2, rooms[i].yPos + 2, 0);
                ePoint = Instantiate(ePrefab, ePos, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(ePoint);

                Vector3 sPos2 = new Vector3(rooms[i].xPos + 5, rooms[i].yPos + 2, 0);
                sPoint2 = Instantiate(sPrefab2, sPos2, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(sPoint2);

                Vector3 gunPos1 = new Vector3(-5, -6, 0);
                GameObject item1 = Instantiate(itemPrefab1, gunPos1, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(item1);

                Vector3 gunPos2 = new Vector3(0, 3, 0);
                GameObject item2 = Instantiate(itemPrefab2, gunPos2, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(item2);

                Vector3 gunPos3 = new Vector3(-6, -8, 0);
                GameObject item3 = Instantiate(itemPrefab3, gunPos3, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(item3);
            }
        }
    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;
                    if( j==0 || k==0 || j==currentRoom.roomWidth-1 || k== currentRoom.roomHeight-1)
                        tiles[xCoord][yCoord] = TileType.Wall;
                    else
                        tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }
    }

    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

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

                for (int k = -1; k < 2; k++)
                {
                    switch (currentCorridor.direction)
                    {
                        case Direction.North:
                        case Direction.South:
                            if(k == 0)
                                tiles[xCoord + k][yCoord] = TileType.Corr;
                            else
                                tiles[xCoord + k][yCoord] = TileType.Wall;
                            break;
                        case Direction.East:
                        case Direction.West:
                            if (k == 0)
                                tiles[xCoord][yCoord + k] = TileType.Corr;
                            else
                                tiles[xCoord][yCoord + k] = TileType.Wall;
                            break;
                    }

                    //// Set the tile at these coordinates to Floor.
                    //tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }

    }

    void SetWall()
    {
        for (int i = 1; i < tiles.Length - 1; i++)
        {
            for (int j = 1; j < tiles[i].Length - 1; j++)
            {
                if (tiles[i][j] == TileType.Wall)
                {
                    bool ifWall = false;
                    for (int xx = -1; xx < 2 && !ifWall; xx++)
                    {
                        for (int yy = -1; yy < 2 && !ifWall; yy++)
                        {
                            if (tiles[i + xx][j + yy] == TileType.Dark)
                                ifWall = true;
                        }
                    }
                    if (!ifWall)
                    {
                        if((tiles[i+1][j]==TileType.Wall && tiles[i-1][j]==TileType.Wall)||
                           (tiles[i][j-1] == TileType.Wall && tiles[i][j+1] == TileType.Wall))
                            tiles[i][j] = TileType.Corr;
                        else
                            tiles[i][j] = TileType.Floor;
                    }
                }
            }
        }
    }

    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // ... and instantiate a floor tile for it.
                if (tiles[i][j] == TileType.Floor)
                {
                    InstantiateFromArray(floorTiles, i, j);
                }
                if (tiles[i][j] == TileType.Corr)
                {
                    InstantiateFromArray(CorridorTiles, i, j);
                }
                //if (tiles[i][j] != TileType.Dark)
                //{
                //    InstantiateFromArray(floorTiles, i, j);
                //}
                // If the tile type is Wall...
                //if (tiles[i][j] == TileType.Dark)
                //{
                //    // ... instantiate a wall over the top.
                //    //InstantiateFromArray(darkTiles, i, j);
                //}
                if (tiles[i][j] == TileType.Wall)
                {
                    //InstantiateFromArray(wallTiles, i, j);
                    InstantiateWall(i, j);
                }
            }
        }
    }


    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(outerWallTiles, xCoord, currentY);

            currentY++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(outerWallTiles, currentX, yCoord);

            currentX++;
        }
    }


    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, depth);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;

        NetworkServer.Spawn(tileInstance);
    }

    void InstantiateWall(float xCoord, float yCoord)
    {
        
        int xCross = 0;
        int yCross = 0;

        if (yCoord < rows-1 && 
            (tiles[(int)xCoord][(int)yCoord + 1] == TileType.Floor || tiles[(int)xCoord][(int)yCoord + 1] == TileType.Corr))
            yCross++;//up
        if (xCoord < columns-1 &&
            (tiles[(int)xCoord+1][(int)yCoord] == TileType.Floor || tiles[(int)xCoord+1][(int)yCoord] == TileType.Corr))
            xCross++;//right
        if (yCoord > 0 &&
            (tiles[(int)xCoord][(int)yCoord - 1] == TileType.Floor || tiles[(int)xCoord][(int)yCoord - 1] == TileType.Corr))
            yCross--;//down
        if(xCoord > 0 &&
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
                InstantiateWallForPosition(xCoord, yCoord, WallType.Lu,true);
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
        if(xCross==0 && yCross==0)
        {
            xCross = -1;
            yCross = 1;
            if (xCoord != 0 && yCoord != rows-1 && 
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

    void CleanRedundantCorr()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(tiles[i][j]==TileType.Corr)
                {
                    if(!((tiles[i+1][j]==TileType.Wall && tiles[i - 1][j] == TileType.Wall) ||
                        (tiles[i][j+1] == TileType.Wall && tiles[i][j-1] == TileType.Wall)))
                    {
                        tiles[i][j] = TileType.Floor;
                    }
                    
                }
            }
        }
    }

    void initComRoom()
    {
        comRoom = new List<Vector2>[numRooms.m_Max];
        for (int i = 0; i < comRoom.Length; i++)
        {
            comRoom[i] = new List<Vector2>();
        }
    }

    void CleanRedundantRoom()
    {
        bool[][] visit = new bool[columns][];
        for (int i = 0; i < tiles.Length; i++)
        {
            visit[i] = new bool[rows];
            for (int j = 0; j < rows; j++)
                visit[i][j] = false;
        }
        
        int roomIter = 0;

        roomSize = new Vector2[comRoom.Length];

        for (int i = 0; i < columns && roomIter < numRooms.m_Max; i++)
        {
            for (int j = 0; j < rows && roomIter < numRooms.m_Max; j++)
            {
                if(tiles[i][j]==TileType.Floor && !visit[i][j])
                {
                    FloorFlood(roomIter, visit,i,j);
                    roomSize[roomIter] = IfRectRoom(i, j);
                    roomIter++;
                }
            }
        }

        


    }
    //To make the overlapping room one.
    void FloorFlood(int r, bool[][] temp, int i, int j)// r is the iter of the comRoom list. i is the x coord, j is the y coord
    {
        temp[i][j] = true;
        comRoom[r].Add(new Vector2(i, j));
        if (tiles[i + 1][j] == TileType.Floor && !temp[i + 1][j])
        {
            FloorFlood(r, temp, i + 1, j);
        }
       if (tiles[i - 1][j] == TileType.Floor && !temp[i - 1][j])
        {
            FloorFlood(r, temp, i - 1, j);
        }
        if (tiles[i][j + 1] == TileType.Floor && !temp[i][j + 1])
        {
            FloorFlood(r, temp, i, j + 1);
        }
        if (tiles[i][j - 1] == TileType.Floor && !temp[i][j - 1])
        {
            FloorFlood(r, temp, i, j - 1);
        }
        return;
    }

    Vector2 IfRectRoom(float x, float y)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].xPos == x-1 && rooms[i].yPos == y-1)
            {
                for (int j = rooms[i].xPos; j < rooms[i].xPos + rooms[i].roomWidth; j++)
                {
                    if (tiles[j][rooms[i].yPos] == TileType.Floor ||
                        tiles[j][rooms[i].yPos + rooms[i].roomHeight - 1] == TileType.Floor)
                        return new Vector2(-1, -1);
                }
                for (int k = rooms[i].yPos; k < rooms[i].yPos + rooms[i].roomHeight; k++)
                {
                    if (tiles[rooms[i].xPos][k] == TileType.Floor ||
                        tiles[rooms[i].xPos + rooms[i].roomWidth - 1][k] == TileType.Floor)
                        return new Vector2(-1, -1);
                }
                return new Vector2(rooms[i].roomWidth - 2, rooms[i].roomHeight - 2);
            }
        }
        return new Vector2(-1, -1);
    }

    void FurnishTheRooms()
    {
        int roomNumber = 0;
        foreach (var item in comRoom)
        {
            if (item.Count == 0) return;
            Vector2 pos = item[0];
            Vector2 size = IfRectRoom((int)pos.x, (int)pos.y);
            int width = (int)size.x;
            int height = (int)size.y;
            roomCreater.FurnishRoom(pos, width, height,roomNumber);
            roomNumber++;
        }
    }

    List<Vector2>[] GetHallWayAndDoor()//1 hallway,2 door
    {
        List<Vector2>[] re = new List <Vector2>[2];
        re[0] = new List<Vector2>();
        re[1] = new List<Vector2>();
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
            {
                if(tiles[i][j] == TileType.Floor)
                {
                    if (tiles[i + 1][j] == TileType.Corr)
                    {
                        re[0].Add(new Vector2(i, j));
                        re[1].Add(new Vector2(i+1,j));
                    }
                    if ( tiles[i - 1][j] == TileType.Corr )
                    {
                        re[0].Add(new Vector2(i, j));
                        re[1].Add(new Vector2(i - 1, j));
                    }
                    if (tiles[i][j-1] == TileType.Corr )
                    {
                        re[0].Add(new Vector2(i, j));
                        re[1].Add(new Vector2(i, j - 1));
                    }
                    if (tiles[i][j+1] == TileType.Corr)
                    {
                        re[0].Add(new Vector2(i, j));
                        re[1].Add(new Vector2(i , j + 1));
                    }
                }

            }
        return re;
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
        var templist = comRoom[roomNumber];
        foreach (var item in templist)
        {
            if (tiles[(int)item.x - 1][(int)item.y] == TileType.Wall ||
               tiles[(int)item.x + 1][(int)item.y] == TileType.Wall ||
               tiles[(int)item.x][(int)item.y - 1] == TileType.Wall ||
               tiles[(int)item.x][(int)item.y + 1] == TileType.Wall)
            {
                if (tiles[(int)item.x - 1][(int)item.y] == TileType.Wall &&
                    tiles[(int)item.x][(int)item.y + 1] == TileType.Wall)
                    leftup.Add(item);
                else if (tiles[(int)item.x - 1][(int)item.y] == TileType.Wall &&
                    tiles[(int)item.x][(int)item.y - 1] == TileType.Wall)
                    leftdown.Add(item);
                else if (tiles[(int)item.x + 1][(int)item.y] == TileType.Wall &&
                    tiles[(int)item.x][(int)item.y - 1] == TileType.Wall)
                    rightdown.Add(item);
                else if (tiles[(int)item.x + 1][(int)item.y] == TileType.Wall &&
                    tiles[(int)item.x][(int)item.y + 1] == TileType.Wall)
                    rightup.Add(item);

                else if (tiles[(int)item.x - 1][(int)item.y] == TileType.Wall)
                    left.Add(item);
                else if (tiles[(int)item.x + 1][(int)item.y] == TileType.Wall)
                    right.Add(item);
                else if (tiles[(int)item.x][(int)item.y + 1] == TileType.Wall)
                    up.Add(item);
                else if (tiles[(int)item.x][(int)item.y - 1] == TileType.Wall)
                    down.Add(item);
                else
                    inter.Add(item);//doesnt exist
            }
            else
                inter.Add(item);
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
        var templist2 = GetHallWayAndDoor();
        result[9] = templist2[0];
        result[10] = templist2[1];
        return result;
    }
}