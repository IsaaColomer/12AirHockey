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
    private playerScript clientPlayer;
    // Start is called before the first frame update
    int recv;
    Socket newSocket;
    byte[] data;
    EndPoint remote;
    IPEndPoint sender;
    IPEndPoint ipep;
    public TMP_InputField inputName, inputIp;
    public GameObject buttonLogin;
    Thread myThread;
    MemoryStream stream;
    MemoryStream streamSerialize;
    private Vector3 newPosEnemy;
    public GameObject disk;
    private Vector3 newPosDisk;
    public bool isLoged = false;
    public bool posChanged = false;
    private GameObject enemyPlayer;
    private GameObject player;
    private Rigidbody enemyPlayerRb;
    private Rigidbody playerRb;
    private Rigidbody diskRb;
    private UnityEngine.Vector3 dir;

    private void Start()
    {
        myThread = new Thread(Receive);
        enemyPlayer = GameObject.Find("Player_2");
        player = GameObject.Find("Player_1");
        enemyPlayerRb = enemyPlayer.GetComponent<Rigidbody>();
        playerRb = player.GetComponent<Rigidbody>();
        diskRb = disk.GetComponent<Rigidbody>();
        clientPlayer = player.GetComponent<playerScript>();
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

    // Update is called once per frame
    void Update()
    {
        if (isLoged)
            StartCoroutine(SendInfo());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
        if (posChanged)
        {
            enemyPlayer.GetComponent<Rigidbody>().velocity = -newPosEnemy;
            //enemyController.transform.position = newPosEnemy;
            disk.GetComponent<Rigidbody>().velocity = -newPosDisk;
            posChanged = false;
        }
    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
    void Serialize()
    {
        streamSerialize = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(streamSerialize);

        writer.Write(clientPlayer.hit.point.x);
        writer.Write(clientPlayer.hit.point.y);
        writer.Write(clientPlayer.hit.point.z);

        //

        writer.Write(clientPlayer.rb.transform.position.x);
        writer.Write(clientPlayer.rb.transform.position.y);
        writer.Write(clientPlayer.rb.transform.position.z);


        //writer.Write(playerRb.velocity.x);
        //writer.Write(playerRb.velocity.y);
        //writer.Write(playerRb.velocity.z);
        Debug.Log(playerRb.velocity);
        Info();
    }
    public void Info()
    {
        newSocket.SendTo(streamSerialize.ToArray(), streamSerialize.ToArray().Length, SocketFlags.None, ipep);
    }
    public void ButtonClicked()
    {
        StartUDP(inputName.text, inputIp.text);
    }
    public void Receive()
    {
        while (true)
        {
            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref remote);
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
}