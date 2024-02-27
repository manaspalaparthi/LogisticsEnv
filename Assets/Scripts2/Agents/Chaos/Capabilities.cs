
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

        public virtual void InitialiseCap(ChaosAgent agent) {

        }

        public virtual void CollectObservations(State StateSpace,  VectorSensor sensor) {

        }
        public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }

    }


    public class ChaosCap_ChangeDestination:Capability {

        public string name = "ChaosCap_ChangeDestination";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(State state, VectorSensor sensor) {
            Debug.Log("Collecting observations for " + name);
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 


    public class ChaosCap_ChangeTarget:Capability {

        public string name = "ChaosCap_ChangeTarget";

        public override void InitialiseCap(ChaosAgent agent) {
            Debug.Log(name + " capability added");
            }

        private bool enabled = true;
        public override void CollectObservations(State state, VectorSensor sensor) {
            Debug.Log("Collecting observations for " + name);
        }
         public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    } 



    

    public class DestInfo : Capability {

        protected bool enabled = true;

        public override void InitialiseCap (ChaosAgent agent) {
            agent.ChaosCaps["DestInfo"].disable();
        }

        private float calculateDistance (State state) {
        // ...
        return 0f;
        }

        public void CollectObservations(State state, VectorSensor sensor) {
        if (this.enabled){
            float distance = calculateDistance(state);
            sensor.AddObservation(distance);
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
