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

    private void SaveNodes(DialogueContainerSO _dialogueContainerSO)
    {
        _dialogueContainerSO.DialogueNodeDatas.Clear();
        _dialogueContainerSO.EventNodeDatas.Clear();
        _dialogueContainerSO.EndNodeDatas.Clear();
        _dialogueContainerSO.StartNodeDatas.Clear();

        nodes.ForEach(node =>
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    _dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                    break;
                default:
                    break;
            }
        });
    }

    private DialogueNodeData SaveNodeData(DialogueNode _node)
    {
        DialogueNodeData dialogueNodeData = new DialogueNodeData
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            TextType = _node.Texts,
            Name = _node.name,
            AudioClips = _node.AudioClips,
            DialogueFaceImageType = _node.FaceImageType,
            Sprite = _node.FaceImage,
            DialogueNodePorts = _node.DialogueNodePorts
        };

        foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
        {
            foreach (Edge edge in edges)
            {
                if(edge.output == nodePort.MyPort)
                {
                    nodePort.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                    nodePort.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                }
            }
        }

        return dialogueNodeData;
    }
}
