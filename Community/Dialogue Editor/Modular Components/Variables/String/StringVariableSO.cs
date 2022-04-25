using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
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

        public static StringVariableSO NewString(ScriptableObject container)
        {
            string name = EditorInputDialogue.Show("New String Variable", "Please Enter Variable Name", "");
            if (string.IsNullOrEmpty(name))
                EditorUtility.DisplayDialog("Canceled", "You're variable was not Created. It had no name", "OK");

            if (!string.IsNullOrEmpty(name))
            {
                StringVariableSO newString = ScriptableObject.CreateInstance<StringVariableSO>();
                newString.name = name;
                EditorUtility.SetDirty(newString);

                AssetDatabase.AddObjectToAsset(newString, container);
                return newString;
            }
            else
                return null;
        }
    }
}