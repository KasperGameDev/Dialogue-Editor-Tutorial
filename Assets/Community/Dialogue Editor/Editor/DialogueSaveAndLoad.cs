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

        public void SaveAs(DialogueContainerSO DialogueContainerSO)
        {
            if (!ValidateDialogueObject())
                return;
            SaveEdges(DialogueContainerSO);
            SaveNodes(DialogueContainerSO);


            string path = EditorUtility.SaveFilePanelInProject(
            "Save new Converation",
            "DialogueObject",
            "asset",
            "Create New Dialogue Object");

            if (DialogueContainerSO != null) {
                EditorUtility.SetDirty(DialogueContainerSO);

                if (path.Length != 0)
                {
                    AssetDatabase.CreateAsset(DialogueContainerSO, path);

                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("Success", "You're Dialogue is Saved!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Canceled", "You're Save was Canceled", "OK");
                }
            }
            else
                EditorUtility.DisplayDialog("Canceled", "You're Dialogue Object is missing. Might you have Deleted it Accidentally? Restart the Dialogue tool, or restore you're scriptable Object", "OK");

        }

        public void Save(DialogueContainerSO DialogueContainerSO)
        {
            if (!ValidateDialogueObject())
                return;
            EditorUtility.ClearProgressBar();

            SaveEdges(DialogueContainerSO);
            SaveNodes(DialogueContainerSO);

            EditorUtility.SetDirty(DialogueContainerSO);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", "You're Dialogue is Saved!", "OK");
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
            DialogueContainerSO.ModifierDatas.Clear();
            DialogueContainerSO.BranchDatas.Clear();
            DialogueContainerSO.DialogueDatas.Clear();
            DialogueContainerSO.ChoiceDatas.Clear();
            DialogueContainerSO.ChoiceConnectorDatas.Clear();
            DialogueContainerSO.RepeatDatas.Clear();
            DialogueContainerSO.RestartDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueNode dialogueNode:
                        DialogueContainerSO.DialogueDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case StartNode startNode:
                        DialogueContainerSO.StartData = SaveNodeData(startNode);
                        break;
                    case EndNode endNode:
                        DialogueContainerSO.EndData = SaveNodeData(endNode);
                        break;
                    case EventNode eventNode:
                        DialogueContainerSO.EventDatas.Add(SaveNodeData(eventNode));
                        break;
                    case BranchNode branchNode:
                        DialogueContainerSO.BranchDatas.Add(SaveNodeData(branchNode));
                        break;
                    case ModifierNode modifierNode:
                        DialogueContainerSO.ModifierDatas.Add(SaveNodeData(modifierNode));
                        break;
                    case ChoiceNode choiceNode:
                        DialogueContainerSO.ChoiceDatas.Add(SaveNodeData(choiceNode));
                        break;
                    case RepeatNode repeatNode:
                        DialogueContainerSO.RepeatDatas.Add(SaveNodeData(repeatNode));
                        break;
                    case RestartNode restartNode:
                        DialogueContainerSO.RestartDatas.Add(SaveNodeData(restartNode));
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

            dialogueData.DialogueData_Character.actor = node.DialogueData.DialogueData_Character.actor;

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
            nodeData.ParticipatingActors.AddRange(node.StartData.ParticipatingActors);

            return nodeData;
        }

        private EndData SaveNodeData(EndNode node)
        {
            EndData nodeData = new EndData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };
            //nodeData.EndNodeType.Value = node.EndData.EndNodeType.Value;

            return nodeData;
        }

        private RestartData SaveNodeData(RestartNode node)
        {
            RestartData nodeData = new RestartData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };
            //nodeData.EndNodeType.Value = node.EndData.EndNodeType.Value;

            return nodeData;
        }

        private RepeatData SaveNodeData(RepeatNode node)
        {
            RepeatData nodeData = new RepeatData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };
            //nodeData.EndNodeType.Value = node.EndData.EndNodeType.Value;

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

            return nodeData;
        }


        private ModifierData SaveNodeData(ModifierNode node)
        {
            ModifierData nodeData = new ModifierData()
            {
                NodeGuid = node.NodeGuid,
                Position = node.GetPosition().position,
            };

            // Save String Event
            foreach (ModifierData_String stringEvents in node.ModifierData.ModifierData_Strings)
            {
                ModifierData_String tmp = new ModifierData_String();
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.VariableSO = stringEvents.VariableSO;
                tmp.EventType.Value = stringEvents.EventType.Value;

                nodeData.ModifierData_Strings.Add(tmp);
            }

            foreach (ModifierData_Float FloatEvents in node.ModifierData.ModifierData_Floats)
            {
                ModifierData_Float tmp = new ModifierData_Float();
                tmp.Value.Value = FloatEvents.Value.Value;
                tmp.VariableSO = FloatEvents.VariableSO;
                tmp.EventType.Value = FloatEvents.EventType.Value;

                nodeData.ModifierData_Floats.Add(tmp);
            }

            foreach (ModifierData_Int IntEvents in node.ModifierData.ModifierData_Ints)
            {
                ModifierData_Int tmp = new ModifierData_Int();
                tmp.Value.Value = IntEvents.Value.Value;
                tmp.VariableSO = IntEvents.VariableSO;
                tmp.EventType.Value = IntEvents.EventType.Value;

                nodeData.ModifierData_Ints.Add(tmp);
            }

            foreach (ModifierData_Bool BoolEvents in node.ModifierData.ModifierData_Bools)
            {
                ModifierData_Bool tmp = new ModifierData_Bool();
                tmp.Value.Value = BoolEvents.Value.Value;
                tmp.VariableSO = BoolEvents.VariableSO;
                tmp.EventType.Value = BoolEvents.EventType.Value;

                nodeData.ModifierData_Bools.Add(tmp);
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

            graphView.startNode = graphView.CreateStartNode(Vector2.zero);
            graphView.endNode = graphView.CreateEndNode(Vector2.right * 500);

            // Start
            if (dialogueContainer.StartData != null)
            {
                if (dialogueContainer.StartData.NodeGuid.Length > 0)
                    graphView.startNode.NodeGuid = dialogueContainer.StartData.NodeGuid;
                graphView.startNode.SetPosition(new Rect(dialogueContainer.StartData.Position, new Vector2(200, 250)));
                foreach(Container_Actor actor in dialogueContainer.StartData.ParticipatingActors)
                {
                    graphView.startNode.AddScriptableActor(actor);
                }
            }

            // End
            if (dialogueContainer.EndData != null)
            {
                if(dialogueContainer.EndData.NodeGuid.Length > 0)
                    graphView.endNode.NodeGuid = dialogueContainer.EndData.NodeGuid;
                graphView.endNode.SetPosition(new Rect(dialogueContainer.EndData.Position, new Vector2(200, 1500)));
            }

            graphView.AddElement(graphView.startNode);
            graphView.AddElement(graphView.endNode);

            // Event Node
            foreach (EventData node in dialogueContainer.EventDatas)
            {
                EventNode tempNode = graphView.CreateEventNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (Container_DialogueEventSO item in node.Container_DialogueEventSOs)
                {
                    tempNode.AddScriptableEvent(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Event Node
            foreach (ModifierData node in dialogueContainer.ModifierDatas)
            {
                ModifierNode tempNode = graphView.CreateModifierNode(node.Position);
                tempNode.NodeGuid = node.NodeGuid;

                foreach (ModifierData_String item in node.ModifierData_Strings)
                {
                    tempNode.AddStringModifier(item);
                }
                foreach (ModifierData_Float item in node.ModifierData_Floats)
                {
                    tempNode.AddFloatModifier(item);
                }
                foreach (ModifierData_Int item in node.ModifierData_Ints)
                {
                    tempNode.AddIntModifier(item);
                }
                foreach (ModifierData_Bool item in node.ModifierData_Bools)
                {
                    tempNode.AddBoolModifier(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            // Branch Node
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
                //tempNode.characterField.value = node.DialogueData_Character.actor;
                //tempNode.DialogueData.DialogueData_Character.actor = node.DialogueData_Character.actor;
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
        private bool ValidateDialogueObject()
        {
            float validation = 0;
            string validationText = "Start Node";
            bool valid = false;

            EditorUtility.DisplayProgressBar("Validating Dialogue Object", validationText, validation);

            if (nodes.Find(x => x is StartNode) != null)
            {

                validation = 0.5f;
                validationText = "End Node";
                valid = true;
            }
            else
            {

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error!", "Start Node Required in this Dialogue", "OK");
            }

            if (nodes.Find(x => x is EndNode) != null)
            {

                validation = 1f;
                validationText = "Validation Completed";
                valid = true;
            }
            else
            {

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error!", "End Node Required in this Dialogue", "OK");
            }

            EditorUtility.ClearProgressBar();
            return valid;
        }
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