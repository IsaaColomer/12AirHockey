using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScript : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    public LayerMask ignore;
    [SerializeField] private Camera cam;
    private int powerType;
    public Client_UDP client;
    public Server_UDP server;
    public Scene currentScene;
    public UnityEngine.Vector3 dir;
    public RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;
        cam = GetComponent<Camera>();
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
                if (this.gameObject.name != "Player_1")
                {
                    dir = hit.point - rb.transform.position;
                    rb.velocity = dir * 1500f * Time.deltaTime;
                }

                Debug.DrawRay(transform.position, mousePos - transform.position, Color.green);
            }
            else
            {
                rb.velocity = UnityEngine.Vector3.zero;
            }
        }
        else
        {
            rb.velocity = UnityEngine.Vector3.Lerp(rb.velocity, UnityEngine.Vector3.zero, Time.deltaTime);
        }
    }
}
