using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueSaveAndLoad
{
    private List<Edge> edges => graphView.edges.ToList();
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

    private DialogueGraphView graphView;

    public DialogueSaveAndLoad(DialogueGraphView _graphView)
    {
        graphView = _graphView;
    }

    public void Save(DialogueContainerSO _dialogueContainerSO)
    {

    }

    public void Load(DialogueContainerSO _dialogueContainerSO)
    {

    }

    private void SaveEdges(DialogueContainerSO _dialogueContainerSO)
    {
        _dialogueContainerSO.NodeLinkDatas.Clear();

        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++)
        {
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

            _dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.NodeGuid,
                TargetNodeGuid = inputNode.NodeGuid
            });
        }
    }
}
