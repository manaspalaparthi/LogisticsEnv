using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class LogisticsAcademy : MonoBehaviour
{
    public GameObject MAP;

    // Environment Parameters
    int mapsize = 13;
    int numbuilding = 3;
    int UAVAgents = 5;
    int slimit = 10;
    int maxstep = 10000;

    public void Awake() {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        var envParams = Academy.Instance.EnvironmentParameters;

        mapsize = (int)(envParams.GetWithDefault("mapsize", 20f)); // map size
        numbuilding = (int)(envParams.GetWithDefault("building_num", 0f)); // number of buildings
        slimit = (int)(envParams.GetWithDefault("slimit", 10f)); // 
        //maxstep = (int)(envParams.GetWithDefault("maxsteps", 10000f)); // 
        UAVAgents = (int)(envParams.GetWithDefault("uavagents", 5f)); //
    }

    public void EnvironmentReset() {
        MAP.GetComponent<Env>().WriteCSV();
        MAP.GetComponent<Env>().InitWorld(mapsize, numbuilding, slimit, maxstep, UAVAgents);
    }
}
