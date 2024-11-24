using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HintEvent : UnityEvent<Hint> { }

public static class HintEventManager
{
    public static HintEvent OnDisplayHint = new HintEvent();
    public static UnityEvent<int> OnAddHintPoints = new UnityEvent<int>();
    public static UnityEvent<int> OnSubtractHintPoints = new UnityEvent<int>();
}
