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
        //��socket ����ͬһ�˿�
        //socket2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); 

        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint address = new IPEndPoint(IPAddress.Parse(ip), port);

        //�����첽���ӷ��� , Connected ���м���
        client.BeginConnect(address, new AsyncCallback(Connected), null); 
        base.
    }
	
	// Update is called once per frame
	void Update () {
        startReceive(); //�ⲽ����Ҫ��������Ȼ���ղ�������������������Ϣ
    }

    void Connected(IAsyncResult iar)    //��������
    {
        //Socket client = (Socket)iar.AsyncState;
        client.EndConnect(iar);
        //client.BeginReceive(data, 0, size, SocketFlags.None, new AsyncCallback(ReceiveData), client);
        Debug.Log("��������");
    }

    void Send(string str)
    {
        byte[] msg = Encoding.UTF8.GetBytes(str);
        client.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(SendData), client);    //��ʼ����
    }

    void SendData(IAsyncResult iar) //��������
    {
        Socket remote = (Socket)iar.AsyncState;
        //int sent = remote.EndSend(iar);         //�رշ���
        remote.EndSend(iar);         //�رշ���
        remote.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), remote);   //��ʼ����
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

    void endReceive(IAsyncResult iar) //��������
    {
        ReceiveFlag = true;
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            string stringData = Encoding.UTF8.GetString(readData, 0, recv);
            text2 += "\n" + "���շ���������:+++++++++++++++" + stringData;
        }

    }
    void ReceiveData(IAsyncResult iar) //��������
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);          //�رս��� ע�⣺����ر��˽��գ��Ͳ��ܽ����� ����������

        string stringData = Encoding.UTF8.GetString(data, 0, recv);
        text2 += "\n" + "���շ�������:+++++++++++++++" + stringData;

    }


    void CloseSocket()                  //�ر�socket
    {
        if (client.Connected)
        {
            Debug.Log("�ر�socket");
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
        if (GUILayout.Button("��������"))
        {
            Send(text);
        }
        GUILayout.BeginScrollView(p);
        text2 = GUILayout.TextArea(text2, GUILayout.Height(300));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

    }



}
