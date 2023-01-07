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
    public Dictionary<int, GameObject> allGO;

    // GET THE DISK
    private Disk_Code diskCode;

    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    private Vector3 clientPlayerPositionFromPlayer;
    public bool didClientScore;
    void Start()
    {
        allGO = new Dictionary<int, GameObject>();
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
        GameObject gameObjectTemp3 = GameObject.Find("Disk").gameObject;
        if (gameObjectTemp1 != null)
        {
            allGO.Add(0, gameObjectTemp1);
            allGO.Add(1, gameObjectTemp2);
            allGO.Add(2, gameObjectTemp3);
        }
        foreach (KeyValuePair<int, GameObject> go in allGO)
        {
            Debug.Log(go.Key + " " + go.Value);
        }

        diskCode = GameObject.Find("Disk").GetComponent<Disk_Code>();
    }
    private void Update()
    {
        enemyDir = newEnemyHit - clientPlayerPositionFromPlayer;
        if (posChanged)
        {
            enemyPlayer.GetComponent<Rigidbody>().velocity = new Vector3(enemyDir.x,0f,enemyDir.z) * 10f;
            enemyPlayer.transform.position = new Vector3(clientPlayerPositionFromPlayer.x, 0.85f, clientPlayerPositionFromPlayer.z);
            posChanged = false;
        }

        clientPlayerVel = enemyPlayer.GetComponent<Rigidbody>().velocity;
        serverPlayerVel = player.GetComponent<Rigidbody>().velocity;
        serverPlayerPosition = player.GetComponent<Transform>().position;
        diskVel = disk.GetComponent<Rigidbody>().velocity;
        diskPosition = disk.GetComponent<Transform>().position;
        didClientScore = diskCode.clientGoal;

        if (connected)
            StartCoroutine(SendInfo());
    }
    
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize(EventType.UPDATE_POS_GO, serverPlayerPosition, didClientScore,0);
        Serialize(EventType.UPDATE_VEL_GO, clientPlayerVel, didClientScore,1);
        Serialize(EventType.UPDATE_VEL_GO, serverPlayerVel, didClientScore,0);
        Serialize(EventType.UPDATE_POS_GO, diskPosition, didClientScore,2);
        Serialize(EventType.UPDATE_VEL_GO, diskVel, didClientScore,2);
        Serialize(EventType.UPDATE_SCORE, Vector3.zero,didClientScore,-1);

    }
    void Serialize(EventType eventType,Vector3 info, bool hasClientScore, int id)
    {
        Debug.Log("Serializing Info");
        int type = 0;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        switch (eventType)
        {
            case EventType.UPDATE_POS_GO:
                type = 0;
                writer.Write(type);
                break;
            case EventType.UPDATE_VEL_GO:
                type = 2;
                writer.Write(type);
                break;
            case EventType.CREATE_GO:
                type = 1;
                writer.Write(type);
                break;
            case EventType.DESTROY_GO:
                type = 3;
                writer.Write(type);
                break;
            case EventType.UPDATE_SCORE:
                type = 4;
                writer.Write(type);
                writer.Write(hasClientScore);
                break;
            default:
                type = -1;
                break;
        }
        if(type != 4)
        {
            writer.Write(id);
            writer.Write(info.x);
            writer.Write(info.z);
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
        int id = reader.ReadInt32();
        Debug.Log(type);
        switch (id)
        { 
            case 1:
                //Client Player
                switch(type)
                {
                    case 0:
                        //Update pos
                        float px = reader.ReadSingle();
                        float pz = reader.ReadSingle();
                        clientPlayerPositionFromPlayer = new Vector3((float)px, 0f, (float)pz);
                        posChanged = true;
                        break;
                    case 5:
                        //Hitpoint
                        float x = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        newEnemyHit = new Vector3((float)x, 0.845f, (float)z);
                        break;
                }
                break;
            default:
                //PowerUps
                break;
        }
    }
}