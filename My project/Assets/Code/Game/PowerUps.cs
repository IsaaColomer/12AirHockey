using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    //gran petit
    //congelar
    //el disc va mes rapid
    [SerializeField] private int type;
    [SerializeField] private string playerName;
    [SerializeField] private GameObject playerToEffect;
    public float timeFreezed;
    [SerializeField] private Vector3 initScale;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 rbVelocity;
    [SerializeField] private string otherName;
    GameObject[] objs;
    // Start is called before the first frame update
    void Start()
    {
        type = Random.Range(0, 3);
        objs = GameObject.FindGameObjectsWithTag("Players");
        Debug.Log("Type: " + type);
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case 0:
                playerToEffect = GameObject.Find(playerName);
                initScale = playerToEffect.transform.localScale;
                playerToEffect.transform.localScale = initScale * 2;
                break;
            case 1:
                playerToEffect = GameObject.Find(playerName);
                initScale = playerToEffect.transform.localScale;
                playerToEffect.transform.localScale = initScale / 1.65f;
                break;
            case 2:

                rb = GameObject.Find(otherName).GetComponent<Rigidbody>();
                if (timeFreezed <= 0)
                {
                    timeFreezed -= Time.deltaTime;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                break;
            case 3:
                playerToEffect = GameObject.Find("Disk");
                rbVelocity = playerToEffect.GetComponent<Rigidbody>().velocity;
                playerToEffect.GetComponent<Rigidbody>().velocity = rbVelocity / 1.85f;
                break;

            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        playerName = other.gameObject.name;
        FindOtherPlayerName();
    }
    private void FindOtherPlayerName()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name != playerName)
            {
                otherName = objs[i].name;
            }
        }
    }
}
