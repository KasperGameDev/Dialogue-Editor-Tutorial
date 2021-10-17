using System;
using UnityEngine;

namespace KasperDev.ModularComponents
{
    [Serializable]
    public class FloatReference
    {
        [SerializeField] private bool _useConstant = true;
        [SerializeField] private float _constantValue;
        [SerializeField] private FloatVariableSO _variable;

        public FloatVariableSO Variable { get => _variable; set => _variable = value; }

        public float Value
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

        public FloatReference(){ }

        public FloatReference(float value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}