using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueEditorWindow editorWindow;
        private DialogueGraphView graphView;

        private Texture2D iconImage;

        public void Configure(DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            this.editorWindow = editorWindow;
            this.graphView = graphView;

            // Icon image that we kinda don't use.
            // However use it to create space left of the text.
            // TODO: find a better way.
            iconImage = new Texture2D(1, 1);
            iconImage.SetPixel(0, 0, new Color(0, 0, 0, 0));
            iconImage.Apply();
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Dialogue Editor"),0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),1),

            AddNodeSearch("Start",new StartNode()),
            AddNodeSearch("Dialogue",new DialogueNode()),
            AddNodeSearch("Branch",new BranchNode()),
            AddNodeSearch("Event",new EventNode()),
            AddNodeSearch("End",new EndNode()),
        };

            return tree;
        }

        private SearchTreeEntry AddNodeSearch(string name, BaseNode baseNode)
        {
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name, iconImage))
            {
                level = 2,
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
                default:
                    break;
            }
            return false;
        }
    }
}