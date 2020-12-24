using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : BaseNode
{
    private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
    private List<LanguageGeneric<AudioClip>> audioClips = new List<LanguageGeneric<AudioClip>>();
    private Sprite faceImage;
    private string name = "";
    private DialogueFaceImageType faceImageType;

    private List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

    public List<LanguageGeneric<string>> Texts { get => texts; set => texts = value; }
    public List<LanguageGeneric<AudioClip>> AudioClips { get => audioClips; set => audioClips = value; }
    public Sprite FaceImage { get => faceImage; set => faceImage = value; }
    public string Name { get => name; set => name = value; }
    public DialogueFaceImageType FaceImageType { get => faceImageType; set => faceImageType = value; }
    public List<DialogueNodePort> DialogueNodePorts { get => dialogueNodePorts; set => dialogueNodePorts = value; }

    private TextField texts_Field;
    private ObjectField audioClips_Field;
    private ObjectField faceImage_Field;
    private TextField name_Feild;
    private EnumField faceImageType_Field;

    public DialogueNode()
    {

    }

    public DialogueNode(Vector2 _position,DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        graphView = _graphView;

        title = "Dialogue";
        SetPosition(new Rect(_position, defaultNodeSide));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);

        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            texts.Add(new LanguageGeneric<string>
            {
                LanguageType = language,
                LanguageGenericType = ""
            });

            audioClips.Add(new LanguageGeneric<AudioClip>
            {
                LanguageType = language,
                LanguageGenericType = null
            });
        }

        // Face Image
        faceImage_Field = new ObjectField
        {
            objectType = typeof(Sprite),
            allowSceneObjects = false,
            value = faceImage
        };
        faceImage_Field.RegisterValueChangedCallback(value => 
        {
            faceImage = value.newValue as Sprite;
        });
        mainContainer.Add(faceImage_Field);

        // Face Image Enum
        faceImageType_Field = new EnumField()
        {
            value = faceImageType
        };
        faceImageType_Field.Init(faceImageType);
        faceImageType_Field.RegisterValueChangedCallback(value => 
        {
            faceImageType = (DialogueFaceImageType)value.newValue;
        });
        mainContainer.Add(faceImage_Field);

        // Audio Chilp
        audioClips_Field = new ObjectField()
        {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
            value = audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType,
        };
        audioClips_Field.RegisterValueChangedCallback(value =>
        {
            audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        mainContainer.Add(audioClips_Field);

        // Text Name
        Label label_name = new Label("Name");
        label_name.AddToClassList("label_name");
        label_name.AddToClassList("Label");
        mainContainer.Add(label_name);

        name_Feild = new TextField("");
        name_Feild.RegisterValueChangedCallback(value =>
        {
            name = value.newValue;
        });
        name_Feild.SetValueWithoutNotify(name);
        name_Feild.AddToClassList("TextName");
        mainContainer.Add(name_Feild);

        // Text Box
        Label label_texts = new Label("Text Box");
        label_texts.AddToClassList("label_texts");
        label_texts.AddToClassList("Label");
        mainContainer.Add(label_texts);

        texts_Field = new TextField("");
        texts_Field.RegisterValueChangedCallback(value =>
        {
            texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        texts_Field.SetValueWithoutNotify(texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        texts_Field.multiline = true;

        texts_Field.AddToClassList("TextBox");
        mainContainer.Add(texts_Field);

        Button button = new Button()
        {
            text = "Add Choice"
        };
        button.clicked += () =>
        {
            AddChoicePort(this);
        };

        titleButtonContainer.Add(button);
    }

    public void ReloadLanguage()
    {
        texts_Field.RegisterValueChangedCallback(value =>
        {
            texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        texts_Field.SetValueWithoutNotify(texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        audioClips_Field.RegisterValueChangedCallback(value =>
        {
            audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        foreach (DialogueNodePort nodePort in dialogueNodePorts)
        {
            nodePort.TextField.RegisterValueChangedCallback(value =>
            {
                nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
            });
            nodePort.TextField.SetValueWithoutNotify(nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        }
    }

    public override void LoadValueInToField()
    {
        texts_Field.SetValueWithoutNotify(texts.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        faceImage_Field.SetValueWithoutNotify(faceImage);
        faceImageType_Field.SetValueWithoutNotify(faceImageType);
        name_Feild.SetValueWithoutNotify(name);
    }

    public Port AddChoicePort(BaseNode _baseNode, DialogueNodePort _dialogueNodePort = null)
    {
        Port port = GetPortInstance(Direction.Output);

        int outputPortCount = _baseNode.outputContainer.Query("connector").ToList().Count();
        string outputPortName = $"Choice {outputPortCount + 1}";

        DialogueNodePort dialogueNodePort = new DialogueNodePort();

        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            dialogueNodePort.TextLanguages.Add(new LanguageGeneric<string>()
            {
                LanguageType = language,
                LanguageGenericType = outputPortName
            });
        }

        if(_dialogueNodePort != null)
        {
            dialogueNodePort.InputGuid = _dialogueNodePort.InputGuid;
            dialogueNodePort.OutputGuid = _dialogueNodePort.OutputGuid;

            foreach (LanguageGeneric<string> languageGeneric in _dialogueNodePort.TextLanguages)
            {
                dialogueNodePort.TextLanguages.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
            }
        }

        // Text for the port
        dialogueNodePort.TextField = new TextField();
        dialogueNodePort.TextField.RegisterValueChangedCallback(value =>
        {
            dialogueNodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        dialogueNodePort.TextField.SetValueWithoutNotify(dialogueNodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        port.contentContainer.Add(dialogueNodePort.TextField);

        // Delete button
        Button deleteButton = new Button(() => DeletePort(_baseNode, port))
        {
            text = "X",
        };
        port.contentContainer.Add(deleteButton);


        dialogueNodePort.MyPort = port;
        port.portName = "";

        dialogueNodePorts.Add(dialogueNodePort);

        _baseNode.outputContainer.Add(port);

        // Refresh
        _baseNode.RefreshPorts();
        _baseNode.RefreshExpandedState();

        return port;
    }


    private void DeletePort(BaseNode _node, Port _port)
    {
        DialogueNodePort tmp = dialogueNodePorts.Find(port => port.MyPort == _port);
        dialogueNodePorts.Remove(tmp);

        IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == _port);

        if (portEdge.Any())
        {
            Edge edge = portEdge.First();
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            graphView.RemoveElement(edge);
        }

        _node.outputContainer.Remove(_port);

        // Refresh
        _node.RefreshPorts();
        _node.RefreshExpandedState();
    }
}
