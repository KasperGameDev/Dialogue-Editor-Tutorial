using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    public class BoolVariableSO : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif
        [SerializeField] private bool _referencevalue;
        private bool _value;

        private void OnEnable()
        {
            _value = _referencevalue;
        }

        public bool Value { get => _value; set => this._value = value; }

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariableSO value)
        {
            Value = value.Value;
        }

        public static BoolVariableSO NewBool(ScriptableObject so)
        {
            string name = EditorInputDialogue.Show("New Bool Variable", "Please Enter Variable Name", "");
            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("Canceled", "You're variable was not Created. It had no name", "OK");
                return null;
            }
            else
            {

                BoolVariableSO newBool = ScriptableObject.CreateInstance<BoolVariableSO>();
                newBool.name = name;
                EditorUtility.SetDirty(newBool);

                AssetDatabase.AddObjectToAsset(newBool, so);
                return newBool;
            }
        }
    }
}