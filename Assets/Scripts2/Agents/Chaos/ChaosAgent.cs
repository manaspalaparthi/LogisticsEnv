using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Reflection;
using Utills;
using PA_DronePack;
using System.IO;
using Spaces;
using Configs;
using Capabilities;

public class ChaosAgent : Agent
{
    
    // Env gameobject
    GameObject Env;

    public State statespace;

    private ChaosConfig config;

    // capabilities


    public Dictionary <string, Capabilities.Capability> ChaosCaps = new Dictionary<string, Capabilities.Capability>();

    public void Start() {

    }

    public override void Initialize()
    {

        Env = GameObject.FindGameObjectWithTag("map");
        // Specify the path to your JSON file
        string filePath = "Assets/Scripts2/Agents/Chaos/ChaosConfig.json";

        // Create a new ChaosConfig object
        config = new ChaosConfig(filePath);

        // initialize state space with the state variables
        statespace = new State();
        foreach (string item in config.State)
        {
            statespace.AddVariable(item, 0f);
        }

        // load capabilities from the config file
        foreach (string item in config.capabilities)
        {
            Capability cap = ChaosCapabilityFactory.loadCapability(item);

            if (cap != null) {
                ChaosCaps[item] = cap;
                cap.InitialiseCap(this);
            } else {
                Debug.Log("Capability not found");
            } 
        }
        
    }

    public override void OnEpisodeBegin()
    {
         Env = GameObject.FindGameObjectWithTag("map");
        // Specify the path to your JSON file
        //string filePath = "DroneConfig.json";
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach(Capabilities.Capability cap in ChaosCaps.Values) {
            cap.CollectObservations(statespace, sensor);
        }
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

    protected override void OnEnable()
    {
        BehaviorParameters behaviorParameters = gameObject.GetComponent<BehaviorParameters>();

        if (behaviorParameters != null)
        {
            behaviorParameters.BrainParameters.VectorObservationSize = 30;
        }

        base.OnEnable();
    
    }


}

