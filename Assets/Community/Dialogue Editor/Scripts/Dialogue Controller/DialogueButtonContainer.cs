using UnityEngine.Events;

namespace DialogueEditor.Dialogue.Scripts
{
    public class DialogueButtonContainer
    {
        public UnityAction UnityAction { get; set; }
        public string Text { get; set; }
        public bool ConditionCheck { get; set; }
        public ChoiceStateType ChoiceState { get; set; }
    }
}
