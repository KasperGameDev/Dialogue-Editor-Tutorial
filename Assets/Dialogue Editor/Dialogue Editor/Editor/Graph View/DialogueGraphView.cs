using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class DialogueGraphView : GraphView
    {
        private string graphViewStyleSheet = "USS/GraphView/GraphViewStyleSheet";   // Name of the graph view style sheet.
        private DialogueEditorWindow editorWindow;
        private NodeSearchWindow searchWindow;

        public DialogueGraphView(DialogueEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;

            // Adding the ability to zoom in and out graph view.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Adding style sheet to graph view.
            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
            styleSheets.Add(tmpStyleSheet);

            this.AddManipulator(new ContentDragger());      // The ability to drag nodes around.
            this.AddManipulator(new SelectionDragger());    // The ability to drag all selected nodes around.
            this.AddManipulator(new RectangleSelector());   // The ability to drag select a rectangle area.
            this.AddManipulator(new FreehandSelector());    // The ability to select a single node.

            // Add a visible grid to the background.
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();
        }

        /// <summary>
        /// Add a search window to graph view.
        /// </summary>
        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        // This is a graph view method that we override.
        // This is where we tell the graph view which nodes can connect to each other.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();  // All the ports that can be connected to.
            Port startPortView = startPort;                 // Start port.

            ports.ForEach((port) =>
            {
                Port portView = port;

            // First we tell that it cannot connect to itself.
            // Then we tell it it cannot connect to a port on the same node.
            // Lastly we tell it a input note cannot connect to another input node and an output node cannot connect to output node.
            if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction && startPortView.portColor == portView.portColor)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts; // return all the acceptable ports.
        }

        /// <summary>
        /// Reload the current selected language.
        /// Normally used when changing language.
        /// </summary>
        public void ReloadLanguage()
        {
            List<BaseNode> allNodes = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in allNodes)
            {
                node.ReloadLanguage();
            }
        }

        // Make Node's -----------------------------------------------------------------------------------

        /// <summary>
        /// Make new Start Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>Start Node</returns>
        public StartNode CreateStartNode(Vector2 position)
        {
            return new StartNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new End Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>End Node</returns>
        public EndNode CreateEndNode(Vector2 position)
        {
            return new EndNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Event Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>Event Node</returns>
        public EventNode CreateEventNode(Vector2 position)
        {
            return new EventNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Dialogue Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>Dialogue Node</returns>
        public DialogueNode CreateDialogueNode(Vector2 position, bool isLoading)
        {
            return new DialogueNode(position, editorWindow, this, isLoading);
        }

        public ChoiceConnectorNode CreateChoiceConnectorNode(Vector2 position)
        {
            return new ChoiceConnectorNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Branch Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>Branch Node</returns>
        public BranchNode CreateBranchNode(Vector2 position)
        {
            return new BranchNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Choice Node and set it's position.
        /// </summary>
        /// <param name="position">position of where to place the node</param>
        /// <returns>Choice Node</returns>
        public ChoiceNode CreateChoiceNode(Vector2 position)
        {
            return new ChoiceNode(position, editorWindow, this);
        }
    }
}