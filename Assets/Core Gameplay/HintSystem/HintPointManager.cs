using UnityEngine;
using UnityEngine.Events;

public class HintPointManager : MonoBehaviour
{
    public HintPoints hintPointsSO;
    public UnityEvent onHintPointsUpdated;
    public UnityEvent onHintButtonDisplay;

    public AudioClip addHintSound; // Sound to play when hint points are added
    public AudioClip subtractHintSound; // Sound to play when hint points are subtracted
    public float soundVolume = 1f; // Volume for the sounds (1 = full volume, 0 = muted)

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

    // Play sound based on the AudioClip passed in with volume control
    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position, soundVolume);
        }
    }
}
