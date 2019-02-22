using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameJudger : NetworkBehaviour
{
    // mode1 大逃杀  至少2人
    // mode2 尸潮    至少1人
    // mode3 BOSS    至少1人

    [SerializeField] int gameMode = 1;
    [SerializeField] bool gameIsLost = false;
    [SerializeField] bool gameIsWin = false;

     public ArrayList playerList;
    [SerializeField] int playerCounter = 0;
    [SerializeField] int playerAmount = 0;

    // 临时使用
    [SerializeField] private bool gameStarted = false;


    // 可以移动到Network Manager， 使用enable调用
    public override void OnStartServer()
    {
        playerList = new ArrayList();
        Debug.Log("仲裁者创建");
    }
    public void AddNewPlayer(GameObject newPlayer)
    {
        Debug.Log("添加了新玩家");
        if (playerList == null) playerList = new ArrayList();
        playerList.Add(newPlayer);
        playerCounter++;

        if (playerCounter >= 2)
        {
            StartGame();
        }
    }
    public void StartGame()
    {
        gameStarted = true;
        playerAmount = playerCounter;

        // 随机选择游戏模式
        switch (playerAmount)
        {
            case 1:
                break;
            case 2:
                gameMode = Random.Range(1,1);
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }


    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        if(gameStarted)
        {
            if (gameIsWin == false && gameIsLost == false)
            {
                foreach (GameObject player in playerList)
                {
                    if (player.GetComponent<PlayerStatusControl>().IsDead == true)
                    {
                        CmdSetLost(player);
                        playerAmount--;                      
                    }
                }
            }

            switch (gameMode)
            {
                case 1:
                    if (playerAmount == 1)
                    {
                        gameIsWin = true;
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            if(gameIsWin)
            {
                foreach (GameObject player in playerList)
                {
                    if (player.GetComponent<PlayerStatusControl>().IsDead == false)
                    {
                        CmdSetWin(player);
                    }
                }
            }

            if (gameIsLost)
            {

            }
        }
    }

    [Command]
    void CmdSetWin(GameObject winplayer)
    {
        // 此处应更改为调用SceneManager函数
        winplayer.GetComponent<PlayerStatusControl>().gameWin = true;
    }

    [Command]
    void CmdSetLost(GameObject lostplayer)
    {
        lostplayer.GetComponent<PlayerStatusControl>().gameLost = true;
    }
}
