using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class ChoiceData : BaseData
    {
#if UNITY_EDITOR
        public TextField TextField { get; set; }
        public ObjectField ObjectField { get; set; }
#endif
        public Container_ChoiceStateType ChoiceStateTypes = new Container_ChoiceStateType();
        public List<LanguageGeneric<string>> Text = new List<LanguageGeneric<string>>();
        public List<LanguageGeneric<AudioClip>> AudioClips = new List<LanguageGeneric<AudioClip>>();
        public List<EventData_StringCondition> EventData_StringConditions = new List<EventData_StringCondition>();
    }
}
