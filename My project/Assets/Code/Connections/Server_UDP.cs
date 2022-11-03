using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;

public class Server_UDP : MonoBehaviour
{
    int recv;
    private string nameUDP;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    IPEndPoint sender;
    Socket newsock;
    EndPoint remote;
    MemoryStream stream;
    public GameObject controller;
    public GameObject disk;
    public bool connected = false;
    // Start is called before the first frame update
    void Start()
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
    private void Update()
    {
        if(connected)
            StartCoroutine(SendInfo());
    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();

    }
    void Serialize()
    {
        Debug.Log("Serializing");
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(controller.transform.position.x);
        writer.Write(controller.transform.position.y);
        writer.Write(controller.transform.position.z);
        writer.Write(disk.transform.position.x);
        writer.Write(disk.transform.position.y);
        writer.Write(disk.transform.position.z);

        Debug.Log("serialized!");
        Debug.Log(controller.transform.position.x);
        Debug.Log(controller.transform.position.y);
        Debug.Log(controller.transform.position.z);
        Info();
    }
    public void Info()
    {
        Debug.Log("Sending");
        newsock.SendTo(stream.ToArray(), stream.ToArray().Length, SocketFlags.None, remote);
        Debug.Log("Sended");
    }
    public void Connection()
    {
        data = new byte[1024];
        recv = newsock.ReceiveFrom(data, ref remote);
        connected = true;
        //nameUDP = Encoding.ASCII.GetString(data, 0, recv);
        //Debug.Log("Connected");
        //while (true)
        //{
        //    data = new byte[1024];
        //    recv = newsock.ReceiveFrom(data, ref remote);
        //    nameUDP = Encoding.ASCII.GetString(data, 0, recv);

        //    //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        //    newsock.SendTo(data, recv, SocketFlags.None, remote);
        //}
    }
}