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

    [SerializeField] TMP_Text chatBox;
    private void Start()
    {
        chatBox = GameObject.Find("ChatBox").GetComponent<TMP_Text>();
    }
    void StartUDP(string name, string ip)
    {
        Thread myThread = new Thread(Connection);
        myThread.Start();
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        message.gameObject.SetActive(true);
        buttonSend.SetActive(true);
        
    }
    public void ButtonClicked()
    {
        string name = inputName.text;
        string ip = inputIp.text;
        StartUDP(name,ip);
    }

    public void SendMessage()
    {
        data = new byte[1024];
        recv = newSocket.ReceiveFrom(data, ref remote);
        nameUDP = Encoding.ASCII.GetString(data, 0, recv);

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(message.text);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        Debug.Log(nameUDP);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
        chatBox.text = nameUDP;
    }
    public void Connection()
    {
        //recv = newSocket.ReceiveFrom(data, ref remote);

        //Debug.Log("Message received from " + remote.ToString() + ":");

        //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

        ////string welcome = "Welcome to my test server";
        ////data = Encoding.ASCII.GetBytes(welcome);
        ////newsock.SendTo(data, data.Length, SocketFlags.None, remote);
        //while (true)
        //{
        //    name = Encoding.ASCII.GetString(data, 0, recv);

        //    data = new byte[1024];

        //    recv = newSocket.ReceiveFrom(data, ref remote);

        //    Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        //    newSocket.SendTo(data, recv, SocketFlags.None, remote);
        //}
    }
}
