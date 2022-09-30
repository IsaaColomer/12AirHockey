    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundScript : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private string name;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        name = this.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.name == "Field_2_bounds" && name == "Player_1")
        {
            rb.velocity = UnityEngine.Vector3.zero;
        }
        if(other.transform.name == "Field_1_bounds" && name == "Player_2")
        {
            rb.velocity = UnityEngine.Vector3.zero;
        }
    }
}
