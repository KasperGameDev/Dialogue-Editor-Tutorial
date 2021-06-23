using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue.Example
{
    public class GameEvents : MonoBehaviour
    {
        private event Action<int> randomColorModel;
        protected UseStringEventCondition useStringEventCondition = new UseStringEventCondition();
        protected UseStringEventModifier useStringEventModifier = new UseStringEventModifier();

        public static GameEvents Instance { get; private set; }
        public Action<int> RandomColorModel { get => randomColorModel; set => randomColorModel = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CallRandomColorModel(int number)
        {
            randomColorModel?.Invoke(number);
        }

        public virtual void DialogueModifierEvents(string stringEvent, StringEventModifierType stringEventModifierType, float value = 0)
        {

        }

        public virtual bool DialogueConditionEvents(string stringEvent, StringEventConditionType stringEventConditionType, float value = 0)
        {
            return false;
        }
    }
}