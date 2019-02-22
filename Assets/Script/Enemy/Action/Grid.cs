using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Grid : NetworkBehaviour
{

    public LayerMask unwalkableMask; //不能走的层
    public LayerMask playerMask; //玩家所在的层
    public Vector2 gridWorldSize; //节点实际全局位置
    public float nodeRadius; //节点尺寸/半径
    Node[,] grid; //节点格式

    float nodeDiameter; //节点尺寸/直径
    int gridSizeX, gridSizeY; //节点抽象化X/Y坐标

    public Vector3 pos1; //矩形的一个角
    public Vector3 pos2; //矩形的对角
    bool mtfk = false;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        pos1 = new Vector3(this.transform.position.x - gridWorldSize.x / 2,
            this.transform.position.y - gridWorldSize.y / 2,
            this.transform.position.z);

        pos2 = new Vector3(this.transform.position.x + gridWorldSize.x / 2,
           this.transform.position.y + gridWorldSize.y / 2,
           this.transform.position.z);

        mtfk = true;
    }

    void Update()
    {
        if(!mtfk)
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            pos1 = new Vector3(this.transform.position.x - gridWorldSize.x / 2,
                this.transform.position.y - gridWorldSize.y / 2,
                this.transform.position.z);

            pos2 = new Vector3(this.transform.position.x + gridWorldSize.x / 2,
               this.transform.position.y + gridWorldSize.y / 2,
               this.transform.position.z);

            mtfk = true;
        }



        if (!Physics2D.OverlapArea(pos1, pos2, playerMask)) return;
        CreateGrid(); //根据地形的变化动态更新地图
    }

    //创建网格
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask)); //碰撞检测 walkable能走和unwalkable不能走网格判断
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        //规定neighbour
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    //世界坐标到节点的转换
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;
    //Scene视图高亮显示障碍/路径
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

    //    if (grid != null)
    //    {
    //        foreach (Node n in grid)
    //        {
    //            Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //            if (path != null)
    //                if (path.Contains(n))
    //                    Gizmos.color = Color.black;
    //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
    //        }
    //    }
    //}
}