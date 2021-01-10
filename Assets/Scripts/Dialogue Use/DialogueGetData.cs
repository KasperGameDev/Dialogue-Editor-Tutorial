using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected DialogueContainerSO dialogueContainer;

    protected BaseNodeData GetNodeByGuid(string _targetNodeGuid)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
    }

    protected BaseNodeData GetNodeByNodePort(DialogueNodePort _nodePort)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
    }

    protected BaseNodeData GetNextNode(BaseNodeData _baseNodeData)
    {
        NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(egde => egde.BaseNodeGuid == _baseNodeData.NodeGuid);

        return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
    }
}
