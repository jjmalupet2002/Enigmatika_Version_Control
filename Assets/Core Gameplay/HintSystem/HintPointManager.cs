using UnityEngine;
using UnityEngine.Events;
using CarterGames.Assets.SaveManager;
using Save; // Assuming your save system is under this namespace

public class HintPointManager : MonoBehaviour
{
    public HintPoints hintPointsSO; // Reference to the HintPoints ScriptableObject
    public HintPointsSaveObject hintPointsSaveObject; // Reference to the HintPointsSaveObject
    public UnityEvent onHintPointsUpdated;
    public UnityEvent onHintButtonDisplay;

    public AudioClip addHintSound; // Sound to play when hint points are added
    public AudioClip subtractHintSound; // Sound to play when hint points are subtracted
    public float soundVolume = 1f; // Volume for the sounds (1 = full volume, 0 = muted)

    private void OnEnable()
    {
        // Subscribe to the save and load events
        SaveEvents.OnSaveGame += SaveHintPoints;
        SaveEvents.OnLoadGame += LoadHintPoints;
    }

    private void OnDisable()
    {
        // Unsubscribe from the save and load events
        SaveEvents.OnSaveGame -= SaveHintPoints;
        SaveEvents.OnLoadGame -= LoadHintPoints;
    }

    public void AddHintPoints(int points)
    {
        hintPointsSO.hintPoints += points;
        onHintPointsUpdated.Invoke();

        PlaySound(addHintSound); // Play sound when points are added
    }

    public void SubtractHintPoints(int points)
    {
        hintPointsSO.hintPoints -= points;
        onHintPointsUpdated.Invoke();
        PlaySound(subtractHintSound); // Play sound when points are subtracted
    }

    public void UpdateHintPoints()
    {
        onHintPointsUpdated.Invoke();
        SaveEvents.SaveGame(); // Auto-save before switching to minigame scene
    }

    public void DisplayHintButton()
    {
        onHintButtonDisplay.Invoke();
    }

    public void ResetHintPoints()
    {
        hintPointsSO.hintPoints = 0;
        onHintPointsUpdated.Invoke();
    }

    // Save the current hint points to the save system
    private void SaveHintPoints()
    {
        // Save the current hint points value
        hintPointsSaveObject.hintPoints.Value = hintPointsSO.hintPoints; // Use 'Value' for SaveValue
        hintPointsSaveObject.Save(); // Call save method from SaveObject
    }

    // Load the hint points from the save system
    private void LoadHintPoints()
    {
        hintPointsSaveObject.Load(); // Load the saved data
        hintPointsSO.hintPoints = hintPointsSaveObject.hintPoints.Value; // Get the value from SaveValue and set it
        onHintPointsUpdated.Invoke(); // Update UI or trigger any other events after loading
    }

    // Play sound based on the AudioClip passed in with volume control
    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position, soundVolume);
        }
    }
}
