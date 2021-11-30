using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class ChoiceConnectorData : BaseData
    {
        public List<DialogueData_Port> DialogueData_Ports = new List<DialogueData_Port>();
    }

    /*
    [System.Serializable]
    public class ChoiceConnectorData
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
    }
    */
}
