using UnityEngine;
using System.Collections;

public class ItemDropper : MonoBehaviour
{
    [Header("Item Settings")]
    public GameObject item1;  // Reference to the item (key1)
    public GameObject item2;  // Reference to the item (key2)
    public GameObject item3;  // Reference to the item (key3)
    public GameObject item4;  // Reference to the item (key4)

    [Header("Audio Sources for Key Items")]
    public AudioSource audioSource1;  // Audio source for key1
    public AudioSource audioSource2;  // Audio source for key2
    public AudioSource audioSource3;  // Audio source for key3
    public AudioSource audioSource4;  // Audio source for key4

    [Header("Key Dropped Booleans")]
    public bool key1isDropped = false; // Boolean to check if key1 is dropped
    public bool key2isDropped = false; // Boolean to check if key2 is dropped
    public bool key3isDropped = false; // Boolean to check if key3 is dropped
    public bool key4isDropped = false; // Boolean to check if key4 is dropped

    [Header("Quest Object Mesh Renderers")]
    public MeshRenderer questMeshRenderer1;  // Reference to quest object mesh renderer for key1
    public MeshRenderer questMeshRenderer2;  // Reference to quest object mesh renderer for key2
    public MeshRenderer questMeshRenderer3;  // Reference to quest object mesh renderer for key3
    public MeshRenderer questMeshRenderer4;  // Reference to quest object mesh renderer for key4

    // Events for each key drop
    public delegate void KeyDropHandler();
    public event KeyDropHandler key1DropEvent;
    public event KeyDropHandler key2DropEvent;
    public event KeyDropHandler key3DropEvent;
    public event KeyDropHandler key4DropEvent;

    void Start()
    {
        // Ensure all AudioSources are assigned
        if (audioSource1 == null) audioSource1 = gameObject.AddComponent<AudioSource>();
        if (audioSource2 == null) audioSource2 = gameObject.AddComponent<AudioSource>();
        if (audioSource3 == null) audioSource3 = gameObject.AddComponent<AudioSource>();
        if (audioSource4 == null) audioSource4 = gameObject.AddComponent<AudioSource>();

        // Disable the MeshRenderers for the items
        DisableQuestMeshRenderers();

        // Disable the Box Colliders for each item
        DisableItemColliders();

    }

    void Update()
    {
        // Enable and play sound for the keys that have been dropped
        if (key1isDropped)
        {
            EnableItem(item1, audioSource1, questMeshRenderer1);
            key1isDropped = false;  // Reset the dropped boolean to avoid re-enabling
        }

        if (key2isDropped)
        {
            EnableItem(item2, audioSource2, questMeshRenderer2);
            key2isDropped = false;
        }

        if (key3isDropped)
        {
            EnableItem(item3, audioSource3, questMeshRenderer3);
            key3isDropped = false;
        }

        if (key4isDropped)
        {
            EnableItem(item4, audioSource4, questMeshRenderer4);
            key4isDropped = false;
        }
    }

    // Method to enable the item and play the sound
    private void EnableItem(GameObject item, AudioSource itemAudioSource, MeshRenderer questMeshRenderer)
    {
        // Enable the quest object's MeshRenderer
        if (questMeshRenderer != null)
        {
            questMeshRenderer.enabled = true;
        }

        // Play the drop sound for the specific key
        if (itemAudioSource != null)
        {
            itemAudioSource.Play();
        }
        else
        {
            UnityEngine.Debug.LogWarning("AudioSource reference is missing. Please assign it in the Inspector.");
        }
    }

    // Disable MeshRenderers for the quest objects
    private void DisableQuestMeshRenderers()
    {
        if (questMeshRenderer1 != null)
        {
            questMeshRenderer1.enabled = false;  // Disable the MeshRenderer for item 1
        }

        if (questMeshRenderer2 != null)
        {
            questMeshRenderer2.enabled = false;  // Disable the MeshRenderer for item 2
        }

        if (questMeshRenderer3 != null)
        {
            questMeshRenderer3.enabled = false;  // Disable the MeshRenderer for item 3
        }

        if (questMeshRenderer4 != null)
        {
            questMeshRenderer4.enabled = false;  // Disable the MeshRenderer for item 4
        }
    }

    // Disable Box Colliders for each item
    private void DisableItemColliders()
    {
        DisableCollider(item1);
        DisableCollider(item2);
        DisableCollider(item3);
        DisableCollider(item4);
    }

    private void DisableCollider(GameObject item)
    {
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;  // Disable the collider for the item
        }
    }

    // Method to invoke the drop event for each key
    public void InvokeKeyDropEvent(int keyNumber)
    {
        switch (keyNumber)
        {
            case 1:
                key1DropEvent?.Invoke();
                break;
            case 2:
                key2DropEvent?.Invoke();
                break;
            case 3:
                key3DropEvent?.Invoke();
                break;
            case 4:
                key4DropEvent?.Invoke();
                break;
            default:
                break;
        }
    }
}
