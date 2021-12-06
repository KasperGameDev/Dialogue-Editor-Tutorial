using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "Dialogue Bool Variable", menuName = "Dialogue Editor/Modular Components/Variable/Bool Variable", order = 1)]
    public class BoolVariableSO : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif
        [SerializeField] private bool _value;

        public bool Value { get => _value; set => this._value = value; }

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariableSO value)
        {
            Value = value.Value;
        }
    }
}