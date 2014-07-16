using System;
using CjClutter.OpenGl.SceneGraph;

namespace CjClutter.OpenGl.Gui
{
    public class SettingsViewModel
    {
        public SettingsViewModel(NoiseFactory.NoiseParameters defaults)
        {
            SettingsChanged += () => { };
            _amplitude = defaults.Amplitude;
            _octaves = defaults.Octaves;
            _frequency = defaults.Frequency;
            _lacunarity = defaults.Lacunarity;
            _h = defaults.H;
            _offset = defaults.Offset;
            _gain = defaults.Gain;
        }

        public NoiseFactory.NoiseParameters Assemble()
        {
            return new NoiseFactory.NoiseParameters
            {
                Amplitude = Amplitude,
                Frequency = Frequency,
                Gain = Gain,
                H = H,
                Lacunarity = Lacunarity,
                Octaves = Octaves,
                Offset = Offset
            };
        }

        public event Action SettingsChanged;

        private int _octaves;
        private double _amplitude;
        private double _frequency;
        private double _lacunarity;
        private double _h;
        private double _offset;
        private double _gain;

        public int Octaves
        {
            get { return _octaves; }
            set
            {
                _octaves = value;
                SettingsChanged();
            }
        }

        public double Amplitude
        {
            get { return _amplitude; }
            set
            {
                _amplitude = value;
                SettingsChanged();
            }
        }

        public double Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                SettingsChanged();
            }
        }

        public double Lacunarity
        {
            get { return _lacunarity; }
            set
            {
                _lacunarity = value;
                SettingsChanged();
            }
        }

        public double H
        {
            get { return _h; }
            set
            {
                _h = value;
                SettingsChanged();
            }
        }

        public double Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                SettingsChanged();
            }
        }

        public double Gain
        {
            get { return _gain; }
            set
            {
                _gain = value;
                SettingsChanged();
            }
        }
    }
}