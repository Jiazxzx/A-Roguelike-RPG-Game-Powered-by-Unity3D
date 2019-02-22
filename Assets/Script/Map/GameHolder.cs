using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单利模式管理类
public class GameHolder
{
    // 是否初始化
    public static bool isInit = false;
    // 是否第一次进入房间
    public static bool[] first_in = new bool[4];
    // 房间类型 0_未支配 1_教室 2_次所
    public static int[] room_type = new int[4];
    // 教室出口位置
    public static Vector3[] classroom_pos = { new Vector3(-6.5f, -2.9f), new Vector3(-0.25f, 4.0f),
        new Vector3(6.5f, 3.5f), new Vector3(0.94f, -2.9f) };
    // 浴室出口位置
    public static Vector3[] bathroom_pos ={ new Vector3(-3.65f, -0.02f), new Vector3(-0.05f, 3.9f),
        new Vector3(3.65f, -0.14f), new Vector3(0.15f, -2.9f) };
    // 房间是否可以到达
    private static bool[] reachable = new bool[4];
    // 房间对应门号连接的房间 0-左侧门 1-上侧门 2-右侧门 3-下侧门
    public static int[,] linkroom = new int[4, 4];
    // 标记两个房间是否已经连接
    private static bool[,] islinked = new bool[4, 4];
    // 初始化
    public static void InitHolder()
    {
        if (isInit == false)
        {
            // 初始化
            for (int i = 0; i < 4; i++)
            {
                first_in[i] = true;
                room_type[i] = Random.Range(0,100)%2+1;
            }
            // 将连接房间初始化为自身
            for (int i = 0; i < 4; i++)//房间号
            {
                for (int j = 0; j < 4; j++)//门号
                {
                    linkroom[i, j] = i;
                    islinked[i, j] = false;
                }
            }
            // 构建房间连接
            for (int i = 0; i < 4; i++)// 房间号
            {
                for (int j = 0; j < 4; j++)// 门号
                {
                    if ((Random.Range(0, 100) % 2 == 1)/*是否要添加连接*/&& (linkroom[i, j] == i)/*对应门是否已经连接*/)
                    {
                        switch (j)// 选择门号
                        {
                            case 0:
                                // 随机在所有房间中选择一个，如果选择的房间号等于本房间号，则重新选择
                                int tolink = Random.Range(0, 3);
                                while (tolink == i) tolink = Random.Range(0, 3);
                                // 如果两个房间未连接，则允许建立连接
                                if (!islinked[i, tolink])
                                {
                                    // 如果对应门号无连接，则建立连接
                                    if (linkroom[tolink, 2] == tolink)
                                    {
                                        linkroom[i, j] = tolink;
                                        linkroom[tolink, 2] = i;
                                        reachable[i] = reachable[tolink] = true;
                                        islinked[i, tolink] = true;
                                        islinked[tolink, i] = true;
                                    }
                                }
                                break;
                            case 1:
                                int tolink2 = Random.Range(0, 3);
                                while (tolink2 == i) tolink2 = Random.Range(0, 3);
                                if (!islinked[i, tolink2])
                                {
                                    if (linkroom[tolink2, 3] == tolink2)
                                    {
                                        linkroom[i, j] = tolink2;
                                        linkroom[tolink2, 3] = i;
                                        reachable[i] = reachable[tolink2] = true;
                                        islinked[i, tolink2] = true;
                                        islinked[tolink2, i] = true;
                                    }
                                }
                                break;
                            case 2:
                                int tolink3 = Random.Range(0, 3);
                                while (tolink3 == i) tolink3 = Random.Range(0, 3);
                                if (!islinked[i, tolink3])
                                {
                                    if (linkroom[tolink3, 0] == tolink3)
                                    {
                                        linkroom[i, j] = tolink3;
                                        linkroom[tolink3, 0] = i;
                                        reachable[i] = reachable[tolink3] = true;
                                        islinked[i, tolink3] = true;
                                        islinked[tolink3, i] = true;
                                    }
                                }
                                break;
                            case 3:
                                int tolink4 = Random.Range(0, 3);
                                while (tolink4 == i) tolink4 = Random.Range(0, 3);
                                if (!islinked[i, tolink4])
                                {
                                    if (linkroom[tolink4, 1] == tolink4)
                                    {
                                        linkroom[i, j] = tolink4;
                                        linkroom[tolink4, 1] = i;
                                        reachable[i] = reachable[tolink4] = true;
                                        islinked[i, tolink4] = true;
                                        islinked[tolink4, i] = true;
                                    }
                                }
                                break;
                        }
                    }
                }
                if (!reachable[i])// 随机四次后若仍无连接则强制连接
                {
                    for (int m = 0; m < 4; m++)// 房间号
                    {
                        for (int n = 0; n < 4; n++)// 门号
                        {
                            if (m == i) break;// 房间号等于自身则跳出
                            if (linkroom[m, n] == m)// 对应房间允许连接
                            {
                                switch (n)
                                {
                                    case 0:
                                        if (!islinked[i, m])
                                        {
                                            linkroom[i, 2] = m;
                                            linkroom[m, 0] = i;
                                            reachable[i] = reachable[m] = true;
                                            islinked[i, m] = true;
                                            islinked[m, i] = true;
                                        }
                                        break;
                                    case 1:
                                        if (!islinked[i, m])
                                        {
                                            linkroom[i, 3] = m;
                                            linkroom[m, 1] = i;
                                            reachable[i] = reachable[m] = true;
                                            islinked[i, m] = true;
                                            islinked[m, i] = true;
                                        }
                                        break;
                                    case 2:
                                        if (!islinked[i, m])
                                        {
                                            linkroom[i, 0] = m;
                                            linkroom[m, 2] = i;
                                            reachable[i] = reachable[m] = true;
                                            islinked[i, m] = true;
                                            islinked[m, i] = true;
                                        }
                                        break;
                                    case 3:
                                        if (!islinked[i, m])
                                        {
                                            linkroom[i, 1] = m;
                                            linkroom[m, 3] = i;
                                            reachable[i] = reachable[m] = true;
                                            islinked[i, m] = true;
                                            islinked[m, i] = true;
                                        }
                                        break;
                                }
                            }
                            if (reachable[i]) break;
                        }
                        if (reachable[i]) break;
                    }
                }
            }
            isInit = true;
        }
    }
}
