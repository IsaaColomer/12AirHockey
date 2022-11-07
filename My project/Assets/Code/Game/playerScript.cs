using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private UnityEngine.Vector3 initScale;
    [SerializeField] private Rigidbody rb;
    public LayerMask ignore;
    [SerializeField] private Camera cam;
    private int powerType;
    public float applyPowerTime = 3;
    private float initApplyPowerTime;
    private UnityEngine.Vector3 reducedScale;
    private UnityEngine.Vector3 growedScale;
    private bool startApply = false;
    private GameObject player;
    private GameObject other;
    private string curName = "MainCamera_";
    private UnityEngine.Vector3 initDiskVel;
    private UnityEngine.Vector3 slowedDiskVel;
    [SerializeField] private Rigidbody disk;
    //gran petit
    //congelar
    //el disc va mes rapid
    void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        cam = GetComponent<Camera>();
        initApplyPowerTime = applyPowerTime;
        string name = this.gameObject.name.Replace(curName, null);
        Debug.Log(name);
        player = GameObject.Find(name).gameObject;
        initScale = player.GetComponent<Transform>().localScale;
        reducedScale = initScale / 1.5f;
        growedScale = initScale * 1.5f;
        disk = GameObject.Find("Disk").GetComponent<Rigidbody>();
        initDiskVel = disk.velocity;
        slowedDiskVel = initDiskVel/1.5f;
        for(int i = 0; i < GameObject.FindGameObjectsWithTag("Players").Length; ++i)
        {
            if(GameObject.FindGameObjectsWithTag("Players")[i].name != player.name)
            {
                other = GameObject.FindGameObjectsWithTag("Players")[i];
            }
        }
    }

    public void GetType(int type)
    {
        powerType = type;
        startApply = true;
    }
    void ApplyPowerUp(int type)
    {
        switch(type)
        {
            case 0:
                if(applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    player.GetComponent<Transform>().localScale = reducedScale;
                    Debug.Log(applyPowerTime);
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    player.GetComponent<Transform>().localScale = initScale;
                    startApply = false;
                }
                break;
            case 1:
                if (applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    player.GetComponent<Transform>().localScale = growedScale;
                    Debug.Log(applyPowerTime);
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    player.GetComponent<Transform>().localScale = initScale;
                    startApply = false;
                }
                break;
            case 2:
                if (applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    disk.velocity = slowedDiskVel;
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    disk.velocity = initDiskVel;
                    startApply = false;
                }
                break;
            case 3:
                if (applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    applyPowerTime = initApplyPowerTime; 
                    other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    startApply = false;
                }
                break;
            default:
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos-transform.position, Color.red);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,100f, ~ignore))
        {
            if(hit.transform.gameObject.tag == "Respawn")
            {
                UnityEngine.Vector3 dir = hit.point-rb.transform.position;
                rb.velocity = dir * 10f;
                Debug.DrawRay(transform.position, mousePos-transform.position, Color.green);
            }
        }

        if(startApply)
        {
            ApplyPowerUp(powerType);
        }
    }
}
