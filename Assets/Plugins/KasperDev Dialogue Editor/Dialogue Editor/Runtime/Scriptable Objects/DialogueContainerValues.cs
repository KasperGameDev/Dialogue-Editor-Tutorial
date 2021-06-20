using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue
{
    public class DialogueContainerValues { }

    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEventSO DialogueEventSO;
    }

    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LanguageType LanguageType;
        public T LanguageGenericType;
    }
}

