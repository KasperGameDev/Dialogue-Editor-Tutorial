using System.Collections.Generic;



namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class ChoiceConnectorData : BaseData
    {
        public List<DialogueData_Port> DialogueData_Ports = new List<DialogueData_Port>();
    }
}
