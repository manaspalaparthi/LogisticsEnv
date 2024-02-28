
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Spaces;

namespace Capabilities
{

    public class ChaosCapabilityFactory {
        public static Capability loadCapability(String capName) {
            Capability cap = null ;

            switch (capName) {
                case "ChaosCap_ChangeDestination":
                    cap = new ChaosCap_ChangeDestination();
                    break;
                case "ChaosCap_ChangeTarget":
                    cap = new ChaosCap_ChangeTarget();
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

        public string name = "ChaosCap_ChangeDestination";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {
            Debug.Log("Collecting observations for " + name);
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class TargetInfo:Capability {

        public string name = "TargetInfo";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {

            GameObject Env = GameObject.FindGameObjectWithTag("map");
            // Add the number of targets to the state space
            sensor.AddObservation(Env.smallBoxSuccCount);
        
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class AgentRewardInfo :Capability {

        public string name = "ChaosCap_ChangeTarget";

        public override void InitialiseCap(Agent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;

        public override void CollectObservations(List<Agent> agents, VectorSensor sensor) {
              foreach (Agent agent in agents)
                Debug.Log("Agent reward: " + agent.GetCumulativeReward());
                sensor.AddObservation(agent.GetCumulativeReward());
            }
        
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 

}

