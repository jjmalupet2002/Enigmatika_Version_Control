using System.Collections;
using UnityEngine;

public class EnterTrigger : MonoBehaviour


{

    private void OnTriggerEnter(Collider other)
    {
        if (other.compareTag("Player"))
        {
            print("IT WORKS!!");
        }
    }
}