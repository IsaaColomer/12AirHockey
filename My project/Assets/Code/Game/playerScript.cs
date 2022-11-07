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
    //gran petit
    //congelar
    //el disc va mes rapid
    void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        cam = GetComponent<Camera>();
        initScale = gameObject.GetComponentInChildren<Transform>().localScale;
        initApplyPowerTime = applyPowerTime;
        reducedScale = initScale / 1.5f;
        growedScale = initScale * 1.5f;
    }

    public void GetType(int type)
    {
        powerType = type;
        startApply = true;
        Debug.Log("Player power: " + powerType);
    }
    void ApplyPowerUp(int type)
    {
        switch(type)
        {
            case 0:
                if(applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = reducedScale;
                    Debug.Log(applyPowerTime);
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = initScale;
                    startApply = false;
                }
                break;
            case 1:
                if (applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = growedScale;
                    Debug.Log(applyPowerTime);
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = initScale;
                    startApply = false;
                }
                break;
            case 2:
                if (applyPowerTime > 0)
                {
                    applyPowerTime -= Time.deltaTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = growedScale;
                    Debug.Log(applyPowerTime);
                }
                else
                {
                    applyPowerTime = initApplyPowerTime;
                    gameObject.GetComponentInChildren<Transform>().localScale = initScale;
                    startApply = false;
                }
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
