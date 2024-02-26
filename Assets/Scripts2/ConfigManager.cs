using UnityEngine;
using System.IO;

[System.Serializable]
public class DroneConfig
{
    public float takeoffAltitude;
    public float altitude;
    public float takeoffSpeed;
    public float maxSpeed ;
    public float rotateSpeed ;
    public float maxAltitude;
    public float maxDistance;
    public float maxBattery;
    public float maxDistanceToTarget;
    public float maxDistanceToReturn;
    public float maxFoundTarget;

    // location
    public Vector3 startPosition;
}

[System.Serializable]
public class EnvironmentConfig
{
    public int NumberOfTargets;

    public int MaxStepsPerEpisode;

    public int NumberOfDroneAgents;

    public float TargetsSpawnRadius;
}


public class ConfigManager : MonoBehaviour
{
    public DroneConfig DroneConfig;

    public EnvironmentConfig EnvironmentConfig;

    private string DroneConfigFilePath;

    private string EnvironmentConfigFilePath;


    public TextAsset MyItemJSONDatabase; 

    private void Awake()
    {

        // Set the file path where the config will be saved
        //configFilePath = "Assets/Resources/config.json";

        string DroneConfigFilePath = "Assets/Resources/DroneConfig.json";
        string EnvironmentConfigFilePath = "Assets/Resources/EnvironmentConfig.json";

        // Load config data or create a new config object
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(DroneConfigFilePath)) 
        {
            // Load existing config file
            TextAsset Dronejson = Resources.Load<TextAsset>("DroneConfig");
            DroneConfig = JsonUtility.FromJson<DroneConfig>(Dronejson.text);
        }
        else
        {
            // Create a new config object if the file doesn't exist
            DroneConfig = new DroneConfig();
            SaveConfig();
        }

        if (File.Exists(EnvironmentConfigFilePath)) 
        {
            // Load existing config file
            TextAsset Environmentjson = Resources.Load<TextAsset>("EnvironmentConfig");
            EnvironmentConfig = JsonUtility.FromJson<EnvironmentConfig>(Environmentjson.text);
        }
        else
        {
            // Create a new config object if the file doesn't exist
            EnvironmentConfig = new EnvironmentConfig();
            SaveConfig();
        }
    }

    private void SaveConfig()
    {
        // Convert the config object to JSON format
        string json = JsonUtility.ToJson(DroneConfig);
        string json2 = JsonUtility.ToJson(EnvironmentConfig);

        // Save the JSON string to the config file
        File.WriteAllText("Assets/Resources/DroneConfig.json", json);
        File.WriteAllText("Assets/Resources/EnvironmentConfig.json", json2);
    }
}