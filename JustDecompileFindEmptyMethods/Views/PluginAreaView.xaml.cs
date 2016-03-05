namespace FindEmptyMethods.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Interaction logic for PluginAreaView.xaml
    /// </summary>
    public partial class PluginAreaView : UserControl
    {
        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(PluginAreaView));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(PluginAreaView), new PropertyMetadata(string.Empty));

        [Obsolete("For design-time use only.")]
        public PluginAreaView()
            : this(() => { })
        {
            this.Text = "Design-time text.";
        }

        public PluginAreaView(Action close)
        {
            this.DataContext = this;

            InitializeComponent();

            this.CloseCommand = new DelegateCommand(close);
        }

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
