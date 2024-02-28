using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
   public class ChaosConfig {
       public string name;
       public List<string> type;
       public List<string> State;
       public List<string> SharedState;
       public List<string> TargetAgentTeam;
       public List<string> capabilities;
       public List<string> Actions;
      
         public ChaosConfig(string filePath) {

            // Read the JSON file as a string
            string json = System.IO.File.ReadAllText(filePath);

            ChaosConfig config = JsonUtility.FromJson<ChaosConfig>(json); 

            this.name = config.name;
            this.State = config.State;
            this.SharedState = config.SharedState;
            this.capabilities = config.capabilities;
            this.Actions = config.Actions;
            this.TargetAgentTeam = config.TargetAgentTeam;
            this.type = config.type;
         }
    }

    public class UAVConfig {
        public string name;
        public List<string> State;
        public List<string> SharedState;
        public List<string> capabilities;
        public List<string> Actions;

        public UAVConfig(string filePath) {

        // Read the JSON file as a string
        string json = System.IO.File.ReadAllText(filePath);

        UAVConfig config = JsonUtility.FromJson<UAVConfig>(json); 

        this.name = config.name;
        this.State = config.State;
        this.SharedState = config.SharedState;
        this.capabilities = config.capabilities;
        this.Actions = config.Actions;
        
         }
    }

}
