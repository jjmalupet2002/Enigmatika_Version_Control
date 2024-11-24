using UnityEngine;

[CreateAssetMenu(fileName = "Hint", menuName = "ScriptableObjects/Hint", order = 1)]
public class Hint : ScriptableObject
{
    public string hintTitle;
    public string hintText;
    public int hintPoints; // Points required to use this hint
}
