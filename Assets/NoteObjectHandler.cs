using UnityEngine;

public class NoteObjectHandler : MonoBehaviour
{
    public GameObject noteUI; // The UI element to display the note image

    private void Start()
    {
        // Register this note with the NoteInspectionManager
        NoteInspectionManager.Instance.RegisterNoteUI(this, noteUI);
    }
}