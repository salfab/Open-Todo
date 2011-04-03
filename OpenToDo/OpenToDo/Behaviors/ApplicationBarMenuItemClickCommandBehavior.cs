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
using Microsoft.Phone.Shell;
using Microsoft.Practices.Prism.Commands;

namespace OpenToDo.Behaviors
{
    public class ApplicationBarMenuItemClickCommandBehavior : DependencyObject
    {
        public ApplicationBarMenuItemClickCommandBehavior()
        {
            
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ApplicationBarMenuItemClickCommandBehavior), new PropertyMetadata(null));



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ApplicationBarMenuItemClickCommandBehavior), new PropertyMetadata(null));

        

        public IApplicationBarMenuItem Button
        {
            get { return (IApplicationBarMenuItem)GetValue(ButtonProperty); }
            set { SetValue(ButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Button.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.Register("Button", typeof(IApplicationBarMenuItem), typeof(ApplicationBarMenuItemClickCommandBehavior), new PropertyMetadata(null, ButtonChangedCallback));

        private static ApplicationBarMenuItemClickCommandBehavior _instance;

        private static void ButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_instance == null)
            {
                // Only the first time
                _instance = (ApplicationBarMenuItemClickCommandBehavior)d;
            }

            if (_instance == null)
            {
                // if d was already null or if the casting could not be done correctly : let's not call the event handler anymore.
                _instance.Button.Click += OnButtonOnClick;
            }

        }

        private static void OnButtonOnClick(object s, EventArgs ea)
        {
            _instance.Command.Execute(ea);
        }
    }
}
