using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScript : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    public bool canMove;
    public LayerMask ignore;
    [SerializeField] private Camera cam;
    private int powerType;
    public UnityEngine.Vector3 lastRaycastInsideBounds;
    public Client_UDP client;
    public Server_UDP server;
    public Scene currentScene;
    public UnityEngine.Vector3 dir;
    public RaycastHit hit;
    private float fraction;
    public Transform playerTransform;

    // ----------------------------- POWER UPS INFO ----------------------------- 
    private bool canApplyPowerUp; // YES
    public float applyPowerTime = 3f; // NO
    private float initApplyPowerTime; // NO
    private UnityEngine.Vector3 reducedScale; // NO
    private UnityEngine.Vector3 growedScale; // NO
    private UnityEngine.Vector3 initScale; // NO
    [SerializeField] private Disk_Code diskCode; // NO
    private UnityEngine.Vector3 slowedDiskVel; // NO
    private UnityEngine.Vector3 initDiskVel; // NO
    [SerializeField] private GameObject other; // NO
    [SerializeField] private string otherName; // YES
    // ----------------------------- POWER UPS INFO ----------------------------- 
    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;
        cam = GetComponent<Camera>();
        playerTransform = rb.gameObject.GetComponentInChildren<Transform>().transform;
        client = GameObject.Find("OnlineGameObject").GetComponent<Client_UDP>();
        server = GameObject.Find("OnlineGameObject").GetComponent<Server_UDP>();
        if(GameObject.Find("Disk") != null)
            diskCode = GameObject.Find("Disk").GetComponent<Disk_Code>();
        currentScene = SceneManager.GetActiveScene();

        lastRaycastInsideBounds = rb.transform.position;

        // POWER UPS INFO
        initApplyPowerTime = applyPowerTime;
        growedScale = gameObject.GetComponentInChildren<Transform>().localScale*1.5f;
        reducedScale = gameObject.GetComponentInChildren<Transform>().localScale / 1.5f;
        initScale = gameObject.GetComponentInChildren<Transform>().localScale;
        slowedDiskVel = new UnityEngine.Vector3(0.5f,0f,0.5f);
        initDiskVel = new UnityEngine.Vector3(1.0f,0f,1.0f);
        if(this.gameObject.name == "Player_1")
        {
            other = GameObject.Find("Player_2");
            otherName = other.name;
        }
        else
        {
            other = GameObject.Find("Player_1");
            otherName = other.name;
        }
    }

    public void GetType(int type)
    {
        powerType = type;
        canApplyPowerUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentScene.name == "GameClientUDP")
        {
            if(client.isLoged)
            {
                PlayerMovement();
            }
        }
        else if(currentScene.name == "GameServerUDP")
        {
            if (server.connected)
            {
                PlayerMovement();
            }
        }
        ApplyPowerUp();
    }
    void ApplyPowerUp()
    {
        if(canApplyPowerUp)
        {
            switch(powerType)
            {
            case 0:
                if(applyPowerTime > 0)
                {
                    if(diskCode != null)
                    {
                        applyPowerTime -= Time.deltaTime;                    
                        GameObject.Find(diskCode.lastPlayerName).GetComponent<Transform>().localScale = reducedScale;                        
                        
                        Debug.Log(applyPowerTime);
                    }
                    else
                    {
                        GameObject.Find(otherName).GetComponent<Transform>().localScale = reducedScale;
                    }
                }
                
                else
                {
                    if(diskCode != null)
                    {
                        applyPowerTime = initApplyPowerTime;
                        GameObject.Find(diskCode.lastPlayerName).GetComponent<Transform>().localScale = initScale;
                        canApplyPowerUp = false;
                    }
                    else
                    {
                        applyPowerTime = initApplyPowerTime;
                        GameObject.Find(otherName).GetComponent<Transform>().localScale = initScale;
                        canApplyPowerUp = false;
                    }
                }
                break;
            case 1:
                if (applyPowerTime > 0)
                {
                    if(diskCode != null)
                    {
                        applyPowerTime -= Time.deltaTime;
                        GameObject.Find(diskCode.lastPlayerName).GetComponent<Transform>().localScale = growedScale;
                        Debug.Log(applyPowerTime);
                    }
                    else
                    {
                        applyPowerTime -= Time.deltaTime;
                        GameObject.Find(otherName).GetComponent<Transform>().localScale = growedScale;
                    }        
                }
                else
                {
                    if(diskCode != null)
                    {
                        applyPowerTime = initApplyPowerTime;
                        GameObject.Find(diskCode.lastPlayerName).GetComponent<Transform>().localScale = initScale;
                        canApplyPowerUp = false;
                    }
                    else
                    {
                        applyPowerTime = initApplyPowerTime;
                        GameObject.Find(otherName).GetComponent<Transform>().localScale = initScale;
                        canApplyPowerUp = false;
                    }                  
                }
                break;
            case 2:
                if (applyPowerTime > 0)
                {
                    if(diskCode != null)
                    {
                        applyPowerTime -= Time.deltaTime;
                        GameObject.Find("Disk").GetComponent<Rigidbody>().velocity = slowedDiskVel;
                    }  
                }
                else
                {
                    if(diskCode != null)
                    {
                        applyPowerTime = initApplyPowerTime;
                        GameObject.Find("Disk").GetComponent<Rigidbody>().velocity = initDiskVel;
                        canApplyPowerUp = false;
                    }
                    
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
                    canApplyPowerUp = false;
                }
                break;
            default:
                break;
            }
        }
    }

    private void PlayerMovement()
    {
        UnityEngine.Vector3 mousePos = Input.mousePosition;

        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position, Color.red);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, ~ignore))
        {
            if (hit.transform.gameObject.tag == "Respawn")
            {
                dir = hit.point - rb.transform.position;
                lastRaycastInsideBounds = hit.point;
                lastRaycastInsideBounds.y = 0.85f;
                rb.velocity = dir * 10f;

                canMove = true;
            }
            else
            {
                playerTransform.position = UnityEngine.Vector3.Lerp(playerTransform.position, lastRaycastInsideBounds, 0.05f);
                rb.velocity = UnityEngine.Vector3.zero;
                canMove = false;
            }
        }
        else
        {
            canMove = false;
            playerTransform.position = UnityEngine.Vector3.Lerp(playerTransform.position, lastRaycastInsideBounds, 0.05f);
        }
    }
}
