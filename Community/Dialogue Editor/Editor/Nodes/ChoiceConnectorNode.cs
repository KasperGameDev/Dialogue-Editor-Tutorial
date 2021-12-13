
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class ChoiceConnectorNode : BaseNode
    {
        private ChoiceConnectorData choiceConnectorData = new ChoiceConnectorData();
        public ChoiceConnectorData ChoiceConnectorData { get => choiceConnectorData; set => choiceConnectorData = value; }

        protected Vector2 choiceConnectorNodeSize = new Vector2(10, 10);

        private ChoiceNode lastChoiceNode = null;

        public ChoiceConnectorNode() { }

        public ChoiceConnectorNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceConnectorNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Connector";
            SetPosition(new Rect(position, choiceConnectorNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            // Add standard ports.
            AddInputPort("", Color.cyan, Port.Capacity.Multi);

            inputContainer.ClearClassList();
            inputContainer.AddToClassList("inputContainer");

            TopContainer();
        }

        private void TopContainer()
        {
            AddPortButton();
        }

        private void AddPortButton()
        {
            Button btn = new Button()
            {
                text = " + ",
            };
            btn.AddToClassList("TopBtn");

            btn.clicked += () =>
            {
                ChoiceNode choiceNode = graphView.CreateChoiceNode(DetermineNextChoiceNodePosition());
                lastChoiceNode = choiceNode;
                graphView.AddElement(choiceNode);
                AddChoicePort(this, choiceNode, false);
            };

            titleButtonContainer.Add(btn);
        }

        // Port ---------------------------------------------------------------------------------------

        public Port AddChoicePort(BaseNode baseNode, ChoiceNode choiceNode, bool load, DialogueData_Port choiceConnectorData_Port = null)
        {
            Port port = GetPortInstance(Direction.Output);
            DialogueData_Port newDialogue_Port = new DialogueData_Port();

            // Check if we load it in with values
            if (choiceConnectorData_Port != null)
            {
                newDialogue_Port.InputGuid = choiceConnectorData_Port.InputGuid;
                newDialogue_Port.OutputGuid = choiceConnectorData_Port.OutputGuid;
                newDialogue_Port.PortGuid = choiceConnectorData_Port.PortGuid;
            }
            else
            {
                newDialogue_Port.PortGuid = Guid.NewGuid().ToString();
            }

            if (choiceNode != null)
            {
                Edge tempEdge = new Edge()
                {
                    output = port,
                    input = (Port)choiceNode.inputContainer[0]
                };
                if (!load)
                {
                    tempEdge.input.Connect(tempEdge);
                    tempEdge.output.Connect(tempEdge);
                    graphView.Add(tempEdge);
                }
            }
            // Delete button
            {
                Button deleteButton = new Button(() => DeletePort(baseNode, port, choiceNode==null?null:choiceNode.NodeGuid))
                {
                    text = " - ",
                };

                deleteButton.AddToClassList("MoveBtn");
                port.contentContainer.Add(deleteButton);
            }

            port.portName = newDialogue_Port.PortGuid;                      // We use portName as port ID
            Label portNameLabel = port.contentContainer.Q<Label>("type");   // Get Labal in port that is used to contain the port name.
            portNameLabel.AddToClassList("PortName");                       // Here we add a uss class to it so we can hide it in the editor window.

            // Set color of the port.
            port.portColor = Color.yellow;

            ChoiceConnectorData.DialogueData_Ports.Add(newDialogue_Port);
            baseNode.outputContainer.Add(port);

            // Refresh
            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();

            return port;
        }

        private void DeletePort(BaseNode node, Port port, string nodeGuid = null)
        {
            DialogueData_Port tmp = ChoiceConnectorData.DialogueData_Ports.Find(findPort => findPort.PortGuid == port.portName);
            ChoiceConnectorData.DialogueData_Ports.Remove(tmp);

            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                if(edge.input != null)
                    edge.input.Disconnect(edge);

                if (edge.output != null)
                    edge.output.Disconnect(edge);

                graphView.RemoveElement(edge);

                if (nodeGuid != null)
                {
                    BaseNode choiceNode = graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList().Where(node => node.NodeGuid == nodeGuid).First();
                    if (choiceNode != null)
                        graphView.RemoveElement(choiceNode);
                }
            }

            node.outputContainer.Remove(port);


            // Refresh
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        // Menu dropdown --------------------------------------------------------------------------------------

        public Vector2 DetermineNextChoiceNodePosition()
        {
            Vector2 returnPosition = GetPosition().position + new Vector2(GetPosition().width + 5, GetPosition().height + 25);
            if (lastChoiceNode != null)
            {
                returnPosition = lastChoiceNode.GetPosition().position + new Vector2(0, lastChoiceNode.GetPosition().height + 25);
            }

            return returnPosition;
        }
    }
}