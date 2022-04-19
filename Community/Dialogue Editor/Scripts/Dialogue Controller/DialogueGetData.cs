using UnityEngine;

namespace DialogueEditor.Dialogue
{
    public class DialogueGetData : MonoBehaviour
    {
        [SerializeField] protected DialogueContainerSO dialogueContainerSO;

        protected BaseData GetNodeByGuid(string targetNodeGuid)
        {
            return dialogueContainerSO.AllData.Find(node => node.NodeGuid == targetNodeGuid);
        }

        protected BaseData GetNodeByNodePort(DialogueData_Port nodePort)
        {
            return dialogueContainerSO.AllData.Find(node => node.NodeGuid == nodePort.InputGuid);
        }

        protected BaseData GetNextNode(BaseData baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainerSO.NodeLinkData.Find(egde => egde.BaseNodeGuid == baseNodeData.NodeGuid);

            return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
        }

    }
}