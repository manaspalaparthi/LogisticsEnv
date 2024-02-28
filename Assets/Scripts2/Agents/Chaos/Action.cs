
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Spaces;

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
        public virtual void Action(List<Agent> agents, VectorSensor sensor) {

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
        public override void Action(List<Agent> agents, VectorSensor sensor) {
            Debug.Log("Collecting observations for " + name);
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
        public override void Action(List<Agent> agents, VectorSensor sensor) {
            Debug.Log("Collecting observations for " + name);
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


}
