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
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        pi_p1 = GameObject.FindGameObjectWithTag("PI_Player1");
        pi_p2 = GameObject.FindGameObjectWithTag("PI_Player2");
        startColor = pi_p1.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Goal_p2")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            pi_p2.GetComponent<Renderer>().material.color = Color.green;
            StartCoroutine(WaitToRestartDisk());
        }
        if(other.gameObject.name == "Goal_p1")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(0f,0f,0f);
            rb.drag = 0f;
            rb.angularDrag = 0f;
            pi_p1.GetComponent<Renderer>().material.color = Color.green;
            StartCoroutine(WaitToRestartDisk());
        }
    }
    IEnumerator WaitToRestartDisk()
    {
        yield return new WaitForSeconds(.7f);
        pi_p1.GetComponent<Renderer>().material.color = startColor;        
        pi_p2.GetComponent<Renderer>().material.color = startColor;        
        transform.position = startPos;
    }
}
