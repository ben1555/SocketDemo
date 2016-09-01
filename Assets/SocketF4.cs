using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketF4 {

    private static Socket clientSocket;

    //第一部分简单示例:连接服务器Ip 端口,发送请求,接收数据同理.
    public static void SocketConnect(string serverIP, int serverPort)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(serverIP),serverPort);
        try
        {
            clientSocket.Connect(ipe);
            //clientSocket.Connect(serverIP, serverPort);
            //clientSocket.Connect(IPAddress.Parse(serverIP), serverPort);
            Debug.Log(" Connect Success IP: " + serverIP + " Port : " + serverPort.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            //throw e;
        }
    }


    //发送数据
    public static void Send(byte[] bytes)
    {
        if (clientSocket == null)
            return;
        if (!clientSocket.Connected)
            return;

        if (clientSocket.Poll(0, SelectMode.SelectWrite))
        {
            try
            {
                clientSocket.Send(bytes);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }


    //第二部分请求数据示例:
    public void _MSG_ACCOUNT(string name, string pwd)
    {
        
    }


}
