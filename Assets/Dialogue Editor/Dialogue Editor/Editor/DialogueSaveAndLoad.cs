using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
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

        public void SaveAs()
        {
            DialogueObject DialogueObject = ScriptableObject.CreateInstance<DialogueObject>();
            SaveEdges(DialogueObject);
            SaveNodes(DialogueObject);


            string path = EditorUtility.SaveFilePanel(
            "Save new Converation",
            "/Assets/Conversations/",
            "Conversation.asset",
            "asset");

            EditorUtility.SetDirty(DialogueObject);
            if (path.Length != 0)
                AssetDatabase.CreateAsset(DialogueObject, $"Assets{path.Substring(Application.dataPath.Length)}" );

            AssetDatabase.SaveAssets();
        }

        public void Save(DialogueObject DialogueObject)
        {
            SaveEdges(DialogueObject);
            SaveNodes(DialogueObject);

            EditorUtility.SetDirty(DialogueObject);
            AssetDatabase.SaveAssets();
        }

        public void Load(DialogueObject DialogueObject)
        {
            ClearGraph();
            GenerateNodes(DialogueObject);
            ConnectNodes(DialogueObject);
        }

        #region Save
        private void SaveEdges(DialogueObject DialogueObject)
        {
            DialogueObject.NodeLinkDatas.Clear();

            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                DialogueObject.NodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.NodeGuid,
                    BasePortName = connectedEdges[i].output.portName,
                    TargetNodeGuid = inputNode.NodeGuid,
                    TargetPortName = connectedEdges[i].input.portName,
                });
            }
        }



        private void SaveNodes(DialogueObject DialogueObject)
        {
            DialogueObject.EventDatas.Clear();
            DialogueObject.EndDatas.Clear();
            DialogueObject.StartDatas.Clear();
            DialogueObject.BranchDatas.Clear();
            DialogueObject.DialogueDatas.Clear();
            DialogueObject.ChoiceDatas.Clear();
            DialogueObject.ChoiceConnectorDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueNode dialogueNode:
                        DialogueObject.DialogueDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case StartNode startNode:
                        DialogueObject.StartDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        DialogueObject.EndDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        DialogueObject.EventDatas.Add(SaveNodeData(eventNode));
                        break;
                    case BranchNode branchNode:
                        DialogueObject.BranchDatas.Add(SaveNodeData(branchNode));
                        break;
                    case ChoiceNode choiceNode:
                        DialogueObject.ChoiceDatas.Add(SaveNodeData(choiceNode));
                        break;
                    case ChoiceConnectorNode choiceConnectorNode:
                        DialogueObject.ChoiceConnectorDatas.Add(SaveNodeData(choiceConnectorNode));
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

            dialogueData.DialogueData_Character.Character = node.DialogueData.DialogueData_Character.Character;

            foreach (DialogueData_BaseContainer baseContainer in node.DialogueData.Dialogue_BaseContainers)
            {

                // Text
                if (baseContainer is DialogueData_Text)
                {
                    DialogueData_Text tmp = (baseContainer as DialogueData_Text);
                    DialogueData_Text tmpData = new DialogueData_Text();

                    tmpData.ID = tmp.ID;
                    tmpData.GuidID = tmp.GuidID;
                    tmpData.sentence.AddRange(tmp.sentence);
                    tmpData.Sprite_Left = tmp.Sprite_Left;
                    tmpData.Sprite_Right = tmp.Sprite_Right;

                    tmpData.AudioClips = tmp.AudioClips;

                    dialogueData.DialogueData_Texts.Add(tmpData);
                }

                // Images
               /* if (baseContainer is DialogueData_Images)
                {
                    DialogueData_Images tmp = (baseContainer as DialogueData_Images);
                    DialogueData_Images tmpData = new DialogueData_Images();

                    tmpData.ID.Value = tmp.ID.Value;
                    tmpData.Sprite_Left.Value = tmp.Sprite_Left.Value;
                    tmpData.Sprite_Right.Value = tmp.Sprite_Right.Value;

                    dialogueData.DialogueData_Images.Add(tmpData);
                }*/
            }

            return dialogueData;
        }


        private ChoiceConnectorData SaveNodeData(ChoiceConnectorNode node)
        {
            ChoiceConnectorData choiceConnectorData = new ChoiceConnectorData
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            //choiceConnectorData.DialogueData_Character.Character = node.DialogueData.DialogueData_Character.Character;

            // Port
            foreach (DialogueData_Port port in node.ChoiceConnectorData.DialogueData_Ports)
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

                choiceConnectorData.DialogueData_Ports.Add(portData);
            }

            return choiceConnectorData;
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
                tmp.Number.Value = stringEvents.Number.Value;
                tmp.StringEventText.Value = stringEvents.StringEventText.Value;
                tmp.StringEventModifierType.Value = stringEvents.StringEventModifierType.Value;

                nodeData.EventData_StringModifiers.Add(tmp);
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
                tmp.Number.Value = stringEvents.Number.Value;
                tmp.StringEventText.Value = stringEvents.StringEventText.Value;
                tmp.StringEventConditionType.Value = stringEvents.StringEventConditionType.Value;

                nodeData.EventData_StringConditions.Add(tmp);
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
                tmp.StringEventText.Value = stringEvents.StringEventText.Value;
                tmp.Number.Value = stringEvents.Number.Value;
                tmp.StringEventConditionType.Value = stringEvents.StringEventConditionType.Value;

                nodeData.EventData_StringConditions.Add(tmp);
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

        private void GenerateNodes(DialogueObject dialogueContainer)
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
                    tempNode.AddCondition(item);
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
                    tempNode.AddCondition(item);
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }

            // Dialogue Node
            foreach (DialogueData node in dialogueContainer.DialogueDatas)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(node.Position, true);
                tempNode.NodeGuid = node.NodeGuid;

                tempNode.CharacterName(node.DialogueData_Character);
                List<DialogueData_BaseContainer> data_BaseContainer = new List<DialogueData_BaseContainer>();

                //data_BaseContainer.AddRange(node.DialogueData_Imagess);
                data_BaseContainer.AddRange(node.DialogueData_Texts);
                //data_BaseContainer.Add(node.DialogueData_Character);

                data_BaseContainer.Sort(delegate (DialogueData_BaseContainer x, DialogueData_BaseContainer y)
                {
                    return x.ID.Value.CompareTo(y.ID.Value);
                });

                foreach (DialogueData_BaseContainer data in data_BaseContainer)
                {
                    switch (data)
                    {
                        case DialogueData_Text Text:
                            tempNode.TextLine(Text);
                            break;
                        //case DialogueData_Images image:
                            //tempNode.ImagePic(image);
                            //break;
                        default:
                            break;
                    }
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }

            foreach (ChoiceConnectorData node in dialogueContainer.ChoiceConnectorDatas)
            {
                ChoiceConnectorNode tempNode = graphView.CreateChoiceConnectorNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (DialogueData_Port port in node.DialogueData_Ports)
                {
                    ChoiceNode choiceNode = graphView.Query<ChoiceNode>().Where(choice => choice.NodeGuid == port.InputGuid).First();
                    tempNode.AddChoicePort(tempNode,choiceNode, true, port);
                }

                tempNode.LoadValueInToField();
                tempNode.ReloadLanguage();
                graphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueObject dialogueContainer)
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