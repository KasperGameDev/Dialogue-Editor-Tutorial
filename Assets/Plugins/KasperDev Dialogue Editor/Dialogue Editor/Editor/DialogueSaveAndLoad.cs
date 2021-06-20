using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class DialogueSaveAndLoad
    {
        //private List<Edge> edges => graphView.edges.ToList();
        //private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

        //private DialogueGraphView graphView;

        //public DialogueSaveAndLoad(DialogueGraphView graphView)
        //{
        //    this.graphView = graphView;
        //}

        //public void Save(DialogueContainerSO dialogueContainerSO)
        //{
        //    SaveEdges(dialogueContainerSO);
        //    SaveNodes(dialogueContainerSO);

        //    EditorUtility.SetDirty(dialogueContainerSO);
        //    AssetDatabase.SaveAssets();
        //}

        //public void Load(DialogueContainerSO dialogueContainerSO)
        //{
        //    ClearGraph();
        //    GenerateNodes(dialogueContainerSO);
        //    ConnectNodes(dialogueContainerSO);
        //}

        //#region Save
        //private void SaveEdges(DialogueContainerSO dialogueContainerSO)
        //{
        //    dialogueContainerSO.NodeLinkDatas.Clear();

        //    Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        //    for (int i = 0; i < connectedEdges.Count(); i++)
        //    {
        //        BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
        //        BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

        //        dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData
        //        {
        //            BaseNodeGuid = outputNode.NodeGuid,
        //            BasePortName = connectedEdges[i].output.portName,
        //            TargetNodeGuid = inputNode.NodeGuid,
        //            TargetPortName = connectedEdges[i].input.portName,
        //        });
        //    }
        //}

        //private void SaveNodes(DialogueContainerSO dialogueContainerSO)
        //{
        //    dialogueContainerSO.DialogueNodeDatas.Clear();
        //    dialogueContainerSO.EventNodeDatas.Clear();
        //    dialogueContainerSO.EndNodeDatas.Clear();
        //    dialogueContainerSO.StartNodeDatas.Clear();
        //    dialogueContainerSO.BranchNodeDatas.Clear();

        //    nodes.ForEach(node =>
        //    {
        //        switch (node)
        //        {
        //            case DialogueNode dialogueNode:
        //                dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
        //                break;
        //            case StartNode startNode:
        //                dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
        //                break;
        //            case EndNode endNode:
        //                dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
        //                break;
        //            case EventNode eventNode:
        //                dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
        //                break;
        //            case BranchNode branchNode:
        //                dialogueContainerSO.BranchNodeDatas.Add(SaveNodeData(branchNode));
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //}

        //private DialogueNodeData SaveNodeData(DialogueNode node)
        //{
        //    DialogueNodeData dialogueNodeData = new DialogueNodeData
        //    {
        //        NodeGuid = node.NodeGuid,
        //        Position = node.GetPosition().position,
        //    };

        //    return dialogueNodeData;
        //}

        //private StartNodeData SaveNodeData(StartNode node)
        //{
        //    StartNodeData nodeData = new StartNodeData()
        //    {
        //        NodeGuid = node.NodeGuid,
        //        Position = node.GetPosition().position,
        //    };

        //    return nodeData;
        //}

        //private EndNodeData SaveNodeData(EndNode node)
        //{
        //    EndNodeData nodeData = new EndNodeData()
        //    {
        //        NodeGuid = node.NodeGuid,
        //        Position = node.GetPosition().position,
        //    };

        //    return nodeData;
        //}

        //private EventNodeData SaveNodeData(EventNode node)
        //{
        //    EventNodeData nodeData = new EventNodeData()
        //    {
        //        NodeGuid = node.NodeGuid,
        //        Position = node.GetPosition().position,

        //    };

        //    return nodeData;
        //}

        //private BranchNodeData SaveNodeData(BranchNode node)
        //{
        //    List<Edge> tmpEdges = edges.Where(x => x.output.node == node).Cast<Edge>().ToList();

        //    Edge trueOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "True");
        //    Edge flaseOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "False");

        //    BranchNodeData nodeData = new BranchNodeData()
        //    {
        //        NodeGuid = node.NodeGuid,
        //        Position = node.GetPosition().position,

        //    };

        //    return nodeData;
        //}
        //#endregion

        //#region Load

        //private void ClearGraph()
        //{
        //    edges.ForEach(edge => graphView.RemoveElement(edge));

        //    foreach (BaseNode node in nodes)
        //    {
        //        graphView.RemoveElement(node);
        //    }
        //}

        //private void GenerateNodes(DialogueContainerSO dialogueContainer)
        //{
        //    // Start
        //    foreach (StartNodeData node in dialogueContainer.StartNodeDatas)
        //    {
        //        StartNode tempNode = graphView.CreateStartNode(node.Position);
        //        tempNode.NodeGuid = node.NodeGuid;

        //        graphView.AddElement(tempNode);
        //    }

        //    // End Node 
        //    foreach (EndNodeData node in dialogueContainer.EndNodeDatas)
        //    {
        //        EndNode tempNode = graphView.CreateEndNode(node.Position);
        //        tempNode.NodeGuid = node.NodeGuid;

        //        tempNode.LoadValueInToField();
        //        graphView.AddElement(tempNode);
        //    }

        //    // Event Node
        //    foreach (EventNodeData node in dialogueContainer.EventNodeDatas)
        //    {
        //        EventNode tempNode = graphView.CreateEventNode(node.Position);
        //        tempNode.NodeGuid = node.NodeGuid;



        //        tempNode.LoadValueInToField();
        //        graphView.AddElement(tempNode);
        //    }

        //    // Breach Node
        //    foreach (BranchNodeData node in dialogueContainer.BranchNodeDatas)
        //    {
        //        BranchNode tempNode = graphView.CreateBranchNode(node.Position);
        //        tempNode.NodeGuid = node.NodeGuid;

        //        foreach (BrancStringIdData item in node.BrancStringIdDatas)
        //        {
        //            tempNode.AddCondition(item);
        //        }

        //        tempNode.LoadValueInToField();
        //        tempNode.ReloadLanguage();
        //        graphView.AddElement(tempNode);
        //    }

        //    // Dialogue Node
        //    foreach (DialogueNodeData node in dialogueContainer.DialogueNodeDatas)
        //    {
        //        DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
        //        tempNode.NodeGuid = node.NodeGuid;




        //        tempNode.LoadValueInToField();
        //        graphView.AddElement(tempNode);
        //    }
        //}

        //private void ConnectNodes(DialogueContainerSO dialogueContainer)
        //{
        //    // Make connection for all node.
        //    // Except for dialogue nodes.
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        List<NodeLinkData> connections = dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

        //        List<Port> allOutputPorts = nodes[i].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();                

        //        for (int j = 0; j < connections.Count; j++)
        //        {
        //            string targetNodeGuid = connections[j].TargetNodeGuid;
        //            BaseNode targetNode = nodes.First(node => node.NodeGuid == targetNodeGuid);

        //            if (targetNode == null)
        //                continue;

        //            foreach (Port item in allOutputPorts)
        //            {
        //                if(item.portName == connections[j].BasePortName)
        //                {
        //                    LinkNodesTogether(item, (Port)targetNode.inputContainer[0]);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void LinkNodesTogether(Port outputPort, Port inputPort)
        //{
        //    Edge tempEdge = new Edge()
        //    {
        //        output = outputPort,
        //        input = inputPort
        //    };
        //    tempEdge.input.Connect(tempEdge);
        //    tempEdge.output.Connect(tempEdge);
        //    graphView.Add(tempEdge);
        //}

        //#endregion
    }
}