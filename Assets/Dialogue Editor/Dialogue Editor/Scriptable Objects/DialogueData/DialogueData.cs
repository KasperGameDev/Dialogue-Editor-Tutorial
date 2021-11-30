using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueData : BaseData
    {
        public List<DialogueData_BaseContainer> Dialogue_BaseContainers { get; set; } = new List<DialogueData_BaseContainer>();
        public List<DialogueData_Text> DialogueData_Texts = new List<DialogueData_Text>();
        public Container_DialogueCharacter DialogueData_Character = new Container_DialogueCharacter();
        // public List<DialogueData_Port> DialogueData_Ports = new List<DialogueData_Port>();
    }

    [System.Serializable]
    public class DialogueData_BaseContainer
    {
        public Container_Int ID = new Container_Int();
    }

    //[System.Serializable]
    //public class DialogueData_Name : DialogueData_BaseContainer
    //{
    //    public Container_String CharacterName = new Container_String();
    //}

    [System.Serializable]
    public class DialogueData_Sentence : DialogueData_BaseContainer
    {
#if UNITY_EDITOR
        public TextField TextField { get; set; }
        public Container_VolumeType volumeType = new Container_VolumeType();
#endif
        public Container_String GuidID = new Container_String();
        public List<LanguageGeneric<string>> Text = new List<LanguageGeneric<string>>();
    }

    [System.Serializable]
    public class DialogueData_Text : DialogueData_BaseContainer
    {
#if UNITY_EDITOR
        public List<DialogueData_Sentence> sentence = new List<DialogueData_Sentence>();
        public ObjectField ObjectField { get; set; }
#endif
        public Container_String GuidID = new Container_String();
        public List<LanguageGeneric<AudioClip>> AudioClips = new List<LanguageGeneric<AudioClip>>();

        public Container_Sprite Sprite_Left = new Container_Sprite();
        public Container_Sprite Sprite_Right = new Container_Sprite();
    }

    [System.Serializable]
    public class DialogueData_Port
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
    }
}
