using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Reflection;
using Utills;
using PA_DronePack;
using System.IO;

public class UAVAgent : Agent
{
    // Drone Controll Scripts (PA_DroneControoller)
    private PA_DroneController dcoScript;

    public LineRenderer line;

    public class UAVConfig {
       public string name;
       public List<string> State;
       public List<string> SharedState;

         public UAVConfig(string filePath) {

            // Read the JSON file as a string
            string json = System.IO.File.ReadAllText(filePath);

            UAVConfig config = JsonUtility.FromJson<UAVConfig>(json); 

            this.name = config.name;
            this.State = config.State;
            this.SharedState = config.SharedState;
         }
    }

    public class StateSpace { 

        // get set method
        public Vector3 pos { get; set; }
        public Vector3 vel { get; set; }
        public int boxType { get; set; }
        public bool isHold { get; set; }

        public Vector3 boxPos { get; set; }
        public Vector3 destinationPos { get; set; }
        public GameObject targetBox { get; set; }
        public int collideCount { get; set; }
        public Vector3 startPos { get; set; }

    
        // constructor with parameters
        public StateSpace(Vector3 startPos) {
            this.pos = pos;
            this.vel = Vector3.zero;
            this.boxType = 0;
            this.isHold = false;
            this.boxPos = Vector3.zero;
            this.destinationPos = Vector3.zero;
            this.targetBox = null;
            this.collideCount = 0;
            this.startPos = startPos;
        }

        public void AgentInfo(VectorSensor sensor) {
            sensor.AddObservation(pos);
            sensor.AddObservation(vel);
            sensor.AddObservation(boxType);
            sensor.AddObservation(isHold);
            sensor.AddObservation(boxPos);
            sensor.AddObservation(destinationPos);
            sensor.AddObservation(collideCount);

        }
    }

    public class SharedStateSpace {

        private Dictionary<string, object> SharedState = new Dictionary<string, object>();

        public List<string> SharedStateList;

        public SharedStateSpace(UAVConfig config) {

            if (config.SharedState == null) {
                Debug.Log("SharedState is null");
            }
            else {
                SharedStateList = config.SharedState;

                foreach (string item in SharedStateList) {
                    SharedState.Add(item, null);
                }
            }
        }
        
        public void AddVariable(string variableName, object value)
        {
            SharedState[variableName] = value;
        }

        public dynamic GetVariable(string variableName)
        {
            if (SharedState.ContainsKey(variableName))
            {
                return SharedState[variableName];
            }
            else
            {
                Debug.LogWarning($"Variable '{variableName}' not found.");
                return 0f;
            }
        } 

        public void SetVariable(string variableName, object value)
        { 
            if (SharedState.ContainsKey(variableName))
            {
                SharedState[variableName] = value;
            }
            else
            {
                Debug.LogWarning($"Variable '{variableName}' not found.");
            }
        }

        public void update(StateSpace statespace)
        {
            foreach (string item in SharedStateList)
            {
                SetVariable(item, Utill.GetProperty(statespace, item));
            }
        }
    }

    // Env gameobject
    GameObject Env;

    public StateSpace statespace;
    public SharedStateSpace sharedstatespace;
    private UAVConfig config;

    // pre-distance current-distance
    float preDist, curDist;

    public void Start() {


    }

    public override void Initialize()
    {

        dcoScript = gameObject.GetComponent<PA_DroneController>();
        Env = GameObject.FindGameObjectWithTag("map");


        // Specify the path to your JSON file
        string filePath = "DroneConfig.json";

        // Create a new UAVConfig object
        config = new UAVConfig(filePath);

        
        // initialize state space
        statespace = new StateSpace(startPos:gameObject.transform.position);


        // initialize SharedStateSpace
        sharedstatespace = new SharedStateSpace(config);

        sharedstatespace.update(statespace);
        
        // initialize preDist, curDist

        preDist = 0f;
        curDist = 0f;

        // parameters
        statespace.isHold = false;
        statespace.boxType = 0;
        statespace.collideCount = 0;

        // Line Renderer
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.05f; line.endWidth = 0.05f;
        line.SetPosition(0, new Vector3(0f, -10f, 0f));
        line.SetPosition(1, new Vector3(0f, -10f, 0f));

    }
    public override void OnEpisodeBegin()
    {
        dcoScript = gameObject.GetComponent<PA_DroneController>();
        Env = GameObject.FindGameObjectWithTag("map");


        // Specify the path to your JSON file
        string filePath = "DroneConfig.json";

        
        // initialize state space
        statespace = new StateSpace(startPos:gameObject.transform.position);

        // initialize SharedStateSpace
        sharedstatespace = new SharedStateSpace(config);

        sharedstatespace.update(statespace);

        // initialize preDist, curDist

        preDist = 0f;
        curDist = 0f;

        // parameters
        statespace.isHold = false;
        statespace.boxType = 0;
        statespace.collideCount = 0;

        // Line Renderer
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.05f; line.endWidth = 0.05f;
        line.SetPosition(0, new Vector3(0f, -10f, 0f));
        line.SetPosition(1, new Vector3(0f, -10f, 0f));

    }

    public void OtherUAVsinfo(VectorSensor sensor) {

        // get all uavs in the scene 
        GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
        foreach (GameObject uav in uavs) {
            if (uav.gameObject.name != gameObject.name) {
                SharedStateSpace OtherUAV = uav.GetComponent<UAVAgent>().sharedstatespace;

                sensor.AddObservation(OtherUAV.GetVariable("pos"));
                sensor.AddObservation((OtherUAV.GetVariable("pos") - statespace.pos).magnitude);
                sensor.AddObservation(OtherUAV.GetVariable("boxType"));
                }
                }
    }

