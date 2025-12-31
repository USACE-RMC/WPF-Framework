Imports System.ComponentModel
Public MustInherit Class DataGridRowItem
    Implements INotifyPropertyChanged

#Region "Construction"

    ''' <summary>
    '''  Contructs a new data grid row item.
    ''' </summary>
    ''' <param name="list">The observable collection of all data grid row items.</param>
    ''' <param name="parentDataGrid">Optional. The parent validation data grid. Default = nothing.</param>
    Public Sub New(list As ObjectModel.ObservableCollection(Of Object), Optional parentDataGrid As ValidationDataGrid = Nothing)
        _parentList = list
        _parentDataGrid = parentDataGrid
        AddValidationRules()
        'this is required to ensure that each property has a rule so that the binding will not throw a key not found error.  this could be fixed by adding an attribute [validates] to each property, and only binding error state display to those columns.
        Dim pinfo As Reflection.PropertyInfo() = [GetType]().GetProperties()
        For Each p As Reflection.PropertyInfo In pinfo
            If p.Name = "RuleMap" Then Continue For
            If _ruleMap.ContainsKey(p.Name) = False Then AddRule(p.Name, Function() False, "")
        Next
    End Sub

#End Region

#Region "Members"

    Protected _ruleMap As New Dictionary(Of String, PropertyRule)()
    Protected _parentList As ObjectModel.ObservableCollection(Of Object)
    Protected _associatedProperties As New Dictionary(Of String, HashSet(Of String))
    Private _parentDataGrid As ValidationDataGrid
    Private _recurse As Boolean = True
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' The list that this row item is an item of. This is used for complex validation rules that require knowledege 
    ''' of neighbors or all other items in the list.
    ''' </summary>
    Public WriteOnly Property ParentList() As ObjectModel.ObservableCollection(Of Object)
        Set(value As ObjectModel.ObservableCollection(Of Object))
            _parentList = value
        End Set
    End Property

    ''' <summary>
    ''' The map of all property rules for this row item.
    ''' </summary>
    Public ReadOnly Property RuleMap() As Dictionary(Of String, PropertyRule)
        Get
            Return _ruleMap
        End Get
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' The required magic for defining when a property is in error. Use the "AddRule" call to add a specific rule.
    ''' </summary>
    Public MustOverride Sub AddValidationRules()

    ''' <summary>
    ''' Allows specification of prettier property names. Default return should be the property name, will appear as column header (unless modified), and as series name if curvedatagridrowitem is used.
    ''' </summary>
    ''' <param name="propertyName">The property that needs to be transformed into a better displayable name.</param>
    Public MustOverride Function PropertyDisplayName(ByVal propertyName As String) As String

    ''' <summary>
    ''' Allows specification of properties to not be displayed in a datagrid
    ''' </summary>
    ''' <param name="propertyName">Property name.</param>
    Public MustOverride Function IsGridDisplayable(ByVal propertyName As String) As Boolean

    ''' <summary>
    ''' Raise property changed event.
    ''' </summary>
    ''' <param name="propertyName">Optional. Name of the property that changed.</param>
    Protected Sub NotifyPropertyChanged(<Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        ValidateProperty(propertyName)
        'this simplifies the number of calls in the setter.. and sets up the default behavior.
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ''' <summary>
    ''' Allows each row item to have all properties update their error state. This is helpful when the grid is first displayed and when rows are added.
    ''' </summary>
    Public Sub ForceValidation()
        For Each propertyName As String In _ruleMap.Keys
            ValidateProperty(propertyName)
        Next
    End Sub

    ''' <summary>
    ''' Add a rule for a specific property. If the error condition is met then the cell of the defined property will turn red and the error message will show as a tooltip.
    ''' </summary>
    ''' <param name="propertyName">Property name that the rule will be applied to.</param>
    ''' <param name="errorCondition">The function that dictates an error has occurred or not. If the errorCondition returns true then the property for the given row will be assumed to have an error.</param>
    ''' <param name="errorMessage">The error message that will be shown in the tooltip when the errorCondition returns true.</param>
    ''' <param name="associatedProperties">Any properties that are associated with the target property. This guarantees proper updating when related properties are changed.</param>
    Protected Sub AddRule(propertyName As String, errorCondition As Func(Of Boolean), errorMessage As String, Optional ByVal associatedProperties() As String = Nothing)
        If _ruleMap.ContainsKey(propertyName) Then
            _ruleMap(propertyName).AddRule(errorCondition, errorMessage)
        Else
            _ruleMap.Add(propertyName, New PropertyRule(errorCondition, errorMessage))
        End If
        If IsNothing(associatedProperties) Then Exit Sub
        ' Update associated properties. The dictionary key is the associated property so that when validate gets called on an associated property it will update the target property.
        If _associatedProperties.ContainsKey(propertyName) = False Then _associatedProperties.Add(propertyName, New HashSet(Of String))
        For Each assProp As String In associatedProperties
            If _associatedProperties.ContainsKey(assProp) = False Then _associatedProperties.Add(assProp, New HashSet(Of String))
            _associatedProperties(assProp).Add(propertyName)
        Next
    End Sub

    ''' <summary>
    ''' Allows selective validation by property. This can be called in a setter with an empty argument to validate the property. 
    ''' By default, this happens in the inotifyproperty changed event. You can define the property name, if you wish for another 
    ''' property other than the one just being set to be validated as well.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to validate.</param>
    Public Sub ValidateProperty(propertyName As String)
        If _parentDataGrid IsNot Nothing AndAlso _parentDataGrid.SuppressValidation = True Then Exit Sub
        If _ruleMap.ContainsKey(propertyName) Then _ruleMap(propertyName).ExecuteRules()
        '
        If _associatedProperties.ContainsKey(propertyName) Then
            For Each _property As String In _associatedProperties(propertyName)
                _ruleMap(_property).ExecuteRules() 'the property is guaranteed to be in the rule map since it was added in the addrule sub.
            Next
        End If
    End Sub

    Protected Function UniqueRule(propertyName As String, errorMessage As String) As Boolean
        If _parentDataGrid IsNot Nothing AndAlso _parentDataGrid.SuppressValidation = True Then Exit Function
        If _parentDataGrid IsNot Nothing AndAlso _parentDataGrid.PerformingBulkValidation = True Then Exit Function
        Dim bool As Boolean = False
        If _parentList.Count >= 1 Then
            For i As Int32 = 0 To _parentList.Count - 1
                Dim iRowItem = CType(_parentList.Item(i), DataGridRowItem)
                Dim iValue As String = [GetType]().GetProperty(propertyName).GetValue(iRowItem, Nothing).ToString()
                Dim hasDuplicate As Boolean = False
                '
                ' Check for duplicates
                For j As Int32 = 0 To _parentList.Count - 1
                    Dim jRowItem = CType(_parentList.Item(j), DataGridRowItem)
                    Dim jValue As String = [GetType]().GetProperty(propertyName).GetValue(jRowItem, Nothing).ToString()
                    If jRowItem.Equals(iRowItem) Then Continue For
                    '
                    If iValue = jValue Then
                        hasDuplicate = True
                    End If
                Next
                '
                If iRowItem.Equals(Me) Then
                    bool = hasDuplicate
                    Continue For
                End If
                '
                ' If not ME, then update row item error boolean and error message.
                Dim hasError As Boolean = iRowItem.RuleMap().Item(propertyName).HasError
                Dim hasErrorMessage As List(Of String) = iRowItem.RuleMap().Item(propertyName).ErrorMessage.Split(CType(Environment.NewLine, Char())).ToList()
                For j As Int32 = hasErrorMessage.Count - 1 To 0 Step -1
                    If hasErrorMessage(j) = Environment.NewLine OrElse
                       hasErrorMessage(j) = "" OrElse
                       hasErrorMessage(j) = errorMessage Then hasErrorMessage.RemoveAt(j)
                Next
                If hasDuplicate = True Then
                    If hasError = False Then iRowItem.RuleMap().Item(propertyName).HasError = hasDuplicate
                    hasErrorMessage.Add(errorMessage)
                Else
                    ' Need to check if there was previously just 1 error, and if that error was this one.
                    If hasError = True AndAlso hasErrorMessage.Count = 0 Then
                        iRowItem.RuleMap().Item(propertyName).HasError = hasDuplicate
                    End If
                End If
                iRowItem.RuleMap().Item(propertyName).ErrorMessage = ""
                For j As Int32 = 0 To hasErrorMessage.Count - 1
                    iRowItem.RuleMap().Item(propertyName).ErrorMessage += If(j = 0, hasErrorMessage(j), Environment.NewLine + hasErrorMessage(j))
                Next
            Next
        End If
        '
        Return bool

        'Dim hasDups As Boolean = False
        'Dim val As String = [GetType]().GetProperty(propertyName).GetValue(Me, Nothing).ToString() 'callBack(DirectCast(Me, T))
        'For Each ex As DataGridRowItem In _parentList
        '    If ex.Equals(Me) Then Continue For

        '    If ex.GetType().GetProperty(propertyName).GetValue(ex, Nothing).ToString() = val Then
        '        hasDups = True
        '        With ex.RuleMap(propertyName)
        '            If .ErrorMessage = "" Then
        '                .ErrorMessage = errorMessage
        '            ElseIf .ErrorMessage.Contains(errorMessage) = False Then
        '                'ElseIf .ErrorMessage.IndexOf(errorMessage) < 0 Then
        '                'ElseIf .ErrorMessage.Contains(errorMessage) = False Then
        '                .ErrorMessage += Environment.NewLine & errorMessage
        '            End If
        '            .HasError = True
        '        End With
        '    Else
        '        If ex.RuleMap(propertyName).HasError And ex.RuleMap(propertyName).ErrorMessage.Contains(errorMessage) = True Then 'ex.RuleMap(propertyName).ErrorMessage.Contains(errorMessage) Then
        '            ex.ValidateProperty(propertyName) '.RuleMap(propertyName).Update()
        '        End If
        '    End If
        'Next
        'Return hasDups
    End Function

    Protected Function OrderRule(Of T As IComparable, DT As DataGridRowItem)(callBack As Func(Of DT, Double), propertyName As String, Optional ByVal ascending As Boolean = True, Optional ByVal canBeEqual As Boolean = True) As Boolean
        If _parentDataGrid IsNot Nothing AndAlso _parentDataGrid.SuppressValidation = True Then Exit Function
        If IsNothing(_parentList) Then Return False
        Dim currentIndex As Int32 = _parentList.IndexOf(Me)
        If currentIndex = -1 Then Return False
        Dim currentValue As Double = callBack(CType(Me, DT)) '_parentList(currentIndex).GetType().GetProperty(propertyName).GetValue(_parentList(currentIndex))
        Dim previousIndex As Int32 = currentIndex - 1
        Dim nextIndex As Int32 = currentIndex + 1
        '
        'value has changed so the next and previous row needs to check to make sure it is still ordered.
        If _recurse = True Then
            _recurse = False
            If previousIndex > 0 Then
                DirectCast(_parentList(previousIndex), DataGridRowItem)._recurse = False
                DirectCast(_parentList(previousIndex), DataGridRowItem).RuleMap(propertyName).ExecuteRules()
                DirectCast(_parentList(previousIndex), DataGridRowItem)._recurse = True
            End If
            If nextIndex < _parentList.Count Then
                DirectCast(_parentList(nextIndex), DataGridRowItem)._recurse = False
                DirectCast(_parentList(nextIndex), DataGridRowItem).RuleMap(propertyName).ExecuteRules()
                DirectCast(_parentList(nextIndex), DataGridRowItem)._recurse = True
            End If
            _recurse = True
        End If
        'Verify that the changed value is not smaller than the previous row value.
        If previousIndex >= 0 Then
            Dim previousValue As Double = callBack(CType(_parentList(previousIndex), DT))
            '
            If currentValue = previousValue Then
                If canBeEqual = False Then Return True
            Else
                If ascending = True Then
                    If currentValue < previousValue Then Return True
                Else
                    If currentValue > previousValue Then Return True
                End If
            End If
        End If
        '
        Return False
    End Function

#End Region
















    'Protected Function OrderRule(Of T As IComparable)(propertyName As String, Optional ByVal ascending As Boolean = True, Optional ByVal canBeEqual As Boolean = True) As Boolean
    '    If IsNothing(_parentList) Then Return False
    '    Dim currentIndex As Int32 = _parentList.IndexOf(Me)
    '    If currentIndex = -1 Then Return False
    '    Dim currentValue As T = CType(_parentList(currentIndex).GetType().GetProperty(propertyName).GetValue(_parentList(currentIndex)), T)
    '    Dim previousIndex As Int32 = currentIndex - 1
    '    Dim nextIndex As Int32 = currentIndex + 1
    '    'value has changed so the next row down needs to check to make sure it is still ordered.
    '    If nextIndex < _parentList.Count Then DirectCast(_parentList(nextIndex), DataGridRowItem).RuleMap(propertyName).Update()
    '    'Verify that the changed value is not smaller than the previous row value.
    '    If previousIndex >= 0 Then
    '        Dim previousValue As T = CType(_parentList(previousIndex).GetType().GetProperty(propertyName).GetValue(_parentList(previousIndex)), T)
    '        Dim compareValue As Integer = currentValue.CompareTo(previousValue)
    '        If ascending = True Then
    '            If compareValue < 0 Then Return True
    '        Else
    '            If compareValue > 0 Then Return True
    '        End If
    '        If canBeEqual = False Then If compareValue = 0 Then Return True
    '    End If
    '    '
    '    Return False
    'End Function
    'Protected Function OrderRule(Of T As IComparable, DT As DataGridRowItem)(callBack As Func(Of DT, T), Optional ByVal ascending As Boolean = True, Optional ByVal canBeEqual As Boolean = True) As Boolean
    '    If IsNothing(_parentList) Then Return False
    '    Dim currentIndex As Int32 = _parentList.IndexOf(Me)
    '    If currentIndex = -1 Then Return False
    '    Dim currentValue As T = callBack(CType(Me, DT)) '_parentList(currentIndex).GetType().GetProperty(propertyName).GetValue(_parentList(currentIndex))
    '    Dim previousIndex As Int32 = currentIndex - 1
    '    Dim nextIndex As Int32 = currentIndex + 1
    '    '
    '    'If ascending = False Then
    '    '    If previousIndex > -1 Then DirectCast(_parentList(previousIndex), DataGridRowItem).RuleMap(propertyName).Update()
    '    '    If nextIndex < _parentList.Count Then
    '    '        Dim nextValue As T = callBack(CType(_parentList(nextIndex), DT))
    '    '        Dim compareValue As Integer = currentValue.CompareTo(nextValue)
    '    '        If compareValue > 0 Then Return True
    '    '        If canBeEqual = False Then If compareValue = 0 Then Return True
    '    '    End If
    '    'Else
    '    'value has changed so the next row down needs to check to make sure it is still ordered.
    '    ' If nextIndex < _parentList.Count Then DirectCast(_parentList(nextIndex), DataGridRowItem).RuleMap(propertyName).Update()
    '    'Verify that the changed value is not smaller than the previous row value.
    '    If previousIndex >= 0 Then
    '        Dim previousValue As T = callBack(CType(_parentList(previousIndex), DT)) '_parentList(previousIndex).GetType().GetProperty(propertyName).GetValue(_parentList(previousIndex))
    '        Dim compareValue As Integer = currentValue.CompareTo(previousValue)
    '        If ascending = True Then
    '            If compareValue < 0 Then Return True
    '        Else
    '            If compareValue > 0 Then Return True
    '        End If

    '        If canBeEqual = False Then If compareValue = 0 Then Return True
    '    End If
    '    'End If
    '    If nextIndex < _parentList.Count Then
    '        Dim nextValue As T = callBack(CType(_parentList(nextIndex), DT))
    '        Dim compareValue As Integer = currentValue.CompareTo(nextValue)
    '        If ascending = True Then
    '            If compareValue > 0 Then Return True
    '        Else
    '            If compareValue < 0 Then Return True
    '        End If
    '        If canBeEqual = False Then If compareValue = 0 Then Return True
    '    End If
    '    '
    '    Return False
    'End Function



    'Protected Function MonotonicallyIncreasing(Of T As IComparable)(propertyName As String, message As String) As Boolean
    '    Dim idx As Int32 = _parentList.IndexOf(Me)
    '    Dim currval As T = _parentList(idx).GetType().GetProperty(propertyName).GetValue(_parentList(idx))
    '    Dim result As Boolean = False
    '    Dim prev As Int32 = -1
    '    Dim subsequent As Int32 = -1
    '    If idx > 0 Then
    '        prev += idx
    '        Dim prevval As T = _parentList(prev).GetType().GetProperty(propertyName).GetValue(_parentList(prev))
    '        If currval.CompareTo(prevval) <= 0 Then
    '            ''current value is not greater than previous value.
    '            result = True
    '            'currently not coloring the previous cell.
    '        Else
    '            Dim prevrow As DataGridRowItem = DirectCast(_parentList(prev), DataGridRowItem)
    '            If (prevrow.RuleMap(propertyName).HasError) Then
    '                If (prevrow.RuleMap(propertyName).ErrorMessage.Contains(message)) Then
    '                    prevrow.ValidateProperty(propertyName) ''this will recursively trigger until there are no errors with this error message...
    '                End If
    '            End If
    '        End If
    '    End If
    '    If idx < _parentList.Count - 1 Then
    '        subsequent = idx + 1
    '        Dim nextval As T = _parentList(subsequent).GetType().GetProperty(propertyName).GetValue(_parentList(subsequent))
    '        Dim compare As Int32 = nextval.CompareTo(currval)
    '        If compare <= 0 Then
    '            ''subsequent value is not greater than current value.
    '            result = True
    '            ''color the subsequent cells.
    '            Dim nextrow As DataGridRowItem = DirectCast(_parentList(subsequent), DataGridRowItem)
    '            nextrow.ValidateProperty(propertyName) ''this will recursively trigger until it is monotonically increasing.
    '        Else
    '            Dim nextrow As DataGridRowItem = DirectCast(_parentList(subsequent), DataGridRowItem)
    '            If (nextrow.RuleMap(propertyName).HasError) Then
    '                If (nextrow.RuleMap(propertyName).ErrorMessage.Contains(message)) Then
    '                    nextrow.ValidateProperty(propertyName) ''this will recursively trigger until there are no errors with this error message...
    '                End If
    '            End If
    '        End If
    '    End If
    '    Return result
    'End Function
    'Protected Function MonotonicallyDecreasing(Of T As IComparable)(propertyName As String, message As String) As Boolean
    '    Dim idx As Int32 = _parentList.IndexOf(Me)
    '    Dim currval As T = _parentList(idx).GetType().GetProperty(propertyName).GetValue(_parentList(idx))
    '    Dim result As Boolean = False
    '    Dim prev As Int32 = -1
    '    Dim subsequent As Int32 = -1
    '    If idx > 0 Then
    '        prev += idx
    '        Dim prevval As T = _parentList(prev).GetType().GetProperty(propertyName).GetValue(_parentList(prev))
    '        If currval.CompareTo(prevval) >= 0 Then
    '            ''current value is not less than previous value.
    '            result = True
    '            'currently not coloring the previous cell.
    '        Else
    '            Dim prevrow As DataGridRowItem = DirectCast(_parentList(prev), DataGridRowItem)
    '            If (prevrow.RuleMap(propertyName).HasError) Then
    '                If (prevrow.RuleMap(propertyName).ErrorMessage.Contains(message)) Then
    '                    prevrow.ValidateProperty(propertyName) ''this will recursively trigger until there are no errors with this error message...
    '                End If
    '            End If
    '        End If
    '    End If
    '    If idx < _parentList.Count - 1 Then
    '        subsequent = idx + 1
    '        Dim nextval As T = _parentList(subsequent).GetType().GetProperty(propertyName).GetValue(_parentList(subsequent))
    '        Dim compare As Int32 = nextval.CompareTo(currval)
    '        If compare >= 0 Then
    '            ''subsequent value is not less than current value.
    '            result = True
    '            ''color the subsequent cells.
    '            Dim nextrow As DataGridRowItem = DirectCast(_parentList(subsequent), DataGridRowItem)
    '            nextrow.ValidateProperty(propertyName) ''this will recursively trigger until it is monotonically decreasing.
    '        Else
    '            Dim nextrow As DataGridRowItem = DirectCast(_parentList(subsequent), DataGridRowItem)
    '            If (nextrow.RuleMap(propertyName).HasError) Then
    '                If (nextrow.RuleMap(propertyName).ErrorMessage.Contains(message)) Then
    '                    nextrow.ValidateProperty(propertyName) ''this will recursively trigger until there are no errors with this error message...
    '                End If
    '            End If
    '        End If
    '    End If
    '    Return result
    'End Function

End Class
