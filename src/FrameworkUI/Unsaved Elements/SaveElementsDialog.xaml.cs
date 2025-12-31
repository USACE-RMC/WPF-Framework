using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for SaveElementsDialog.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class SaveElementsDialog : Window
    {
        /// <summary>
        /// Construct a new save elements dialog.
        /// </summary>
        public SaveElementsDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Dependency property for the save elements list.
        /// </summary>
        public static DependencyProperty ElementItemsProperty = DependencyProperty.Register(nameof(ElementItems), typeof(ObservableCollection<UnsavedElement>), typeof(SaveElementsDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the save elements list.
        /// </summary>
        public ObservableCollection<UnsavedElement> ElementItems
        {
            get { return (ObservableCollection<UnsavedElement>)GetValue(ElementItemsProperty); }
            set
            {
                SetValue(ElementItemsProperty, value);
                MyDataGrid.ItemsSource = null;
                MyDataGrid.ItemsSource = ElementItems;
            }
        }

        /// <summary>
        /// Enumeration of dialog result types.
        /// </summary>
        public enum DialogResultType
        {
            YesSave,
            NoSave,
            Cancel
        }

        /// <summary>
        /// Gets the save elements dialog result.
        /// </summary>
        public DialogResultType Result { get; private set; } = DialogResultType.Cancel;

        /// <summary>
        /// On Save, save all elements, set dialog result to true and close.
        /// </summary>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save all elements
                for (int i = 0; i < ElementItems.Count; i++)
                    ElementItems[i].Element.Save();
                Result = DialogResultType.YesSave;
            }
            catch (Exception ex)
            {
                Result = DialogResultType.Cancel;
                MessageBox.Show(ex.ToString());
            }
            Close();
        }

        /// <summary>
        /// On Don't Save, set dialog result to true and close. Do not save.
        /// </summary>
        private void Btn_DontSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Instead of saving, restore the last saved inputs from disk.
                for (int i = 0; i < ElementItems.Count; i++)
                    ElementItems[i].Element.Open();
                Result = DialogResultType.NoSave;
            }
            catch (Exception ex)
            {
                Result = DialogResultType.Cancel;
                MessageBox.Show(ex.ToString());
            }
            Close();
        }

        /// <summary>
        /// On Cancel, set dialog result to false and close.
        /// </summary>
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResultType.Cancel;
            Close();
        }

    }
}
