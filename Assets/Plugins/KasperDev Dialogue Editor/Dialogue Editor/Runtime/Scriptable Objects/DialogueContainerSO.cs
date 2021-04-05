using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();

        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
        public List<StartNodeData> StartNodeDatas = new List<StartNodeData>();
        public List<EventNodeData> EventNodeDatas = new List<EventNodeData>();
        public List<BranchNodeData> BranchNodeDatas = new List<BranchNodeData>();

        public List<BaseNodeData> AllNodes {
            get {
                List<BaseNodeData> tmp = new List<BaseNodeData>();
                tmp.AddRange(DialogueNodeDatas);
                tmp.AddRange(EndNodeDatas);
                tmp.AddRange(StartNodeDatas);
                tmp.AddRange(EventNodeDatas);
                tmp.AddRange(BranchNodeDatas);

                return tmp;
            }
        }
    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }

    [System.Serializable]
    public class BaseNodeData
    {
        public string NodeGuid;
        public Vector2 Position;
    }

    [System.Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public Sprite FaceImage;
        public DialogueFaceImageType DialogueFaceImageType;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public string CharacterName;
        public List<LanguageGeneric<string>> TextLanguages;
    }

    [System.Serializable]
    public class EndNodeData : BaseNodeData
    {
        public EndNodeType EndNodeType;
    }

    [System.Serializable]
    public class StartNodeData : BaseNodeData
    {

    }

    #region Branch Node
    [System.Serializable]
    public class BranchNodeData : BaseNodeData
    {
        public string trueGuidNode;
        public string falseGuidNode;
        public List<BrancStringIdData> BrancStringIdDatas;
    }
    [System.Serializable]
    public class BrancStringIdData
    {
        public string stringEvent;
        public int idNumber;
    }
    #endregion

    #region Event Node
    [System.Serializable]
    public class EventNodeData : BaseNodeData
    {
        public List<EventScriptableObjectData> EventScriptableObjectDatas;
        public List<EventStringIdData> EventStringIdDatas;
    }
    [System.Serializable]
    public class EventStringIdData
    {
        public string stringEvent;
        public int idNumber;
    }
    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEventSO DialogueEventSO;
    }
    #endregion

    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LanguageType LanguageType;
        public T LanguageGenericType;
    }

    [System.Serializable]
    public class DialogueNodePort
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
        public TextField TextField;
        public List<LanguageGeneric<string>> TextLanguages = new List<LanguageGeneric<string>>();
    }
}