
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Spaces;
using DroneAgent;

namespace Capabilities
{

    public class ChaosCapabilityFactory {
        public static Capability loadCapability(String capName) {
            Capability cap = null ;

            switch (capName) {
                case "EnvInfo":
                    cap = new EnvInfo();
                    break;
                case "AgentTargetInfo":
                    cap = new AgentTargetInfo();
                    break;
                case "AgentRewardInfo":
                    cap = new AgentRewardInfo();
                    break;
                case "AgentDestInfo":
                    cap = new AgentDestInfo();
                    break;
                default:
                    Debug.Log("Capability not found");
                    break;
            }
            return cap;
        }
    }

    public class Capability {

        public string name = "";

        protected bool enabled = true;

        public virtual void InitialiseCap(Agent agent) {

        }

        // target Agents
        public virtual void CollectObservations(List<Agent> agents, VectorSensor sensor) {

        }
        public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }

    }


    public class EnvInfo:Capability {

        public string name = "EnvInfo";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {
          
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class AgentTargetInfo:Capability {

        public string name = "TargetInfo";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {

            GameObject envGameobject = GameObject.FindGameObjectWithTag("map");

            Env env  = envGameobject.GetComponent<Env>();
            // Add the number of targets to the state space
            sensor.AddObservation(env.smallBoxSuccCount);
        
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 
    public class AgentDestInfo:Capability {

        public string name = "TargetInfo";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;

       
        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {

            foreach (UAVAgent agent in agents){
                sensor.AddObservation(agent.statespace.destinationPos);
            }
        
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class AgentRewardInfo:Capability {

        public string name = "AgentRewardInfo";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;

        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {
              foreach (UAVAgent agent in agents){
                sensor.AddObservation(agent.GetCumulativeReward());
            }
            }
        
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 

}

