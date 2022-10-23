using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TMPro;

public class Client_TCP : MonoBehaviour
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
    public GameObject chatImage;
    [SerializeField] private TMP_Text chatBox;
    [SerializeField] private bool canUpdateChatLog = false;
    Thread myThread;
    [SerializeField] private List<string> allMessages = new List<string>();
    private void Start()
    {
        myThread = new Thread(Receive);
    }
    void StartTCP(string name, string ip)
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            newSocket.Connect(ipep);
            Debug.Log("Connected");
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
            Debug.Log("Not Connected");
            return;
        }

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.Send(data, data.Length, SocketFlags.None);
        myThread.Start();

        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        message.gameObject.SetActive(true);
        buttonSend.SetActive(true);
        chatImage.SetActive(true);

    }
    public void ButtonClicked()
    {
        StartTCP(inputName.text, inputIp.text);
    }
    public void Receive()
    {
        while (true)
        {
            data = new byte[1024];
            recv = newSocket.Receive(data);
            nameUDP = "";
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
        }
    }
    public void SendMessage()
    {
        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        newSocket.Send(data, data.Length, SocketFlags.None);
        //textList.allTexts.Add(message.text);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
