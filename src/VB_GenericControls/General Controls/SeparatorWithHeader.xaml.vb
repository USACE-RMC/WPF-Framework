Public Class SeparatorWithHeader

    Public Shared HeaderProperty As DependencyProperty = DependencyProperty.Register(NameOf(Header), GetType(String), GetType(SeparatorWithHeader), New UIPropertyMetadata(""))

    Public Property Header As String
        Get
            Return CStr(GetValue(HeaderProperty))
        End Get
        Set(ByVal value As String)
            SetValue(HeaderProperty, value)
        End Set
    End Property

    Public Shared LeftSeparatorWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(LeftSeparatorWidth), GetType(GridLength), GetType(SeparatorWithHeader), New UIPropertyMetadata(New GridLength(0.5, GridUnitType.Star)))

    Public Property LeftSeparatorWidth As GridLength
        Get
            Return CType(GetValue(LeftSeparatorWidthProperty), GridLength)
        End Get
        Set(ByVal value As GridLength)
            SetValue(LeftSeparatorWidthProperty, value)
        End Set
    End Property

    Public Shared RightSeparatorWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(RightSeparatorWidth), GetType(GridLength), GetType(SeparatorWithHeader), New UIPropertyMetadata(New GridLength(0.5, GridUnitType.Star)))

    Public Property RightSeparatorWidth As GridLength
        Get
            Return CType(GetValue(RightSeparatorWidthProperty), GridLength)
        End Get
        Set(ByVal value As GridLength)
            SetValue(RightSeparatorWidthProperty, value)
        End Set
    End Property

End Class
