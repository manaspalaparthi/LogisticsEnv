using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Unity.MLAgents;


using PA_DronePack;
public class Env : MonoBehaviour
{
    public int maxSmallBoxNum;
    public int smallBoxSuccCount;
    
    public int smallSpawnCount;

    // UAV Prefab
    public GameObject uavPrefab;
    public Vector3 UAVinitPos;
    public Vector3 LastUAVPos;
  
    // Building Obstacles
    public GameObject buildingPrefab;

    // Prefabs
    public GameObject smallBoxPrefab;
   
    public GameObject smallDestinationPrefab;
    public GameObject smallHubPrefab;

    public GameObject wallPrefab;

    // Destination Instance

    GameObject destinationInstance;

    // Hub Instance
    public GameObject smallHub;

    // map parameter
    int mapSize;
    int numBuilding;

    // map table array
    public int[,] world;

    public Text infoText;

    public float seconds;

    public string starttime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
    public string filepath = null, timepath = null;
    public string collidefile;
    public bool writelock;

    public int episode = -1;

    Stopwatch stopwatch = new Stopwatch();

    public int maxsteps = 10000;

    public void Awake() { 
        // directory check
        if (!Directory.Exists("./CSV/")) {
            Directory.CreateDirectory("./CSV/");
        }

        // csv file path
        filepath = "./CSV/count" + starttime + ".csv";
        timepath = "./CSV/time" + starttime + ".csv";
        collidefile = "./CSV/collide" + starttime + ".csv";

        // file check
        if (!File.Exists(filepath)) {
            File.Create(filepath);
        }
        if (!File.Exists(timepath)) {
            File.Create(timepath);
        }

        if (!File.Exists(collidefile)) {
            File.Create(collidefile);
        }
    }
    public void InitWorld(int ms, int nb, int slimit, int steps, int UAVAgents) {

   
        // start stopwatch
        stopwatch.Reset();

        // update episode
        episode++;

        maxsteps = steps;

        mapSize = ms;

        numBuilding = nb;

        infoText.text = "";

        smallSpawnCount = 0;
    
        smallBoxSuccCount = 0;
      
        maxSmallBoxNum = slimit;

        seconds = 0f;
        writelock = false;

        // delete all
        GameObject[] hubs = GameObject.FindGameObjectsWithTag("hub");
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("destination");
        GameObject[] parcels = GameObject.FindGameObjectsWithTag("parcel");
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("obstacle");
        GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");

        foreach (GameObject hub in hubs) {
            Destroy(hub);
        }

        foreach (GameObject destination in destinations) {
            Destroy(destination);
        }

        foreach (GameObject parcel in parcels) {
            Destroy(parcel);
        }

        foreach (GameObject building in buildings) {
            if (building.name != "wall") {
                Destroy(building);
            }
        }
        // init table
        world = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                world[i, j] = 0;
            }
        }

        // Spawn UAV Agents 

        LastUAVPos = UAVinitPos;
       //SpawnUAVAgents(UAVAgents);

        ResetUAVPosition();

        int x1 = Random.Range(0, mapSize);
        int z1 = Random.Range(0, mapSize);
        int x2 = Random.Range(0, mapSize);
        int z2 = Random.Range(0, mapSize);
        world[x1, z1] = 1;
        world[x2, z2] = 1;


        // GenerateWalls(mapSize);
        
        //spawn hub
        //smallHub = Instantiate(smallHubPrefab);
        //bigHub = Instantiate(bigHubPrefab);
        //smallHub.transform.position = new Vector3((float)(x1 - mapSize / 2), 0f, (float)(z1 - mapSize / 2));
        //bigHub.transform.position = new Vector3((float)(x2 - mapSize / 2), 0f, (float)(z2 - mapSize / 2));

        //spawn buildings
        for (int k = 0; k < numBuilding; k++) {
            while (true) {
                int x = Random.Range(0, mapSize);
                int z = Random.Range(0, mapSize);

                if (world[x, z] == 0) {
                    world[x, z] = 1;
                    GameObject building = Instantiate(buildingPrefab);
                    building.transform.position = new Vector3((float)(x - mapSize / 2), Random.Range(0f, 1f), (float)(z - mapSize / 2));
                    break;
                }
            }
        }

        //spawn destination
        
        destinationInstance = Instantiate(smallDestinationPrefab);
        destinationInstance.name = "small_dest(" + x1.ToString() + "," + z1.ToString() + ")";
        destinationInstance.transform.position = new Vector3((float)(x1 - mapSize / 2), 0f, (float)(z1 - mapSize / 2));
    

        //SpawnSmallBox();
        SpawnSmallBoxsRandom(slimit);

        // SpawnBigBox();

        stopwatch.Start();  
    }

   
    public void SpawnSmallBoxsRandom(int count)
    {
        // Spawn small boxs based on count
        for (int i = 0; i < count; i++)
        {
            GameObject boxInstance;

            while (true)
            {
                int x = Random.Range(0, mapSize);
                int z = Random.Range(0, mapSize);

                if (world[x, z] == 0)
                {
                    boxInstance = Instantiate(smallBoxPrefab);
                    boxInstance.transform.position =  new Vector3((float)(x - mapSize / 2), 5.0f, (float)(z - mapSize / 2));
                    boxInstance.name = "small_box(" + x.ToString() + "," + z.ToString() + ")";
                    //destinationInstance = Instantiate(smallDestinationPrefab);
                    //destinationInstance.name = "small_dest(" + x.ToString() + "," + z.ToString() + ")";
                    //destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
                    boxInstance.GetComponent<smallbox>().destPos = destinationInstance.transform.position;
                    boxInstance.GetComponent<smallbox>().dx = x;
                    boxInstance.GetComponent<smallbox>().dz = z;
                    break;
                }
            }

             smallSpawnCount++;
        }

    }

    public void EndEpisode() {
        // find all uav agents and call EndEpisode function 
        GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
        foreach (GameObject uav in uavs) {
            uav.GetComponent<UAVAgent>().AddReward(30f);
            uav.GetComponent<UAVAgent>().EndEpisode();
        }
    }
    
    // Update Information on screen
    void Update()
    {   
        infoText.text = "small : " + smallBoxSuccCount.ToString() + "/" + smallSpawnCount + "\n\ntime : " + stopwatch.ElapsedMilliseconds.ToString();
        // if no boxes spawn 10 more
        if (smallBoxSuccCount == smallSpawnCount) {
            EndEpisode();
            //SpawnSmallBoxsRandom(maxSmallBoxNum);
        }
    }

    public void NumberCheck() {
        if (!writelock && smallBoxSuccCount == smallSpawnCount) {
            writelock = true;
            WriteTime();
        }
    }

    public void WriteCSV() {
        string countstr = episode.ToString() + "," + smallBoxSuccCount.ToString() + "\n";
        if (!System.String.IsNullOrEmpty(filepath)) File.AppendAllText(filepath, countstr);

        GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
        string agentstr = episode.ToString();
        int s = 0;
        foreach (GameObject uav in uavs) {
            agentstr = agentstr + "," + uav.GetComponent<UAVAgent>().statespace.collideCount.ToString();
            s += uav.GetComponent<UAVAgent>().statespace.collideCount;
        }

        agentstr = agentstr + "," + s.ToString() + "\n";

        if (!System.String.IsNullOrEmpty(collidefile)) {
            File.AppendAllText(collidefile, agentstr);
        }
    }

    public void WriteTime() {
        stopwatch.Stop();
        string timestr = episode.ToString() + "," + stopwatch.ElapsedMilliseconds.ToString() + "\n";
        if (!System.String.IsNullOrEmpty(timepath)) File.AppendAllText(timepath, timestr);
    }

    // public void SpawnSmallBox() {

    //     if (smallSpawnCount < maxSmallBoxNum) {
    //         smallSpawnCount++;

    //         GameObject boxInstance;
    //         GameObject destinationInstance;
    //         while (true) {
    //             int x = Random.Range(0, mapSize);
    //             int z = Random.Range(0, mapSize);

    //             if (world[x, z] == 0) {
    //                 boxInstance = Instantiate(smallBoxPrefab);
    //                 Vector3 hubPos = smallHub.transform.position;
    //                 hubPos.y += 5f;
    //                 boxInstance.transform.position = hubPos;
    //                 boxInstance.name = "small_box(" + x.ToString() + "," + z.ToString() + ")";
    //                 destinationInstance = Instantiate(smallDestinationPrefab);
    //                 destinationInstance.name = "small_dest(" + x.ToString() + "," + z.ToString() + ")";
    //                 destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
    //                 boxInstance.GetComponent<smallbox>().destPos = destinationInstance.transform.position;
    //                 boxInstance.GetComponent<smallbox>().dx = x;
    //                 boxInstance.GetComponent<smallbox>().dz = z;
    //                 break;
    //             }
    //         }
    //     }
    // }

    public void SpawnUAVAgents(int count)
    {
        // Spawn UAV agents based on count
        for (int i = 0; i < count; i++)
        {
            GameObject uavInstance = Instantiate(uavPrefab);
            uavInstance.transform.position = GenerateUAVPosition();
            uavInstance.name = "uav" + i;
        }
    }

    public void ResetUAVPosition()
    {
        // reset all UAV positions
        GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
        foreach (GameObject uav in uavs)
        {
            uav.transform.position = GenerateUAVPosition();
        }
    }


    public Vector3 GenerateUAVPosition()
    {
        // UAVinitPos add 2 to x and z

        LastUAVPos.x += 2;
        LastUAVPos.z += 2;

        return LastUAVPos;
    }


    public void GenerateWalls(int mapSize) {

        // Generate Walls using prefab 

        void InstantiateWall(Vector3 position)
        {
        Instantiate(wallPrefab, position, Quaternion.identity);
        }

         // Create walls on the top and bottom sides
        for (int x = 0; x < mapSize+2; x++)
        {
            InstantiateWall(new Vector3(x-mapSize/2, 0, -mapSize/2-1));

            InstantiateWall(new Vector3(x-mapSize/2, 0, mapSize/2+1));
            
        }

        // Create walls on the left and right sides and rotate them accordingly
        for (int z = 0; z < mapSize+2; z++)
        {
            Instantiate(wallPrefab, new Vector3(-mapSize/2-1, 0, z-mapSize/2), Quaternion.Euler(0, 90, 0));
            Instantiate(wallPrefab, new Vector3(mapSize/2+1, 0, z-mapSize/2), Quaternion.Euler(0, 90, 0));
        }
    }
}