    public void TargetsInfo(VectorSensor sensor) {
        if (!statespace.targetBox) {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("parcel");
            GameObject minSmallBox = null;
            float minSmallBoxDist = 50f;
            foreach (GameObject b in boxes) {
                if (b.name.Contains("small") && (statespace.pos - b.transform.position).magnitude < minSmallBoxDist && !b.GetComponent<smallbox>().isHold) {
                    minSmallBox = b;
                    minSmallBoxDist = (statespace.pos - b.transform.position).magnitude;
                }
            }
            statespace.targetBox = minSmallBox;
        }

        if (statespace.targetBox) sensor.AddObservation(statespace.targetBox.transform.position);
        else sensor.AddObservation(Vector3.zero);

        if (statespace.targetBox) sensor.AddObservation((statespace.pos - statespace.targetBox.transform.position).magnitude);
        else sensor.AddObservation(0f);
    }

    public void DestinationInfo(VectorSensor sensor) {
        if (statespace.isHold) {
            sensor.AddObservation(statespace.destinationPos);
            sensor.AddObservation((statespace.destinationPos - statespace.pos).magnitude);
        }
        else {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }
    }
    

    public override void CollectObservations(VectorSensor sensor)
    {

        statespace.pos = gameObject.transform.position;
        statespace.vel = gameObject.GetComponent<Rigidbody>().velocity;
        
        sharedstatespace.update(statespace);
        // total 29 + ( 7 x (num_of_uavs - 1) ) + raycast ()

        statespace.AgentInfo(sensor);

        OtherUAVsinfo(sensor); 

        TargetsInfo(sensor);

        DestinationInfo(sensor);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   
        // Discrete Action
        float drive = 0f;
        float strafe = 0f;
        float lift = 0f;
        
        // forward
        if (actionBuffers.DiscreteActions[1] == 1) {
            drive = 1f;
        }

        // backward
        else if (actionBuffers.DiscreteActions[2] == 1) {
            drive = -1f;
        }

        // left
        if (actionBuffers.DiscreteActions[3] == 1) {
            strafe = 1f;
        }

        // right
        if (actionBuffers.DiscreteActions[4] == 1) {
            strafe = -1f;
        }

        // up
        if (actionBuffers.DiscreteActions[5] == 1) {
            lift = 1f;
        }

        // down
        if (actionBuffers.DiscreteActions[6] == 1) {
            lift = -1f;
        }

        // make UAV drive
        dcoScript.DriveInput(drive);
        dcoScript.StrafeInput(strafe);
        dcoScript.LiftInput(lift);

        // Give Reward following Magnitude between destination and this, when this holds parcel.
        if (statespace.isHold) {
            curDist = (statespace.destinationPos - gameObject.transform.position).magnitude;
            float reward = (preDist - curDist) * 0.5f;
            if (preDist != 0f) {
                AddReward(reward);
            }
            preDist = curDist;
        }
        else {
            if(statespace.targetBox){
                curDist = (statespace.targetBox.transform.position - gameObject.transform.position).magnitude;
            }
            else {
                curDist = 0f;
            }
            float reward = (preDist - curDist) * 0.5f;
            if (preDist != 0f) {
                AddReward(reward);
            }
            preDist = curDist;
        }
        

        // if UAV is holding
        if (statespace.isHold) {
            if (gameObject.transform.position.y < 1f) {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1f, gameObject.transform.position.z);
            }
            line.SetPosition(0, gameObject.transform.position);
            line.SetPosition(1, statespace.boxPos);
        }
        else {
            line.SetPosition(0, new Vector3(0f, -10f, 0f));
            line.SetPosition(1, new Vector3(0f, -10f, 0f));
        }

        if (statespace.boxType == 0 || !statespace.isHold) {
            statespace.boxType = 0;
            statespace.isHold = false;
        }

        // check shipped box number
        Env.GetComponent<Env>().NumberCheck();


    }

    // Player Heuristic Controll
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;
        discreteActionsOut[2] = 0;
        discreteActionsOut[3] = 0;
        discreteActionsOut[4] = 0;
        discreteActionsOut[5] = 0;
        discreteActionsOut[6] = 0;

        // forward, backward
        if (Input.GetKey(KeyCode.W)) {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            discreteActionsOut[2] = 1;
        }

        // left, right
        if (Input.GetKey(KeyCode.D)) {
            discreteActionsOut[3] = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            discreteActionsOut[4] = 1;
        }

        // up, down
        if (Input.GetKey(KeyCode.Q)) {
            discreteActionsOut[5] = 1;
        }
        if (Input.GetKey(KeyCode.E)) {
            discreteActionsOut[6] = 1;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // collide with another agent
        if (other.gameObject.CompareTag("uav"))
        {
            AddReward(-10.0f);
            statespace.collideCount++;
        }

        // collide with building
        if (other.gameObject.CompareTag("obstacle")) {
            AddReward(-10.0f);
            statespace.collideCount++;
        }
    }

    void OnCollisionStay(Collision other) {
        //
    }

    void OnCollisionExit(Collision other) {
        //
    }

    // Give Reward to this UAV at outside
    public void GiveReward(float reward)
    {
        AddReward(reward);
    }

    public void MakeEpisodeEnd() {
        EndEpisode();
    }

}

