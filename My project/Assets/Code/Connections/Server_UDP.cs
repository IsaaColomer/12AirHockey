using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;
using System;
using Unity.VisualScripting;

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
    public playerScript ps;
    public GameObject enemyPlayer;
    public GameObject disk;
    private Vector3 newEnemyHit;
    public bool connected = false;
    public bool posChanged = false;
    UnityEngine.Vector3 enemyDir;
    public int clientSendType = 0;
    public Dictionary<int, GameObject> allGO;

    // GET THE DISK
    private Disk_Code diskCode;

    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    private Vector3 clientPlayerPositionFromPlayer;
    private int serverGoals = 0;
    private int clientGoals = 0;
    private TextMeshPro serverTextMesh;
    private TextMeshPro clientTextMesh;
    public string lastPlayerName;
    public GameObject powerUpPrefab;
    int sendTypePowerUp;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    //private bool
    public UnityEngine.Vector3 pwrUpSpawnLocation;
    public float timeToSpawn = 5f;
    [SerializeField] public float restartTimeToSpawn;
    void Start()
    {
        restartTimeToSpawn = timeToSpawn;
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
        serverTextMesh = GameObject.Find("ServerGoals").GetComponent<TextMeshPro>();
        clientTextMesh = GameObject.Find("ClientGoals").GetComponent<TextMeshPro>();
        ps = GameObject.Find("Main Camera").GetComponent<playerScript>();
    }
    private void Update()
    {
        enemyDir = newEnemyHit - clientPlayerPositionFromPlayer;
        CheckRestartGame();
        if (connected)
        {
            StartCoroutine(SendInfo());
            SpawnPowerUp();
        }
    }
    void FixedUpdate()
    {
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

    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize(EventType.UPDATE_POS_GO, serverPlayerPosition, 0);
        Serialize(EventType.UPDATE_VEL_GO, clientPlayerVel, 1);
        Serialize(EventType.UPDATE_VEL_GO, serverPlayerVel, 0);
        Serialize(EventType.UPDATE_POS_GO, diskPosition, 2);
        Serialize(EventType.UPDATE_VEL_GO, diskVel, 2);
        Serialize(EventType.UPDATE_SCORE, Vector3.zero,5);

    }
    public void Serialize(EventType eventType,Vector3 info, int id)
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

                writer.Write(id);
                writer.Write(info.x);
                writer.Write(info.z);
                break;
            case EventType.UPDATE_VEL_GO:
                type = 2;
                writer.Write(type);
                writer.Write(id);
                writer.Write(info.x);
                writer.Write(info.z);
                break;
            case EventType.CREATE_POWERUP:
                type = 1;
                writer.Write(type);
                writer.Write(id);                
                writer.Write(clientSendType);
                writer.Write(info.x);
                writer.Write(info.z);
                break;
            case EventType.DESTROY_POWERUP:
                type = 3;
                writer.Write(type);
                writer.Write(id);
                break;
            case EventType.UPDATE_SCORE:
                type = 4;
                writer.Write(type);
                writer.Write(id);
                writer.Write(serverGoals);
                writer.Write(clientGoals);
                break;
            case EventType.UPDATE_POWERUP:
                type = 5;
                writer.Write(type);
                writer.Write(id);
                writer.Write(ps.canApplyPowerUp);
                writer.Write(GameObject.Find("Disk").GetComponent<Disk_Code>().lastPlayerName);
                writer.Write(clientSendType);
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
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }
    void Deserialize()
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        int type;
        try
        {
            type = reader.ReadInt32();
        }
        catch (Exception e)
        {
            print(e);
            return;
        }
        int id = reader.ReadInt32();
        Debug.Log(id + " " + " " + type);
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
    public void CheckRestartGame()
    {
        if(serverGoals >= 3 || clientGoals >= 3)
        {
            serverGoals = 0;
            clientGoals = 0;
            serverTextMesh.text = serverGoals.ToString();
            clientTextMesh.text = clientGoals.ToString();
        }
    }
    public void ServerScoredGoal()
    {
        serverGoals++;
        serverTextMesh.text = serverGoals.ToString();
    }
    public void ClientScoredGoal()
    {
        clientGoals++;
        clientTextMesh.text = clientGoals.ToString();
    }
    public void SpawnPowerUp()
    {
        Bounds bounds = GameObject.Find("PowerUps_Spawn").GetComponent<BoxCollider>().bounds;
        offsetX = UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x);
        offsetY = 0.8801f;
        offsetZ = UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z);
        pwrUpSpawnLocation = new Vector3(offsetX, offsetY, offsetZ);
        if(timeToSpawn > 0)
        {
            timeToSpawn-=Time.deltaTime;
        }
        else
        {
            if(!GameObject.FindGameObjectWithTag("PowerUps"))
            {
                // CALL TO SERIALIZE THE POSITION
                int sendTypePowerUp = UnityEngine.Random.Range(0, 1);
                clientSendType = sendTypePowerUp;
                GameObject pwu = Instantiate(powerUpPrefab, pwrUpSpawnLocation, Quaternion.identity);
                pwu.GetComponent<PowerUps>().SendInfo(pwrUpSpawnLocation, sendTypePowerUp);
                allGO.Add(pwu.GetComponent<PowerUps>().GetId(), pwu);
            }
        }
    }
}