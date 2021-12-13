using UnityEditor;
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

        public static BoolVariableSO NewBool()
        {
            string path = EditorUtility.SaveFilePanelInProject(
            "Create a new Dialogue Actor",
            "<Fill Bool Variable Name Here>.asset",
            "asset",
            "");

            BoolVariableSO newBool = ScriptableObject.CreateInstance<BoolVariableSO>();
            EditorUtility.SetDirty(newBool);

            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(newBool, path);

                AssetDatabase.SaveAssets();

                newBool.SetValue(true);

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newBool;
            }
            else
                return null;
        }
    }
}