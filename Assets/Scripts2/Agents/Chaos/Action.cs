
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using Spaces;
using DroneAgent;

namespace ActionCapabilities
{

    public class ChaosActionFactory {
        public static ActionCapability loadAction(String capName) {
            ActionCapability cap = null ;

            switch (capName) {
                case "ChaosCap_ChangeDestination":
                    cap = new ChaosCap_ChangeDestination();
                    break;
                case "ChaosCap_ChangeTarget":
                    cap = new ChaosCap_ChangeTarget();
                    break;
                case "ChaosCap_ChangecollideCount":
                    cap = new ChaosCap_ChangecollideCount();
                    break;
                case "ChaosCap_DropTarget":
                    cap = new ChaosCap_DropTarget();
                    break;
                default:
                    Debug.Log(capName + " action not found");
                    break;
            }
            return cap;
        }
    }

  
    public class ActionCapability {

        public string name = "";

        protected bool enabled = true;

        public virtual void InitialiseCap(ChaosAgent agent) {

        }
        public virtual void Action(List<Agent> agents, ActionBuffers actionBuffers) {

        }
        public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        } 
      }


    public class ChaosCap_ChangeDestination:ActionCapability {

        public string name = "ChaosCap_ChangeDestination";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " action added");
            }

        private bool enabled = true;
        public override void Action(List<Agent> agents, ActionBuffers actionBuffers) {
            Debug.Log("Chaos action Performed " + name);
               foreach (UAVAgent agent in agents) {
                agent.statespace.destinationPos = GetRandomPosition();
            }
        }
        private Vector3 GetRandomPosition() {
            return new Vector3(UnityEngine.Random.Range(-10, 10), 1, UnityEngine.Random.Range(-10, 10));
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class ChaosCap_ChangeTarget:ActionCapability {

        public string name = "ChaosCap_ChangeTarget";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " Action  added");
            }
        private bool enabled = true;
        public override void Action(List<Agent> agents, ActionBuffers actionBuffers) {
            Debug.Log("Chaos action Performed " + name);

            foreach (UAVAgent agent in agents) {
                agent.statespace.boxPos = GetRandomPosition();
            }
        }

        private Vector3 GetRandomPosition() {
            return new Vector3(UnityEngine.Random.Range(-10, 10), 1, UnityEngine.Random.Range(-10, 10));
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 



    public class ChaosCap_ChangecollideCount : ActionCapability {

        public string name = "ChaosCap_ChangecollideCount";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " capability added");
            }
        private bool enabled = true;
        public override void Action(List<Agent> agents, ActionBuffers actionBuffers) {
            Debug.Log("Chaos action Performed " + name);
            foreach (UAVAgent agent in agents) {
                agent.statespace.collideCount = UnityEngine.Random.Range(0, 99);
            }
        }

         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    }

    public class ChaosCap_DropTarget : ActionCapability {

        public string name = "ChaosCap_DropTarget";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " capability added");
            }
        private bool enabled = true;
        public override void Action(List<Agent> agents, ActionBuffers actionBuffers) {
            Debug.Log("Chaos action Performed " + name);
            foreach (UAVAgent agent in agents) {
                agent.statespace.isHold = false;
                agent.statespace.targetBox.GetComponent<smallbox>().isHold = false;
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
