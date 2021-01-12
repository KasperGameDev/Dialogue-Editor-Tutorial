using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Color Event")]
[System.Serializable]
public class Event_RandomColors : DialogueEventSO
{
    [SerializeField] private int number;
    public override void RunEvent()
    {
        GameEvents.Instance.CallRandomColorModel(number);
    }
}
