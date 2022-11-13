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
    public playerScript clientPlayer;
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
    private Vector3 diskRbVel;
    private UnityEngine.Vector3 dir;

    private void Start()
    {
        myThread = new Thread(Receive);
        enemyPlayer = GameObject.Find("Player_2");
        player = GameObject.Find("Player_1");
        enemyPlayerRb = enemyPlayer.GetComponent<Rigidbody>();
        playerRb = player.GetComponent<Rigidbody>();
        diskRb = disk.GetComponent<Rigidbody>();
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
            enemyPlayer.GetComponent<Rigidbody>().velocity = -dir;
            posChanged = false;
        }
        
        disk.GetComponent<Rigidbody>().velocity = -diskRbVel;
        StartCoroutine(UpdateDisAndEnemykPos());
    }
    IEnumerator UpdateDisAndEnemykPos()
    {
        yield return new WaitForSeconds(0.16f);
        UnityEngine.Vector3 newnewPos = new Vector3(-newPosDisk.x, 0.8529103f, -newPosDisk.z);
        Vector3.Lerp(disk.transform.position, newnewPos, 0.16f);
        UnityEngine.Vector3 newnewPos2 = new Vector3(-newPosEnemy.x, 0.85f, -newPosEnemy.z);
        Vector3.Lerp(disk.transform.position, newnewPos2, 0.16f);
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

        writer.Write(player.transform.position.x);
        writer.Write(player.transform.position.y);
        writer.Write(player.transform.position.z);
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
        dir = new Vector3((float)x, (float)y, (float)z);

        float dx = reader.ReadSingle();
        float dy = reader.ReadSingle();
        float dz = reader.ReadSingle();
        diskRbVel = new Vector3((float)dx, (float)dy, (float)dz);

        float rx = reader.ReadSingle();
        float ry = reader.ReadSingle();
        float rz = reader.ReadSingle();
        newPosDisk = new Vector3((float)rx, (float)ry, (float)rz);
        float px = reader.ReadSingle();
        float py = reader.ReadSingle();
        float pz = reader.ReadSingle();
        newPosEnemy = new Vector3((float)px, (float)py, (float)pz);
        posChanged = true;
    }

    public void SendMessage()
    {
        //    data = new byte[1024];
        //    data = Encoding.ASCII.GetBytes("dsa");
        //    newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }
}