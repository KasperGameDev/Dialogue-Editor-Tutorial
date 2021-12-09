using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "Int Variable", menuName = "Dialogue Editor/Modular Components/Variable/Int Variable", order = 1)]
    public class IntVariableSO : ScriptableObject
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif
        [SerializeField] private int _value;

        public int Value { get => _value; set => _value = value; }

        public void SetValue(int value)
        {
            _value = value;
        }

        public void SetValue(IntVariableSO value)
        {
            _value = value._value;
        }

        public void ApplyChange(int amount)
        {
            _value += amount;
        }

        public void ApplyChange(IntVariableSO amount)
        {
            _value += amount._value;
        }

        public static IntVariableSO NewInt()
        {
            string path = EditorUtility.SaveFilePanelInProject(
            "Create a new Dialogue Actor",
            "<Fill Int Variable Name Here>.asset",
            "asset",
            "");

            IntVariableSO newInt = ScriptableObject.CreateInstance<IntVariableSO>();
            EditorUtility.SetDirty(newInt);

            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(newInt, path);

                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newInt;
            }
            else
                return null;
        }
    }
}