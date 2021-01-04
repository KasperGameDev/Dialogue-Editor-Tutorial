using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
        SaveEdges(_dialogueContainerSO);
        SaveNodes(_dialogueContainerSO);

        EditorUtility.SetDirty(_dialogueContainerSO);
        AssetDatabase.SaveAssets();
    }

    public void Load(DialogueContainerSO _dialogueContainerSO)
    {
        ClearGraph();
        GenerateNodes(_dialogueContainerSO);
        ConnectNodes(_dialogueContainerSO);
    }

    #region Save
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
                case StartNode startNode:
                    _dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    _dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
                    break;
                case EventNode eventNode:
                    _dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
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
            Name = _node.Name,
            AudioClips = _node.AudioClips,
            DialogueFaceImageType = _node.FaceImageType,
            Sprite = _node.FaceImage,
            DialogueNodePorts = _node.DialogueNodePorts
        };

        foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
        {
            nodePort.OutputGuid = string.Empty;
            nodePort.InputGuid = string.Empty;
            foreach (Edge edge in edges)
            {
                if (edge.output == nodePort.MyPort)
                {
                    nodePort.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                    nodePort.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                }
            }
        }

        return dialogueNodeData;
    }

    private StartNodeData SaveNodeData(StartNode _node)
    {
        StartNodeData nodeData = new StartNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
        };

        return nodeData;
    }

    private EndNodeData SaveNodeData(EndNode _node)
    {
        EndNodeData nodeData = new EndNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            EndNodeType = _node.EndNodeType
        };

        return nodeData;
    }

    private EventNodeData SaveNodeData(EventNode _node)
    {
        EventNodeData nodeData = new EventNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            DialogueEventSO = _node.DialogueEvent
        };

        return nodeData;
    }
    #endregion

    #region Load

    private void ClearGraph()
    {
        edges.ForEach(edge => graphView.RemoveElement(edge));

        foreach (BaseNode node in nodes)
        {
            graphView.RemoveElement(node);
        }
    }

    private void GenerateNodes(DialogueContainerSO _dialogueContainer)
    {
        // Start
        foreach (StartNodeData node in _dialogueContainer.StartNodeDatas)
        {
            StartNode tempNode = graphView.CreateStartNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;

            graphView.AddElement(tempNode);
        }

        // End Node 
        foreach (EndNodeData node in _dialogueContainer.EndNodeDatas)
        {
            EndNode tempNode = graphView.CreateEndNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.EndNodeType = node.EndNodeType;

            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }

        // Event Node
        foreach (EventNodeData node in _dialogueContainer.EventNodeDatas)
        {
            EventNode tempNode = graphView.CreateEventNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.DialogueEvent = node.DialogueEventSO;

            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }

        // Dialogue Node
        foreach (DialogueNodeData node in _dialogueContainer.DialogueNodeDatas)
        {
            DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.Name = node.Name;
            tempNode.Texts = node.TextType;
            tempNode.FaceImage = node.Sprite;
            tempNode.FaceImageType = node.DialogueFaceImageType;
            tempNode.AudioClips = node.AudioClips;

            foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
            {
                tempNode.AddChoicePort(tempNode, nodePort);
            }

            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes(DialogueContainerSO _dialogueContainer)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            List<NodeLinkData> connections = _dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].TargetNodeGuid;
                BaseNode targetNode = nodes.First(node => node.NodeGuid == targetNodeGuid);

                if ((nodes[i] is DialogueNode) == false)
                {
                    LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                }
            }
        }

        List<DialogueNode> dialogueNodes = nodes.FindAll(node => node is DialogueNode).Cast<DialogueNode>().ToList();

        foreach (DialogueNode dialogueNode in dialogueNodes)
        {
            foreach (DialogueNodePort nodePort in dialogueNode.DialogueNodePorts)
            {
                if (nodePort.InputGuid != string.Empty)
                {
                    BaseNode targetNode = nodes.First(Node => Node.NodeGuid == nodePort.InputGuid);
                    LinkNodesTogether(nodePort.MyPort, (Port)targetNode.inputContainer[0]);
                }

            }
        }
    }

    private void LinkNodesTogether(Port _outputPort, Port _inputPort)
    {
        Edge tempEdge = new Edge()
        {
            output = _outputPort,
            input = _inputPort
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        graphView.Add(tempEdge);
    }

    #endregion
}
