using System;
using System.Collections.Generic;
using System.Drawing;
using CjClutter.OpenGl.Noise;
using Gwen;
using Gwen.Control;
using System.Linq;

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
        private readonly List<IPropertyRowContainer> _propertyRowContainers = new List<IPropertyRowContainer>();

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
            
            _button = new Button(_dockWithBackground)
                {
                    Dock = Pos.Top,
                    Margin = new Margin(0, 5, 0, 0),
                    Text = "Apply",
                    AutoSizeToContents = true,
                };

            _button.Clicked += OnSettingsChange;

            AddField("Octaves", () => _octaves, x => _octaves = x);
            AddField("Amplitude", () => _amplitude, x => _amplitude = x);
            AddField("Frequency", () => _frequency, x => _frequency = x);
        }

        public event Action<FractalBrownianMotionSettings> GenerationSettingsChanged;

        private void OnSettingsChange(Base control)
        {
            if (GenerationSettingsChanged != null)
                GenerationSettingsChanged(GetSettings());
        }

        private void AddField<T>(string label, Func<T> getter, Action<T> setter)
        {
            var propertyRow = _properties.Add(label);
            propertyRow.SizeToChildren();

            var propertyRowContainer = new PropertyRowContainer<T>(propertyRow)
                {
                    Value = getter()
                };

            propertyRowContainer.ValueChanged += () => setter(propertyRowContainer.Value);

            AddPropertyRowContainer(propertyRowContainer);
        }

        private void AddPropertyRowContainer(IPropertyRowContainer propertyRowContainer)
        {
            propertyRowContainer.IsValidChanged += () => _button.IsDisabled = !_propertyRowContainers.All(x => x.IsValid);
            _propertyRowContainers.Add(propertyRowContainer);
        }

        private FractalBrownianMotionSettings GetSettings()
        {
            return new FractalBrownianMotionSettings(_octaves, _amplitude, _frequency);
        }

        public void SetSettings(FractalBrownianMotionSettings settings)
        {
            _octaves = settings.Octaves;
            _amplitude = settings.Amplitude;
            _frequency = settings.Frequency;

            //AddField("Octaves", () => _octaves, x => _octaves = x);
            //AddField("Amplitude", () => _amplitude, x => _amplitude = x);
            //AddField("Frequency", () => _frequency, x => _frequency = x);
        }

        public void Update()
        {
            //todo: update here or in render or instantiation only?
            //bool sizeToChildren = _properties.SizeToChildren();
        }
    }
}