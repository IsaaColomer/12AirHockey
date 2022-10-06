using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Online : MonoBehaviour
{
    // Start is called before the first frame update
    Socket newSocket, client;
    int recv;
    byte[] data;
    EndPoint remote;
    int port;
    IPEndPoint clientep;
    bool connected;
    void Start()
    {
        Thread myThread = new Thread(Connection);
        myThread.Start();
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
        data = new byte[1024];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);
   

    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Connection()
    {
        Debug.Log("aAAAAAAAaAAaAA");
    }
}
