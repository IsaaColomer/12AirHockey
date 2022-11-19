using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk_Code : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject pi_p1;
    [SerializeField] private GameObject pi_p2;
    [SerializeField] private Color startColor;
    public float maxDiskVel = 3;
    public string lastPlayerName;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        pi_p1 = GameObject.FindGameObjectWithTag("PI_Player1");
        pi_p2 = GameObject.FindGameObjectWithTag("PI_Player2");
        startColor = pi_p1.GetComponent<Renderer>().material.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Goal_p2")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            pi_p1.GetComponent<Renderer>().material.color = Color.yellow;
            StartCoroutine(WaitToRestartDisk());
        }
        if(other.gameObject.name == "Goal_p1")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            pi_p2.GetComponent<Renderer>().material.color = Color.yellow;
            StartCoroutine(WaitToRestartDisk());
        }
    }
    IEnumerator WaitToRestartDisk()
    {
        // Just in case we wait a few moments to restart the disk
        yield return new WaitForSeconds(.7f);
        pi_p1.GetComponent<Renderer>().material.color = startColor;        
        pi_p2.GetComponent<Renderer>().material.color = startColor;        
        transform.position = startPos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Players")
        {
            // Here we detect which is the last player that has touched the disk
            lastPlayerName = collision.gameObject.name;
        }
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
    }
}
