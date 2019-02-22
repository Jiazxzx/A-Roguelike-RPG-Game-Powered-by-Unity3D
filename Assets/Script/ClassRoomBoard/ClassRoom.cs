using UnityEngine;

public class ClassRoom
{
    public int xPos_grid;
    public int yPos_grid;
    public int xPos;                          // The x coordinate of the lower left tile of the roomGrid.
    public int yPos;                          // The y coordinate of the lower left tile of the roomGrid.

    public int roomWidth;                     // How many tiles wide the room is.
    public int roomHeight;                    // How many tiles high the room is.
    public RoomType type;

    public void SetupRoom(IntRange widthRange, IntRange heightRange, int grid_x, int grid_y, int randType)
    {
        type = (RoomType)randType;

        xPos_grid = grid_x;
        yPos_grid = grid_y;

        switch (type)
        {
            case RoomType.CorrNode:
                SetupCorrNode();
                break;
            case RoomType.SmallRoom:
                SetupSmallRoom(widthRange, heightRange);
                break;
            case RoomType.BigRoom:
                SetupBigRoom(widthRange, heightRange);
                break;
        }
    }

    void SetupSmallRoom(IntRange widthRange, IntRange heightRange)
    {
        // Set a random width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;
        // Set the x and y coordinates so the room is roughly in the middle of the board.
        xPos = Mathf.RoundToInt(xPos_grid * ClassBoardCreater.roomScale + ClassBoardCreater.roomScale / 2 - roomWidth / 2);
        yPos = Mathf.RoundToInt(yPos_grid * ClassBoardCreater.roomScale + ClassBoardCreater.roomScale / 2 - roomHeight / 2);
    }

    void SetupCorrNode()
    {
        roomWidth = 1;
        roomHeight = 1;

        xPos = Mathf.RoundToInt(xPos_grid * ClassBoardCreater.roomScale + ClassBoardCreater.roomScale / 2 + 1);
        yPos = Mathf.RoundToInt(yPos_grid * ClassBoardCreater.roomScale + ClassBoardCreater.roomScale / 2 + 1);
    }

    void SetupBigRoom(IntRange widthRange, IntRange heightRange)
    {
        roomWidth = widthRange.Random ;
        roomHeight = heightRange.Random;

        xPos = Mathf.RoundToInt((xPos_grid + 1) * ClassBoardCreater.roomScale - roomWidth / 2);
        yPos = Mathf.RoundToInt((yPos_grid + 1) * ClassBoardCreater.roomScale - roomHeight / 2);
    }
}