using UnityEngine;

public class NoteObjectHandler : MonoBehaviour
{
    public GameObject noteUI; // The UI element to display the note
    public string noteText; // Unique text content for the note
    public Font noteFont; // Font asset for the note text

    private void Start()
    {
        // Register this note with the NoteInspectionManager
        NoteInspectionManager.Instance.RegisterNoteUI(this, noteUI); // Only two arguments now
    }
}
