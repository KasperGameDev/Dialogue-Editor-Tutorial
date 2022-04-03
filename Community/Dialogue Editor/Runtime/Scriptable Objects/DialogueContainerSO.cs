using System.Collections.Generic;
using UnityEngine;

namespace DialogueEditor.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue Editor/Dialogue Object")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {

        public int participatingSpeakers;
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();

        public EndData EndData = new EndData();
        public StartData StartData = new StartData();
        public List<RestartData> RestartDatas = new List<RestartData>();
        public List<RepeatData> RepeatDatas = new List<RepeatData>();
        public List<EventData> EventDatas = new List<EventData>();
        public List<ModifierData> ModifierDatas = new List<ModifierData>();
        public List<BranchData> BranchDatas = new List<BranchData>();
        public List<DialogueData> DialogueDatas = new List<DialogueData>();
        public List<ChoiceConnectorData> ChoiceConnectorDatas = new List<ChoiceConnectorData>();
        public List<ChoiceData> ChoiceDatas = new List<ChoiceData>();
        public List<ContextData> ContextDatas = new List<ContextData>();

        public List<BaseData> AllDatas
        {
            get
            {
                List<BaseData> tmp = new List<BaseData>();
                tmp.Add(EndData);
                tmp.Add(StartData);
                tmp.AddRange(RepeatDatas);
                tmp.AddRange(RestartDatas);
                tmp.AddRange(EventDatas);
                tmp.AddRange(ModifierDatas);
                tmp.AddRange(BranchDatas);
                tmp.AddRange(DialogueDatas);
                tmp.AddRange(ChoiceConnectorDatas);
                tmp.AddRange(ChoiceDatas);

                return tmp;
            }
        }
    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }
}