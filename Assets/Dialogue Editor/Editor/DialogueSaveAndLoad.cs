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
                    BasePortName = connectedEdges[i].output.portName,
                    TargetNodeGuid = inputNode.NodeGuid,
                    TargetPortName = connectedEdges[i].input.portName,
                });
            }
        }

        private void SaveNodes(DialogueContainerSO dialogueContainerSO)
        {
            dialogueContainerSO.EventDatas.Clear();
            dialogueContainerSO.EndDatas.Clear();
            dialogueContainerSO.StartDatas.Clear();
            dialogueContainerSO.BranchDatas.Clear();
            dialogueContainerSO.DialogueDatas.Clear();
            dialogueContainerSO.ChoiceDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueNode dialogueNode:
                        dialogueContainerSO.DialogueDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case StartNode startNode:
                        dialogueContainerSO.StartDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        dialogueContainerSO.EndDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        dialogueContainerSO.EventDatas.Add(SaveNodeData(eventNode));
                        break;
                    case BranchNode branchNode:
                        dialogueContainerSO.BranchDatas.Add(SaveNodeData(branchNode));
                        break;
                    case ChoiceNode choiceNode:
                        dialogueContainerSO.ChoiceDatas.Add(SaveNodeData(choiceNode));
                        break;
                    default:
                        break;
                }
            });
        }

        private DialogueData SaveNodeData(DialogueNode node)
        {
            DialogueData dialogueData = new DialogueData
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            // Set ID
            for (int i = 0; i < node.DialogueData.Dialogue_BaseContainers.Count; i++)
            {
                node.DialogueData.Dialogue_BaseContainers[i].ID.Value = i;
            }

            foreach (DialogueData_BaseContainer baseContainer in node.DialogueData.Dialogue_BaseContainers)
            {
                // Name
                if (baseContainer is DialogueData_Name)
                {
                    DialogueData_Name tmp = (baseContainer as DialogueData_Name);
                    DialogueData_Name tmpData = new DialogueData_Name();

                    tmpData.ID.Value = tmp.ID.Value;
                    tmpData.CharacterName.Value = tmp.CharacterName.Value;

                    dialogueData.DialogueData_Names.Add(tmpData);
                }

                // Text
                if (baseContainer is DialogueData_Text)
                {
                    DialogueData_Text tmp = (baseContainer as DialogueData_Text);
                    DialogueData_Text tmpData = new DialogueData_Text();

                    tmpData.ID = tmp.ID;
                    tmpData.GuidID = tmp.GuidID;
                    tmpData.Text = tmp.Text;
                    tmpData.AudioClips = tmp.AudioClips;

                    dialogueData.DialogueData_Texts.Add(tmpData);
                }

                // Images
                if (baseContainer is DialogueData_Images)
                {
                    DialogueData_Images tmp = (baseContainer as DialogueData_Images);
                    DialogueData_Images tmpData = new DialogueData_Images();

                    tmpData.ID.Value = tmp.ID.Value;
                    tmpData.Sprite_Left.Value = tmp.Sprite_Left.Value;
                    tmpData.Sprite_Right.Value = tmp.Sprite_Right.Value;

                    dialogueData.DialogueData_Imagess.Add(tmpData);
                }
            }

            // Port
            foreach (DialogueData_Port port in node.DialogueData.DialogueData_Ports)
            {
                DialogueData_Port portData = new DialogueData_Port();

                portData.OutputGuid = string.Empty;
                portData.InputGuid = string.Empty;
                portData.PortGuid = port.PortGuid;

                foreach (Edge edge in edges)
                {
                    if (edge.output.portName == port.PortGuid)
                    {
                        portData.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                        portData.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                    }
                }

                dialogueData.DialogueData_Ports.Add(portData);
            }

            return dialogueData;
        }

        private StartData SaveNodeData(StartNode node)
        {
            StartData nodeData = new StartData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            return nodeData;
        }

        private EndData SaveNodeData(EndNode node)
        {
            EndData nodeData = new EndData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };
            nodeData.EndNodeType.Value = node.EndData.EndNodeType.Value;

            return nodeData;
        }

        private EventData SaveNodeData(EventNode node)
        {
            EventData nodeData = new EventData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            // Save Dialogue Event
            foreach (Container_DialogueEventSO dialogueEvent in node.EventData.Container_DialogueEventSOs)
            {
                nodeData.Container_DialogueEventSOs.Add(dialogueEvent);
            }

            // Save String Event
            foreach (EventData_StringModifier stringEvents in node.EventData.EventData_StringModifiers)
            {
                EventData_StringModifier tmp = new EventData_StringModifier();
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.VariableSO = stringEvents.VariableSO;
                tmp.EventType.Value = stringEvents.EventType.Value;

                nodeData.EventData_StringModifiers.Add(tmp);
            }

            foreach (EventData_FloatModifier FloatEvents in node.EventData.EventData_FloatModifiers)
            {
                EventData_FloatModifier tmp = new EventData_FloatModifier();
                tmp.Value.Value = FloatEvents.Value.Value;
                tmp.VariableSO = FloatEvents.VariableSO;
                tmp.EventType.Value = FloatEvents.EventType.Value;

                nodeData.EventData_FloatModifiers.Add(tmp);
            }

            foreach (EventData_IntModifier IntEvents in node.EventData.EventData_IntModifiers)
            {
                EventData_IntModifier tmp = new EventData_IntModifier();
                tmp.Value.Value = IntEvents.Value.Value;
                tmp.VariableSO = IntEvents.VariableSO;
                tmp.EventType.Value = IntEvents.EventType.Value;

                nodeData.EventData_IntModifiers.Add(tmp);
            }

            foreach (EventData_BoolModifier BoolEvents in node.EventData.EventData_BoolModifiers)
            {
                EventData_BoolModifier tmp = new EventData_BoolModifier();
                tmp.Value.Value = BoolEvents.Value.Value;
                tmp.VariableSO = BoolEvents.VariableSO;
                tmp.EventType.Value = BoolEvents.EventType.Value;

                nodeData.EventData_BoolModifiers.Add(tmp);
            }

            return nodeData;
        }

        private BranchData SaveNodeData(BranchNode node)
        {
            List<Edge> tmpEdges = edges.Where(x => x.output.node == node).Cast<Edge>().ToList();

            Edge trueOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "True");
            Edge flaseOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "False");

            BranchData nodeData = new BranchData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
                trueGuidNode = (trueOutput != null ? (trueOutput.input.node as BaseNode).NodeGuid : string.Empty),
                falseGuidNode = (flaseOutput != null ? (flaseOutput.input.node as BaseNode).NodeGuid : string.Empty),
            };

            foreach (EventData_StringCondition stringEvents in node.BranchData.EventData_StringConditions)
            {
                EventData_StringCondition tmp = new EventData_StringCondition();
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.VariableSO = stringEvents.VariableSO;
                tmp.EventType.Value = stringEvents.EventType.Value;

                nodeData.EventData_StringConditions.Add(tmp);
            }

            foreach (EventData_FloatCondition FloatEvents in node.BranchData.EventData_FloatConditions)
            {
                EventData_FloatCondition tmp = new EventData_FloatCondition();
                tmp.Value.Value = FloatEvents.Value.Value;
                tmp.VariableSO = FloatEvents.VariableSO;
                tmp.EventType.Value = FloatEvents.EventType.Value;

                nodeData.EventData_FloatConditions.Add(tmp);
            }

            foreach (EventData_IntCondition IntEvents in node.BranchData.EventData_IntConditions)
            {
                EventData_IntCondition tmp = new EventData_IntCondition();
                tmp.Value.Value = IntEvents.Value.Value;
                tmp.VariableSO = IntEvents.VariableSO;
                tmp.EventType.Value = IntEvents.EventType.Value;

                nodeData.EventData_IntConditions.Add(tmp);
            }

            foreach (EventData_BoolCondition BoolEvents in node.BranchData.EventData_BoolConditions)
            {
                EventData_BoolCondition tmp = new EventData_BoolCondition();
                tmp.Value.Value = BoolEvents.Value.Value;
                tmp.VariableSO = BoolEvents.VariableSO;
                tmp.EventType.Value = BoolEvents.EventType.Value;

                nodeData.EventData_BoolConditions.Add(tmp);
            }

            return nodeData;
        }

        private ChoiceData SaveNodeData(ChoiceNode node)
        {
            ChoiceData nodeData = new ChoiceData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,

                Text = node.ChoiceData.Text,
                AudioClips = node.ChoiceData.AudioClips,
            };
            nodeData.ChoiceStateTypes.Value = node.ChoiceData.ChoiceStateTypes.Value;

            foreach (EventData_StringCondition stringEvents in node.ChoiceData.EventData_StringConditions)
            {
                EventData_StringCondition tmp = new EventData_StringCondition();
                tmp.VariableSO = stringEvents.VariableSO;
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.EventType.Value = stringEvents.EventType.Value;

                nodeData.EventData_StringConditions.Add(tmp);
            }

            foreach (EventData_FloatCondition FloatEvents in node.ChoiceData.EventData_FloatConditions)
            {
                EventData_FloatCondition tmp = new EventData_FloatCondition();
                tmp.VariableSO = FloatEvents.VariableSO;
                tmp.Value.Value = FloatEvents.Value.Value;
                tmp.EventType.Value = FloatEvents.EventType.Value;

                nodeData.EventData_FloatConditions.Add(tmp);
            }

            foreach (EventData_IntCondition IntEvents in node.ChoiceData.EventData_IntConditions)
            {
                EventData_IntCondition tmp = new EventData_IntCondition();
                tmp.VariableSO = IntEvents.VariableSO;
                tmp.Value.Value = IntEvents.Value.Value;
                tmp.EventType.Value = IntEvents.EventType.Value;

                nodeData.EventData_IntConditions.Add(tmp);
            }

            foreach (EventData_BoolCondition BoolEvents in node.ChoiceData.EventData_BoolConditions)
            {
                EventData_BoolCondition tmp = new EventData_BoolCondition();
                tmp.VariableSO = BoolEvents.VariableSO;
                tmp.Value.Value = BoolEvents.Value.Value;
                tmp.EventType.Value = BoolEvents.EventType.Value;

                nodeData.EventData_BoolConditions.Add(tmp);
            }

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
            foreach (StartData node in dialogueContainer.StartDatas)
            {
                StartNode tempNode = graphView.CreateStartNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                graphView.AddElement(tempNode);
            }

            // End Node 
            foreach (EndData node in dialogueContainer.EndDatas)
            {
                EndNode tempNode = graphView.CreateEndNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;
                tempNode.EndData.EndNodeType.Value = node.EndNodeType.Value;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Event Node
            foreach (EventData node in dialogueContainer.EventDatas)
            {
                EventNode tempNode = graphView.CreateEventNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (Container_DialogueEventSO item in node.Container_DialogueEventSOs)
                {
                    tempNode.AddScriptableEvent(item);
                }

                foreach (EventData_StringModifier item in node.EventData_StringModifiers)
                {
                    tempNode.AddStringEvent(item);
                }
                foreach (EventData_FloatModifier item in node.EventData_FloatModifiers)
                {
                    tempNode.AddFloatEvent(item);
                }
                foreach (EventData_IntModifier item in node.EventData_IntModifiers)
                {
                    tempNode.AddIntEvent(item);
                }
                foreach (EventData_BoolModifier item in node.EventData_BoolModifiers)
                {
                    tempNode.AddBoolEvent(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Breach Node
            foreach (BranchData node in dialogueContainer.BranchDatas)
            {
                BranchNode tempNode = graphView.CreateBranchNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (EventData_StringCondition item in node.EventData_StringConditions)
                {
                    tempNode.AddStringCondition(item);
                }

                foreach (EventData_FloatCondition item in node.EventData_FloatConditions)
                {
                    tempNode.AddFloatCondition(item);
                }

                foreach (EventData_IntCondition item in node.EventData_IntConditions)
                {
                    tempNode.AddIntCondition(item);
                }

                foreach (EventData_BoolCondition item in node.EventData_BoolConditions)
                {
                    tempNode.AddBoolCondition(item);
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }

            // Choice Node
            foreach (ChoiceData node in dialogueContainer.ChoiceDatas)
            {
                ChoiceNode tempNode = graphView.CreateChoiceNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                tempNode.ChoiceData.ChoiceStateTypes.Value = node.ChoiceStateTypes.Value;

                foreach (LanguageGeneric<string> dataText in node.Text)
                {
                    foreach (LanguageGeneric<string> editorText in tempNode.ChoiceData.Text)
                    {
                        if (editorText.LanguageType == dataText.LanguageType)
                        {
                            editorText.LanguageGenericType = dataText.LanguageGenericType;
                        }
                    }
                }
                foreach (LanguageGeneric<AudioClip> dataAudioClip in node.AudioClips)
                {
                    foreach (LanguageGeneric<AudioClip> editorAudioClip in tempNode.ChoiceData.AudioClips)
                    {
                        if (editorAudioClip.LanguageType == dataAudioClip.LanguageType)
                        {
                            editorAudioClip.LanguageGenericType = dataAudioClip.LanguageGenericType;
                        }
                    }
                }

                foreach (EventData_StringCondition item in node.EventData_StringConditions)
                {
                    tempNode.AddStringCondition(item);
                }
                foreach (EventData_FloatCondition item in node.EventData_FloatConditions)
                {
                    tempNode.AddFloatCondition(item);
                }
                foreach (EventData_IntCondition item in node.EventData_IntConditions)
                {
                    tempNode.AddIntCondition(item);
                }
                foreach (EventData_BoolCondition item in node.EventData_BoolConditions)
                {
                    tempNode.AddBoolCondition(item);
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }

            // Dialogue Node
            foreach (DialogueData node in dialogueContainer.DialogueDatas)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                List<DialogueData_BaseContainer> data_BaseContainer = new List<DialogueData_BaseContainer>();

                data_BaseContainer.AddRange(node.DialogueData_Imagess);
                data_BaseContainer.AddRange(node.DialogueData_Texts);
                data_BaseContainer.AddRange(node.DialogueData_Names);

                data_BaseContainer.Sort(delegate (DialogueData_BaseContainer x, DialogueData_BaseContainer y)
                {
                    return x.ID.Value.CompareTo(y.ID.Value);
                });

                foreach (DialogueData_BaseContainer data in data_BaseContainer)
                {
                    switch (data)
                    {
                        case DialogueData_Name Name:
                            tempNode.CharacterName(Name);
                            break;
                        case DialogueData_Text Text:
                            tempNode.TextLine(Text);
                            break;
                        case DialogueData_Images image:
                            tempNode.ImagePic(image);
                            break;
                        default:
                            break;
                    }
                }

                foreach (DialogueData_Port port in node.DialogueData_Ports)
                {
                    tempNode.AddChoicePort(tempNode, port);
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueContainerSO dialogueContainer)
        {
            // Make connection for all node.
            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeLinkData> connections = dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

                List<Port> allOutputPorts = nodes[i].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    BaseNode targetNode = nodes.First(node => node.NodeGuid == targetNodeGuid);

                    if (targetNode == null)
                        continue;

                    foreach (Port item in allOutputPorts)
                    {
                        if (item.portName == connections[j].BasePortName)
                        {
                            LinkNodesTogether(item, (Port)targetNode.inputContainer[0]);
                        }
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