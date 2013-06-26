using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using CjClutter.OpenGl.Noise;
using Gwen;
using Gwen.Control;

namespace CjClutter.OpenGl.Gui
{
    public class GenerationSettingsControl
    {
        private int _octaves;
        private double _amplitude;
        private double _frequency;
        private readonly Properties _properties;
        private readonly DockWithBackground _dockWithBackground;
        private readonly Button _button;

        private readonly IList<Base> _invalidControls = new List<Base>();  

        public GenerationSettingsControl(Base parent)
        {
            _dockWithBackground = new DockWithBackground(parent)
                {
                    Dock = Pos.Top,
                    Padding = new Padding(5, 5, 5, 5),
                    BackgroundColor = Color.LightBlue,
                };

            _properties = new Properties(_dockWithBackground)
                {
                    Dock = Pos.Top,
                };

            CreateFieldFor("Octaves", () => _octaves, x => _octaves = x);
            CreateFieldFor("Amplitude", () => _amplitude, x => _amplitude = x);
            CreateFieldFor("Frequency", () => _frequency, x => _frequency = x);

            _button = new Button(_dockWithBackground)
                {
                    Dock = Pos.Top,
                    Margin = new Margin(0, 5, 0, 0),
                    Text = "Apply",
                    AutoSizeToContents = true,
                };

            _button.Clicked += OnSettingsChange;
        }

        public event Action<FractalBrownianMotionSettings> GenerationSettingsChanged;

        private void OnSettingsChange(Base control)
        {
            if (GenerationSettingsChanged != null)
                GenerationSettingsChanged(GetSettings());
        }

        private void CreateFieldFor<T>(string label, Func<T> getter, Action<T> setter)
        {
            var propertyRow = _properties.Add(label);
            propertyRow.SizeToChildren();

            propertyRow.Value = getter().ToString();
            propertyRow.ValueChanged += x =>
                {
                    var text = propertyRow.Value;
                    var converter = TypeDescriptor.GetConverter(typeof (T));

                    try
                    {
                        var newValue = (T) converter.ConvertFromInvariantString(text);
                        setter(newValue);

                        RemoveValidControl(propertyRow);
                    }
                    catch (Exception e)
                    {
                        AddInvalidControl(propertyRow);
                    }
                };
        }

        private void AddInvalidControl(Base control)
        {
            if (!_invalidControls.Contains(control))
                _invalidControls.Add(control);

            _button.IsDisabled = _invalidControls.Any();
        }

        private void RemoveValidControl(Base control)
        {
            _invalidControls.Remove(control);
            _button.IsDisabled = _invalidControls.Any();
        }

        private FractalBrownianMotionSettings GetSettings()
        {
            return new FractalBrownianMotionSettings(_octaves, _amplitude, _frequency);
        }

        public void Update()
        {
            //todo: update here or in render or instantiation only?
            //bool sizeToChildren = _properties.SizeToChildren();
        }
    }
}