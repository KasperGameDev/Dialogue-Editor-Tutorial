using UnityEngine;
using DialogueEditor.Events;
using DialogueEditor.ModularComponents;

#if UNITY_EDITOR

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
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public ChoiceStateType Value = ChoiceStateType.Hide;
    }

    [System.Serializable]
    public class Container_EndNodeType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public EndNodeType Value = EndNodeType.End;
    }


    #region String Event
    [System.Serializable]
    public class Container_StringModifierType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public StringDialogueModifierType Value;
    }

    [System.Serializable]
    public class Container_StringEventConditionType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public StringDialogueEventConditionType Value;
    }
    #endregion

    #region Float Event
    [System.Serializable]
    public class Container_FloatModifierType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public FloatDialogueModifierType Value;
    }

    [System.Serializable]
    public class Container_FloatEventConditionType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public FloatDialogueEventConditionType Value;
    }
    #endregion

    #region Int Event
    [System.Serializable]
    public class Container_IntModifierType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public IntDialogueModifierType Value;
    }

    [System.Serializable]
    public class Container_IntEventConditionType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public IntDialogueEventConditionType Value;
    }
    #endregion

    #region Bool Event
    [System.Serializable]
    public class Container_BoolModifierType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public BoolDialogueModifierType Value;
    }

    [System.Serializable]
    public class Container_BoolEventConditionType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public BoolDialogueEventConditionType Value;
    }
    #endregion

    [System.Serializable]
    public class Container_FloatType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public StringDialogueEventConditionType Value;
    }

    [System.Serializable]
    public class Container_VolumeType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
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
    public class ModifierData_String
    {
        public StringVariableSO VariableSO;
        public Container_String Value = new Container_String();

        public Container_StringModifierType EventType = new Container_StringModifierType();
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
    public class ModifierData_Float
    {
        public FloatVariableSO VariableSO;
        public Container_Float Value = new Container_Float();

        public Container_FloatModifierType EventType = new Container_FloatModifierType();
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
    public class ModifierData_Int
    {
        public IntVariableSO VariableSO;
        public Container_Int Value = new Container_Int();

        public Container_IntModifierType EventType = new Container_IntModifierType();
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
    public class ModifierData_Bool
    {
        public BoolVariableSO VariableSO;
        public Container_Bool Value = new Container_Bool();

        public Container_BoolModifierType EventType = new Container_BoolModifierType();
    }


    [System.Serializable]
    public class EventData_BoolCondition
    {
        public BoolVariableSO VariableSO;
        public Container_Bool Value = new Container_Bool();

        public Container_BoolEventConditionType EventType = new Container_BoolEventConditionType();
    }
    #endregion

    #region Actor
    [System.Serializable]
    public class Container_Actor
    {
        public Actor actor;
    }

    [System.Serializable]
    public class Container_SelectActor
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.PopupField<Actor> PopupField;
#endif
        public Container_Actor Actor;
    }

    #endregion
}

