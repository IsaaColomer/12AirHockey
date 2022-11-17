using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    public LayerMask ignore;
    [SerializeField] private Camera cam;
    private int powerType;
    public UnityEngine.Vector3 dir;
    public RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        cam = GetComponent<Camera>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void GetType(int type)
    {
        powerType = type;
        Debug.Log("Player power: " + powerType);
    }

    // Update is called once per frame
    void Update()
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
                    if (this.gameObject.name != "Player_1")
                    {
                        dir = hit.point - rb.transform.position;
                        rb.velocity = dir * 10f;
                    }

                    Debug.DrawRay(transform.position, mousePos - transform.position, Color.green);
                }
            }
            else
            {
                rb.velocity = dir * 10f;
            }
                    
    }
}
