using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server : MonoBehaviour
{
    int recv;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    IPEndPoint sender;
    Socket newsock;
    EndPoint remote;
    // Start is called before the first frame update
    void CreateServer()
    {
        Thread myThread = new Thread(Connection);

        ipep = new IPEndPoint(IPAddress.Any, 9050);
        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newsock.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
        
        myThread.Start();
        
    }
    public void Connection()
    {
        recv = newsock.ReceiveFrom(data, ref remote);

        Debug.Log("Message received from " + remote.ToString() + ":");

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        newsock.SendTo(data, data.Length, SocketFlags.None, remote);

        while (true)
        {
            data = new byte[1024];
            recv = newsock.ReceiveFrom(data, ref remote);

            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            newsock.SendTo(data, recv, SocketFlags.None, remote);
        }
    }
}
