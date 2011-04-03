using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenToDo
{
    public static class BindingUpdateHelper
    {
        public static void RefreshBinding(Control elementToUpdate)
        {
            object safelyCastedElement = elementToUpdate as TextBox;
            
            // For textbox.
            if (safelyCastedElement != null)
            {
                BindingExpression bindingExpression = elementToUpdate.GetBindingExpression(TextBox.TextProperty);
                RefreshBindingExpression(bindingExpression);
                return;
            }

            safelyCastedElement = elementToUpdate as ContentControl;

            // For content controls.
            if (safelyCastedElement != null)
            {
                BindingExpression bindingExpression = elementToUpdate.GetBindingExpression(ContentControl.ContentProperty);
                RefreshBindingExpression(bindingExpression);
                return;
            }

            Console.WriteLine("Log-Warning : " + string.Format("Binding update for type {0} is not supported.", elementToUpdate.GetType()));

        }

        private static void RefreshBindingExpression(BindingExpression bindingExpression)
        {
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
            }
        }
    }
}