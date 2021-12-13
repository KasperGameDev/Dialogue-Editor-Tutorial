using System.Collections.Generic;

namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class BranchData : BaseData
    {
        public string trueGuidNode;
        public string falseGuidNode;
        public List<EventData_StringCondition> EventData_StringConditions = new List<EventData_StringCondition>();
        public List<EventData_FloatCondition> EventData_FloatConditions = new List<EventData_FloatCondition>();
        public List<EventData_IntCondition> EventData_IntConditions = new List<EventData_IntCondition>();
        public List<EventData_BoolCondition> EventData_BoolConditions = new List<EventData_BoolCondition>();
    }
}
