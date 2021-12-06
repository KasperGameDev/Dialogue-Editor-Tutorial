using System;
using UnityEngine;

namespace DialogueEditor.Dialogue.Scripts
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

        public virtual void DialogueModifierEvents(string stringEvent, StringEventModifierType stringEventModifierType, NPC character, float value = 0)
        {
            switch (stringEvent.ToLower())
            {
                case "spoketocharacter":
                    character.spokeToPlayer = stringEventModifierType == StringEventModifierType.SetTrue ? true : false;
                    character.spokeToPlayer = useStringEventModifier.ModifierBoolCheck(stringEventModifierType);
                    break;
                default:
                    break;
            }
        }

        public virtual bool DialogueConditionEvents(string stringEvent, StringEventConditionType stringEventConditionType, NPC character, float value = 0)
        {
            switch (stringEvent.ToLower())
            {
                case "spoketocharacter":
                    return useStringEventCondition.ConditionBoolCheck(character.spokeToPlayer, stringEventConditionType);
                default:
                    return false;
            }

        }
    }
}