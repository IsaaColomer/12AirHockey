using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    private Rigidbody rb;
    public LayerMask ignore;
    private Camera cam;
    public CharacterController player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.Find("Player_1").GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
    }
}
