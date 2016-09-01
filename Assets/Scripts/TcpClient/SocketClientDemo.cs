using UnityEngine;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

public class SocketClientDemo : MonoBehaviour {

    //ip地址
    private static readonly string ipAddress = "127.0.0.1";
    //端口
    private static readonly int port = 10001;
    TcpClient client;

    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Client();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SimulateMultiUserClient();
        }
    }

    private void Client()
    { 
        client = new TcpClient();
        try
        {
            //同步方法，连接成功、抛出异常、服务器不存在等之前程序会被阻塞  
            client.Connect(IPAddress.Parse(ipAddress), port);
        }
        catch (Exception ex)
        {
            Debug.Log("客户端连接异常：" + ex.Message);
        }
        Debug.Log("LocalEndPoint = " + client.Client.LocalEndPoint + 
            ". RemoteEndPoint = " + client.Client.RemoteEndPoint);

        //客户端发送数据部分  
        for (int i = 0; i < 2; i++)
        {
            try
            {
                string msg = "hello server i am No " + i;
                NetworkStream streamToServer = client.GetStream();//获得客户端的流  
                byte[] buffer = Encoding.Unicode.GetBytes(msg);//将字符串转化为二进制  
                streamToServer.Write(buffer, 0, buffer.Length);//将转换好的二进制数据写入流中并发送  
                Debug.Log("发出消息：" + msg);
            }
            catch (Exception ex)
            {
                Debug.Log("服务端产生异常：" + ex.Message);
            }
        }
    }

    private void SimulateMultiUserClient()
    {
        for (int i = 0; i < 2; i++)
        {
            client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Parse(ipAddress), port);//连接到服务器端  
            }
            catch (Exception ex)//抛出连接错误的异常  
            {
                Debug.Log("客户端连接异常：" + ex.Message);
            }
            Debug.Log("LocalEndPoint = " + client.Client.LocalEndPoint + 
                ". RemoteEndPoint = " + client.Client.RemoteEndPoint);
            //客户端发送数据部分  
            string msg = "hello server i am No " + i;
            NetworkStream streamToServer = client.GetStream();//获得客户端的流  
            byte[] buffer = Encoding.Unicode.GetBytes(msg);//将字符串转化为二进制  
            streamToServer.Write(buffer, 0, buffer.Length);//将转换好的二进制数据写入流中并发送  
            Debug.Log("发出消息：" + msg);
        }
    }

    private void SendMsgToServer(string content)
    {

    }

}
