using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/*
  A*算法
  公式表示为： f(n)=g(n)+h(n),
  其中， f(n) 是从初始状态经由状态n到目标状态的代价估计，
  g(n) 是在状态空间中从初始状态到状态n的实际代价，
  h(n) 是从状态n到目标状态的最佳路径的估计代价。
  （对于路径搜索问题，状态就是图中的节点，代价就是距离, 但此距离在游戏中经过优化处理）
*/

public class Pathfinding : NetworkBehaviour
{
    public ArrayList pList = new ArrayList();
    public Transform seeker, target;        //起始和目标
    Grid grid;                              //创建寻路网格
    public float speed;
    public int EnemyType;  //1为近战2为远程
    public int minDis;
    public int maxDis;

    void Start()
    {
        seeker = this.transform;
        //grid = this.transform.parent.GetComponent<Grid>();
        //grid = GetComponent<Grid>();
    }

    void LateUpdate()
    {
        if (!Physics2D.OverlapArea(GetComponent<Grid>().pos1, GetComponent<Grid>().pos2, GetComponent<Grid>().playerMask)) return;
        if (!isServer) return;
        target = ClosestTarget();
        //grid = this.transform.parent.GetComponent<Grid>();
        grid = GetComponent<Grid>();
        if (target != null)
        {
            FindPath(seeker.position - seeker.position, target.position - seeker.position); //每帧调用一次,实时动态规划路线及移动  
            PathMoving(seeker.position, target.position);
        }
        MonsterAction(target);
    }

    private Transform ClosestTarget()
    {
        pList = GameObject.FindWithTag("Judger").GetComponent<GameJudger>().playerList;
        if (pList.Count >= 1)
        {
            Transform temp = null;
            float dis = (((GameObject)pList[0]).transform.position - seeker.position).magnitude;
            foreach (GameObject player in pList)
            {
                if (((player.transform.position - seeker.position).magnitude <= dis) && (!player.GetComponent<PlayerStatusControl>().IsHidden))
                {
                    dis = (player.transform.position - seeker.position).magnitude;
                    temp = player.transform;
                }
            }
            return temp;
        }
        return null;
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>(); //表示待处理的节点
        HashSet<Node> closedSet = new HashSet<Node>(); //表示已经最优计算节点,无需再处理
        openSet.Add(startNode); //预处理

        //循环寻找节点 直至openSet中没有元素,此时终点已到达,最佳路径已经找到
        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //处理neighbour节点 相邻的定义在Grid类中定义
            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //得到最终路径path 类型为List
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

    }

    //hCost代价预测
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void PathMoving(Vector3 startPos, Vector3 targetPos)
    {
        if (grid.path == null) return;
        if (grid.path.Count >= minDis && grid.path.Count <= maxDis)
        {
            Vector3 start = seeker.position;
            Vector3 end = grid.path[0].worldPosition;
            seeker.position = Vector3.MoveTowards(start, end, speed * Time.deltaTime);
        }
    }

    void MonsterAction(Transform target)
    {
        //根据怪物不同属性/tag来调用不同的脚本
        if (EnemyType == 1)
            GetComponent<JerryMeleeAction>().Action(target);
        if (EnemyType == 2)
            GetComponent<JerryRangedAction>().Action(target);
        if (EnemyType == 3)
            GetComponent<JerryBossAction>().Action(target);
    }

}
