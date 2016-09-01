using UnityEngine;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class SocketServerDemo : MonoBehaviour {

    //定义启动socket的线程  
    private Thread serverThread;
    //ip地址
    private static readonly string ipAddress = "127.0.0.1";
    //端口
    private static readonly int port = 10001;

	// Use this for initialization
	void Start () {
        serverThread = new Thread(StartServer);
        serverThread.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void StartServer()
    {
        const int bufferSize = 2048;//缓存大小,2048字节
        IPAddress ip = IPAddress.Parse(ipAddress);
        TcpListener tListener = new TcpListener(ip,port);
        tListener.Start();
        Debug.Log("Socket服务器监听启动......");

        TcpClient remoteClient = tListener.AcceptTcpClient();//接收已连接的客户端,阻塞方法  
        Debug.Log("客户端已连接！local:" + remoteClient.Client.LocalEndPoint +
            "<---Client:" + remoteClient.Client.RemoteEndPoint);

        NetworkStream streamToClient = remoteClient.GetStream();

        while (true)
        {
            try  //直接关掉客户端，服务器端会抛出异常  
            {
                //定义一个缓存buffer数组 
                byte[] buffer = new byte[bufferSize];
                // 将数据搞入缓存中（有朋友说read()是阻塞方法，测试中未发现程序阻塞）  
                int byteRead = streamToClient.Read(buffer, 0, bufferSize);
                if (byteRead == 0)//连接断开，或者在TCPClient上调用了Close()方法，或者在流上调用了Dispose()方法。  
                {
                    Debug.Log("客户端连接断开......");
                    break;
                }

                //从二进制转换为字符串对应的客户端会有从字符串转换为二进制的方法  
                string msg = Encoding.Unicode.GetString(buffer, 0, byteRead);
                Debug.Log("接收数据：" + msg + ".数据长度:[" + byteRead + "byte]");
            }
            catch (System.Exception ex)
            {
                Debug.Log("客户端异常：" + ex.Message);
                break;
            }

        }
    }

    void OnApplicationQuit()
    {
        serverThread.Abort();//在程序结束时杀掉线程，经测试不擦第二次启动unity会无响应  
    }

}
