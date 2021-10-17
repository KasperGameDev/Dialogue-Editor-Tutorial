using System;
using UnityEngine;

namespace KasperDev.ModularComponents
{
    [Serializable]
    public class StringReference
    {
        [SerializeField] private bool _useConstant = true;
        [SerializeField] private string _constantValue;
        [SerializeField] private StringVariableSO _variable;

        public StringVariableSO Variable { get => _variable; set => _variable = value; }

        public string Value
        {
            get { return _useConstant ? _constantValue : _variable.Value; }
            set
            {
                if (_useConstant)
                {
                    _constantValue = value;
                }
                else
                {
                    _variable.Value = value;
                }
            }
        }

        public StringReference() { }

        public StringReference(string value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator string(StringReference reference)
        {
            return reference.Value;
        }
    }
}