using UnityEngine;
using System;
using System.Reflection;

namespace Utills {

    public static class Utill 
    {
        // Method to dynamically add a property to a class
        public static void AddProperty<T>(object obj, string propertyName, T defaultValue)
        {
            Type type = obj.GetType();

            // Check if the property already exists
            PropertyInfo existingProperty = type.GetProperty(propertyName);
            if (existingProperty != null)
            {
                Debug.LogWarning($"Property '{propertyName}' already exists.");
                return;
            }

            // Create a new property with the specified name and type
            PropertyInfo newProperty = type.GetProperty(propertyName) ??
                                        type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (newProperty == null)
            {
                newProperty = type.GetProperty(propertyName,
                                                BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (newProperty == null)
            {
                // Create a new property with the specified name and type
                newProperty = type.GetProperty(propertyName,
                                                BindingFlags.NonPublic | BindingFlags.Instance) ??
                                type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) ??
                                type.GetProperty(propertyName);
            }

            // Add the new property to the class
            if (newProperty == null)
            {
                Debug.LogError($"Could not add property '{propertyName}'.");
                return;
            }

            // Set the default value for the new property
            newProperty.SetValue(obj, defaultValue);

            Debug.Log($"Property '{propertyName}' added successfully.");
        }


        public static void SetProperty(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();

            // Check if the property exists
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                Debug.LogError($"Property '{propertyName}' not found.");
                return;
            }

            // Set the value of the property
            property.SetValue(obj, value);

            Debug.Log($"Property '{propertyName}' set successfully.");
        }

        // Method to dynamically get the value of a property
        public static dynamic GetProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();

            // Check if the property exists
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                Debug.LogError($"Property '{propertyName}' not found.");
                return null;
            }

            // Get the value of the property
            return property.GetValue(obj);
        }

    }
}
