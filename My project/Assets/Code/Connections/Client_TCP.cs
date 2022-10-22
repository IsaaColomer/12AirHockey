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
    [SerializeField] private TMP_Text chatBox;
    Thread myThread;
    private void Start()
    {
        chatBox = GameObject.Find("ChatBox").GetComponent<TMP_Text>();
        myThread = new Thread(Receive);
    }
    void StartTCP(string name, string ip)
    {
        Debug.Log("Starting");
        data = new byte[1024];
        Debug.Log("1");
        Debug.Log(ip);
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        Debug.Log("2");
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("3");

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

        data = Encoding.ASCII.GetBytes(name);
        newSocket.Send(data, data.Length, SocketFlags.None);
        myThread.Start();

        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        message.gameObject.SetActive(true);
        buttonSend.SetActive(true);        
    }
    public void ButtonClicked()
    {
        StartTCP(inputName.text, inputIp.text);
    }
    public void Receive()
    {
        Debug.Log("Receiving");
        int recv = newSocket.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        while (true)
        {
            data = new byte[1024];
            recv = newSocket.Receive(data);
            nameUDP = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(nameUDP);
        }        
    }
    public void SendMessage()
    {
        data = new byte[1024];
        string mess = "[" + inputName.text + "]:" +  message.text;
        data = Encoding.ASCII.GetBytes(mess);
        newSocket.Send(data);
        //textList.allTexts.Add(message.text);
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
}
