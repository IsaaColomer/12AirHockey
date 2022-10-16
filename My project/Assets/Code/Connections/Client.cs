using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    Socket newSocket;
    int recv;
    byte[] data;
    EndPoint remote;
    IPEndPoint clienstep;
    IPEndPoint ipep;
    bool connected;
    string input, stringData;

    void StartUDP(string id, string name)
    {
        data = new byte[1024];
        Thread myThread = new Thread(Connection);
        myThread.Start();
        ipep = new IPEndPoint(IPAddress.Parse(id), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string welcome = "Hello, are you there?";
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

        clienstep = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)clienstep;

        recv = newSocket.ReceiveFrom(data, ref remote);
        Debug.Log("Message recived from ");

    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Connection()
    {
        Debug.Log("aAAAAAAAaAAaAA");
        while(true)
        {
        }
        
    }
}
