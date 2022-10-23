using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TMPro;

public class Client_UDP : MonoBehaviour
{
    // Start is called before the first frame update
    Socket newSocket;
    int recv;
    byte[] data;
    EndPoint remote;
    IPEndPoint sender;
    IPEndPoint ipep;
    bool connected;
    string input, stringData;
    public string nameUDP;
    public TMP_InputField inputName, inputIp;
    public GameObject buttonLogin, buttonSend;
    public TMP_InputField message;
    public Server_UDP textList;
    [SerializeField] private TMP_Text chatBox;
    [SerializeField] private bool canUpdateChatLog = false;
    Thread myThread;
    [SerializeField] private List<string> allMessages = new List<string>();
    private void Start()
    {
        chatBox = GameObject.Find("ChatBox").GetComponent<TMP_Text>();
        myThread = new Thread(Receive);
    }
    void StartUDP(string name, string ip)
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        myThread.Start();

        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        message.gameObject.SetActive(true);
        buttonSend.SetActive(true);        
    }
    public void ButtonClicked()
    {
        StartUDP(inputName.text, inputIp.text);
    }
    public void Receive()
    {
        while(true)
        {
            
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
            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref remote);
            Debug.Log(nameUDP.ToString());
        }        
    }
    public void SendMessage()
    {
        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
        if (canUpdateChatLog)
        {
            chatBox.text += allMessages[allMessages.Count - 1] + "\n";
            canUpdateChatLog = false;
        }
    }
}
