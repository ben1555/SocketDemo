using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;


public class SocketClientDemo1 : MonoBehaviour {

    private Socket client = null;
    private string ip = "127.0.0.1";
    private int port = 8989;

    //private int size = 1024;
    private byte[] readData = new byte[1024];
    private byte[] data = new byte[1024];

    // Use this for initialization
    void Start () {
        //多socket 复用同一端口
        //socket2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); 

        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint address = new IPEndPoint(IPAddress.Parse(ip), port);

        //建立异步连接服务 , Connected 进行监听
        client.BeginConnect(address, new AsyncCallback(Connected), null); 
    }
	
	// Update is called once per frame
	void Update () {
        startReceive(); //这步很重要，，，不然会收不到服务器发过来的消息
    }

    void Connected(IAsyncResult iar)    //建立连接
    {
        //Socket client = (Socket)iar.AsyncState;
        client.EndConnect(iar);
        //client.BeginReceive(data, 0, size, SocketFlags.None, new AsyncCallback(ReceiveData), client);
        Debug.Log("建立连接");
    }

    void Send(string str)
    {
        byte[] msg = Encoding.UTF8.GetBytes(str);
        client.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(SendData), client);    //开始发送
    }

    void SendData(IAsyncResult iar) //发送数据
    {
        Socket remote = (Socket)iar.AsyncState;
        //int sent = remote.EndSend(iar);         //关闭发送
        remote.EndSend(iar);         //关闭发送
        remote.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), remote);   //开始接收
    }


    bool ReceiveFlag = true;
    void startReceive()
    {
        if (ReceiveFlag)
        {
            ReceiveFlag = false;
            client.BeginReceive(readData, 0, readData.Length, SocketFlags.None, new AsyncCallback(endReceive), client);
        }
    }

    void endReceive(IAsyncResult iar) //接收数据
    {
        ReceiveFlag = true;
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            string stringData = Encoding.UTF8.GetString(readData, 0, recv);
            text2 += "\n" + "接收服务器数据:+++++++++++++++" + stringData;
        }

    }
    void ReceiveData(IAsyncResult iar) //接收数据
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);          //关闭接收 注意：如果关闭了接收，就不能接收了 测试是这样

        string stringData = Encoding.UTF8.GetString(data, 0, recv);
        text2 += "\n" + "回收发送数据:+++++++++++++++" + stringData;

    }


    void CloseSocket()                  //关闭socket
    {
        if (client.Connected)
        {
            Debug.Log("关闭socket");
            client.Close();
        }
    }
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    string text = "";
    string text2 = "";
    Vector2 p = new Vector2(600, 300);
    void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(500));
        text = GUILayout.TextField(text);
        if (GUILayout.Button("发送数据"))
        {
            Send(text);
        }
        GUILayout.BeginScrollView(p);
        text2 = GUILayout.TextArea(text2, GUILayout.Height(300));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

    }



}
