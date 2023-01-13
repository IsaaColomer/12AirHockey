using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;
using System;
using UnityEngine.Rendering;

public class Client_UDP : MonoBehaviour
{
    private playerScript playerscript;
    public playerScript clientPlayer;
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
    public bool posChangedServer = false;
    private bool posChangedDisk = false;
    private bool velChangedServer = false;
    private bool velChangedClient = false;
    private bool velChangedDisk = false;
    [SerializeField]private GameObject player;
    public Dictionary<int,GameObject> allGO;

    private Vector3 serverPlayerVector;
    private Vector3 clientPlayerVector;
    private Vector3 diskVector;
    public GameObject myClientPlayer;
    public GameObject myServerPlayer;
    public GameObject powerUpPrefab;
    private bool spawnPower = false;
    private bool destroyPower = false;
    private Vector3 posPowUp;
    public bool sendBool;
    public string sendString;
    private int powerupId = -5;
    private bool hasClientScored;

    GameObject selectedGO;
    
    private TextMeshPro serverTextMesh;
    private TextMeshPro clientTextMesh;

    private int serverGoals = 0;
    private int clientGoals = 0;

    private void Start()
    {
        allGO = new Dictionary<int, GameObject>();
        playerscript = GameObject.Find("Main Camera_Player2").GetComponent<playerScript>();
        Screen.SetResolution(1280, 720, false);
        myThread = new Thread(Receive);
        player = GameObject.Find("Player_1");
        serverTextMesh = GameObject.Find("ServerGoals").GetComponent<TextMeshPro>();
        clientTextMesh = GameObject.Find("ClientGoals").GetComponent<TextMeshPro>();
        if(serverTextMesh != null && clientTextMesh != null)
        {
            Debug.Log("Oriol fes debugs del codi pq sino no entenem res quan els altres volem treballar");
        }
    }
    void StartUDP(string name, string ip)
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 1234);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Log("Login Started");
        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;
        Debug.Log("IPEndopoint created");

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        Debug.Log("Fist messaje sended");
        myThread.Start();

        Debug.Log("Login Finished");
        inputName.gameObject.SetActive(false);
        inputIp.gameObject.SetActive(false);
        buttonLogin.SetActive(false);
        isLoged = true;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoged)
        {
            StartCoroutine(SendInfo());
            Powerup();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
    }

    private void Powerup()
    {
        if(spawnPower)
        {
            GameObject go = Instantiate(powerUpPrefab, posPowUp, Quaternion.identity);
            allGO.Add(powerupId, go);
            spawnPower= false;
        }
        if(destroyPower)
        {
            //allGO.Add(powerupId, go);
            spawnPower= false;
        }

    }

    void FixedUpdate()
    {
        if (posChangedDisk || posChangedServer || velChangedClient || velChangedDisk || velChangedServer)
        {
            FixEnemyPlayerAndDisk();
            Debug.Log(selectedGO);
            serverTextMesh.text = serverGoals.ToString();
            clientTextMesh.text = clientGoals.ToString();
        }
    }
    private void FixEnemyPlayerAndDisk()
    {
        // SET THE CLIENT PLAYER VELOCITY
        if(!playerscript.canMove)
        {
            myClientPlayer.GetComponent<Rigidbody>().velocity = UnityEngine.Vector3.zero;
        }
        else if(velChangedClient)
        {
            myClientPlayer.GetComponent<Rigidbody>().velocity = new Vector3(clientPlayerVector.x, 0f, clientPlayerVector.z);
            velChangedClient = false;
        }
        
        // SET SERVER PLAYER VELOCITY
        if(velChangedServer)
        {
            myServerPlayer.GetComponent<Rigidbody>().velocity = new Vector3(serverPlayerVector.x, 0f, serverPlayerVector.z);
            velChangedServer = false;
        }

        // CORRECT THE SERVER PLAYER POSITION
        if(posChangedServer)
        {
            myServerPlayer.transform.position = new Vector3(serverPlayerVector.x, 0.85f, serverPlayerVector.z);
            posChangedServer = false;
        }

        // SET THE DISK VELOCITY
        if(velChangedDisk)
        {
            disk.GetComponent<Rigidbody>().velocity = new Vector3(diskVector.x, 0f, diskVector.z);
            velChangedDisk=false;
        }

        // CORRECT THE DISK POSITION
        if(posChangedDisk) 
        {
            disk.transform.position = new Vector3(diskVector.x, 0.8529103f, diskVector.z);
            posChangedDisk = false;
        }
    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        if(playerscript.canMove)
            Serialize(EventType.HITPOINT, clientPlayer.hit.point, 1);
        Serialize(EventType.UPDATE_POS_GO, myClientPlayer.transform.position, 1);
    }
    void Serialize(EventType eventType, Vector3 info, int id)
    {
        int type = 0;
        streamSerialize = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(streamSerialize);
        switch (eventType)
        {
            case EventType.UPDATE_POS_GO:
                type = 0;
                writer.Write(type);
                break;
            case EventType.HITPOINT:
                type = 5;
                writer.Write(type);
                break;
            default:
                type = -1;
                break;
        }
        if(type != 2)
        {
            writer.Write(id);
            writer.Write(info.x);
            writer.Write(info.z);
        }
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
        int type = reader.ReadInt32();
        int id = reader.ReadInt32();
        switch (id)
        {
            case 0:
                //Server Player (Player_2)
                selectedGO = allGO[0];
                switch (type)
                {
                    case 0:
                        //Update server pos
                        float spx = reader.ReadSingle();
                        float spz = reader.ReadSingle();
                        serverPlayerVector = new Vector3((float)spx, 0f, (float)spz);
                        posChangedServer = true;
                        break;
                    case 1:
                        //Update server vel
                        float svx = reader.ReadSingle();
                        float svz = reader.ReadSingle();
                        serverPlayerVector = new Vector3((float)svx, 0f, (float)svz);
                        velChangedServer = true;
                        break;
                }
                break;
            case 1:
                //Client Player (Player_1)
                selectedGO = allGO[1];
                float cvx = reader.ReadSingle();
                float cvz = reader.ReadSingle();
                clientPlayerVector = new Vector3((float)cvx, 0f, (float)cvz);
                velChangedClient = true;
                break;
            case 2:
                //Disk
                selectedGO = allGO[2];
                switch (type)
                {
                    case 0:
                        //Update server pos
                        float spx = reader.ReadSingle();
                        float spz = reader.ReadSingle();
                        diskVector = new Vector3((float)spx, 0f, (float)spz);
                        posChangedDisk = true;
                        break;
                    case 1:
                        //Update server vel
                        float svx = reader.ReadSingle();
                        float svz = reader.ReadSingle();
                        diskVector = new Vector3((float)svx, 0f, (float)svz);
                        velChangedDisk = true;
                        break;
                }
                break;
            case 3:
                //
                break;
            case 5:
                serverGoals = reader.ReadInt32();
                clientGoals = reader.ReadInt32();
                break;
            case 401:
                sendBool = reader.ReadBoolean();
                sendString = reader.ReadString();
                break;
            default:
                //PowerUps
                switch(type)
                {
                    case 1:
                        //Create
                        float pux = reader.ReadSingle();
                        float puz = reader.ReadSingle();
                        posPowUp = new Vector3(pux, 0.8801f, puz);
                        powerupId = id;
                        spawnPower = true;
                        break;
                    case 3:
                        //Destroy
                        break;
                }
                break;

        }       
    }
}