using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetManager : NetworkManager
{
    private NetworkDiscovery nwd;
    private TimeoutControl toc;
    private bool ServerFound;
    private Text ErrorMsgText;
    private Button JoinBtn;
    private bool HasGameJudger;
    private GameObject judger;
    private Vector3 initPoint = new Vector3(-13, -10);
    public GameObject gjd;


    public void Start()
    {
        nwd = GetComponent<CustomNetworkDiscovery>();
        ErrorMsgText = GameObject.Find("MessageText").GetComponent<Text>();
        ServerFound = false;
        HasGameJudger = false;
       toc = GetComponent<TimeoutControl>();
        JoinBtn = GameObject.Find("JoinGameBtn").GetComponent<Button>();

    }


    //房主正式开始游戏
    public void StartGame()
    {
        Debug.Log("Entering main map!");
    }


    //搜索服务器，监听服务器的广播
    //客户端执行
    public void SearchGame()
    {
        nwd.Initialize();
        nwd.broadcastPort = 7777;
        nwd.StartAsClient();

        toc.IsSearching = true;

        ErrorMsgText.text = "SEARCHING GAME";

        Debug.Log("Searching Game");
    }



    //建立服务器
    //服务器端执行
    public void StartGameServer()
    {
        Debug.Log("Start Server");

        //服务器所使用的端口
        base.networkAddress = Network.player.ipAddress;
        base.networkPort = 7776;

        //广播端口，发送与监听广播所用端口
        nwd.broadcastPort = 7777;
        nwd.Initialize();

        string data = NetworkManager.singleton.networkPort.ToString();
        nwd.broadcastData = data;

        base.StartHost();
        nwd.StartAsServer();

    }


    public void SearchTimeout()
    {
        ErrorMsgText.text = "NO GAME FOUND";
        nwd.Initialize();
    }


    //客户端调用
    //客户端出现错误时
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        ErrorMsgText.text = "NETWORK ERROR";
        base.OnClientError(conn, errorCode);
        nwd.Initialize();
    }


    //客户端调用
    //服务器做Host时也会调用
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client Connected: " + conn);
        IntegerMessage msg = new IntegerMessage(GetComponent<CharacterSelect>().CurrentIndex);

        if (!clientLoadedScene)
        {
            ClientScene.AddPlayer(conn, 0, msg);
        }
        Debug.Log("Client Add Player");


        if (nwd.isClient)
        {
            nwd.StopBroadcast();
        }
    }



    //客户端调用
    //当客户端加载场景时
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        IntegerMessage msg = new IntegerMessage(GetComponent<CharacterSelect>().CurrentIndex);
        ClientScene.Ready(conn);

        bool addPlayer = (ClientScene.localPlayers.Count == 0);
        bool foundPlayer = false;
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].gameObject != null)
            {
                foundPlayer = true;
                break;
            }
        }

        if (!foundPlayer)
        {
            // there are players, but their game objects have all been deleted
            addPlayer = true;
        }

        if (addPlayer)
        {
            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
        }
    }


    //服务器端调用
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {

        int CharacterIndex = 0;
        if (extraMessageReader != null)
        {
            var i = extraMessageReader.ReadMessage<IntegerMessage>();
            CharacterIndex = i.value;
        }

        playerPrefab = GetComponent<CharacterSelect>().CharacterList[CharacterIndex];

        var player = Instantiate(playerPrefab, initPoint, Quaternion.identity) as GameObject;
        initPoint += new Vector3(8, 0, 0);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        if (!HasGameJudger)
        {
            judger = Instantiate(gjd, Vector3.zero, Quaternion.identity) as GameObject;
            HasGameJudger = true;
        }


        Debug.Log(judger.name);

        GameObject.FindGameObjectWithTag("Judger").GetComponent<GameJudger>().AddNewPlayer(player);

    }

}
