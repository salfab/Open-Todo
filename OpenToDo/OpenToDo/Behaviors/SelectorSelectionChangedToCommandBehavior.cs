using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OpenToDo.Behaviors
{
    public class SelectorSelectionChangedToCommandBehavior
    {
        public ICommand Command { get; set; }



        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(SelectorSelectionChangedToCommandBehavior), new PropertyMetadata(null));


        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(SelectorSelectionChangedToCommandBehavior), new PropertyMetadata(null, CommandChangedCallback));

        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonBase button = (ButtonBase)d;
            button.Click += (o, ea) =>
                                {
                                    DependencyObject dependencyObject = (DependencyObject)o;
                                    var command = GetCommand(dependencyObject);
                                    var commandParameter = GetCommandParameter(dependencyObject);
                                    command.Execute(commandParameter);
                                };
        }
    }
}
