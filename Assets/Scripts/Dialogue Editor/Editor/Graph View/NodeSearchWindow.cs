using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueEditorWindow editorWindow;
    private DialogueGraphView graphView;

    private Texture2D pic;

    public void Configure(DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        graphView = _graphView;

        pic = new Texture2D(1,1);
        pic.SetPixel(0, 0, new Color(0, 0, 0, 0));
        pic.Apply();
    }


    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"),1),

            AddNodeSearch("Start Node",new StartNode()),
            AddNodeSearch("Dialogue Node",new DialogueNode()),
            AddNodeSearch("Event Node",new EventNode()),
            AddNodeSearch("End Node",new EndNode()),
        };

        return tree;
    }

    private SearchTreeEntry AddNodeSearch(string _name, BaseNode _baseNode)
    {
        SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(_name, pic))
        {
            level = 2,
            userData = _baseNode
        };

        return tmp;
    }

    public bool OnSelectEntry(SearchTreeEntry _SearchTreeEntry, SearchWindowContext _context)
    {
        Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo
            (
            editorWindow.rootVisualElement.parent,_context.screenMousePosition - editorWindow.position.position
            );

        Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

        return CheckForNodeType(_SearchTreeEntry, graphMousePosition) ;
    }

    private bool CheckForNodeType(SearchTreeEntry _searchTreeEntry,Vector2 _pos)
    {
        switch (_searchTreeEntry.userData)
        {
            case StartNode node:
                graphView.AddElement(graphView.CreateStartNode(_pos));
                return true;
            case DialogueNode node:
                graphView.AddElement(graphView.CreateDialogueNode(_pos));
                return true;
            case EventNode node:
                graphView.AddElement(graphView.CreateEventNode(_pos));
                return true;
            case EndNode node:
                graphView.AddElement(graphView.CreateEndNode(_pos));
                return true;
            default:
                break;
        }
        return false;
    }
}
