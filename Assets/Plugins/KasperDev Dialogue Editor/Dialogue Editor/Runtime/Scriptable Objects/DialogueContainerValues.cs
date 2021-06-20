using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace KasperDev.Dialogue
{
    public class DialogueContainerValues { }

    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LanguageType LanguageType;
        public T LanguageGenericType;
    }

    [System.Serializable]
    public class Container_DialogueEventSO
    {
        public DialogueEventSO DialogueEventSO;
    }

    // Values --------------------------------------

    [System.Serializable]
    public class Container_String
    {
        public string Value;
    }

    [System.Serializable]
    public class Container_Int
    {
        public int Value;
    }

    [System.Serializable]
    public class Container_Float
    {
        public float Value;
    }

    [System.Serializable]
    public class Container_Sprite
    {
        public Sprite Value;
    }

    // Enums --------------------------------------

    [System.Serializable]
    public class Container_ChoiceStateType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public ChoiceStateType Value = ChoiceStateType.Hide;
    }

    [System.Serializable]
    public class Container_EndNodeType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public EndNodeType Value = EndNodeType.End;
    }

    [System.Serializable]
    public class Container_StringEventModifierType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public StringEventModifierType Value = StringEventModifierType.SetTrue;
    }

    [System.Serializable]
    public class Container_StringEventConditionType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public StringEventConditionType Value = StringEventConditionType.True;
    }


    // Event --------------------------------------

    [System.Serializable]
    public class EventData_StringModifier
    {
        public Container_String StringEventText = new Container_String();
        public Container_Float Number = new Container_Float();

        public Container_StringEventModifierType StringEventModifierType = new Container_StringEventModifierType();
    }

    [System.Serializable]
    public class EventData_StringCondition
    {
        public Container_String StringEventText = new Container_String();
        public Container_Float Number = new Container_Float();

        public Container_StringEventConditionType StringEventConditionType = new Container_StringEventConditionType();
    }
}

