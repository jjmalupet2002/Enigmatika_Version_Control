using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject[] buttons; // Array of button GameObjects
    public Animator[] animators; // Array of button animator controllers
    public AudioSource[] buttonSounds; // Audio sources for each button
    public Material litMaterial; // Material for lit up button
    private bool[] buttonStates; // Track the current state (false = initial, true = lit)
    private Material[] originalMaterials; // Store the original materials for each button

    void Start()
    {
        buttonStates = new bool[buttons.Length];
        originalMaterials = new Material[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonStates[i] = false; // Initially all buttons are unlit
            originalMaterials[i] = buttons[i].GetComponent<Renderer>().material; // Store the original material
        }
    }

    void Update()
    {
        // Loop through all active touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Only check for the touch phase that started (i.e., when the touch begins)
            if (touch.phase == TouchPhase.Began)
            {
                // Convert the touch position to a ray
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the raycast hit any of the buttons
                    for (int j = 0; j < buttons.Length; j++)
                    {
                        if (hit.collider.gameObject == buttons[j])
                        {
                            OnButtonPress(j); // Call OnButtonPress for the corresponding button
                            break;
                        }
                    }
                }
            }
        }
    }

    public void OnButtonPress(int buttonIndex)
    {
        // Toggle the button's state and update its material immediately
        if (buttonStates[buttonIndex])
        {
            // If it's lit, turn it off and restore the original material
            buttons[buttonIndex].GetComponent<Renderer>().material = originalMaterials[buttonIndex];
            buttonStates[buttonIndex] = false;
        }
        else
        {
            // If it's not lit, light it up
            buttons[buttonIndex].GetComponent<Renderer>().material = litMaterial; // Apply material
            buttonStates[buttonIndex] = true;
        }

        // Trigger animation for the button press
        animators[buttonIndex].SetTrigger("ButtonPress");

        // Play the audio for the button press
        buttonSounds[buttonIndex].Play();

        // Handle the pattern logic for each button press
        HandleButtonPattern(buttonIndex);
    }

    private void HandleButtonPattern(int buttonIndex)
    {
        // Logic to handle the state of other buttons based on the pressed button
        switch (buttonIndex)
        {
            case 0: // Button 1 pressed
                buttonStates[1] = false; // Button 2 remains unlit
                buttonStates[2] = true;  // Button 3 lights up
                buttonStates[3] = true;  // Button 4 lights up
                break;
            case 1: // Button 2 pressed
                buttonStates[0] = false; // Button 1 remains unlit
                buttonStates[2] = false; // Button 3 remains unlit
                buttonStates[3] = true;  // Button 4 remains lit
                break;
            case 2: // Button 3 pressed
                buttonStates[0] = true;  // Button 1 lights up
                buttonStates[1] = false; // Button 2 remains unlit
                buttonStates[3] = false; // Button 4 remains unlit
                break;
            case 3: // Button 4 pressed
                buttonStates[0] = false; // Button 1 remains unlit
                buttonStates[1] = true;  // Button 2 lights up
                buttonStates[2] = true;  // Button 3 lights up
                break;
        }

        // Update the visual states for all buttons
        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        // Ensure that all buttons are visually updated
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonStates[i])
            {
                buttons[i].GetComponent<Renderer>().material = litMaterial;
            }
            else
            {
                buttons[i].GetComponent<Renderer>().material = originalMaterials[i];
            }
        }
    }
}