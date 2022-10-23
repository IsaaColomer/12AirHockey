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
        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newsock.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);      

        myThread.Start();        
    }
    private void Update()
    {
        if(canUpdateChatLog)
        {
            chatBox.text += allMessages[allMessages.Count - 1] + "\n";
            canUpdateChatLog = false;
        }
    }
    public void SendMessage()
    {
        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        newsock.SendTo(data, data.Length, SocketFlags.None, remote);
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
        data = new byte[1024];
        recv = newsock.ReceiveFrom(data, ref remote);
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
        newsock.SendTo(data, data.Length, SocketFlags.None, remote);
        while (true)
        {
            data = new byte[1024];
            recv = newsock.ReceiveFrom(data, ref remote);
            nameUDP = Encoding.ASCII.GetString(data, 0, recv);
            string newMessage = "";
            for(int i = 0; i < nameUDP.Length; i++)
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
            newsock.SendTo(data, recv, SocketFlags.None, remote);
        }        
    }
}
