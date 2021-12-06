using UnityEngine;

namespace DialogueEditor.Dialogue.Scripts
{
    public class UseStringEventCondition
    {
        public bool ConditionFloatCheck(float currentValue, float checkValue, StringEventConditionType stringEventConditionType)
        {
            switch (stringEventConditionType)
            {
                case StringEventConditionType.Equals:
                    return ValueEquals(currentValue, checkValue);

                case StringEventConditionType.EqualsAndBigger:
                    return ValueEqualsAndBigger(currentValue, checkValue);

                case StringEventConditionType.EqualsAndSmaller:
                    return ValueEqualsAndSmaller(currentValue, checkValue);

                case StringEventConditionType.Bigger:
                    return ValueBigger(currentValue, checkValue);

                case StringEventConditionType.Smaller:
                    return ValueSmaller(currentValue, checkValue);

                default:
                    Debug.LogWarning("GameEvents dint find a event");
                    return false;
            }
        }

        public bool ConditionBoolCheck(bool currentValue, StringEventConditionType stringEventConditionType)
        {
            switch (stringEventConditionType)
            {
                case StringEventConditionType.True:
                    return ValueBool(currentValue, true);

                case StringEventConditionType.False:
                    return ValueBool(currentValue, false);

                default:
                    Debug.LogWarning("GameEvents dint find a event");
                    return false;
            }
        }

        private bool ValueBool(bool currentValue, bool checkValue)
        {
            return currentValue == checkValue;
        }

        private bool ValueEquals(float currentValue, float checkValue)
        {
            return currentValue == checkValue;
        }

        private bool ValueEqualsAndBigger(float currentValue, float checkValue)
        {
            return currentValue >= checkValue;
        }

        private bool ValueEqualsAndSmaller(float currentValue, float checkValue)
        {
            return currentValue <= checkValue;
        }

        private bool ValueBigger(float currentValue, float checkValue)
        {
            return currentValue > checkValue;
        }

        private bool ValueSmaller(float currentValue, float checkValue)
        {
            return currentValue < checkValue;
        }
    }
}
