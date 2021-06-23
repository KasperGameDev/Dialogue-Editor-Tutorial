using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class GameEvents_Example01 : GameEvents
    {
        public override bool DialogueConditionEvents(string stringEvent, StringEventConditionType stringEventConditionType, float value = 0)
        {
            switch (stringEvent.ToLower())
            {
                case "money":
                    return useStringEventCondition.ConditionFloatCheck(FindObjectOfType<Player>().Money, value, stringEventConditionType);

                case "health":
                    return useStringEventCondition.ConditionFloatCheck(FindObjectOfType<Player>().Health, value, stringEventConditionType);

                case "didaskforhelp":
                    return useStringEventCondition.ConditionBoolCheck(FindObjectOfType<Player>().DidWeTalk, stringEventConditionType);

                default:
                    Debug.LogWarning("No stringEvent was fount");
                    return false;
            }
        }

        public override void DialogueModifierEvents(string stringEvent, StringEventModifierType stringEventModifierType, float value = 0)
        {
            switch (stringEvent.ToLower())
            {
                case "money":
                    FindObjectOfType<Player>().ModifyMoeny((int)useStringEventModifier.ModifierFloatCheck(value, stringEventModifierType));
                    break;
                case "health":
                    FindObjectOfType<Player>().ModifyHealth((int)useStringEventModifier.ModifierFloatCheck(value, stringEventModifierType));
                    break;
                case "didaskforhelp":
                    FindObjectOfType<Player>().DidWeTalk = (stringEventModifierType == StringEventModifierType.SetTrue ? true : false);
                    FindObjectOfType<Player>().DidWeTalk = useStringEventModifier.ModifierBoolCheck(stringEventModifierType);
                    break;
                default:
                    break;
            }
        }
    }
}
