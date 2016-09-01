using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class Demo : MonoBehaviour {

    string editString = "hello wolrd"; //�༭������

    Socket socket; //Ŀ��socket
    EndPoint serverEnd; //�����
    IPEndPoint ipEnd; //����˶˿�
    string recvStr; //���յ��ַ���
    string sendStr; //���͵��ַ���
    byte[] recvData = new byte[1024]; //���յ����ݣ�����Ϊ�ֽ�
    byte[] sendData = new byte[1024]; //���͵����ݣ�����Ϊ�ֽ�
    int recvLen; //���յ����ݳ���
    Thread connectThread; //�����߳�

    //��ʼ��
    void InitSocket()
    {
        //�������ӵķ�����ip�Ͷ˿ڣ������Ǳ���ip����������������
        ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
        //�����׽�������,�����߳��ж���
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //��������
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending Tcp Stream");

        //������ʼ���ӣ����ǳ���Ҫ����һ�����ӳ�ʼ����serverEnd��������յ���Ϣ
        SocketSend("hello");

        //����һ���߳����ӣ�����ģ��������߳̿���
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr)
    {
        //��շ��ͻ���
        sendData = new byte[1024];
        //��������ת��
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //���͸�ָ�������
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    //����������
    void SocketReceive()
    {
        //�������ѭ��
        while (true)
        {
            //��data����
            recvData = new byte[1024];
            //��ȡ�ͻ��ˣ���ȡ����˶����ݣ������ø�����˸�ֵ��ʵ���Ϸ�����Ѿ�����ò�����Ҫ��ֵ
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString()); //��ӡ�������Ϣ
            //������յ�������
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
        }
    }

    //���ӹر�
    void SocketQuit()
    {
        //�ر��߳�
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //���ر�socket
        if (socket != null)
            socket.Close();
    }



    // Use this for initialization
    void Start () {
        InitSocket(); //�������ʼ��
    }

    void OnGUI()
    {
        editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
        if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
            SocketSend(editString);
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }

}