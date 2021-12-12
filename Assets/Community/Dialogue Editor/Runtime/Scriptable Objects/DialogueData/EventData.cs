using System.Collections.Generic;

namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class EventData : BaseData
    {
        public List<Container_DialogueEventSO> Container_DialogueEventSOs = new List<Container_DialogueEventSO>();
    }
}
