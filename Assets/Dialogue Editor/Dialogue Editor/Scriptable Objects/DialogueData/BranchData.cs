using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class BranchData : BaseData
    {
        public string trueGuidNode;
        public string falseGuidNode;
        public List<EventData_StringCondition> EventData_StringConditions = new List<EventData_StringCondition>();
    }
}
