using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    //gran petit
    //congelar
    //el disc va mes rapid
    [SerializeField] private int type;
    private Disk_Code disk;
    public int id = -5414;
    public Server_UDP manager;
    public Client_UDP client;
    // Start is called before the first frame update
    private void Start()
    {

    }
    public void SendInfo(Vector3 pos, int sendType)
    {
        manager = GameObject.Find("OnlineGameObject").GetComponent<Server_UDP>();
        client = GameObject.Find("OnlineGameObject").GetComponent<Client_UDP>();

        disk = GameObject.Find("Disk").GetComponent<Disk_Code>();
        id = UnityEngine.Random.Range(6, 400);
        type = sendType;
        manager.Serialize(EventType.CREATE_POWERUP, pos, id);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Disk" && manager != null)
        {
            GameObject.Find(disk.lastPlayerName).GetComponentInParent<playerScript>().SetPowerupType(type);
            manager.Serialize(EventType.DESTROY_POWERUP,Vector3.zero, id);
            manager.timeToSpawn = manager.restartTimeToSpawn;
            Destroy(this.gameObject);
        }
    }
    public int GetId()
    {
        return id;
    }
    
}
