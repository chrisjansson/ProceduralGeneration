using System;
using System.ComponentModel;
using Gwen.Control;

namespace CjClutter.OpenGl.Gui
{
    public interface IPropertyRowContainer
    {
        bool IsValid { get; }
        event Action IsValidChanged;
        Action Refresh { get; }
    }

    public class PropertyRowContainer<T> : IPropertyRowContainer
    {
        private readonly PropertyRow _propertyRow;
        private bool _isSettingValue;

        public event Action ValueChanged;
        public event Action IsValidChanged;

        public PropertyRowContainer(PropertyRow propertyRow)
        {
            _propertyRow = propertyRow;
            propertyRow.ValueChanged += OnValueChanged;
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                if (IsValidChanged != null)
                    IsValidChanged();
            }
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isSettingValue = true;
                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof (T));
                _propertyRow.Value = typeConverter.ConvertToInvariantString(value);
                _isSettingValue = false;
            }
        }

        public Action Refresh { get; set; }

        private void OnValueChanged(Base control)
        {
            if (_isSettingValue)
            {
                return;
            }

            var text = _propertyRow.Value;
            var converter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                var newValue = (T)converter.ConvertFromInvariantString(text);
                _value = newValue;
                IsValid = true;
                if (ValueChanged != null)
                    ValueChanged();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }

    }
}