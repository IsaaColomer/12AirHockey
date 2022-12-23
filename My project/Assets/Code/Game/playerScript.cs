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
    private UnityEngine.Vector3 previouslyStoredvelocity;
    public Transform playerTransform;
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
        currentScene = SceneManager.GetActiveScene();
    }

    public void GetType(int type)
    {
        powerType = type;
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
                rb.velocity = dir * 10f;
                previouslyStoredvelocity= rb.velocity;

                canMove = true;
                Debug.DrawRay(transform.position, mousePos - transform.position, Color.green);
            }
            else
            {
                playerTransform.position = UnityEngine.Vector3.Lerp(playerTransform.position, lastRaycastInsideBounds, 10.0f);
                rb.velocity = UnityEngine.Vector3.Lerp(dir*10f, UnityEngine.Vector3.zero, 0.5f);
                
                canMove = false;
            }
        }
        else
        {
            canMove = false;
            
            rb.velocity = UnityEngine.Vector3.Lerp(dir*10f, UnityEngine.Vector3.zero, 0.5f);

        }
    }
}
