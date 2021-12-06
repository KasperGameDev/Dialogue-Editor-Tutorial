using UnityEngine;
using DialogueEditor.Events;
using DialogueEditor.ModularComponents;

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace DialogueEditor.Dialogue
{
    public class DialogueContainerValues { }

    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LanguageType LanguageType;
        public T LanguageGenericType;
    }

    // Values --------------------------------------

    [System.Serializable]
    public class Container_String
    {
        public string Value;
    }

    [System.Serializable]
    public class Container_Bool
    {
        public bool Value;
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

    #region String Event
    [System.Serializable]
    public class Container_StringEventModifierType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public StringDialogueEventModifierType Value;
    }

    [System.Serializable]
    public class Container_StringEventConditionType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public StringDialogueEventConditionType Value;
    }
    #endregion

    #region Float Event
    [System.Serializable]
    public class Container_FloatEventModifierType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public FloatDialogueEventModifierType Value;
    }

    [System.Serializable]
    public class Container_FloatEventConditionType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public FloatDialogueEventConditionType Value;
    }
    #endregion

    #region Int Event
    [System.Serializable]
    public class Container_IntEventModifierType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public IntDialogueEventModifierType Value;
    }

    [System.Serializable]
    public class Container_IntEventConditionType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public IntDialogueEventConditionType Value;
    }
    #endregion

    #region Bool Event
    [System.Serializable]
    public class Container_BoolEventModifierType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public BoolDialogueEventModifierType Value;
    }

    [System.Serializable]
    public class Container_BoolEventConditionType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public BoolDialogueEventConditionType Value;
    }
    #endregion

    [System.Serializable]
    public class Container_FloatType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public StringDialogueEventConditionType Value;
    }

    [System.Serializable]
    public class Container_VolumeType
    {
#if UNITY_EDITOR
        public EnumField EnumField;
#endif
        public VolumeType Value = VolumeType.Neutral;
    }

    // Event --------------------------------------

    [System.Serializable]
    public class Container_DialogueEventSO
    {
        public GameEventSO GameEvent;
    }


    #region String
    [System.Serializable]
    public class EventData_StringModifier
    {
        public StringVariableSO VariableSO;
        public Container_String Value = new Container_String();

        public Container_StringEventModifierType EventType = new Container_StringEventModifierType();
    }


    [System.Serializable]
    public class EventData_StringCondition
    {
        public StringVariableSO VariableSO;
        public Container_String Value = new Container_String();

        public Container_StringEventConditionType EventType = new Container_StringEventConditionType();
    }
    #endregion

    #region Float
    [System.Serializable]
    public class EventData_FloatModifier
    {
        public FloatVariableSO VariableSO;
        public Container_Float Value = new Container_Float();

        public Container_FloatEventModifierType EventType = new Container_FloatEventModifierType();
    }


    [System.Serializable]
    public class EventData_FloatCondition
    {
        public FloatVariableSO VariableSO;
        public Container_Float Value = new Container_Float();

        public Container_FloatEventConditionType EventType = new Container_FloatEventConditionType();
    }
    #endregion

    #region Int
    [System.Serializable]
    public class EventData_IntModifier
    {
        public IntVariableSO VariableSO;
        public Container_Int Value = new Container_Int();

        public Container_IntEventModifierType EventType = new Container_IntEventModifierType();
    }


    [System.Serializable]
    public class EventData_IntCondition
    {
        public IntVariableSO VariableSO;
        public Container_Int Value = new Container_Int();

        public Container_IntEventConditionType EventType = new Container_IntEventConditionType();
    }
    #endregion

    #region Bool
    [System.Serializable]
    public class EventData_BoolModifier
    {
        public BoolVariableSO VariableSO;
        public Container_Bool Value = new Container_Bool();

        public Container_BoolEventModifierType EventType = new Container_BoolEventModifierType();
    }


    [System.Serializable]
    public class EventData_BoolCondition
    {
        public BoolVariableSO VariableSO;
        public Container_Bool Value = new Container_Bool();

        public Container_BoolEventConditionType EventType = new Container_BoolEventConditionType();
    }
    #endregion
}

