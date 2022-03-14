﻿using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "Float Dialogue Variable", menuName = "Dialogue Editor/Modular Components/Variable/Float Variable", order = 1)]
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

        public static FloatVariableSO NewFloat()
        {
            string path = EditorUtility.SaveFilePanelInProject(
            "Create a new Dialogue Actor",
            "<Fill Float Variable Name Here>.asset",
            "asset",
            "");

            FloatVariableSO newFloat = ScriptableObject.CreateInstance<FloatVariableSO>();
            EditorUtility.SetDirty(newFloat);

            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(newFloat, path);

                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newFloat;
            }
            else
                return null;
        }
    }
}