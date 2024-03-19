using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{

    public float windStrength = 0.1f;

    public Vector3 windDirection = new Vector3(0, 0, 0);

    // import MAP from Unity scene
    public GameObject MAP;

    public WindZone windZone; // Wind zone from the unity scene

    // Rigidbody of the attached object

    Rigidbody rb;

    public void SetWind(Vector3 direction, float strength)
    {
        windDirection = direction;
        windStrength = strength;
    }
   
    // Start is called before the first frame update
    void Start()
    {

        // Get the wind zone from the unity scene

        MAP =GameObject.FindGameObjectWithTag("map");

        windZone = MAP.GetComponent<WindZone>();

        // Get the rigidbody of the attached object
        rb = GetComponent<Rigidbody>();
 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the wind direction and strength from the wind zone

        windDirection = windZone.transform.forward;

        windStrength = windZone.windMain;

        // Apply the wind force to the object
        rb.AddForce(windDirection * windStrength, ForceMode.Impulse);

    }

}
