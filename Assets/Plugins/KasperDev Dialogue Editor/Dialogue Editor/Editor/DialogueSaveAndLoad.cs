using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    public class DialogueSaveAndLoad
    {
        private List<Edge> edges => graphView.edges.ToList();
        private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

        private DialogueGraphView graphView;

        public DialogueSaveAndLoad(DialogueGraphView graphView)
        {
            this.graphView = graphView;
        }

        public void Save(DialogueContainerSO dialogueContainerSO)
        {
            SaveEdges(dialogueContainerSO);
            SaveNodes(dialogueContainerSO);

            EditorUtility.SetDirty(dialogueContainerSO);
            AssetDatabase.SaveAssets();
        }

        public void Load(DialogueContainerSO dialogueContainerSO)
        {
            ClearGraph();
            GenerateNodes(dialogueContainerSO);
            ConnectNodes(dialogueContainerSO);
        }

        #region Save
        private void SaveEdges(DialogueContainerSO dialogueContainerSO)
        {
            dialogueContainerSO.NodeLinkDatas.Clear();

            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.NodeGuid,
                    TargetNodeGuid = inputNode.NodeGuid
                });
            }
        }

        private void SaveNodes(DialogueContainerSO dialogueContainerSO)
        {
            dialogueContainerSO.DialogueNodeDatas.Clear();
            dialogueContainerSO.EventNodeDatas.Clear();
            dialogueContainerSO.EndNodeDatas.Clear();
            dialogueContainerSO.StartNodeDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueNode dialogueNode:
                        dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case StartNode startNode:
                        dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
                        break;
                    default:
                        break;
                }
            });
        }

        private DialogueNodeData SaveNodeData(DialogueNode node)
        {
            DialogueNodeData dialogueNodeData = new DialogueNodeData
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
                TextLanguages = node.TextLanguages,
                CharacterName = node.CharacterName,
                AudioClips = node.AudioClips,
                DialogueFaceImageType = node.DialogueFaceImageType,
                FaceImage = node.FaceImage,
                DialogueNodePorts = new List<DialogueNodePort>(node.DialogueNodePorts)
            };

            foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output.portName == nodePort.PortGuid)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                    }
                }
            }

            return dialogueNodeData;
        }

        private StartNodeData SaveNodeData(StartNode node)
        {
            StartNodeData nodeData = new StartNodeData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            return nodeData;
        }

        private EndNodeData SaveNodeData(EndNode node)
        {
            EndNodeData nodeData = new EndNodeData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
                EndNodeType = node.EndNodeType
            };

            return nodeData;
        }

        private EventNodeData SaveNodeData(EventNode node)
        {
            EventNodeData nodeData = new EventNodeData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
                EventScriptableObjectDatas = node.EventScriptableObjectDatas,
                EventStringIdDatas = node.EventStringIdDatas,
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

        private void GenerateNodes(DialogueContainerSO dialogueContainer)
        {
            // Start
            foreach (StartNodeData node in dialogueContainer.StartNodeDatas)
            {
                StartNode tempNode = graphView.CreateStartNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                graphView.AddElement(tempNode);
            }

            // End Node 
            foreach (EndNodeData node in dialogueContainer.EndNodeDatas)
            {
                EndNode tempNode = graphView.CreateEndNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;
                tempNode.EndNodeType = node.EndNodeType;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Event Node
            foreach (EventNodeData node in dialogueContainer.EventNodeDatas)
            {
                EventNode tempNode = graphView.CreateEventNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (EventScriptableObjectData item in node.EventScriptableObjectDatas)
                {
                    tempNode.AddScriptableEvent(item);
                }
                foreach (EventStringIdData item in node.EventStringIdDatas)
                {
                    tempNode.AddStringEvent(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Dialogue Node
            foreach (DialogueNodeData node in dialogueContainer.DialogueNodeDatas)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;
                tempNode.CharacterName = node.CharacterName;
                tempNode.FaceImage = node.FaceImage;
                tempNode.DialogueFaceImageType = node.DialogueFaceImageType;

                foreach (LanguageGeneric<string> languageGeneric in node.TextLanguages)
                {
                    tempNode.TextLanguages.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClips.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueContainerSO dialogueContainer)
        {
            // Make connection for all node.
            // Except for dialogue nodes.
            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeLinkData> connections = dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

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

            // Make connection for dialogue nodes.
            List<DialogueNode> dialogueNodes = nodes.FindAll(node => node is DialogueNode).Cast<DialogueNode>().ToList();

            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                foreach (DialogueNodePort nodePort in dialogueNode.DialogueNodePorts)
                {
                    // Check if port has a connection.
                    if (nodePort.InputGuid != string.Empty)
                    {
                        // Find target node with ID.
                        BaseNode targetNode = nodes.First(Node => Node.NodeGuid == nodePort.InputGuid);

                        Port myPort = null;

                        // Check all ports in nodes outputContainer.
                        for (int i = 0; i < dialogueNode.outputContainer.childCount; i++)
                        {
                            // Find port with same ID, we use portName as ID.
                            if (dialogueNode.outputContainer[i].Q<Port>().portName == nodePort.PortGuid)
                            {
                                myPort = dialogueNode.outputContainer[i].Q<Port>();
                            }
                        }

                        // Make a connection between the ports.
                        LinkNodesTogether(myPort, (Port)targetNode.inputContainer[0]);
                    }

                }
            }
        }

        private void LinkNodesTogether(Port outputPort, Port inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = outputPort,
                input = inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        #endregion
    }
}