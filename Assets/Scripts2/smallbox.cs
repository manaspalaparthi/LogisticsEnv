using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PA_DronePack;

public class smallbox: MonoBehaviour
{
    public GameObject UAV;
    public bool isHold;

    public Vector3 destPos;
    // MAP
    public GameObject Env;

    public int dx, dz;

    void Start()
    {
        isHold = false;

        Env = GameObject.FindGameObjectWithTag("map");
    }

    void Update()
    {
        if (isHold)
        {
            Vector3 uavPos = UAV.transform.position;
            uavPos.y = Mathf.Max(0.3f, uavPos.y - 1.2f);
            gameObject.transform.position = uavPos;
            UAV.GetComponent<UAVAgent>().statespace.boxPos = gameObject.transform.position;
        }

        /*if (UAV) {
            if (!UAV.GetComponent<UAVAgent>().isHold) {
                UAV = null;
                isHold = false;

                Vector3 temp = MAP.GetComponent<map>().smallHub.transform.position;
                temp.y = 5f;
                gameObject.transform.position = temp;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }*/

        if (gameObject.transform.position.y < -3f) {
            Vector3 temp = gameObject.transform.position;
            temp.y = 5f;
            gameObject.transform.position = temp;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isHold && other.gameObject.CompareTag("uav"))
        {   
            UAV = GameObject.Find(other.gameObject.name);

            if (!UAV.GetComponent<UAVAgent>().statespace.isHold)
            {
                isHold = true;
                UAV.GetComponent<UAVAgent>().statespace.boxPos = gameObject.transform.position;
                UAV.GetComponent<UAVAgent>().statespace.isHold = true;
                UAV.GetComponent<UAVAgent>().statespace.boxType = 1;
                UAV.GetComponent<UAVAgent>().statespace.destinationPos = destPos;
                UAV.GetComponent<UAVAgent>().GiveReward(20f);

                // Spawn new parcel
                //MAP.GetComponent<map>().SpawnSmallBox();
            }
            else {
                UAV = null;
            }
        }

        if (other.gameObject.CompareTag("destination"))
        {
            if (destPos == other.transform.position && other.gameObject.name.Contains("small_dest")) {
                isHold = false;
                UAV.GetComponent<UAVAgent>().statespace.isHold = false;
                UAV.GetComponent<UAVAgent>().statespace.boxType = 0;
                UAV.GetComponent<UAVAgent>().GiveReward(20f);

                Destroy(gameObject);
                //Destroy(GameObject.Find(other.gameObject.name));
                Env.GetComponent<Env>().world[dx, dz] = 0;
                Env.GetComponent<Env>().smallBoxSuccCount++;
            }
        }
    }
}
