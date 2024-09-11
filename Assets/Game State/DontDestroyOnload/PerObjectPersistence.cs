
using UnityEngine;

public class PerObjectPersistence : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<PersistentObjectManager>().RegisterPersistentObject(gameObject);
    }
}
