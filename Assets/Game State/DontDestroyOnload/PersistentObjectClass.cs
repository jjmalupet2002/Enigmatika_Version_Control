using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PersistentObjectManager : MonoBehaviour
{
    private static List<GameObject> persistentObjects = new List<GameObject>();

    public void RegisterPersistentObject(GameObject obj)
    {
        if (!persistentObjects.Contains(obj))
        {
            persistentObjects.Add(obj);
            DontDestroyOnLoad(obj);
            UnityEngine.Debug.Log($"Registered persistent object: {obj.name}");
        }
        else
        {
            UnityEngine.Debug.Log($"Object already registered: {obj.name}");
        }
    }
}