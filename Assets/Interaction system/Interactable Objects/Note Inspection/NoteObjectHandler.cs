using System.Collections.Generic;
using UnityEngine;

public class NoteObjectHandler : MonoBehaviour
{
    public List<GameObject> notePages; // List to store pages of the note

    private void Start()
    {
        if (notePages == null || notePages.Count == 0)
        {
            Debug.LogWarning("No pages assigned to the note object handler.");
            return;
        }

        // Register this note with the NoteInspectionManager
        NoteInspectionManager.Instance.RegisterNoteUI(this, notePages);
    }
}
