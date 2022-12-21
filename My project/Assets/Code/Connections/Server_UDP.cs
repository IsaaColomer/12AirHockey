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
    private Rigidbody playerRb;
    private Rigidbody diskRb;
    public GameObject enemyPlayer;
    public GameObject disk;
    private Vector3 newPosEnemy;
    private Vector3 newEnemyHit;
    public bool connected = false;
    public bool posChanged = false;
    UnityEngine.Vector3 enemyDir;


    // ISAAC
    private Vector3 clientPlayerPosition;
    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    private Vector3 clientPlayerPositionFromPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(480,320,false);
        Thread myThread = new Thread(Connection);

        ipep = new IPEndPoint(IPAddress.Any, 9050);
        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newsocket.Bind(ipep);
        playerRb = player.GetComponent<Rigidbody>();
        diskRb = disk.GetComponent<Rigidbody>();
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
        myThread.Start();
    }
    private void Update()
    {
        enemyDir = newEnemyHit - clientPlayerPositionFromPlayer;



        // ISAAC

        clientPlayerPosition = enemyPlayer.GetComponent<Transform>().position;
        clientPlayerVel = enemyPlayer.GetComponent<Rigidbody>().velocity;
        serverPlayerVel = player.GetComponent<Rigidbody>().velocity;
        serverPlayerPosition = player.GetComponent<Transform>().position;
        diskVel = disk.GetComponent<Rigidbody>().velocity;
        diskPosition = disk.GetComponent<Transform>().position;

        // ISAAC



        if (connected)
            StartCoroutine(SendInfo());
        if (posChanged)
        {
            enemyPlayer.GetComponent<Rigidbody>().velocity = (enemyDir*10f);
            enemyPlayer.transform.position = new Vector3(clientPlayerPositionFromPlayer.x, 0.85f, clientPlayerPositionFromPlayer.z);
            posChanged = false;
        }       
    }
    
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize(EventType.UPDATE);

    }
    void Serialize(EventType eventType)
    {
        Debug.Log("Serializing Info");
        //EventData data = new EventData();
        //data.ID = 0;
        //data.EventType = EventType.UPDATE;
        //data.trans = new Vector3(0, 0, 0);
        //string json = JsonUtility.ToJson(data);
        int type = 0;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        switch (eventType)
        {
            case EventType.UPDATE:
                type = 0;
                writer.Write(type);
                Debug.Log(type);
                /*
                writer.Write(playerRb.velocity.x);
                writer.Write(playerRb.velocity.y);
                writer.Write(playerRb.velocity.z);

                writer.Write(diskRb.velocity.x);
                writer.Write(diskRb.velocity.y);
                writer.Write(diskRb.velocity.z);

                writer.Write(diskRb.transform.position.x);
                writer.Write(diskRb.transform.position.y);
                writer.Write(diskRb.transform.position.z);

                writer.Write(player.transform.position.x);
                writer.Write(player.transform.position.y);
                writer.Write(player.transform.position.z);

                // ENEMY PLAYER POSITION
                writer.Write(enemyPlayer.transform.position.x);
                writer.Write(enemyPlayer.transform.position.y);
                writer.Write(enemyPlayer.transform.position.z);
                */

                // ISAAC

                // SEND CLIENT PLAYER POSITION
                writer.Write(clientPlayerPosition.x);
                writer.Write(clientPlayerPosition.y);
                writer.Write(clientPlayerPosition.z);

                // SEND SERVER PLAYER POSITION
                writer.Write(serverPlayerPosition.x);
                writer.Write(serverPlayerPosition.y);
                writer.Write(serverPlayerPosition.z);

                // SEND CLIENT PLAYER VEL
                writer.Write(clientPlayerVel.x);
                writer.Write(clientPlayerVel.y);
                writer.Write(clientPlayerVel.z);

                // SEND SERVER PLAYER VEL
                writer.Write(serverPlayerVel.x);
                writer.Write(serverPlayerVel.y);
                writer.Write(serverPlayerVel.z);

                // SEND DISK POSITION
                writer.Write(diskPosition.x);
                writer.Write(diskPosition.y);
                writer.Write(diskPosition.z);

                // SEND DISK VEL
                writer.Write(diskVel.x);
                writer.Write(diskVel.y);
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
        Debug.Log("serialized!");



        Info();
    }
    public void Info()
    {
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
        //EventData data = new EventData();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        int type = reader.ReadInt32();
        Debug.Log(type);
        switch (type)
        {
            case 0:
                //string json = reader.ReadString();
                //Debug.Log(json);
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                newEnemyHit = new Vector3((float)x, (float)y, (float)z);

                float px = reader.ReadSingle();
                float py = reader.ReadSingle();
                float pz = reader.ReadSingle();
                clientPlayerPositionFromPlayer = new Vector3((float)px, (float)py, (float)pz);

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