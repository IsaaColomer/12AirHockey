using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;


public class Server_TCP : MonoBehaviour
{
    int recv;
    [SerializeField] TMP_Text chatBox;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    IPEndPoint sender;
    EndPoint remote;
    Socket newsock;
    Socket client;
    [SerializeField] public List<string> allTexts = new List<string>();
    //Chat
    string nameUDP = "";
    [SerializeField] private GameObject buttonSend;
    public TMP_InputField message;
    // Start is called before the first frame update
    void Start()
    {
        Thread myThread = new Thread(Connection);
        chatBox = GameObject.Find("ChatBox").GetComponent<TMP_Text>();
        buttonSend = GameObject.Find("ChatBox");

        ipep = new IPEndPoint(IPAddress.Any, 9050);
        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        newsock.Bind(ipep);
        
        Debug.Log("Waiting for a client...");     

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
        newsock.SendTo(data, data.Length, SocketFlags.None, remote);
        nameUDP = Encoding.ASCII.GetString(data, 0, data.Length);
    }
    public void Connection()
    {
        newsock.Listen(10);

        client = newsock.Accept();

        sender = (IPEndPoint)client.RemoteEndPoint;
        remote = (EndPoint)(sender);

        Debug.Log("Message received from " + remote.ToString() + ":");

        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        
        newsock.SendTo(data, data.Length, SocketFlags.None, remote);
        while (true)
        {
            data = new byte[1024];

            recv = client.Receive(data);
            if (recv == 0)
                break;

            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            client.Send(data, recv, SocketFlags.None);
        }

        client.Close();
        newsock.Close();
    }
    public void SetName()
    {
        chatBox.text = Encoding.ASCII.GetString(data, 0, recv);
    }
}
