using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
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
            DialogueContainerSO DialogueContainerSO = ScriptableObject.CreateInstance<DialogueContainerSO>();
            SaveEdges(DialogueContainerSO);
            SaveNodes(DialogueContainerSO);


            string path = EditorUtility.SaveFilePanel(
            "Save new Converation",
            "/Assets/Conversations/",
            "NewDialogueObject.asset",
            "asset");

            if (path.Contains(Application.dataPath))
            {

                EditorUtility.SetDirty(DialogueContainerSO);
                if (path.Length != 0)
                    AssetDatabase.CreateAsset(DialogueContainerSO, $"Assets{path.Substring(Application.dataPath.Length)}");

                AssetDatabase.SaveAssets();
                Debug.Log($"<color=green>Error: </color>Saved succesfully to {Application.dataPath}/.....");
            }
            else if(!path.Equals(null) && !path.Equals(""))
                Debug.Log($"<color=red>Error: </color>You must Save the Dialogue Object in this Project! Save your File under {Application.dataPath}/.....");
        }

        public void Save(DialogueContainerSO DialogueContainerSO)
        {

            Debug.Log($"Validating Dialogue Object ... ");

            if (nodes.Find(x => x is StartNode) != null)
            {

                Debug.Log($"<color=green>Has Start Node: </color>");
            }
            else
                Debug.Log($"<color=red>Has No Start Node: </color>");

            if (nodes.Find(x => x is EndNode) != null)
            {

                Debug.Log($"<color=green>Has End Node: </color>");
            }
            else
                Debug.Log($"<color=red>Has No End Node: </color>");

            SaveEdges(DialogueContainerSO);
            SaveNodes(DialogueContainerSO);

            EditorUtility.SetDirty(DialogueContainerSO);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>Success: </color>Saved succesfully!");
        }

        public void Load(DialogueContainerSO DialogueContainerSO)
        {
            ClearGraph();
            GenerateNodes(DialogueContainerSO);
            ConnectNodes(DialogueContainerSO);
        }

        #region Save
        private void SaveEdges(DialogueContainerSO DialogueContainerSO)
        {
            DialogueContainerSO.NodeLinkDatas.Clear();

            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                DialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.NodeGuid,
                    BasePortName = connectedEdges[i].output.portName,
                    TargetNodeGuid = inputNode.NodeGuid,
                    TargetPortName = connectedEdges[i].input.portName,
                });
            }
        }

        private void SaveNodes(DialogueContainerSO DialogueContainerSO)
        {
            DialogueContainerSO.EventDatas.Clear();
            DialogueContainerSO.EndDatas.Clear();
            DialogueContainerSO.StartDatas.Clear();
            DialogueContainerSO.BranchDatas.Clear();
            DialogueContainerSO.DialogueDatas.Clear();
            DialogueContainerSO.ChoiceDatas.Clear();
            DialogueContainerSO.ChoiceConnectorDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueNode dialogueNode:
                        DialogueContainerSO.DialogueDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case StartNode startNode:
                        DialogueContainerSO.StartDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        DialogueContainerSO.EndDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        DialogueContainerSO.EventDatas.Add(SaveNodeData(eventNode));
                        break;
                    case BranchNode branchNode:
                        DialogueContainerSO.BranchDatas.Add(SaveNodeData(branchNode));
                        break;
                    case ChoiceNode choiceNode:
                        DialogueContainerSO.ChoiceDatas.Add(SaveNodeData(choiceNode));
                        break;
                    case ChoiceConnectorNode choiceConnectorNode:
                        DialogueContainerSO.ChoiceConnectorDatas.Add(SaveNodeData(choiceConnectorNode));
                        break;
                    default:
                        break;
                }
            });
        }

        private DialogueData SaveNodeData(DialogueNode node)
        {
            ValidateDialogueNode(node);

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

            dialogueData.DialogueData_Character.Value = node.DialogueData.DialogueData_Character.Value;

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
                tempNode.characterField.value = node.DialogueData_Character.Value;
                tempNode.DialogueData.DialogueData_Character.Value = node.DialogueData_Character.Value;
                data_BaseContainer.AddRange(node.DialogueData_Texts);

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
                    tempNode.AddChoicePort(tempNode, choiceNode, true, port);
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

        #region validate
        private void ValidateDialogueNode(DialogueNode node)
        {
            bool warn = false;
            foreach (DialogueData_Text baseContainer in node.DialogueData.Dialogue_BaseContainers)
            {
                foreach (DialogueData_Sentence sentence in baseContainer.sentence)
                {
                    sentence.TextField.parent.parent.RemoveFromClassList("Error");
                    sentence.TextField.parent.RemoveFromClassList("Error");
                    sentence.TextField.RemoveFromClassList("Error");


                    if ((sentence.TextField.value == null || sentence.TextField.value == "" || sentence.TextField.value == "Dialogue Text"))
                    {
                        sentence.TextField.parent.parent.AddToClassList("Error");
                        sentence.TextField.parent.AddToClassList("Error");
                        sentence.TextField.AddToClassList("Error");
                        warn = true;
                    }
                }


            }

            if(warn)
                Debug.Log($"<color=orange>Warning: </color>You have empty dialogue boxes! Fill these in for complete dialogue!");


        }
        #endregion
    }
}