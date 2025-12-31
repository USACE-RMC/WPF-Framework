using System;
using System.Windows;
using System.Windows.Input;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        public NameDialog()
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        public NameDialog(int charLimit)
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            NameTBox.CharacterLimit = charLimit;
        }

        public NameDialog(int charLimit, string initialText, bool canBeBlank, string[] existingNames, char[] invalidCharacters = null)
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            NameTBox.CharacterLimit = charLimit;
            NameTBox.CanBeBlank = canBeBlank;
            NameTBox.InvalidStrings = existingNames;
            if (invalidCharacters != null) NameTBox.InvalidCharacters = invalidCharacters;
            Text = initialText;
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(NameDialog), new FrameworkPropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set{ SetValue(TextProperty, value);}
        }

        public static DependencyProperty InnerContentProperty = DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(NameDialog), new FrameworkPropertyMetadata(null));
        public object InnerContent
        {
            get { return GetValue(InnerContentProperty); }

            set { SetValue(InnerContentProperty, value);}
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTBox.IsValid == false)
            {
                MessageBox.Show("Invalid name for the following reasons:" + Environment.NewLine + "\t" + "- " + string.Join("\t" + "- ", this.NameTBox.GetErrorMessages()), "Invalid Name", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            // 
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NameDialog_ContentRendered(object sender, EventArgs e)
        {
            NameTBox.ValidateText();
            NameTBox.NameTBox.Focus();
            NameTBox.NameTBox.CaretIndex = NameTBox.Text.Length;
        }

    }
}
