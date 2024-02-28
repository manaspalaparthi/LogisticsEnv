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
using ActionCapabilities;

public class ChaosAgent : Agent
{
    
    // Env gameobject
    GameObject Env;

    public State statespace;

    private ChaosConfig config;

    // capabilities
    public Dictionary <string, Capabilities.Capability> ChaosCaps = new Dictionary<string, Capabilities.Capability>();

    // actions
    public Dictionary <string, ActionCapabilities.ActionCapability> ChaosActions = new Dictionary<string, ActionCapabilities.ActionCapability>();

    public List<Agent> TargetAgents = null;
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

        // load actions from the config file
        foreach (string item in config.Actions)
        {
            ActionCapability cap = ChaosActionFactory.loadAction(item);

            if (cap != null) {
                ChaosActions[item] = cap;
                cap.InitialiseCap(this);
            } else {
                Debug.Log("Action not found");
            } 
        }

        // Get all agents in the scene based on the tag
        GameObject[] AgentGameObject = GameObject.FindGameObjectsWithTag(config.TargetAgentTeam);
         foreach (GameObject agent in AgentGameObject) {
                TargetAgents.Add(agent.GetComponent<UAVAgent>());
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
            cap.CollectObservations(TargetAgents, sensor);
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

