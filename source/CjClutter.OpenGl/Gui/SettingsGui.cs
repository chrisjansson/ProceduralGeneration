using System;
using System.Globalization;
using System.Linq;
using Awesomium.Core;

namespace CjClutter.OpenGl.Gui
{
    public class SettingsGui : AwesomiumGui
    {
        private JSObject _viewModel;
        private object _dataContext;
        private JSObject _communicator;

        public SettingsGui(OpenGlWindow openGlWindow)
            : base(openGlWindow)
        {
            SettingsChanged += () => { };
        }

        public event Action SettingsChanged;

        protected override void OnDocumentReady()
        {
            _viewModel = CreateJsObject("viewModel");
            _communicator = CreateJsObject("communicator");
            _communicator.Bind("apply", false, Apply);
        }

        public void SetDataContext(object viewModel)
        {
            _dataContext = viewModel;
            Run(() =>
            {
                ClearViewModel();

                BindTo(_viewModel, viewModel);
            });
        }

        private void ClearViewModel()
        {
            foreach (var viewModelProperty in _viewModel.GetPropertyNames())
            {
                _viewModel.RemoveProperty(viewModelProperty);
            }
        }

        private void Apply(object sender, JavascriptMethodEventArgs e)
        {
            var type = _viewModel.GetType();
            var properties = type.GetProperties();
            foreach (var propertyName in _viewModel.GetPropertyNames())
            {
                var propertyValue = _viewModel[propertyName];
                var property = properties.Single(x => x.Name == propertyName);
                var propertyType = property.PropertyType;
                var result = Convert.ChangeType((string)propertyValue, propertyType, CultureInfo.InvariantCulture);
                property.SetValue(_dataContext, result, null);
            }
        }

        public T GetSettings<T>() where T : new()
        {
            //return GetAs<T>(_settings);
            return new T();
        }
    }
}