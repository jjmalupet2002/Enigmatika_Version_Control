using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [HideInInspector]
    public string objectID;

    private void Awake()
    {
        objectID = name + transform.position.ToString() + transform.eulerAngles.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        var dontDestroyObjects = UnityEngine.Object.FindObjectsOfType<DontDestroy>();
        UnityEngine.Debug.Log("Found objects: " + dontDestroyObjects.Length);

        for (int i = 0; i < dontDestroyObjects.Length; i++)
        {
            UnityEngine.Debug.Log("Checking objectID: " + dontDestroyObjects[i].objectID);
            if (dontDestroyObjects[i] != this && dontDestroyObjects[i].objectID == objectID)
            {
                UnityEngine.Debug.Log("Destroying duplicate object");
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
        UnityEngine.Debug.Log("Object marked as DontDestroyOnLoad");
    }
}
