using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OpenToDo.Behaviors
{
    public class StyleBooleanTriggerBehavior
    {



        public static bool GetTriggerProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(TriggerPropertyProperty);
        }

        public static void SetTriggerProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(TriggerPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for TriggerProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriggerPropertyProperty =
            DependencyProperty.RegisterAttached("TriggerProperty", typeof(bool), typeof(StyleBooleanTriggerBehavior), new PropertyMetadata(true, TriggerPropertyChangedCallback));

        private static void TriggerPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement) d;
            if (d != null)
            {
                bool newValue = (bool) e.NewValue;                
                TriggerStyle(d, newValue, element);
            }
        }

        private static void TriggerStyle(DependencyObject d, bool newValue, FrameworkElement element)
        {
            Style style = newValue == true ? GetPropertyTrueStyle(d) : GetPropertyFalseStyle(d);
            element.Style = style;
        }


        public static Style GetPropertyTrueStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(PropertyTrueStyleProperty);
        }

        public static void SetPropertyTrueStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(PropertyTrueStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyTrueStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyTrueStyleProperty =
            DependencyProperty.RegisterAttached("PropertyTrueStyle", typeof(Style), typeof(StyleBooleanTriggerBehavior), new PropertyMetadata(null));


        public static Style GetPropertyFalseStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(PropertyFalseStyleProperty);
        }

        public static void SetPropertyFalseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(PropertyFalseStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyFalseStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyFalseStyleProperty =
            DependencyProperty.RegisterAttached("PropertyFalseStyle", typeof(Style), typeof(StyleBooleanTriggerBehavior), new PropertyMetadata(null));
    }
}
