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
    private Rigidbody rb;
    private Vector3 diskRbVel;
    private UnityEngine.Vector3 dir;


    // ISAAC
    private Vector3 clientPlayerPosition;
    private Vector3 serverPlayerPosition;
    private Vector3 clientPlayerVel;
    private Vector3 serverPlayerVel;
    private Vector3 diskVel;
    private Vector3 diskPosition;
    public GameObject myClientPlayer;
    public GameObject myServerPlayer;

    private void Start()
    {
        myThread = new Thread(Receive);
        enemyPlayer = GameObject.Find("Player_2");
        player = GameObject.Find("Player_1");
        rb = player.GetComponent<Rigidbody>();
    }
    void StartUDP(string name, string ip)
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
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
        }
        //disk.GetComponent<Rigidbody>().velocity = diskRbVel;
    }

    private void FixEnemyPlayerAndDisk()
    {
        //enemyPlayer.GetComponent<Rigidbody>().velocity = new Vector3(dir.x, 0.85f, dir.z);
        //UnityEngine.Vector3 newnewPos = new Vector3(newPosDisk.x, 0.8529103f, newPosDisk.z);
        //disk.transform.position = newnewPos;
        //UnityEngine.Vector3 newnewPos2 = new Vector3(newPosEnemy.x, 0.85f, newPosEnemy.z);
        //enemyPlayer.transform.position = newnewPos2;

        // SET THE CLIENT PLAYER VELOCITY
        UnityEngine.Vector3 correctedClientPlayerVelocity = new Vector3(clientPlayerVel.x, 0.85f, clientPlayerVel.z);
        myClientPlayer.GetComponent<Rigidbody>().velocity = correctedClientPlayerVelocity;

        // CORRECT THE CLIENT PLAYER POSITION
        UnityEngine.Vector3 correctedClientPlayerPosition = new Vector3(clientPlayerPosition.x, 0.85f, clientPlayerPosition.z);
        //myClientPlayer.transform.position = correctedClientPlayerPosition;

        // SET SERVER PLAYER VELOCITY
        UnityEngine.Vector3 correctedServerPlayerVelocity = new Vector3(serverPlayerVel.x, 0.85f, serverPlayerVel.z);
        myServerPlayer.GetComponent<Rigidbody>().velocity = correctedServerPlayerVelocity;

        // CORRECT THE SERVER PLAYER POSITION
        UnityEngine.Vector3 correctedServerPlayerPosition = new Vector3(serverPlayerPosition.x, 0.85f, serverPlayerPosition.z);
        myServerPlayer.transform.position = correctedServerPlayerPosition;

        // SET THE DISK VELOCITY
        disk.GetComponent<Rigidbody>().velocity = new Vector3(diskVel.x, 0.85f, diskVel.z);

        // CORRECT THE DISK POSITION
        UnityEngine.Vector3 diskCorrectedPosition = new Vector3(diskPosition.x, 0.8529103f, diskPosition.z);
        disk.transform.position = diskCorrectedPosition;
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
                writer.Write(clientPlayer.hit.point.y);
                writer.Write(clientPlayer.hit.point.z);

                // ISAAC
                writer.Write(myClientPlayer.transform.position.x);
                writer.Write(myClientPlayer.transform.position.y);
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
        Debug.Log("Serializing!!");
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
                /*
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
                if (rx != 0 && rz != 0)
                    newPosDisk = new Vector3((float)rx, (float)ry, (float)rz);
                float px = reader.ReadSingle();
                float py = reader.ReadSingle();
                float pz = reader.ReadSingle();
                if (px != 0 && pz != 0)
                    newPosEnemy = new Vector3((float)px, (float)py, (float)pz);

                float ex = reader.ReadSingle();
                float ey = reader.ReadSingle();
                float ez = reader.ReadSingle();
                if (px != 0 && pz != 0)
                    transform.position = new Vector3((float)ex, (float)ey, (float)ez);
                */



                // ISAAC

                // RECEIVE CLIENT PLAYER POSITION
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                if (x != 0 && z != 0)
                    clientPlayerPosition = new Vector3((float)x, (float)y, (float)z);

                // RECEIVE SERVER PLAYER POSITION
                float sx = reader.ReadSingle();
                float sy = reader.ReadSingle();
                float sz = reader.ReadSingle();
                if (sx != 0 && sz != 0)
                    serverPlayerPosition = new Vector3((float)sx, (float)sy, (float)sz);

                // RECEIVE CLEINT PLAYER VEL
                float vx = reader.ReadSingle();
                float vy = reader.ReadSingle();
                float vz = reader.ReadSingle();
                clientPlayerVel = new Vector3((float)vx, (float)vy, (float)vz);

                // RECEIVE SERVER PLAYER VEL
                float svx = reader.ReadSingle();
                float svy = reader.ReadSingle();
                float svz = reader.ReadSingle();
                serverPlayerVel = new Vector3((float)svx, (float)svy, (float)svz);

                // RECEIVE DISK POSITION
                float px = reader.ReadSingle();
                float py = reader.ReadSingle();
                float pz = reader.ReadSingle();
                if (px != 0 && pz != 0)
                    diskPosition = new Vector3((float)px, (float)py, (float)pz);

                // RECEIVE DISK VEL
                float dvx = reader.ReadSingle();
                float dvy = reader.ReadSingle();
                float dvz = reader.ReadSingle();
                diskVel = new Vector3((float)dvx, (float)dvy, (float)dvz);

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