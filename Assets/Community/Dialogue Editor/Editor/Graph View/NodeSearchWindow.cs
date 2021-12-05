using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueEditorWindow editorWindow;
        private DialogueGraphView graphView;


        public void Configure(DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            this.editorWindow = editorWindow;
            this.graphView = graphView;
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Dialogue Editor"),0),
            //new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),1),

            AddNodeSearch("Dialogue",new DialogueNode()),
            AddNodeSearch("Choice Connector",new ChoiceConnectorNode()),
            AddNodeSearch("Branch",new BranchNode()),
            AddNodeSearch("Event",new EventNode()),
            AddNodeSearch("Start",new StartNode()),
            AddNodeSearch("End",new EndNode()),
        };

            return tree;
        }

        private SearchTreeEntry AddNodeSearch(string name, BaseNode baseNode)
        {
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name))
            {
                level = 1,
                userData = baseNode
            };

            return tmp;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            // Get mouse position on the screen.
            Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);

            // Now we use mouse position to calculator where it is in the graph view.
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            return CheckForNodeType(searchTreeEntry, graphMousePosition);
        }

        private bool CheckForNodeType(SearchTreeEntry searchTreeEntry, Vector2 position)
        {
            switch (searchTreeEntry.userData)
            {
                case StartNode node:
                    graphView.AddElement(graphView.CreateStartNode(position));
                    return true;
                case DialogueNode node:
                    graphView.AddElement(graphView.CreateDialogueNode(position));
                    return true;
                case EventNode node:
                    graphView.AddElement(graphView.CreateEventNode(position));
                    return true;
                case EndNode node:
                    graphView.AddElement(graphView.CreateEndNode(position));
                    return true;
                case BranchNode node:
                    graphView.AddElement(graphView.CreateBranchNode(position));
                    return true;
                case ChoiceConnectorNode node:
                    graphView.AddElement(graphView.CreateChoiceConnectorNode(position));
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}