using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TMPro;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    Socket newSocket;
    int recv;
    byte[] data;
    EndPoint remote;
    IPEndPoint sender;
    IPEndPoint ipep;
    bool connected;
    string input, stringData;
    public TMP_InputField inputName, inputIp;

    void StartUDP(string name, string ip)
    {
        //Thread myThread = new Thread(Connection);
        // myThread.Start();
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse(ip), 9050);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)sender;

        data = new byte[1024];
        data = Encoding.ASCII.GetBytes(name);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        
    }
    public void ButtonClicked()
    {
        string name = inputName.text;
        string ip = inputIp.text;
        StartUDP(name,ip);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            newSocket.Close();
            Application.Quit();
        }
    }
    public void Connection()
    {
        //data = new byte[1024];
        //int recv = newSocket.ReceiveFrom(data, ref remote);

        //Debug.Log("Message received from " + remote.ToString() +":");
        ////Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

        //while (true)
        //{
        //    input = Console.ReadLine();
        //    if (input == "exit")
        //        break;
        //    newSocket.SendTo(Encoding.ASCII.GetBytes(input), remote);
        //    data = new byte[1024];
        //    recv = newSocket.ReceiveFrom(data, ref remote);
        //    stringData = Encoding.ASCII.GetString(data, 0, recv);
        //    Console.WriteLine(stringData);
        //}
        //Console.WriteLine("Stopping client");
        //newSocket.Close();

    }
}
