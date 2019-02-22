using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class CustomNetworkDiscovery : NetworkDiscovery
{

    private NetworkManager nwm;
    private TimeoutControl toc;
    private bool isConnected;
    private bool isBroadcasting;

    void Start()
    {
        Initialize();
        base.showGUI = false;
        nwm = GetComponent<NetworkManager>();
        toc = GetComponent<TimeoutControl>();
        isConnected = false;
        isBroadcasting = true;
    }

    //接收到广播时执行内容
    //客户端执行
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        
        LanConnectionInfo GameInfo = new LanConnectionInfo(fromAddress, data);

        Debug.Log("Broadcast Received Server: " + GameInfo.ipAddress + " Port : " + GameInfo.port);


        if ( !isConnected)
        {
            Debug.Log("Connect");
            nwm.networkPort = GameInfo.port;
            nwm.networkAddress = GameInfo.ipAddress;
            nwm.StartClient();

            toc.IsSearching = false;

            isConnected = true;
        }

    }
}


