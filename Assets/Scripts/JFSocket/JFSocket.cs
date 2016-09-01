using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class JFSocket
{

    //Socket�ͻ��˶���
    private Socket clientSocket;
    //JFPackage.WorldPackage���ҷ�װ�Ľṹ�壬
    //���������������ʱ��ᴫ������ṹ��
    //���ͻ��˽ӵ������������ص����ݰ�ʱ���Ұѽṹ��add���������С�
    public List<JFPackage.WorldPackage> worldpackage;
    //����ģʽ
    private static JFSocket instance;
    public static JFSocket GetInstance()
    {
        if (instance == null)
        {
            instance = new JFSocket();
        }
        return instance;
    }

    //�����Ĺ��캯��
    JFSocket()
    {
        //����Socket���� �����ҵ�����������TCP
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //������IP��ַ
        IPAddress ipAddress = IPAddress.Parse("192.168.1.100");
        //�������˿�
        IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, 10060);
        //����һ���첽�Ľ������ӣ������ӽ����ɹ�ʱ����connectCallback����
        IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), clientSocket);
        //������һ����ʱ�ļ�⣬�����ӳ���5�뻹û�ɹ���ʾ��ʱ
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success)
        {
            //��ʱ
            Closed();
            Debug.Log("connect Time Out");
        }
        else
        {
            //��socket�������ӳɹ��������߳̽��ܷ�������ݡ�
            worldpackage = new List<JFPackage.WorldPackage>();
            Thread thread = new Thread(new ThreadStart(ReceiveSorket));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void connectCallback(IAsyncResult asyncConnect)
    {
        Debug.Log("connectSuccess");
    }

    private void ReceiveSorket()
    {
        //������߳��н��ܷ��������ص�����
        while (true)
        {

            if (!clientSocket.Connected)
            {
                //��������Ͽ���������ѭ��
                Debug.Log("Failed to clientSocket server.");
                clientSocket.Close();
                break;
            }
            try
            {
                //�������ݱ�����bytes����
                byte[] bytes = new byte[4096];
                //Receive�����л�һֱ�ȴ�����˻ط���Ϣ
                //���û�лط���һֱ��������š�
                int i = clientSocket.Receive(bytes);
                if (i <= 0)
                {
                    clientSocket.Close();
                    break;
                }

                //���������ɸ������������жϡ�
                //��Ϊ��Ŀǰ����Ŀ��Ҫ����ͷ���ȣ�
                //�ҵİ�ͷ������2��������������һ���ж�
                if (bytes.Length > 2)
                {
                    SplitPackage(bytes, 0);
                }
                else
                {
                    Debug.Log("length is not  >  2");
                }

            }
            catch (Exception e)
            {
                Debug.Log("Failed to clientSocket error." + e);
                clientSocket.Close();
                break;
            }
        }
    }

    private void SplitPackage(byte[] bytes, int index)
    {
        //��������в������Ϊһ�η��ص����ݰ��������ǲ�����
        //������Ҫ�����ݰ����в�֡�
        while (true)
        {
            //��ͷ��2���ֽ�
            byte[] head = new byte[2];
            int headLengthIndex = index + 2;
            //�����ݰ���ǰ�����ֽڿ�������
            Array.Copy(bytes, index, head, 0, 2);
            //�����ͷ�ĳ���
            short length = BitConverter.ToInt16(head, 0);
            //����ͷ�ĳ��ȴ���0 ��ô��Ҫ���ΰ���ͬ���ȵ�byte���鿽������
            if (length > 0)
            {
                byte[] data = new byte[length];
                //�������������ȫ���ֽ���
                Array.Copy(bytes, headLengthIndex, data, 0, length);
                //�����ݰ��е��ֽ�����ǿ��ת�������ݰ��Ľṹ��
                //BytesToStruct()������������ת����
                //������Ҫ�����ǵķ���˳���������
                JFPackage.WorldPackage wp = new JFPackage.WorldPackage();
                wp = (JFPackage.WorldPackage)BytesToStruct(data, wp.GetType());
                //��ÿ�����Ľṹ���������������С�
                worldpackage.Add(wp);
                //������ָ����һ�����İ�ͷ
                index = headLengthIndex + length;

            }
            else
            {
                //�����ͷΪ0��ʾû�а��ˣ���ô����ѭ��
                break;
            }
        }
    }

    //�����˷���һ���ַ���
    //һ�㲻�ᷢ���ַ��� Ӧ���Ƿ������ݰ�
    public void SendMessage(string str)
    {
        byte[] msg = Encoding.UTF8.GetBytes(str);

        if (!clientSocket.Connected)
        {
            clientSocket.Close();
            return;
        }
        try
        {
            //int i = clientSocket.Send(msg);
            IAsyncResult asyncSend = clientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                clientSocket.Close();
                Debug.Log("Failed to SendMessage server.");
            }
        }
        catch
        {
            Debug.Log("send message error");
        }
    }

    //�����˷������ݰ���Ҳ����һ���ṹ�����
    public void SendMessage(object obj)
    {

        if (!clientSocket.Connected)
        {
            clientSocket.Close();
            return;
        }
        try
        {
            //�ȵõ����ݰ��ĳ���
            short size = (short)Marshal.SizeOf(obj);
            //�����ݰ��ĳ���д��byte������
            byte[] head = BitConverter.GetBytes(size);
            //�ѽṹ�����ת�������ݰ���Ҳ�����ֽ�����
            byte[] data = StructToBytes(obj);

            //��ʱ�����������ֽ����飬һ���Ǳ�����ݰ��ĳ����ֽ����飬 һ�������ݰ��ֽ����飬
            //ͬʱ���������ֽ�����ϲ���һ���ֽ�����

            byte[] newByte = new byte[head.Length + data.Length];
            Array.Copy(head, 0, newByte, 0, head.Length);
            Array.Copy(data, 0, newByte, head.Length, data.Length);

            //������µ��ֽ�����ĳ���
            int length = Marshal.SizeOf(size) + Marshal.SizeOf(obj);

            //�������첽��������ֽ�����
            IAsyncResult asyncSend = clientSocket.BeginSend(newByte, 0, length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
            //��ⳬʱ
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                clientSocket.Close();
                Debug.Log("Time Out !");
            }

        }
        catch (Exception e)
        {
            Debug.Log("send message error: " + e);
        }
    }

    //�ṹ��ת�ֽ�����
    public byte[] StructToBytes(object structObj)
    {

        int size = Marshal.SizeOf(structObj);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structObj, buffer, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
    //�ֽ�����ת�ṹ��
    public object BytesToStruct(byte[] bytes, Type strcutType)
    {
        int size = Marshal.SizeOf(strcutType);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, buffer, size);
            return Marshal.PtrToStructure(buffer, strcutType);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

    }

    private void sendCallback(IAsyncResult asyncSend)
    {

    }

    //�ر�Socket
    public void Closed()
    {

        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        clientSocket = null;
    }

}