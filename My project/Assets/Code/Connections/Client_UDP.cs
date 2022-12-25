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
    public bool posChanged = false;
    private GameObject player;
    public Dictionary<GameObject, int> allGO;

    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    public GameObject myClientPlayer;
    public GameObject myServerPlayer;
    private bool hasClientScored;

    private void Start()
    {
        allGO = new Dictionary<GameObject, int>();
        playerscript = GameObject.Find("Main Camera_Player2").GetComponent<playerScript>();
        Screen.SetResolution(1280, 720, false);
        myThread = new Thread(Receive);
        player = GameObject.Find("Player_1");
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
            FixEnemyPlayerAndDisk();
            posChanged = false;
            Debug.Log("Has client scored_ " + hasClientScored);
        }
    }

    private void FixEnemyPlayerAndDisk()
    {
        // SET THE CLIENT PLAYER VELOCITY
        if(!playerscript.canMove)
        {
            myClientPlayer.GetComponent<Rigidbody>().velocity = UnityEngine.Vector3.zero;
        }
        else
        {
            myClientPlayer.GetComponent<Rigidbody>().velocity = new Vector3(clientPlayerVel.x, 0f, clientPlayerVel.z);
        }

        // SET SERVER PLAYER VELOCITY
        myServerPlayer.GetComponent<Rigidbody>().velocity = new Vector3(serverPlayerVel.x, 0f, serverPlayerVel.z);

        // CORRECT THE SERVER PLAYER POSITION
        myServerPlayer.transform.position = new Vector3(serverPlayerPosition.x, 0.85f, serverPlayerPosition.z);

        // SET THE DISK VELOCITY
        disk.GetComponent<Rigidbody>().velocity = new Vector3(diskVel.x, 0f, diskVel.z);

        // CORRECT THE DISK POSITION
        disk.transform.position = new Vector3(diskPosition.x, 0.8529103f, diskPosition.z); ;
    }
    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize(EventType.UPDATE);
    }
    void Serialize(EventType eventType)
    {
        int type = 0;
        streamSerialize = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(streamSerialize);
        switch (eventType)
        {
            case EventType.UPDATE:
                type = 0;
                writer.Write(type);
                writer.Write(clientPlayer.hit.point.x);
                writer.Write(clientPlayer.hit.point.z);

                writer.Write(myClientPlayer.transform.position.x);
                writer.Write(myClientPlayer.transform.position.z);

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
        switch(type)
        {
            case 0:
              
                // RECEIVE SERVER PLAYER POSITION
                float sx = reader.ReadSingle();
                float sz = reader.ReadSingle();
                    serverPlayerPosition = new Vector3((float)sx, 0f, (float)sz);

                // RECEIVE CLEINT PLAYER VEL
                float vx = reader.ReadSingle();
                float vz = reader.ReadSingle();
                clientPlayerVel = new Vector3((float)vx, 0f, (float)vz);

                // RECEIVE SERVER PLAYER VEL
                float svx = reader.ReadSingle();
                float svz = reader.ReadSingle();
                serverPlayerVel = new Vector3((float)svx, 0f, (float)svz);

                // RECEIVE DISK POSITION
                float px = reader.ReadSingle();
                float pz = reader.ReadSingle();
                if (px != 0 && pz != 0)
                    diskPosition = new Vector3((float)px, 0f, (float)pz);

                // RECEIVE DISK VEL
                float dvx = reader.ReadSingle();
                float dvz = reader.ReadSingle();
                diskVel = new Vector3((float)dvx, 0f, (float)dvz);

                // RECEIVE IF CLIENT HAS SCORED
                bool clientScored = reader.ReadBoolean();
                hasClientScored = clientScored;

                posChanged = true;
                break;
            case 1:
                //Create powerup
                break;
            default:
                //Destroy powerup
                break;

        }
    }
}