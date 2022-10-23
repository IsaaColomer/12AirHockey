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
   [SerializeField] bool canUpdateChatLog = false;
    //Chat
    string nameUDP = "";
    [SerializeField] private GameObject buttonSend;
    public TMP_InputField message;
    [SerializeField] private List<string> allMessages = new List<string>();
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
        if (canUpdateChatLog)
        {
            chatBox.text += allMessages[allMessages.Count - 1] + "\n";
            canUpdateChatLog = false;
        }
    }
    public void SendMessage()
    {
        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        client.Send(data, data.Length, SocketFlags.None);

        nameUDP = Encoding.ASCII.GetString(data, 0, data.Length);
        string newMessage = "";
        for (int i = 0; i < nameUDP.Length; i++)
        {
            if (nameUDP[i] != 0)
            {
                newMessage += nameUDP[i];
            }
        }
        if (nameUDP != "")
        {
            allMessages.Add("[You] ->" + newMessage);
            canUpdateChatLog = true;
        }
    }
    public void Connection()
    {
        newsock.Listen(10);
        client = newsock.Accept();

        nameUDP = Encoding.ASCII.GetString(data, 0, recv);
        string newMessage1 = "";
        for (int i = 0; i < nameUDP.Length; i++)
        {
            if (nameUDP[i] != 0)
            {
                newMessage1 += nameUDP[i];
            }
        }
        if (nameUDP != "")
        {
            allMessages.Add("[Foreign] ->" + newMessage1);
            canUpdateChatLog = true;
        }
        client.Send(data, data.Length, SocketFlags.None);
        while (true)
        {
            data = new byte[1024];
            recv = client.Receive(data);
            nameUDP = Encoding.ASCII.GetString(data, 0, recv);
            string newMessage = "";
            for (int i = 0; i < nameUDP.Length; i++)
            {
                if (nameUDP[i] != 0)
                {
                    newMessage += nameUDP[i];
                }
            }
            if (nameUDP != "")
            {
                allMessages.Add("[Foreign] ->" + newMessage);
                canUpdateChatLog = true;
            }

            //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            client.Send(data, recv, SocketFlags.None);
        }
    }
}
