using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;

public class Server_UDP : MonoBehaviour
{
    int recv;
    private string nameUDP;
    byte[] data = new byte[1024];
    IPEndPoint ipep;
    IPEndPoint sender;
    Socket newsocket;
    EndPoint remote;
    MemoryStream stream;
    public GameObject player;
    public GameObject enemyPlayer;
    public GameObject disk;
    private Vector3 newEnemyHit;
    public bool connected = false;
    public bool posChanged = false;
    UnityEngine.Vector3 enemyDir;
    public Dictionary<GameObject, int> allGO;

    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    private Vector3 clientPlayerPositionFromPlayer;
    void Start()
    {
        allGO = new Dictionary<GameObject, int>();
        Screen.SetResolution(1280,720,false);
        Thread myThread = new Thread(Connection);

        ipep = new IPEndPoint(IPAddress.Any, 1234);
        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newsocket.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
        myThread.Start();

        GameObject gameObjectTemp1 = GameObject.Find("Player_2").gameObject;
        GameObject gameObjectTemp2 = GameObject.Find("Player_1").gameObject;
        if (gameObjectTemp1 != null)
        {
            allGO.Add(gameObjectTemp1, 0);
            allGO.Add(gameObjectTemp2, 1);
        }
        foreach (KeyValuePair<GameObject, int> go in allGO)
        {
            Debug.Log(go.Key + " " + go.Value);
        }
    }
    private void Update()
    {
        if (posChanged)
        {
            enemyPlayer.GetComponent<Rigidbody>().velocity = new Vector3(enemyDir.x,0f,enemyDir.z) * 10f;
            enemyPlayer.transform.position = new Vector3(clientPlayerPositionFromPlayer.x, 0.85f, clientPlayerPositionFromPlayer.z);
            posChanged = false;
        }
        enemyDir = newEnemyHit - clientPlayerPositionFromPlayer;

        clientPlayerVel = enemyPlayer.GetComponent<Rigidbody>().velocity;
        serverPlayerVel = player.GetComponent<Rigidbody>().velocity;
        serverPlayerPosition = player.GetComponent<Transform>().position;
        diskVel = disk.GetComponent<Rigidbody>().velocity;
        diskPosition = disk.GetComponent<Transform>().position;

        if (connected)
            StartCoroutine(SendInfo());
    }
    
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize(EventType.UPDATE);

    }
    void Serialize(EventType eventType)
    {
        Debug.Log("Serializing Info");
        int type = 0;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        switch (eventType)
        {
            case EventType.UPDATE:
                type = 0;
                writer.Write(type);
                Debug.Log(type);

                // SEND SERVER PLAYER POSITION
                writer.Write(serverPlayerPosition.x);
                writer.Write(serverPlayerPosition.z);

                // SEND CLIENT PLAYER VEL
                writer.Write(clientPlayerVel.x);
                writer.Write(clientPlayerVel.z);

                // SEND SERVER PLAYER VEL
                writer.Write(serverPlayerVel.x);
                writer.Write(serverPlayerVel.z);

                // SEND DISK POSITION
                writer.Write(diskPosition.x);
                writer.Write(diskPosition.z);

                // SEND DISK VEL
                writer.Write(diskVel.x);
                writer.Write(diskVel.z);

                break;
            case EventType.CREATE:
                type = 1;
                break;
            case EventType.DESTROY:
                type = 2;
                break;
            default:
                type = -1;
                break;
        }
        Info();
    }
    public void Info()
    {
        if(stream.ToArray().Length<= stream.ToArray().Length)
            newsocket.SendTo(stream.ToArray(), stream.ToArray().Length, SocketFlags.None, remote);
    }
    public void Connection()
    {
        data = new byte[1024];
        recv = newsocket.ReceiveFrom(data, ref remote);
        connected = true;
        nameUDP = Encoding.ASCII.GetString(data, 0, recv);
        Debug.Log("First user connected");
        while (true)
        {
            data = new byte[1024];
            recv = newsocket.ReceiveFrom(data, ref remote);
            stream = new MemoryStream(data);
            Deserialize();
        }
    }
    void Deserialize()
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        int type = reader.ReadInt32();
        Debug.Log(type);
        switch (type)
        {
            case 0:
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                newEnemyHit = new Vector3((float)x, 0.845f, (float)z);

                float px = reader.ReadSingle();
                float pz = reader.ReadSingle();
                clientPlayerPositionFromPlayer = new Vector3((float)px, 0f, (float)pz);

                posChanged = true;
                break;
            case 1:
                //Create powerup
                break;
            case 2:
                //Destroy powerup
                break;
        }
    }
}