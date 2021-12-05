using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "String Variable", menuName = "Dialogue Editor/Modular Components/Variable/String Variable", order = 1)]
    public class StringVariableSO : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif
        [SerializeField] private string _value;

        public string Value { get => _value; set => _value = value; }

        public void SetValue(string value)
        {
            _value = value;
        }

        public void SetValue(StringVariableSO value)
        {
            _value = value.Value;
        }

        public void ApplyChange(string amount)
        {
            _value += amount;
        }

        public void ApplyChange(StringVariableSO amount)
        {
            _value += amount.Value;
        }
    }
}