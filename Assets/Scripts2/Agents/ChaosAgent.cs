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

public class ChaosAgent : Agent
{
    

    public class ChaosConfig {
       public string name;
       public List<string> State;
       public List<string> SharedState;

         public ChaosConfig(string filePath) {

            // Read the JSON file as a string
            string json = System.IO.File.ReadAllText(filePath);

            UAVConfig config = JsonUtility.FromJson<UAVConfig>(json); 

            this.name = config.name;
            this.State = config.State;
            this.SharedState = config.SharedState;
         }
    }

    public class StateSpace { 

        public StateSpace(Vector3 startPos) {
            
        }

        public void AddStateSpace(VectorSensor sensor) {
            
          
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


    public void Start() {


    }

    public override void Initialize()
    {

        Env = GameObject.FindGameObjectWithTag("map");


        // Specify the path to your JSON file
        string filePath = "DroneConfig.json";


        // initialize state space
        statespace = new StateSpace(startPos:gameObject.transform.position);

        // initialize SharedStateSpace
        sharedstatespace = new SharedStateSpace(config);

        sharedstatespace.update(statespace);
        

    }
    public override void OnEpisodeBegin()
    {
         Env = GameObject.FindGameObjectWithTag("map");


        // Specify the path to your JSON file
        string filePath = "DroneConfig.json";


        // initialize state space
        statespace = new StateSpace(startPos:gameObject.transform.position);

        // initialize SharedStateSpace
        sharedstatespace = new SharedStateSpace(config);

        sharedstatespace.update(statespace);
        

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
        
    }

    // Player Heuristic Controll
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
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

