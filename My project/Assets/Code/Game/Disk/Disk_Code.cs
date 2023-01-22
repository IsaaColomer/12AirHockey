using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk_Code : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject piPlayer1;
    [SerializeField] private GameObject piPlayer2;
    [SerializeField] private Color startColor;
    [SerializeField] private Vector3 vel;
    public float maxDiskVel = 3;
    public string lastPlayerName = "Default";
    public bool clientGoal;
    private Server_UDP server;
    public float angle = 0;
    public float increment = 18f;
    private playerScript ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("Main Camera").GetComponent<playerScript>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        piPlayer1 = GameObject.FindGameObjectWithTag("PI_Player1");
        piPlayer2 = GameObject.FindGameObjectWithTag("PI_Player2");
        startColor = piPlayer1.GetComponent<Renderer>().material.color;
        server = GameObject.Find("OnlineGameObject").GetComponent<Server_UDP>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Goal_p2")
        {
            server.ClientScoredGoal();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            piPlayer1.GetComponent<Renderer>().material.color = Color.yellow;
            StartCoroutine(WaitToRestartDisk());
        }
        if(other.gameObject.name == "Goal_p1")
        {
            server.ServerScoredGoal();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            piPlayer2.GetComponent<Renderer>().material.color = Color.yellow;
            StartCoroutine(WaitToRestartDisk());
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Restart"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f, 0f, 0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            StartCoroutine(WaitToRestartDisk());
        }
    }
    IEnumerator WaitToRestartDisk()
    {
        // Just in case we wait a few moments to restart the disk
        yield return new WaitForSeconds(0.01f);
        clientGoal = false;
        piPlayer1.GetComponent<Renderer>().material.color = startColor;        
        piPlayer2.GetComponent<Renderer>().material.color = startColor;        
        transform.position = startPos;
    }
    public void Update()
    {
        if(rb.velocity.x > maxDiskVel)
        {
            rb.velocity = new Vector3 (maxDiskVel,rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.z > maxDiskVel)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxDiskVel);
        }
        vel = rb.velocity;
        RaycastDream();
    }
    private void RaycastDream()
    {
        for (int i = 0; i < 20; i++)
    {
        // Create a vector for the direction of the raycast in the x,z plane
        Vector3 direction = Quaternion.Euler(0, angle, 0) * new Vector3(transform.forward.x, 0, transform.forward.z);

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 0.1f))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Players") || hit.transform.gameObject.tag == "MainCamera")
            {
                if(hit.transform.gameObject.name == "Player_1")
                {
                    rb.AddForce(hit.normal*10f);
                    lastPlayerName = "Player_1";
                    if(ps.canApplyPowerUp)
                        server.lastPlayerName = lastPlayerName;
                }
                if(hit.transform.gameObject.name == "Player_2")
                {
                    rb.AddForce(hit.normal*10f);
                    lastPlayerName = "Player_2";
                    if(ps.canApplyPowerUp)
                        server.lastPlayerName = lastPlayerName;
                }
                
                Debug.DrawLine(transform.position, transform.position + direction * 0.3f, Color.green);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + direction * 0.3f, Color.blue);
            }
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Triggers"))
            {
                if(!hit.transform.gameObject.GetComponent<BoxCollider>().isTrigger)
                {
                    Debug.DrawLine(transform.position, transform.position + direction * 0.3f, Color.cyan);
                    rb.AddForce(hit.normal*5f);
                }
                    
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + direction * 0.3f, Color.blue);
        }  

        // Increase the angle for the next raycast
        angle += increment;
        if(angle >= 360f)
        {
            angle = 0f;
        }
    }
    }
}
