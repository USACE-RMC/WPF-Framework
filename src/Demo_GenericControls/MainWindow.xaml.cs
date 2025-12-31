/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GenericControls;
using Themes;

namespace Demo_GenericControls
{
    /// <summary>
    /// Main window for demonstrating GenericControls library functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This demo showcases various custom controls including color pickers, numeric inputs,
    /// data grids, line style selectors, and other WPF UI components.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        private string _textProperty;
        private string _fontProperty;
        private FontWeight _testFontWeightProperty;
        private bool _booleanProperty;
        private double _numericSelectorProperty;
        private double _numericProperty;
        private DoubleCollection _lineStyleProperty;
        private double _lineWidthProperty;
        private SolidColorBrush _colorProperty;
        private HorizontalAlignment _testHorizontalAlignmentProperty;
        private CalendarWeekRule _calendarWeekProperty;
        private IList<string> _stringListProperty;

        public static DependencyProperty TestGridLengthProperty = DependencyProperty.Register(nameof(TestGridLength), typeof(GridLength), typeof(MainWindow), new FrameworkPropertyMetadata(new GridLength(100d)));

        /// <summary>
        /// Gets/sets the test grid length, used for layout measurements.
        /// </summary>
        public GridLength TestGridLength
        {
            get
            {
                return (GridLength)this.GetValue(TestGridLengthProperty);
            }
            set
            {
                this.SetValue(TestGridLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        public string TextProperty
        {
            get
            {
                return _textProperty;
            }
            set
            {
                _textProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the font family name.
        /// </summary>
        public string FontProperty
        {
            get
            {
                return _fontProperty;
            }
            set
            {
                _fontProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontProperty)));

            }
        }

        /// <summary>
        /// Gets/sets the font weight.
        /// </summary>
        public FontWeight TestFontWeightProperty
        {
            get
            {
                return _testFontWeightProperty;
            }
            set
            {
                _testFontWeightProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestFontWeightProperty)));

            }
        }

        /// <summary>
        /// Gets or sets the boolean toggle value.
        /// </summary>
        public bool BooleanProperty
        {
            get
            {
                return _booleanProperty;
            }
            set
            {
                _booleanProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BooleanProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the numeric value for selection control testing.
        /// </summary>
        public double NumericSelectorProperty
        {
            get
            {
                return _numericSelectorProperty;
            }
            set
            {
                _numericSelectorProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumericSelectorProperty)));
            }
        }

        /// <summary>
        /// Gets or sets a numeric value.
        /// </summary>
        public double NumericProperty
        {
            get
            {
                return _numericProperty;
            }
            set
            {
                _numericProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumericProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the line style (dash pattern) as a <see cref="DoubleCollection"/>.
        /// </summary>
        public DoubleCollection LineStyleProperty
        {
            get
            {
                return _lineStyleProperty;
            }
            set
            {
                _lineStyleProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineStyleProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the line width.
        /// </summary>
        public double LineWidthProperty
        {
            get
            {
                return _lineWidthProperty;
            }
            set
            {
                _lineWidthProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineWidthProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the color as a <see cref="SolidColorBrush"/>.
        /// </summary>
        public SolidColorBrush ColorProperty
        {
            get
            {
                return _colorProperty;
            }
            set
            {
                _colorProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorProperty)));
            }
        }

        /// <summary>
        /// Gets or sets a horizontal alignment test value.
        /// </summary>
        public HorizontalAlignment TestHorizontalAlignmentProperty
        {
            get
            {
                return _testHorizontalAlignmentProperty;
            }
            set
            {
                _testHorizontalAlignmentProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestHorizontalAlignmentProperty)));
            }
        }

        /// <summary>
        /// Gets or sets the calendar week rule.
        /// </summary>
        public CalendarWeekRule CalendarWeekProperty
        {
            get
            {
                return _calendarWeekProperty;
            }
            set
            {
                _calendarWeekProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalendarWeekProperty)));
            }
        }

        /// <summary>
        /// Gets or sets a list of string values.
        /// </summary>
        public IList<string> StringListProperty
        {
            get
            {
                return _stringListProperty;
            }
            set
            {
                _stringListProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringListProperty)));
            }
        }

        /// <summary>
        /// Raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <remarks>
        /// Sets up the data context, initializes sample data for demonstration controls,
        /// and configures default property values.
        /// </remarks>
        public MainWindow()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.


            this.DataContext = this;


            var cats = new ObservableCollection<object>();
            cats.Add(new Cat() { Color = "Orange", Age = "3", Sex = "Male" });
            cats.Add(new Cat() { Color = "Gray", Age = "12", Sex = "Female" });
            cats.Add(new Cat() { Color = "White", Age = "5", Sex = "Male" });
            CPDataGrid.ItemsSource = cats;
            CPDataGrid2.ItemsSource = cats;
            // Properties

            TextProperty = "Sample Text";
            FontProperty = "Segoe UI";
            TestFontWeightProperty = FontWeights.Bold;
            BooleanProperty = true;
            NumericSelectorProperty = 12d;
            NumericProperty = double.MaxValue; // 123.456
            LineStyleProperty = LineStyleSelectorControl.LineStyleOptions[0];
            LineWidthProperty = 4d;
            ColorProperty = new SolidColorBrush(Color.FromArgb(150, 105, 205, 125));
            TestHorizontalAlignmentProperty = HorizontalAlignment.Center;

            var t = new List<ColorItem>();
            t.Add(new ColorItem() { ColorTest = Brushes.Red, Name = "Red" });
            t.Add(new ColorItem() { ColorTest = Brushes.Green, Name = "Green" });
            t.Add(new ColorItem() { ColorTest = Brushes.Blue, Name = "Blue" });

            ColorItemsControl.ItemsSource = t;
        }

        /// <summary>
    /// When the mouse enters the datagrid set the popup to staysopen=true to allow the datagrid to keep capture of the mouse and give the datagrid the focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void CPDataGrid2_MouseEnter(object sender, MouseEventArgs e)
        {
            this.DataGridPopup.StaysOpen = true;
            this.CPDataGrid2.Focus();
        }

        /// <summary>
    /// When the mouse leaves the datagrid set the popup to staysopen=false and give it focus so that clicking outside the popup will close it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void CPDataGrid2_MouseLeave(object sender, MouseEventArgs e)
        {
            this.DataGridPopup.StaysOpen = false;
            this.DataGridPopup.Focus();
        }

        /// <summary>
    /// When the context menu closes the focus gets all out of whack in the popup and needs to be reset. Maybe because it is a popup on a popup? WPF Inception
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void CPDataGrid2_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            this.DataGridPopup.IsOpen = false;
            this.DataGridPopup.IsOpen = true;
        }

        /// <summary>
        /// Adjusts the header column width to match combined width of content columns (with padding).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.HeaderColumn.MaxWidth = this.HeaderColumn.ActualWidth + this.PropertyWidthColumn.ActualWidth - 22d;
        }

        /// <summary>
        /// Represents a sample cat entity used for demonstrating data grid functionality.
        /// </summary>
        public class Cat
        {
            /// <summary>
            /// Gets or sets the color of the cat.
            /// </summary>
            public string Color { get; set; }

            /// <summary>
            /// Gets or sets the age of the cat.
            /// </summary>
            public string Age { get; set; }

            /// <summary>
            /// Gets or sets the sex of the cat.
            /// </summary>
            public string Sex { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cat"/> class.
            /// </summary>
            public Cat()
            {
            }
        }

        /// <summary>
        /// Simple debug method for mouse interaction with a control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HorizontalControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Hooyaa!");
        }

        /// <summary>
        /// Handles the AutoGeneratedColumns event to inspect property names bound to each column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CPDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            foreach (var column in this.CPDataGrid.Columns)
            {
                string propertyName = ((Binding)((DataGridTextColumn)column).Binding).Path.Path.ToString();

                Debug.Print(propertyName);
            }
        }

        /// <summary>
        /// (Disabled) Starts Example 1 progress task using a progress reporter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressExample1Button_Click(object sender, RoutedEventArgs e)
        {
            // ProgressExample2ProgressBar.Foreground = New SolidColorBrush(Color.FromArgb(255, 6, 176, 37))
            // _progressReporter1 = New SafeProgressReporter("Example 1")
            // AddHandler _progressReporter1.ProgressReported, AddressOf ReportProgress1
            // AddHandler _progressReporter1.MessageReported, AddressOf ReportMessage1
            // AddHandler _progressReporter1.TaskEnded, Sub()
            // If _progressReporter1.CancelRequested Then
            // Dispatcher.Invoke(New Action(Sub() ProgressExample1Textblock.Text = "Progress Canceled."))
            // ProgressExample1ProgressBar.Value = 0
            // Else
            // ProgressExample1ProgressBar.Foreground = Brushes.LightBlue
            // End If
            // '
            // ProgressExample1CancelButton.IsEnabled = False
            // End Sub
            // '
            // ProgressExample1CancelButton.IsEnabled = True
            // '
            // Dim t As New Task(Sub()
            // _progressReporter1.IndicateTaskStart()
            // Dim totalSteps As Int32 = 100
            // For i As Int32 = 1 To totalSteps
            // If _progressReporter1.CancelRequested Then
            // _progressReporter1.IndicateTaskEnded()
            // Exit Sub
            // End If
            // '
            // System.Threading.Thread.Sleep(100)
            // _progressReporter1.Report(i / totalSteps, (100 * i / totalSteps).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
            // Next
            // _progressReporter1.Report(100, "Complete", SafeProgressReporter.MessageType.Success)
            // _progressReporter1.IndicateTaskEnded()
            // End Sub)
            // '
            // t.Start()
        }

        // Private Sub ReportMessage1(msg As SafeProgressReporter.MessageContentStruct)
        // ProgressExample1Textblock.Text = msg.message
        // If msg.msgType = SafeProgressReporter.MessageType.FatalError Then
        // ProgressExample1Textblock.Foreground = New SolidColorBrush(Colors.Red)
        // Else
        // ProgressExample1Textblock.Foreground = New SolidColorBrush(Colors.Black)
        // End If
        // End Sub

        // Private Sub ReportProgress1(reporter As SafeProgressReporter, prog As Double, progDelta As Double)
        // ProgressExample1ProgressBar.Value = prog
        // End Sub

        /// <summary>
        /// (Disabled) Cancels Example 1 progress task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressExample1CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // _progressReporter1.RequestCancel()
        }

        /// <summary>
        /// (Disabled) Starts Example 2 progress task using two sub-reporters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressExample2Button_Click(object sender, RoutedEventArgs e)
        {
            // ProgressExample2ProgressBar.Foreground = New SolidColorBrush(Color.FromArgb(255, 6, 176, 37))
            // _progressReporter2 = New SafeProgressReporter("Example 1")
            // AddHandler _progressReporter2.ProgressReported, Sub(reporter As SafeProgressReporter, prog As Double, progDelta As Double) ProgressExample2ProgressBar.Value = prog
            // AddHandler _progressReporter2.MessageReported, AddressOf ReportMessage2
            // AddHandler _progressReporter2.TaskEnded, Sub()
            // If _progressReporter2.CancelRequested Then
            // Dispatcher.Invoke(New Action(Sub() ProgressExample2Textblock.Text = "Progress Canceled."))
            // ProgressExample2ProgressBar.Value = 0
            // Else
            // ProgressExample2ProgressBar.Foreground = Brushes.LightBlue
            // End If
            // '
            // ProgressExample2CancelButton.IsEnabled = False
            // End Sub
            // '
            // ProgressExample2CancelButton.IsEnabled = True
            // '
            // Dim t As New Task(Sub()
            // _progressReporter2.IndicateTaskStart()

            // Dim subReporter1 = _progressReporter2.CreateProgressModifier(0.5, "Set 1")
            // 'new thread 1
            // Dim t1 As New Task(Sub()
            // subReporter1.IndicateTaskStart()
            // Dim totalSteps As Int32 = 100
            // For i As Int32 = 1 To totalSteps
            // If subReporter1.CancelRequested Then
            // subReporter1.IndicateTaskEnded()
            // Exit Sub
            // End If
            // '
            // System.Threading.Thread.Sleep(50)
            // subReporter1.ReportProgress(i / totalSteps)
            // subReporter1.ReportMessage(CInt(50 * i / totalSteps).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
            // Next
            // subReporter1.IndicateTaskEnded()
            // End Sub)
            // '
            // t1.Start()
            // t1.Wait()

            // 'two is slightly faster for example purposes when you have to estimate percent of total progress a task will take.
            // Dim subReporter2 = _progressReporter2.CreateProgressModifier(0.5, "Set 2")
            // Dim t2 As New Task(Sub()
            // subReporter2.IndicateTaskStart()
            // Dim totalSteps As Int32 = 100
            // For i As Int32 = 1 To totalSteps
            // If subReporter2.CancelRequested Then
            // subReporter2.IndicateTaskEnded()
            // Exit Sub
            // End If
            // '
            // System.Threading.Thread.Sleep(40)
            // subReporter2.ReportProgress(i / totalSteps)
            // subReporter2.ReportMessage(CInt(50 + (50 * i / totalSteps)).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
            // Next
            // subReporter2.IndicateTaskEnded()
            // End Sub)
            // '
            // t2.Start()
            // t2.Wait()
            // '
            // _progressReporter2.Report(100, "Complete", SafeProgressReporter.MessageType.Success)
            // _progressReporter2.IndicateTaskEnded()
            // End Sub)
            // '
            // t.Start()
        }

        // Private Sub ReportMessage2(msg As SafeProgressReporter.MessageContentStruct)
        // ProgressExample2Textblock.Text = msg.message
        // If msg.msgType = SafeProgressReporter.MessageType.FatalError Then
        // ProgressExample2Textblock.Foreground = New SolidColorBrush(Colors.Red)
        // Else
        // ProgressExample2Textblock.Foreground = New SolidColorBrush(Colors.Black)
        // End If
        // End Sub

        /// <summary>
        /// (Disabled) Cancels Example 2 progress task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressExample2CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // _progressReporter2.RequestCancel()
        }

        /// <summary>
        /// Sets or updates the value of <c>>AutoPropControl.DefaultNumber</c> on button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestAutoButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.IsNaN(AutoPropControl.DefaultNumber))
            {
                this.AutoPropControl.DefaultNumber = 2.4274301210834244d;
            }
            else
            {
                this.AutoPropControl.DefaultNumber += 1;
            }

        }

        /// <summary>
        /// Displays a color picker popup bound to the clicked color rectangle's data context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle r = (Rectangle)sender;
            ColorItem ci = (ColorItem)r.DataContext;
            var p = new System.Windows.Controls.Primitives.Popup() { IsOpen = false, StaysOpen = false, AllowsTransparency = true, Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom, PlacementTarget = r };
            p.Closed += PopupClosed;

            var cp = new ColorPicker() { Width = 132, Height = 220, Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1d), Padding = new Thickness(2d) };

            BindingOperations.SetBinding(cp, ColorPicker.ColorProperty, new Binding(nameof(ColorItem.ColorTest)) { Source = ci, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });

            p.Child = cp;
            p.IsOpen = true;
        }

        /// <summary>
        /// Cleans up the color picker binding when the popup is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupClosed(object sender, EventArgs e)
        {
            System.Windows.Controls.Primitives.Popup p = (System.Windows.Controls.Primitives.Popup)sender;
            if (p.Child is null || p.Child.GetType() != typeof(ColorPicker))
                return;

            ColorPicker cp = (ColorPicker)p.Child;
            BindingOperations.ClearBinding(cp, ColorPicker.ColorProperty);
        }

        /// <summary>
        /// Handles theme radio button selection changes.
        /// </summary>
        /// <param name="sender">The radio button that was checked.</param>
        /// <param name="e">Event arguments.</param>
        private void ThemeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                Theme theme = Theme.Light;

                if (radioButton == LightThemeRadio)
                    theme = Theme.Light;
                else if (radioButton == BlueThemeRadio)
                    theme = Theme.Blue;
                else if (radioButton == DarkThemeRadio)
                    theme = Theme.Dark;

                ThemeService.Instance.SetTheme(theme);
            }
        }
    }
}