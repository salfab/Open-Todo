using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OpenToDo.Services
{
    /// <summary>
    /// A view model locator which can register a ViewModel to a specific view type
    /// </summary>
    /// <remarks>This implemntation focuses on returning only one view model for a specific view which is perfect for a small number of views and only one instance of a view in the whole app.</remarks>
    static public class ViewModelLocatorService
    {
        private static Dictionary<Type, object> _viewModels = new Dictionary<Type, object>();

        static public object GetViewModel(Type viewType)
        {
            if (_viewModels.ContainsKey(viewType))
            {
                return _viewModels[viewType];                
            }
            return null;
        }

        /// <summary>
        /// Sets the view model.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns>true if a view model had to be overwritten, false otherwise.</returns>
        public static bool SetViewModel(Type viewType, object viewModel)
        {
            if (_viewModels.ContainsKey(viewType))
            {
                _viewModels[viewType] = viewModel;
                return true;
            }
            _viewModels.Add(viewType, viewModel);
            return false;
        }
    }
}
