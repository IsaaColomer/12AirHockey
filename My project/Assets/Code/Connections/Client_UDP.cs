using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;

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
    public GameObject buttonLogin;
    Thread myThread;
    MemoryStream stream;
    public GameObject enemyController;
    private Vector3 newPosEnemy;
    public GameObject disk;
    private Vector3 newPosDisk;
    public bool isLoged = false;
    public bool posChanged = false;

    private void Start()
    {
        myThread = new Thread(Receive);
    }
    void StartUDP(string name, string ip)
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Log("Login 1");
        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;
        Debug.Log("Login 2");

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        Debug.Log("Login 3");
        myThread.Start();

        Debug.Log("Login 4");
        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        isLoged = true;
    }
    public void ButtonClicked()
    {
        Debug.Log("Starting login");
        StartUDP(inputName.text, inputIp.text);
    }
    public void Receive()
    {
        while (true)
        {
            data = new byte[1024];
            Debug.Log("Reciving info");
            recv = newSocket.ReceiveFrom(data, ref remote);
            Debug.Log("Info recived");
            stream = new MemoryStream(data);
            Deserialize();
        }
    }
    void Deserialize()
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        newPosEnemy = new Vector3((float)x, (float)y, (float)z);
        x = reader.ReadSingle();
        y = reader.ReadSingle();
        z = reader.ReadSingle();
        newPosDisk = new Vector3((float)x, (float)y, (float)z);
        posChanged = true;
    }

    public void SendMessage()
    {
        //    data = new byte[1024];
        //    data = Encoding.ASCII.GetBytes("dsa");
        //    newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
        if(posChanged)
        {
            enemyController.GetComponent<Rigidbody>().velocity = -newPosEnemy;
            //enemyController.transform.position = newPosEnemy;
            Debug.Log("New Enemy Pos: " + newPosEnemy);
            disk.GetComponent<Rigidbody>().velocity = -newPosDisk;
            Debug.Log(newPosDisk);
            posChanged = false;
        }
    }
}