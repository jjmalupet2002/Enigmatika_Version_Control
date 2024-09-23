using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectHandler : MonoBehaviour
{
    public SwitchCamera switchCameraInstance; // Reference to the specific SwitchCamera instance

    public void CallOnBackButtonPressed()
    {
        switchCameraInstance.OnBackButtonPressed();
    }
}