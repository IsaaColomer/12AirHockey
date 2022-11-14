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
    private Rigidbody enemyPlayerRb;
    public GameObject disk;
    private Transform diskTransform;
    private Vector3 newPosEnemy;
    private Vector3 newEnemyHit;
    private Vector3 newPosDisk;
    public bool connected = false;
    public bool posChanged = false;
    UnityEngine.Vector3 enemyDir;
    // Start is called before the first frame update
    void Start()
    {
        Thread myThread = new Thread(Connection);

        ipep = new IPEndPoint(IPAddress.Any, 9050);
        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newsocket.Bind(ipep);
        enemyPlayerRb = enemyPlayer.GetComponent<Rigidbody>();
        playerRb = player.GetComponent<Rigidbody>();
        diskRb = disk.GetComponent<Rigidbody>();
        diskTransform = disk.GetComponent<Transform>().transform;
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
        myThread.Start();
    }
    private void Update()
    {
        enemyDir = newEnemyHit - newPosEnemy;
        if (connected)
            StartCoroutine(SendInfo());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            connected = false;
            Serialize();
            newsocket.Close();
            Application.Quit();
        }
        if (posChanged)
        {
            
            enemyPlayer.GetComponent<Rigidbody>().velocity = (enemyDir*10);
            enemyPlayer.transform.position = new Vector3(newPosEnemy.x, 0.85f, newPosEnemy.z);
           
            //Debug.Log("New Enemy Pos: " + newPosEnemy);
            //Debug.Log(newPosDisk);
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
        Debug.Log("Serializing");
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        //writer.Write(connected.ToString());
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
        Debug.Log(playerRb.velocity);

        Debug.Log("serialized!");
        Info();
    }
    public void Info()
    {
        Debug.Log("Sending");
        newsocket.SendTo(stream.ToArray(), stream.ToArray().Length, SocketFlags.None, remote);
        Debug.Log("Sended");
    }
    public void Connection()
    {
        data = new byte[1024];
        recv = newsocket.ReceiveFrom(data, ref remote);
        connected = true;
        nameUDP = Encoding.ASCII.GetString(data, 0, recv);
        Debug.Log("Connected");
        while (true)
        {
            data = new byte[1024];
            Debug.Log("Reciving info");
            recv = newsocket.ReceiveFrom(data, ref remote);
            Debug.Log("Info recived");
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
        newEnemyHit = new Vector3((float)x, (float)y, (float)z);
        float dx = reader.ReadSingle();
        float dy = reader.ReadSingle();
        float dz = reader.ReadSingle();
        newPosEnemy = new Vector3((float)dx, (float)dy, (float)dz);
        posChanged = true;
    }
}