using System.Collections.Generic;

namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class EventData : BaseData
    {
        public List<EventData_StringModifier> EventData_StringModifiers = new List<EventData_StringModifier>();
        public List<EventData_FloatModifier> EventData_FloatModifiers = new List<EventData_FloatModifier>();
        public List<EventData_IntModifier> EventData_IntModifiers = new List<EventData_IntModifier>();
        public List<EventData_BoolModifier> EventData_BoolModifiers = new List<EventData_BoolModifier>();
        public List<Container_DialogueEventSO> Container_DialogueEventSOs = new List<Container_DialogueEventSO>();
    }
}
