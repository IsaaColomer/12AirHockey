using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;


public class Server_UDP : MonoBehaviour
{
    int recv;
    [SerializeField] TMP_Text chatBox;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    IPEndPoint sender;
    Socket newsock;
    EndPoint remote;

    //Chat
    string nameUDP = "";
    public GameObject buttonSend;
    public TMP_InputField message;
    // Start is called before the first frame update
    void Start()
    {
        Thread myThread = new Thread(Connection);
        chatBox = GameObject.Find("ChatBox").GetComponent<TMP_Text>();

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
        chatBox.text = nameUDP;
    }
    public void SendMessage()
    {
        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        newsock.SendTo(data, recv, SocketFlags.None, remote);
    }
    public void Connection()
    {
        Debug.Log("adasdasdasdadasdsadsadasdasd");
        recv = newsock.ReceiveFrom(data, ref remote);

        Debug.Log("Message received from " + remote.ToString() + ":");

        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        
        //string welcome = "Welcome to my test server";
        //data = Encoding.ASCII.GetBytes(welcome);
        //newsock.SendTo(data, data.Length, SocketFlags.None, remote);
        while (true)
        {
            nameUDP = Encoding.ASCII.GetString(data, 0, recv);

            data = new byte[1024];

            recv = newsock.ReceiveFrom(data, ref remote);

            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            newsock.SendTo(data, recv, SocketFlags.None, remote);
        }
    }
    public void SetName()
    {
        chatBox.text = Encoding.ASCII.GetString(data, 0, recv);
    }
}
