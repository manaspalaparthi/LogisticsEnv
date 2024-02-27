
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Spaces;

namespace Capabilities
{

    // class Capability {

    //     public static Capability Create(string name, bool enabled) {
    //         Capability cap = null;
    //         switch (name) {
    //             case "changeDestination":
    //                 cap = new changeDestination();
    //                 break;
    //         }
    //         return cap;
    //     }
    // }

    public class Capability {

        protected bool enabled = true;

        // public void Initialise(Agents.AwareAgent agent) {

        // }

        public void CollectObservations(State StateSpace,  VectorSensor sensor) {

        }

        public void disable() {
        enabled = false;
        }

        public void enable() {
        enabled = false;
        }
    }

    public class ChaosCap_ChangeDestination: Capability {

        // public void override Initialise(Agent agent) {
        //     agent.capabilities["ChaosCap_ChangeDestination"].disable();
        //     }
        private string name = "changeDestination";
        private bool enabled = true;
        public void run() {


            Debug.Log("destination changed");
        }

    } 

    public class ChaosCapabilityFactory {
        public static Capability loadCapability(String capName) {
            Capability cap = null;
            switch (capName) {
                case "ChaosCap_ChangeDestination":
                cap = new ChaosCap_ChangeDestination();
                break;
                }
                return cap;
            }
    }

    // class changeDestination : Capability{

    //     private string name = "changeDestination";
    //     private bool enabled = true;

    //     public void run() {
    //         Debug.Log("destination changed");
    //     }

    // }
    // class changeDestination2 : Capability{

    //     private string name = "changeDestination";
    //     private bool enabled = true;

    //     public void run() {
    //         Debug.Log("destination changed");
    //     }

    // }


    public class DestInfo : Capability {

        protected bool enabled = true;

        // public override Initialise (Agent agent) {

        // }

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
    }

}
