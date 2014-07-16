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
        }

        protected override void OnDocumentReady()
        {
            _viewModel = CreateJsObject("viewModel");
            _communicator = CreateJsObject("communicator");
            _communicator.Bind("apply", false, Apply);
        }

        public void SetDataContext(object viewModel)
        {
            _dataContext = viewModel;
            Run(x =>
            {
                if (!x.IsDocumentReady && _viewModel == null)
                    x.DocumentReady += (sender, args) => RefreshViewModel();
                else
                    RefreshViewModel();
            });
        }

        private void RefreshViewModel()
        {
            ClearViewModel();
            BindTo(_viewModel, _dataContext);
            ExecuteJs("echo();");
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
            var type = _dataContext.GetType();
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
    }
}