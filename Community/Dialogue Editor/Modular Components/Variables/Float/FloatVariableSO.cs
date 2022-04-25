using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    public class FloatVariableSO : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif
        [SerializeField] private float _value;

        public float Value { get => _value; set => this._value = value; }

        public void SetValue(float value)
        {
            _value = value;
        }

        public void SetValue(FloatVariableSO value)
        {
            _value = value.Value;
        }

        public void ApplyChange(float amount)
        {
            _value += amount;
        }

        public void ApplyChange(FloatVariableSO amount)
        {
            _value += amount.Value;
        }

        public static FloatVariableSO NewFloat(ScriptableObject so)
        {
            string name = EditorInputDialogue.Show("New Float Variable", "Please Enter Variable Name", "");
            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("Canceled", "You're variable was not Created. It had no name", "OK");
                return null;
            }
            else
            {

                FloatVariableSO newFloat = ScriptableObject.CreateInstance<FloatVariableSO>();
                newFloat.name = name;
                EditorUtility.SetDirty(newFloat);
                AssetDatabase.AddObjectToAsset(newFloat, so);
                    return newFloat;
                }
        }
    }
}