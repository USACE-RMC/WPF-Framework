Imports System.Globalization

Public Class LineStyleSelectorControl
    Public Shared ReadOnly Property LineStyleOptions As New List(Of DoubleCollection)({New DoubleCollection({0}),
                                                                                      New DoubleCollection(),
                                                                                      New DoubleCollection({4, 1}),
                                                                                      New DoubleCollection({1, 1}),
                                                                                      New DoubleCollection({4, 1, 1, 1}),
                                                                                      New DoubleCollection({4, 1, 4, 1, 1, 1}),
                                                                                      New DoubleCollection({4, 1, 1, 1, 1, 1}),
                                                                                      New DoubleCollection({4, 1, 4, 1, 1, 1, 1, 1}),
                                                                                      New DoubleCollection({10, 1}),
                                                                                      New DoubleCollection({10, 1, 1, 1}),
                                                                                      New DoubleCollection({10, 1, 1, 1, 1, 1})}
        )

    Public Shared SelectedDashArrayProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedDashArray), GetType(DoubleCollection), GetType(LineStyleSelectorControl), New UIPropertyMetadata(New DoubleCollection()))
    Public Property SelectedDashArray As DoubleCollection
        Get
            Return DirectCast(GetValue(SelectedDashArrayProperty), DoubleCollection)
        End Get
        Set(value As DoubleCollection)
            SetValue(SelectedDashArrayProperty, value)
        End Set
    End Property
    '
    Public Shared DashArrayOptionsProperty As DependencyProperty = DependencyProperty.Register(NameOf(DashArrayOptions), GetType(IList(Of DoubleCollection)), GetType(LineStyleSelectorControl), New FrameworkPropertyMetadata(LineStyleOptions))
    Public Property DashArrayOptions As IList(Of DoubleCollection)
        Get
            Return DirectCast(GetValue(DashArrayOptionsProperty), IList(Of DoubleCollection))
        End Get
        Set(value As IList(Of DoubleCollection))
            SetValue(DashArrayOptionsProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(LineStyleSelectorControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(LineStyleSelectorControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(LineStyleSelectorControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(LineStyleSelectorControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(LineStyleSelectorControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

End Class
Public Class DoubleCollectionConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim blank = LineStyleSelectorControl.LineStyleOptions(0)
        If IsNothing(value) Then Return blank
        If value.GetType <> GetType(DoubleCollection) Then Return blank
        '
        Dim selectedStyle = DirectCast(value, DoubleCollection)
        If selectedStyle.Count = 0 Then Return LineStyleSelectorControl.LineStyleOptions(1)
        For Each lineStyle In LineStyleSelectorControl.LineStyleOptions
            If lineStyle Is Nothing Then Continue For
            If lineStyle.SequenceEqual(selectedStyle) Then Return lineStyle
        Next
        '
        Return blank
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return New DoubleCollection()
        If value.GetType <> GetType(DoubleCollection) Then Return New DoubleCollection()
        Return DirectCast(value, DoubleCollection).Clone
    End Function
End Class




