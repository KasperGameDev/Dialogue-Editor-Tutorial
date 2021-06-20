using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue
{
    [System.Serializable]
    public class DialogueEventSO : ScriptableObject
    {
        public virtual void RunEvent()
        {
            Debug.Log("Event was Call");
        }
    }
}