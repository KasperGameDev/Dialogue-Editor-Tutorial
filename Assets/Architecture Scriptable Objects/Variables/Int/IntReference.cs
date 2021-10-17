using System;
using UnityEngine;

namespace KasperDev.ModularComponents
{
    [Serializable]
    public class IntReference
    {
        [SerializeField] private bool _useConstant = true;
        [SerializeField] private int _constantValue;
        [SerializeField] private IntVariableSO _variable;

        public IntVariableSO Variable { get => _variable; set => _variable = value; }

        public int Value
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

        public IntReference() { }

        public IntReference(int value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}