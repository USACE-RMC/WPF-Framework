
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports GenericControls

Class MainWindow
    Implements INotifyPropertyChanged


    Private _textProperty As String
    Private _fontProperty As String
    Private _testFontWeightProperty As FontWeight
    Private _booleanProperty As Boolean
    Private _numericSelectorProperty As Double
    Private _numericProperty As Double
    Private _lineStyleProperty As DoubleCollection
    Private _lineWidthProperty As Double
    Private _colorProperty As SolidColorBrush
    Private _testHorizontalAlignmentProperty As HorizontalAlignment
    Private _calendarWeekProperty As CalendarWeekRule
    Private _stringListProperty As IList(Of String)

    Public Shared TestGridLengthProperty As DependencyProperty = DependencyProperty.Register(NameOf(TestGridLength), GetType(GridLength), GetType(MainWindow), New FrameworkPropertyMetadata(New GridLength(100)))

    Public Property TestGridLength As GridLength
        Get
            Return GetValue(TestGridLengthProperty)
        End Get
        Set(value As GridLength)
            SetValue(TestGridLengthProperty, value)
        End Set
    End Property

    Public Property TextProperty As String
        Get
            Return _textProperty
        End Get
        Set(value As String)
            _textProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TextProperty)))
        End Set
    End Property

    Public Property FontProperty As String
        Get
            Return _fontProperty
        End Get
        Set(value As String)
            _fontProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FontProperty)))

        End Set
    End Property

    Public Property TestFontWeightProperty As FontWeight
        Get
            Return _testFontWeightProperty
        End Get
        Set(value As FontWeight)
            _testFontWeightProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TestFontWeightProperty)))

        End Set
    End Property

    Public Property BooleanProperty As Boolean
        Get
            Return _booleanProperty
        End Get
        Set(value As Boolean)
            _booleanProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BooleanProperty)))
        End Set
    End Property

    Public Property NumericSelectorProperty As Double
        Get
            Return _numericSelectorProperty
        End Get
        Set(value As Double)
            _numericSelectorProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NumericSelectorProperty)))
        End Set
    End Property

    Public Property NumericProperty As Double
        Get
            Return _numericProperty
        End Get
        Set(value As Double)
            _numericProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NumericProperty)))
        End Set
    End Property

    Public Property LineStyleProperty As DoubleCollection
        Get
            Return _lineStyleProperty
        End Get
        Set(value As DoubleCollection)
            _lineStyleProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LineStyleProperty)))
        End Set
    End Property

    Public Property LineWidthProperty As Double
        Get
            Return _lineWidthProperty
        End Get
        Set(value As Double)
            _lineWidthProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LineWidthProperty)))
        End Set
    End Property

    Public Property ColorProperty As SolidColorBrush
        Get
            Return _colorProperty
        End Get
        Set(value As SolidColorBrush)
            _colorProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ColorProperty)))
        End Set
    End Property

    Public Property TestHorizontalAlignmentProperty As HorizontalAlignment
        Get
            Return _testHorizontalAlignmentProperty
        End Get
        Set(value As HorizontalAlignment)
            _testHorizontalAlignmentProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TestHorizontalAlignmentProperty)))
        End Set
    End Property

    Public Property CalendarWeekProperty As CalendarWeekRule
        Get
            Return _calendarWeekProperty
        End Get
        Set(value As CalendarWeekRule)
            _calendarWeekProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalendarWeekProperty)))
        End Set
    End Property

    Public Property StringListProperty As IList(Of String)
        Get
            Return _stringListProperty
        End Get
        Set(value As IList(Of String))
            _stringListProperty = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StringListProperty)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    'Private _progressReporter1 As SafeProgressReporter
    'Private _progressReporter2 As SafeProgressReporter
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


        DataContext = Me


        Dim cats As New ObservableCollection(Of Object)
        cats.Add(New Cat() With {.Color = "Orange", .Age = "3", .Sex = "Male"})
        cats.Add(New Cat() With {.Color = "Gray", .Age = "12", .Sex = "Female"})
        cats.Add(New Cat() With {.Color = "White", .Age = "5", .Sex = "Male"})
        CPDataGrid.ItemsSource = cats
        CPDataGrid2.ItemsSource = cats
        'Properties

        TextProperty = "Sample Text"
        FontProperty = "Segoe UI"
        TestFontWeightProperty = FontWeights.Bold
        BooleanProperty = True
        NumericSelectorProperty = 12
        NumericProperty = Double.MaxValue '123.456
        LineStyleProperty = LineStyleSelectorControl.LineStyleOptions(0)
        LineWidthProperty = 4
        ColorProperty = New SolidColorBrush(Color.FromArgb(150, 105, 205, 125))
        TestHorizontalAlignmentProperty = HorizontalAlignment.Center

        Dim t As New List(Of ColorItem)
        t.Add(New ColorItem() With {.ColorTest = Brushes.Red, .Name = "Red"})
        t.Add(New ColorItem() With {.ColorTest = Brushes.Green, .Name = "Green"})
        t.Add(New ColorItem() With {.ColorTest = Brushes.Blue, .Name = "Blue"})

        ColorItemsControl.ItemsSource = t
    End Sub

    ''' <summary>
    ''' When the mouse enters the datagrid set the popup to staysopen=true to allow the datagrid to keep capture of the mouse and give the datagrid the focus.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CPDataGrid2_MouseEnter(sender As Object, e As MouseEventArgs)
        DataGridPopup.StaysOpen = True
        CPDataGrid2.Focus()
    End Sub

    ''' <summary>
    ''' When the mouse leaves the datagrid set the popup to staysopen=false and give it focus so that clicking outside the popup will close it.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CPDataGrid2_MouseLeave(sender As Object, e As MouseEventArgs)
        DataGridPopup.StaysOpen = False
        DataGridPopup.Focus()
    End Sub

    ''' <summary>
    ''' When the context menu closes the focus gets all out of whack in the popup and needs to be reset. Maybe because it is a popup on a popup? WPF Inception
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CPDataGrid2_ContextMenuClosing(sender As Object, e As ContextMenuEventArgs)
        DataGridPopup.IsOpen = False
        DataGridPopup.IsOpen = True
    End Sub

    Private Sub ControlGrid_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        HeaderColumn.MaxWidth = HeaderColumn.ActualWidth + PropertyWidthColumn.ActualWidth - 22
    End Sub

    Public Class Cat
        Public Property Color As String
        Public Property Age As String
        Public Property Sex As String
        Public Sub New()

        End Sub
    End Class

    Private Sub HorizontalControl_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Debug.Print("Hooyaa!")
    End Sub

    Private Sub CPDataGrid_AutoGeneratedColumns(sender As Object, e As EventArgs)
        For Each column In CPDataGrid.Columns
            Dim propertyName As String = CType(CType(column, DataGridTextColumn).Binding, Binding).Path.Path.ToString

            Debug.Print(propertyName)
        Next
    End Sub

    Private Sub ProgressExample1Button_Click(sender As Object, e As RoutedEventArgs)
        'ProgressExample2ProgressBar.Foreground = New SolidColorBrush(Color.FromArgb(255, 6, 176, 37))
        '_progressReporter1 = New SafeProgressReporter("Example 1")
        'AddHandler _progressReporter1.ProgressReported, AddressOf ReportProgress1
        'AddHandler _progressReporter1.MessageReported, AddressOf ReportMessage1
        'AddHandler _progressReporter1.TaskEnded, Sub()
        '                                             If _progressReporter1.CancelRequested Then
        '                                                 Dispatcher.Invoke(New Action(Sub() ProgressExample1Textblock.Text = "Progress Canceled."))
        '                                                 ProgressExample1ProgressBar.Value = 0
        '                                             Else
        '                                                 ProgressExample1ProgressBar.Foreground = Brushes.LightBlue
        '                                             End If
        '                                             '
        '                                             ProgressExample1CancelButton.IsEnabled = False
        '                                         End Sub
        ''
        'ProgressExample1CancelButton.IsEnabled = True
        ''
        'Dim t As New Task(Sub()
        '                      _progressReporter1.IndicateTaskStart()
        '                      Dim totalSteps As Int32 = 100
        '                      For i As Int32 = 1 To totalSteps
        '                          If _progressReporter1.CancelRequested Then
        '                              _progressReporter1.IndicateTaskEnded()
        '                              Exit Sub
        '                          End If
        '                          '
        '                          System.Threading.Thread.Sleep(100)
        '                          _progressReporter1.Report(i / totalSteps, (100 * i / totalSteps).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
        '                      Next
        '                      _progressReporter1.Report(100, "Complete", SafeProgressReporter.MessageType.Success)
        '                      _progressReporter1.IndicateTaskEnded()
        '                  End Sub)
        ''
        't.Start()
    End Sub

    'Private Sub ReportMessage1(msg As SafeProgressReporter.MessageContentStruct)
    '    ProgressExample1Textblock.Text = msg.message
    '    If msg.msgType = SafeProgressReporter.MessageType.FatalError Then
    '        ProgressExample1Textblock.Foreground = New SolidColorBrush(Colors.Red)
    '    Else
    '        ProgressExample1Textblock.Foreground = New SolidColorBrush(Colors.Black)
    '    End If
    'End Sub

    'Private Sub ReportProgress1(reporter As SafeProgressReporter, prog As Double, progDelta As Double)
    '    ProgressExample1ProgressBar.Value = prog
    'End Sub
    Private Sub ProgressExample1CancelButton_Click(sender As Object, e As RoutedEventArgs)
        '    _progressReporter1.RequestCancel()
    End Sub


    Private Sub ProgressExample2Button_Click(sender As Object, e As RoutedEventArgs)
        '    ProgressExample2ProgressBar.Foreground = New SolidColorBrush(Color.FromArgb(255, 6, 176, 37))
        '    _progressReporter2 = New SafeProgressReporter("Example 1")
        '    AddHandler _progressReporter2.ProgressReported, Sub(reporter As SafeProgressReporter, prog As Double, progDelta As Double) ProgressExample2ProgressBar.Value = prog
        '    AddHandler _progressReporter2.MessageReported, AddressOf ReportMessage2
        '    AddHandler _progressReporter2.TaskEnded, Sub()
        '                                                 If _progressReporter2.CancelRequested Then
        '                                                     Dispatcher.Invoke(New Action(Sub() ProgressExample2Textblock.Text = "Progress Canceled."))
        '                                                     ProgressExample2ProgressBar.Value = 0
        '                                                 Else
        '                                                     ProgressExample2ProgressBar.Foreground = Brushes.LightBlue
        '                                                 End If
        '                                                 '
        '                                                 ProgressExample2CancelButton.IsEnabled = False
        '                                             End Sub
        '    '
        '    ProgressExample2CancelButton.IsEnabled = True
        '    '
        '    Dim t As New Task(Sub()
        '                          _progressReporter2.IndicateTaskStart()

        '                          Dim subReporter1 = _progressReporter2.CreateProgressModifier(0.5, "Set 1")
        '                          'new thread 1
        '                          Dim t1 As New Task(Sub()
        '                                                 subReporter1.IndicateTaskStart()
        '                                                 Dim totalSteps As Int32 = 100
        '                                                 For i As Int32 = 1 To totalSteps
        '                                                     If subReporter1.CancelRequested Then
        '                                                         subReporter1.IndicateTaskEnded()
        '                                                         Exit Sub
        '                                                     End If
        '                                                     '
        '                                                     System.Threading.Thread.Sleep(50)
        '                                                     subReporter1.ReportProgress(i / totalSteps)
        '                                                     subReporter1.ReportMessage(CInt(50 * i / totalSteps).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
        '                                                 Next
        '                                                 subReporter1.IndicateTaskEnded()
        '                                             End Sub)
        '                          '
        '                          t1.Start()
        '                          t1.Wait()

        '                          'two is slightly faster for example purposes when you have to estimate percent of total progress a task will take.
        '                          Dim subReporter2 = _progressReporter2.CreateProgressModifier(0.5, "Set 2")
        '                          Dim t2 As New Task(Sub()
        '                                                 subReporter2.IndicateTaskStart()
        '                                                 Dim totalSteps As Int32 = 100
        '                                                 For i As Int32 = 1 To totalSteps
        '                                                     If subReporter2.CancelRequested Then
        '                                                         subReporter2.IndicateTaskEnded()
        '                                                         Exit Sub
        '                                                     End If
        '                                                     '
        '                                                     System.Threading.Thread.Sleep(40)
        '                                                     subReporter2.ReportProgress(i / totalSteps)
        '                                                     subReporter2.ReportMessage(CInt(50 + (50 * i / totalSteps)).ToString("G4", CultureInfo.InvariantCulture) & "% Complete", SafeProgressReporter.MessageType.Status)
        '                                                 Next
        '                                                 subReporter2.IndicateTaskEnded()
        '                                             End Sub)
        '                          '
        '                          t2.Start()
        '                          t2.Wait()
        '                          '
        '                          _progressReporter2.Report(100, "Complete", SafeProgressReporter.MessageType.Success)
        '                          _progressReporter2.IndicateTaskEnded()
        '                      End Sub)
        '    '
        '    t.Start()
    End Sub

    'Private Sub ReportMessage2(msg As SafeProgressReporter.MessageContentStruct)
    '    ProgressExample2Textblock.Text = msg.message
    '    If msg.msgType = SafeProgressReporter.MessageType.FatalError Then
    '        ProgressExample2Textblock.Foreground = New SolidColorBrush(Colors.Red)
    '    Else
    '        ProgressExample2Textblock.Foreground = New SolidColorBrush(Colors.Black)
    '    End If
    'End Sub

    Private Sub ProgressExample2CancelButton_Click(sender As Object, e As RoutedEventArgs)
        '    _progressReporter2.RequestCancel()
    End Sub

    Private Sub TestAutoButton_Click(sender As Object, e As RoutedEventArgs)
        If Double.IsNaN(AutoPropControl.DefaultNumber) Then
            AutoPropControl.DefaultNumber = 2.4274301210834244
        Else
            AutoPropControl.DefaultNumber += 1
        End If

    End Sub

    Private Sub ColorRectangle_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim r As Rectangle = DirectCast(sender, Rectangle)
        Dim ci As ColorItem = r.DataContext
        Dim p As New Primitives.Popup() With {.IsOpen = False, .StaysOpen = False, .AllowsTransparency = True, .Placement = Primitives.PlacementMode.Bottom, .PlacementTarget = r}
        AddHandler p.Closed, AddressOf PopupClosed

        Dim cp As New Color_Picker() With {.Width = 132, .Height = 220, .Background = Brushes.White, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1), .Padding = New Thickness(2)}

        BindingOperations.SetBinding(cp, Color_Picker.ColorProperty, New Binding(NameOf(ColorItem.ColorTest)) With {.Source = ci, .UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .Mode = BindingMode.TwoWay})

        p.Child = cp
        p.IsOpen = True
    End Sub

    Private Sub PopupClosed(sender As Object, e As EventArgs)
        Dim p As Primitives.Popup = DirectCast(sender, Primitives.Popup)
        If p.Child Is Nothing OrElse p.Child.GetType <> GetType(Color_Picker) Then Exit Sub

        Dim cp = DirectCast(p.Child, Color_Picker)
        BindingOperations.ClearBinding(cp, Color_Picker.ColorProperty)
    End Sub
End Class