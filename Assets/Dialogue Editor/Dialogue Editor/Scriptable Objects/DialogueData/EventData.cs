using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class EventData : BaseData
    {
        public List<EventData_StringModifier> EventData_StringModifiers = new List<EventData_StringModifier>();
        public List<Container_DialogueEventSO> Container_DialogueEventSOs = new List<Container_DialogueEventSO>();
    }
}
