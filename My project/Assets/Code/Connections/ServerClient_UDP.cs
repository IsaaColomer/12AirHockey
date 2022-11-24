using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;

public class ServerClient_UDP : MonoBehaviour
{
    public ScenesManager scenesManager;
    bool connected;
    public bool posChanged = false;
    public playerScript clientPlayer;
    Thread myThread;

    //Servidor
    int recv;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    Socket newSocket;
    IPEndPoint sender;
    EndPoint remote;

    public GameObject playerPrefab1, playerPrefab2, enemyPrefab;
    public GameObject player;
    private Rigidbody playerRb;
    public GameObject enemyPlayer;
    private Rigidbody enemyPlayerRb;
    public GameObject disk;
    private Rigidbody diskRb;
    private Transform diskTransform;

    //ISAAC STUFFFF

    MemoryStream streamSerialize;
    MemoryStream streamDeserilize;
    private Vector3 vector1;
    private Vector3 vector2;//newPosEnemy//newPosDisk
    private Vector3 newPosDisk;
    private Vector3 newPosEnemy;
    private Vector3 enemyDir;

    //Cliente
    public TMP_InputField inputName, inputIp;
    public GameObject buttonLogin;

    // Start is called before the first frame update
    void Start()
    {
        scenesManager = GameObject.Find("Manager").GetComponent<ScenesManager>();
        if(scenesManager.type == ScenesManager.UserType.HOST)
        {
            player = Instantiate(playerPrefab1).gameObject;
            enemyPlayer = Instantiate(enemyPrefab).gameObject;
            clientPlayer = player.GetComponent<playerScript>();

            myThread = new Thread(Connection);

            ipep = new IPEndPoint(IPAddress.Any, 9050);
            newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            newSocket.Bind(ipep);
            enemyPlayerRb = enemyPlayer.GetComponent<Rigidbody>();
            playerRb = player.GetComponentInChildren<Rigidbody>();
            diskTransform = disk.GetComponent<Transform>().transform;
            Debug.Log("Waiting for a client...");

            sender = new IPEndPoint(IPAddress.Any, 0);
            remote = (EndPoint)(sender);
            myThread.Start();
        }
        else
        {
            player = Instantiate(playerPrefab2).gameObject;
            enemyPlayer = Instantiate(enemyPrefab).gameObject;
            clientPlayer = player.GetComponent<playerScript>();

            myThread = new Thread(Connection);
            enemyPlayerRb = enemyPlayer.GetComponent<Rigidbody>();
            playerRb = player.GetComponentInChildren<Rigidbody>();
            diskRb = disk.GetComponent<Rigidbody>();
            inputName.gameObject.SetActive(true);
            inputIp.gameObject.SetActive(true);
            buttonLogin.SetActive(true);
            //Display de name and ip
        }
    }
    public void ButtonClicked()
    {
        StartUDP(inputName.text, inputIp.text);
    }
    public void StartUDP(string name, string ip)
    {
        data = new byte[1024];
        Debug.Log(0);
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
        connected = true;
    }
    void Connection()
    {
        //data = new byte[1024];
        //recv = newSocket.ReceiveFrom(data, ref remote);
        //Debug.Log("Connected");
        while (true)
        {
            data = new byte[1024];
            //Debug.Log("Reciving info");
            recv = newSocket.ReceiveFrom(data, ref remote);
            connected = true;
            //Debug.Log("Info recived");
            streamDeserilize = new MemoryStream(data);
            Deserialize();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
        if (connected)
            StartCoroutine(SendInfo());
        if(posChanged && scenesManager.type == ScenesManager.UserType.CLIENT)
        {
            enemyPlayer.GetComponent<Rigidbody>().velocity = vector1;
            posChanged = false;
            Debug.Log("velocity change client");

        }
        if (scenesManager.type == ScenesManager.UserType.CLIENT)
        {
            disk.GetComponent<Rigidbody>().velocity = vector2;
            UnityEngine.Vector3 newnewPos = new Vector3(newPosDisk.x, 0.8529103f, newPosDisk.z);
            Vector3.Lerp(disk.transform.position, newnewPos, 0.16f);
            UnityEngine.Vector3 newnewPos2 = new Vector3(newPosEnemy.x, 0.85f, newPosEnemy.z);
            Vector3.Lerp(disk.transform.position, newnewPos2, 0.16f);
        }
        if(posChanged && scenesManager.type == ScenesManager.UserType.HOST)
        {
            Debug.Log("velocity change host");
            enemyPlayer.GetComponent<Rigidbody>().velocity = (enemyDir * 10);
            enemyPlayer.transform.position = new Vector3(newPosEnemy.x, 0.85f, newPosEnemy.z);
            posChanged = false;
        }
    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        if (scenesManager.type == ScenesManager.UserType.HOST)
        {
            // SERVER
            Serialize(playerRb.velocity, diskRb.velocity, diskRb.transform.position, player.transform.position);
        }

        else
        {
            // CLIENT 
            Serialize(clientPlayer.hit.point, clientPlayer.transform.position, Vector3.zero, Vector3.zero);
        }
    }
    void Serialize(Vector3 firstInfo, Vector3 secondInfo, Vector3 thirdInfo, Vector3 fourthInfo)
    {
        Debug.Log("Serializing");
        streamSerialize = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(streamSerialize);

        //Server: playerRb.Velocity
        //Client: clientPlayer.hit.point
        writer.Write("Hola");
        writer.Write(firstInfo.x);
        writer.Write(firstInfo.y);
        writer.Write(firstInfo.z);
        //Debug.Log(firstInfo);

        //Server: diskRb
        //Client: clientPlayer.transform.position
        writer.Write(secondInfo.x);
        writer.Write(secondInfo.y);
        writer.Write(secondInfo.z);

        if(scenesManager.type == ScenesManager.UserType.HOST)
        {
            writer.Write(thirdInfo.x);
            writer.Write(thirdInfo.y);
            writer.Write(thirdInfo.z);

            writer.Write(fourthInfo.x);
            writer.Write(fourthInfo.y);
            writer.Write(fourthInfo.z);
        }

        Debug.Log("serialized!");
        Info();
    }
    void Deserialize()
    {
        BinaryReader reader = new BinaryReader(streamDeserilize);
        streamDeserilize.Seek(0, SeekOrigin.Begin);
        string a = reader.ReadString();
        Debug.Log(a);
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        Debug.Log(new Vector3(x, y, z));
        vector1 = new Vector3((float)x, (float)y, (float)z);
        float dx = reader.ReadSingle();
        float dy = reader.ReadSingle();
        float dz = reader.ReadSingle();
        vector2 = new Vector3((float)dx, (float)dy, (float)dz);
        if (scenesManager.type == ScenesManager.UserType.HOST)
        {
            float rx = reader.ReadSingle();
            float ry = reader.ReadSingle();
            float rz = reader.ReadSingle();
            newPosDisk = new Vector3((float)rx, (float)ry, (float)rz);
            float px = reader.ReadSingle();
            float py = reader.ReadSingle();
            float pz = reader.ReadSingle();
            newPosEnemy = new Vector3((float)px, (float)py, (float)pz);
        }
        posChanged = true;
    }

    public void Info()
    {
        //Debug.Log("Sending");
        if (scenesManager.type == ScenesManager.UserType.HOST)
            newSocket.SendTo(streamSerialize.ToArray(), streamSerialize.ToArray().Length, SocketFlags.None, remote);
        else if (scenesManager.type == ScenesManager.UserType.CLIENT)
            newSocket.SendTo(streamSerialize.ToArray(), streamSerialize.ToArray().Length, SocketFlags.None, ipep);
        //Debug.Log("Sended");
    }
}
