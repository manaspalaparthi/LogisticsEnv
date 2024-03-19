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
using DroneAgent;

public class ChaosAgent : Agent
{
    
    // Env gameobject
    GameObject Env;

    public State statespace;

    private ChaosConfig config;

    // capabilities
    public Dictionary <string, Capabilities.Capability> ChaosCaps = new Dictionary<string, Capabilities.Capability>();

    // actions
    public Dictionary <string, ActionCapabilities.ActionCapability > ChaosActions = new Dictionary<string, ActionCapabilities.ActionCapability>();

    public List<Agent> TargetAgents = null;

    public int ContinuousActionSize = 0;
    public int DiscreteActionSize = 0;

    public void Start() {

    }

    public  void Initializer()
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

        // load Chaos actions from the config file
        foreach (string item in config.Actions)
        {
            ActionCapability cap = new ActionCapability();
            
            cap = ChaosActionFactory.loadAction(item);

            if (cap != null) {
                ChaosActions[item] = cap ;
                cap = cap.InitialiseCap(this);

                if (cap.actionType == "Discrete") {
                    DiscreteActionSize = DiscreteActionSize + cap.actionSize;
                } else if (cap.actionType == "Continuous") {
                    ContinuousActionSize = ContinuousActionSize + cap.actionSize;
                }

                Debug.Log("Action added: " + cap.name + " " + cap.actionType + " " + cap.actionSize);

            } else {
                Debug.Log("Action not found");
            } 

        }

        // Get all agents in the scene based on the tag
        GameObject[] AgentGameObject = GameObject.FindGameObjectsWithTag(config.TargetAgentTeam);

        TargetAgents = new List<Agent>();

        foreach (GameObject agent in AgentGameObject) {
            TargetAgents.Add(agent.GetComponent<Agent>());
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


        //map the actionbuffer to the action capability
        
        // foreach (string action in actionBuffers.DiscreteActions) {
        //     Debug.Log("Action: " + action);
        //     ChaosActions[action].Action(TargetAgents, actionBuffers);
        // }

    }

    // Player Heuristic Controll
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var discreteActionsOut = actionsOut.DiscreteActions;
    
    
        if (Input.GetKey(KeyCode.W)) {
            discreteActionsOut[0] = 1;
            ChaosActions["ChaosCap_ChangeDestination"].Action(TargetAgents, actionsOut);
        }
        if (Input.GetKey(KeyCode.E)) {
            discreteActionsOut[1] = 1;
            ChaosActions["ChaosCap_ChangeTarget"].Action(TargetAgents, actionsOut);
        }
        if (Input.GetKey(KeyCode.R)) {
            discreteActionsOut[2] = 1;
            ChaosActions["ChaosCap_ChangecollideCount"].Action(TargetAgents, actionsOut);
        }
        if (Input.GetKey(KeyCode.T)) {
            discreteActionsOut[3] = 1;
            ChaosActions["ChaosCap_DropTarget"].Action(TargetAgents, actionsOut);
        }


        var continuousActionsOut = actionsOut.ContinuousActions;


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

        Initializer();

        BehaviorParameters behaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        
        int [] DiscreteActions = new int[ChaosActions.Count];


        for (int i = 0; i < ChaosActions.Count; i++)
        {
            DiscreteActions[i] = 1;
        }

        if (behaviorParameters != null)
        {
            behaviorParameters.BrainParameters.VectorObservationSize = 30;

            behaviorParameters.BrainParameters.ActionSpec =  ActionSpec.MakeDiscrete(DiscreteActions);

            behaviorParameters.BrainParameters.ActionSpec =  ActionSpec.MakeContinuous(ContinuousActionSize);
        }

        base.OnEnable();
    }

}

