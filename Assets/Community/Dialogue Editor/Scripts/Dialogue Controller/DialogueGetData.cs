using UnityEngine;

namespace DialogueEditor.Dialogue
{
    public class DialogueGetData : MonoBehaviour
    {
        [SerializeField] protected DialogueContainerSO dialogueContainer;

        protected BaseData GetNodeByGuid(string targetNodeGuid)
        {
            return dialogueContainer.AllDatas.Find(node => node.NodeGuid == targetNodeGuid);
        }

        protected BaseData GetNodeByNodePort(DialogueData_Port nodePort)
        {
            return dialogueContainer.AllDatas.Find(node => node.NodeGuid == nodePort.InputGuid);
        }

        protected BaseData GetNextNode(BaseData baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(egde => egde.BaseNodeGuid == baseNodeData.NodeGuid);

            return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
        }

    }
}