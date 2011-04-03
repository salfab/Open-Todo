using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;

namespace OpenToDo.Behaviors
{
    public class ApplicationBarBindingUpdateHelperBehavior
    {


        public static object GetPhonePage(DependencyObject obj)
        {
            return (object)obj.GetValue(PhonePageProperty);
        }

        public static void SetPhonePage(DependencyObject obj, object value)
        {
            obj.SetValue(PhonePageProperty, value);
        }

        // Using a DependencyProperty as the backing store for PhonePage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhonePageProperty =
            DependencyProperty.RegisterAttached("PhonePage", typeof(object), typeof(ApplicationBarBindingUpdateHelperBehavior), new PropertyMetadata(null, PhonePageChangedCallBack));

        private static void PhonePageChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = ((IList<IApplicationBarIconButton>)(e.NewValue))[0];
            button.Click += (o, ea) =>
                                {
                                    UpdateBinding(d);
                                };
        }

        private static void UpdateBinding(DependencyObject dependencyObject)
        {
            // Support for text boxes.
            if (dependencyObject as TextBox != null)
            {
                BindingExpression bindingExpression = ((TextBox) dependencyObject).GetBindingExpression(TextBox.TextProperty);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateSource();
                }
            }

            // TODO : Support for Content Controls.
        }


        public static string GetApplicationBarButtonName(DependencyObject obj)
        {
            return (string)obj.GetValue(ApplicationBarButtonNameProperty);
        }

        public static void SetApplicationBarButtonName(DependencyObject obj, string value)
        {
            obj.SetValue(ApplicationBarButtonNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplicationBarButtonName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationBarButtonNameProperty =
            DependencyProperty.RegisterAttached("ApplicationBarButtonName", typeof(string), typeof(ApplicationBarBindingUpdateHelperBehavior), new PropertyMetadata(null, ApplicationBarButtonNameChangedCallback));

        private static void ApplicationBarButtonNameChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var phonePage = GetPhonePage(d);
        }
    }

    public class ApplicationBarButtonsListWithSpecifiedIndexToApplicationBarButtonConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="index">The index to use to address the Buttons list.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <remarks>
        /// This value converter is necessary to bind to an item of the Buttons list in the ApplicationBar.Buttons property of the page.
        /// The reason is this list is of type ApplicationBarItemList, which is internal, and trying to bind to an element with the [i] syntax doesn't work since the get_item method is not accessible.
        /// The sad thing is that by casting it to IList this would have been possible, but this is not done automatically by the framework, so we need to do this manually, hence this converter.
        /// </remarks>
        public object Convert(object value, Type targetType, object index, CultureInfo culture)
        {
            var typedIndex = (int)index;
            object element = ((IList)value)[typedIndex];
            return element;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
