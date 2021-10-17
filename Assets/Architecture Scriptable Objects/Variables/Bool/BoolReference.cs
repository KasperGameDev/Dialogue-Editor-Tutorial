using System;
using UnityEngine;

namespace KasperDev.ModularComponents
{
    [Serializable]
    public class BoolReference
    {
        [SerializeField] private bool _useConstant = true;
        [SerializeField] private bool _constantValue;
        [SerializeField] private BoolVariableSO _variable;

        public BoolVariableSO Variable { get => _variable; set => _variable = value; }

        public bool Value
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

        public BoolReference() { }

        public BoolReference(bool value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator bool(BoolReference reference)
        {
            return reference.Value;
        }
    }
}