namespace DialogueEditor.Dialogue.Scripts
{
    class DialogueMathCalculator
    {
    }

    public class DialogueMathCalculatorModifier
    {
        #region String
        public void StringModifier(EventData_StringModifier item)
        {
            switch (item.EventType.Value)
            {
                case StringDialogueEventModifierType.Add:
                    StringModifier_Add(item);
                    break;
                case StringDialogueEventModifierType.Subtract:
                    StringModifier_Subtract(item);
                    break;
                default:
                    break;
            }

        }

        private void StringModifier_Add(EventData_StringModifier item)
        {
            item.VariableSO.Value += item.Value.Value;
        }

        private void StringModifier_Subtract(EventData_StringModifier item)
        {
            item.VariableSO.Value.Replace(item.Value.Value, "");
        }
        #endregion

        #region Float
        public void FloatModifier(EventData_FloatModifier item)
        {
            switch (item.EventType.Value)
            {
                case FloatDialogueEventModifierType.Add:
                    FloatModifier_Add(item);
                    break;
                case FloatDialogueEventModifierType.Subtract:
                    FloatModifier_Subtract(item);
                    break;
                default:
                    break;
            }
        }

        private void FloatModifier_Add(EventData_FloatModifier item)
        {
            item.VariableSO.Value += item.Value.Value;

        }

        private void FloatModifier_Subtract(EventData_FloatModifier item)
        {
            item.VariableSO.Value -= item.Value.Value;
        }

        #endregion

        #region Int
        public void IntModifier(EventData_IntModifier item)
        {
            switch (item.EventType.Value)
            {
                case IntDialogueEventModifierType.Add:
                    IntModifier_Add(item);
                    break;
                case IntDialogueEventModifierType.Subtract:
                    IntModifier_Subtract(item);
                    break;
                default:
                    break;
            }
        }

        private void IntModifier_Add(EventData_IntModifier item)
        {
            item.VariableSO.Value += item.Value.Value;

        }

        private void IntModifier_Subtract(EventData_IntModifier item)
        {
            item.VariableSO.Value -= item.Value.Value;
        }


        #endregion

        #region Bool
        public void BoolModifier(EventData_BoolModifier item)
        {
            switch (item.EventType.Value)
            {
                case BoolDialogueEventModifierType.SetTrue:
                    BoolModifier_SetTrue(item);
                    break;
                case BoolDialogueEventModifierType.SetFalse:
                    BoolModifier_SetFalse(item);
                    break;
                default:
                    break;
            }
        }

        private void BoolModifier_SetTrue(EventData_BoolModifier item)
        {
            item.VariableSO.Value = true;

        }

        private void BoolModifier_SetFalse(EventData_BoolModifier item)
        {
            item.VariableSO.Value = false;
        }
        #endregion
    }

    public class DialogueMathCalculatorCondition
    {
        #region String
        public bool StringCondition(EventData_StringCondition item)
        {
            switch (item.EventType.Value)
            {
                case StringDialogueEventConditionType.Equals:
                    return StringCondition_Equals(item);
                default:
                    return false;
            }

        }

        private bool StringCondition_Equals(EventData_StringCondition item)
        {
            if (item.Value.Value == item.VariableSO.Value)
                return true;
            else
                return false;
        }
        #endregion

        #region Float
        public bool FloatCondition(EventData_FloatCondition item)
        {
            switch (item.EventType.Value)
            {
                case FloatDialogueEventConditionType.Equals:
                    return FloatCondition_Equals(item);
                case FloatDialogueEventConditionType.EqualsAndBiggerIn:
                    return FloatCondition_EqualsAndBigger(item);
                case FloatDialogueEventConditionType.EqualsAndSmallerIn:
                    return FloatCondition_EqualsAndSmaller(item);
                case FloatDialogueEventConditionType.BiggerIn:
                    return FloatCondition_Bigger(item);
                case FloatDialogueEventConditionType.SmallerIn:
                    return FloatCondition_Smaller(item);
                default:
                    return false;
            }
        }

        private bool FloatCondition_Equals(EventData_FloatCondition item)
        {
            if (item.Value.Value == item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool FloatCondition_EqualsAndBigger(EventData_FloatCondition item)
        {
            if (item.Value.Value <= item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool FloatCondition_EqualsAndSmaller(EventData_FloatCondition item)
        {
            if (item.Value.Value >= item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool FloatCondition_Bigger(EventData_FloatCondition item)
        {
            if (item.Value.Value < item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool FloatCondition_Smaller(EventData_FloatCondition item)
        {
            if (item.Value.Value > item.VariableSO.Value)
                return true;
            else
                return false;
        }
        #endregion

        #region Int
        public bool IntCondition(EventData_IntCondition item)
        {
            switch (item.EventType.Value)
            {
                case IntDialogueEventConditionType.Equals:
                    return IntCondition_Equals(item);
                case IntDialogueEventConditionType.EqualsAndBiggerIn:
                    return IntCondition_EqualsAndBigger(item);
                case IntDialogueEventConditionType.EqualsAndSmallerIn:
                    return IntCondition_EqualsAndSmaller(item);
                case IntDialogueEventConditionType.BiggerIn:
                    return IntCondition_Bigger(item);
                case IntDialogueEventConditionType.SmallerIn:
                    return IntCondition_Smaller(item);
                default:
                    return false;
            }
        }

        private bool IntCondition_Equals(EventData_IntCondition item)
        {
            if (item.Value.Value == item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool IntCondition_EqualsAndBigger(EventData_IntCondition item)
        {
            if (item.Value.Value <= item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool IntCondition_EqualsAndSmaller(EventData_IntCondition item)
        {
            if (item.Value.Value >= item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool IntCondition_Bigger(EventData_IntCondition item)
        {
            if (item.Value.Value < item.VariableSO.Value)
                return true;
            else
                return false;
        }

        private bool IntCondition_Smaller(EventData_IntCondition item)
        {
            if (item.Value.Value > item.VariableSO.Value)
                return true;
            else
                return false;
        }
        #endregion

        #region Bool
        public bool BoolCondition(EventData_BoolCondition item)
        {
            switch (item.EventType.Value)
            {
                case BoolDialogueEventConditionType.True:
                    return BoolCondition_True(item);
                case BoolDialogueEventConditionType.False:
                    return BoolCondition_False(item);
                default:
                    return false;
            }
        }

        private bool BoolCondition_True(EventData_BoolCondition item)
        {
            if (item.VariableSO.Value == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool BoolCondition_False(EventData_BoolCondition item)
        {
            if (item.VariableSO.Value == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
