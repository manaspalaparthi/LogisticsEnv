
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Reflection;
using Utills;
using System.IO;

namespace Spaces
{
public class State { 
        private Dictionary<string, object> StateSpace = new Dictionary<string, object>();

        public void AddStateSpace(VectorSensor sensor) {
            foreach (string item in StateSpace.Keys)
            {
                // convert the value to float
                sensor.AddObservation((float)StateSpace[item]);
            }
        }
        public void AddVariable(string variableName, object value)
        {
            StateSpace[variableName] = value;
        }

        public dynamic GetVariable(string variableName)
        {
            if (StateSpace.ContainsKey(variableName))
            {
                return StateSpace[variableName];
            }
            else
            {
                Debug.LogWarning($"Variable '{variableName}' not found.");
                return 0f;
            }
        } 

        public void SetVariable(string variableName, object value)
        { 
            if (StateSpace.ContainsKey(variableName))
            {
                StateSpace[variableName] = value;
            }
            else
            {
                Debug.LogWarning($"Variable '{variableName}' not found.");
            }
        }

    }

}