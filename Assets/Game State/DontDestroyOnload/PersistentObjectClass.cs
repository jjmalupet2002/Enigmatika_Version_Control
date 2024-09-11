using System.Collections.Generic;
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
        }
    }
}