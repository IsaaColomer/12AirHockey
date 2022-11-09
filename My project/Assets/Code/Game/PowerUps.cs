using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    //gran petit
    //congelar
    //el disc va mes rapid
    [SerializeField] private int type;
    private Disk_Code disk;
    // Start is called before the first frame update
    void Start()
    {
        type = Random.Range(0, 3);
        disk = GameObject.Find("Disk").GetComponent<Disk_Code>();
        Debug.Log(type);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Disk")
        {
            GameObject.Find(disk.lastPlayerName).GetComponentInParent<playerScript>().GetType(type);
            Destroy(this.gameObject);
        }
    }
}
