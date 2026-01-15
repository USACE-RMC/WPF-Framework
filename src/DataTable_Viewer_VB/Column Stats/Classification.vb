Imports MS.Internal
Imports Numerics

Public Module Classification

    ''' <summary>
    ''' Determines classification range break values into equally sized intervals.
    ''' </summary>
    ''' <param name="data">The data to be classified.</param>
    ''' <param name="nClasses">The number of classes to determine break values.</param>
    ''' <param name="dataIsSorted">Boolean value indicating if the data Is sorted in ascending order.</param>
    ''' <returns>An array of upper bound break values.</returns>
    Public Function EqualInterval(data As Double(), nClasses As Integer, dataIsSorted As Boolean) As Double()
        If nClasses <= 0 Then Return New Double(-1) {}
        If (data Is Nothing OrElse data.Count() = 0) Then Return New Double(-1) {}
        If data.Count = 1 Then Return New Double() {data(0)}
        ' Sort the data if it needs to be sorted.
        Dim sortedData = data
        If dataIsSorted = False Then
            sortedData = data.ToArray()
            Array.Sort(sortedData)
        End If

        If Double.IsInfinity(sortedData(0)) OrElse Double.IsNaN(sortedData(0)) Then

            For i As Integer = 1 To sortedData.Length - 1
                If Double.IsInfinity(sortedData(i)) = False AndAlso Double.IsNaN(sortedData(i)) = False Then Return EqualInterval(sortedData(i), sortedData.Last(), nClasses)
            Next

            Return Array.Empty(Of Double)()
        Else
            Return EqualInterval(sortedData(0), sortedData.Last(), nClasses)
        End If
    End Function

    ''' <summary>
    ''' Determines classification range break values into equally sized intervals. 
    ''' </summary>
    ''' <param name="minValue">Minimum value for determining classification.</param>
    ''' <param name="maxValue">Maximum value for determining classification.</param>
    ''' <param name="nClasses">The number of classes to determine break values for.</param>
    ''' <returns></returns>
    Public Function EqualInterval(minValue As Double, maxValue As Double, nClasses As Integer) As Double()
        If nClasses <= 0 Then Return Array.Empty(Of Double)()
        If nClasses = 1 Then Return {maxValue}
        Dim result = New Double(nClasses - 1) {}
        Dim multiplier As Double = (maxValue - minValue) / nClasses

        For i As Integer = 1 To nClasses
            result(i - 1) = minValue + i * multiplier
        Next

        result(nClasses - 1) = maxValue
        Return result
    End Function

    ''' <summary>
    ''' Determines classification range break values for a predefined interval.
    ''' </summary>
    ''' <param name="data">The data to be classified.</param>
    ''' <param name="intervalSize">The size of the interval to classify break values.</param>
    ''' <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
    ''' <returns>An array of upper bound break values.</returns>
    Public Function DefinedInterval(data As IList(Of Double), intervalSize As Double, dataIsSorted As Boolean) As Double()
        If (data Is Nothing OrElse data.Count() = 0) Then Return New Double(-1) {}
        If data.Count = 1 Then Return New Double() {data(0)}
        If dataIsSorted Then Return DefinedInterval(data(0), data.Last(), intervalSize)

        Dim sortedData = data.ToArray()
        Array.Sort(sortedData)
        Return DefinedInterval(sortedData(0), sortedData.Last(), intervalSize)
    End Function

    ''' <summary>
    ''' Determines classification range break values for a predefined interval.
    ''' </summary>
    ''' <param name="minValue">Minimum value for determining classification.</param>
    ''' <param name="maxValue">Maximum value for determining classification.</param>
    ''' <param name="intervalSize">The size of the interval to classify break values.</param>
    ''' <returns>An array of upper bound break values.</returns>
    Public Function DefinedInterval(minValue As Double, maxValue As Double, intervalSize As Double) As Double()
        If intervalSize <= 0 OrElse Double.IsNaN(intervalSize) Then Return Array.Empty(Of Double)()
        Dim nBreaks As Integer = CInt(Math.Floor((maxValue - minValue) / intervalSize))
        If Math.Abs((maxValue - minValue) / intervalSize - nBreaks) <= Tools.DoubleMachineEpsilon Then nBreaks -= 1
        Dim results = New Double(nBreaks + 1 - 1) {}

        For i As Integer = 0 To nBreaks - 1
            results(i) = minValue + intervalSize * (i + 1)
        Next

        results(nBreaks) = maxValue
        Return results
    End Function


    Public Function Quantiles(data As IList(Of Double), nClasses As Integer, dataIsSorted As Boolean) As Double()
        If nClasses <= 0 Then Return New Double(-1) {}
        If (data Is Nothing OrElse data.Count() = 0) Then Return New Double(-1) {}
        If data.Count = 1 Then Return New Double() {data(0)}

        If nClasses > data.Count Then
            If dataIsSorted = True Then Return data.ToArray()
            Dim sortedData = data.ToArray()
            Array.Sort(sortedData)
            Return sortedData
        End If

        If nClasses = 1 Then Return If(dataIsSorted = True, (New Double() {data.Last()}), (New Double() {data.Max()}))
        Dim countPerBin As Double = data.Count / CDbl(nClasses)
        Dim results = New Double(nClasses - 1) {}

        If dataIsSorted Then

            For i As Integer = 1 To nClasses - 1
                results(i - 1) = data(Convert.ToInt32(countPerBin * i - 1))
            Next

            results(nClasses - 1) = data(data.Count - 1)
        Else
            Dim sortedData = data.ToArray()
            Array.Sort(sortedData)

            For i As Integer = 1 To nClasses - 1
                results(i - 1) = sortedData(Convert.ToInt32(countPerBin * i - 1))
            Next

            results(nClasses - 1) = sortedData(sortedData.Length - 1)
        End If

        Return results
    End Function

    Public Function HeadTailInterval(data As Double(), dataIsSorted As Boolean, Optional threshold As Double = 0.4R) As Double()
        If (data Is Nothing) Then Return Array.Empty(Of Double)()
        If data.Length = 0 Then Return Array.Empty(Of Double)()
        If data.Length = 1 Then Return {data(0)}
        Dim sortedData = data

        If dataIsSorted = False Then
            sortedData = data.ToArray()
            Array.Sort(sortedData)
        End If

        Dim avg As Double = sortedData.Average()
        Dim results = New List(Of Double)() From {
            avg
        }
        Dim initialSplit As Integer = Array.BinarySearch(sortedData, avg)
        If initialSplit < 0 Then initialSplit = -1 * initialSplit - 1
        Dim splitIndex As Integer = initialSplit
        Dim previousCount As Integer = sortedData.Length

        While (sortedData.Length - splitIndex) / CDbl(previousCount) <= threshold OrElse (sortedData.Length - splitIndex <= 1)
            avg = 0R

            For i As Integer = splitIndex To sortedData.Length - 1
                avg += sortedData(i)
            Next

            avg = avg / (sortedData.Length - splitIndex)
            results.Add(avg)
            previousCount = sortedData.Length - splitIndex
            splitIndex = Array.BinarySearch(sortedData, splitIndex, sortedData.Length - splitIndex, avg)
            If splitIndex < 0 Then splitIndex = -1 * splitIndex - 1
            If sortedData.Length - splitIndex = previousCount Then Exit While
        End While

        If results.Last() <> sortedData.Last() Then results.Add(sortedData(sortedData.Length - 1))
        results.Sort()
        Return results.ToArray()
    End Function

    Public Function StandardDeviationInterval(data As IList(Of Double), nDeviations As Double, dataIsSorted As Boolean) As Double()
        If nDeviations <= 0R Then Return Array.Empty(Of Double)()
        If (data Is Nothing) Then Return Array.Empty(Of Double)()
        If data.Count = 0 Then Return Array.Empty(Of Double)()
        If data.Count = 1 Then Return {data(0)}
        Dim sortedData = data

        If dataIsSorted = False Then
            sortedData = data.ToArray()
            Array.Sort(CType(sortedData, Double()))
        End If

        Dim summaryData As Double = Numerics.Data.Statistics.Statistics.PopulationStandardDeviation(sortedData)
        Return StandardDeviationInterval(sortedData.Average(), summaryData, sortedData(0), sortedData(sortedData.Count - 1), nDeviations)
    End Function

    Public Function StandardDeviationInterval(mean As Double, standardDeviation As Double, minValue As Double, maxValue As Double, nDeviations As Double) As Double()
        If nDeviations <= 0R Then Return Array.Empty(Of Double)()
        If minValue = maxValue Then Return {maxValue}
        If Double.IsNaN(mean) Or Double.IsNaN(standardDeviation) Then Return {maxValue}
        Dim results = New List(Of Double)()
        Dim break As Double = mean - standardDeviation * nDeviations / 2.0R
        If break > minValue Then results.Add(break)

        While break >= minValue
            break = break - nDeviations * standardDeviation
            If break > minValue Then results.Add(break)
        End While

        break = mean + standardDeviation * nDeviations / 2.0R
        If break < maxValue Then results.Add(break)

        While break <= maxValue
            break = break + nDeviations * standardDeviation
            If break < maxValue Then results.Add(break)
        End While

        results.Add(maxValue)
        results.Sort()
        Return results.ToArray()
    End Function

    Public Function JenksNaturalBreaks(ByVal data As Double(), ByVal nClasses As Integer, ByVal dataIsSorted As Boolean, ByRef breakCounts As Integer()) As Double()
        If nClasses <= 0 Then Return New Double(-1) {}
        If data Is Nothing Then Return New Double(-1) {}
        If data.Length = 0 Then Return New Double(-1) {}
        If data.Length = 1 Then Return New Double() {data(0)}

        Dim sortedData = data
        If dataIsSorted = False Then
            sortedData = data.ToArray()
            Array.Sort(sortedData)
        End If

        If sortedData.Length <= nClasses Then
            If (breakCounts Is Nothing OrElse breakCounts.Length <> sortedData.Length) Then breakCounts = New Integer(sortedData.Length) {}
            For i As Integer = 0 To breakCounts.Length - 1
                breakCounts(i) = 1
            Next

            Return sortedData
        End If

        Dim squaredValues = New Double(sortedData.Length - 1) {}

        For i As Integer = 0 To sortedData.Length - 1
            squaredValues(i) = sortedData(i) * sortedData(i)
        Next

        Dim avg As Double = sortedData.Average()
        Dim SDAM As Double = 0

        For i As Integer = 0 To sortedData.Length - 1
            SDAM += Math.Pow(sortedData(i) - avg, 2.0)
        Next

        ' Initiate Breaks
        If (nClasses > sortedData.Length) Then nClasses = sortedData.Length
        If (nClasses = 1) Then
            breakCounts = New Integer() {sortedData.Length}
            Return New Double() {sortedData(sortedData.Length - 1)}
        End If

        ' calculate initial classes.
        Dim classes(nClasses - 1) As JenksBreak
        For i As Integer = 0 To nClasses - 1
            classes(i) = New JenksBreak()
        Next


        Dim distinctValues = sortedData.Distinct().ToArray()
        If distinctValues.Length <= 2 Then
            breakCounts = New Integer() {sortedData.Length - 1}
            Return New Double() {sortedData(sortedData.Length - 1)}
        End If

        If distinctValues.Length <= nClasses Then
            If breakCounts Is Nothing OrElse breakCounts.Length <> distinctValues.Length Then breakCounts = New Integer(distinctValues.Length) {}
            Dim binIdx As Integer = 0
            For i As Integer = 0 To sortedData.Length - 1
                If (sortedData(i) <= distinctValues(binIdx)) Then
                    breakCounts(binIdx) += 1
                Else

                    i -= 1
                    binIdx += 1
                End If
            Next
            Return distinctValues
        End If

        Dim classIdx As Integer = 0
        Dim lastClassIdx As Integer = -1

        If distinctValues.Length <> sortedData.Length Then
            Dim argbreakCounts As Integer() = Nothing
            Dim initialBreaks = JenksNaturalBreaks(distinctValues, nClasses, True, breakCounts:=argbreakCounts)

            For i As Integer = 0 To sortedData.Length - 1

                If sortedData(i) > initialBreaks(classIdx) Then
                    classIdx += 1
                    If classIdx > classes.Length Then classIdx = classes.Length - 1
                End If

                classes(classIdx).Sum += sortedData(i)
                classes(classIdx).SumofSquares += squaredValues(i)

                If classIdx <> lastClassIdx Then
                    classes(classIdx).StartIdx = i
                    lastClassIdx = classIdx
                    If classIdx > 0 Then classes(classIdx - 1).EndIdx = i - 1
                End If
            Next
        Else
            Dim classCount As Double = sortedData.Length / CDbl(nClasses)

            For i As Integer = 0 To sortedData.Length - 1
                classIdx = Convert.ToInt32(i / classCount)
                If classIdx > classes.Length - 1 Then classIdx = classes.Length - 1
                classes(classIdx).Sum += sortedData(i)
                classes(classIdx).SumofSquares += squaredValues(i)

                If classIdx <> lastClassIdx Then
                    classes(classIdx).StartIdx = i
                    lastClassIdx = classIdx
                    If classIdx > 0 Then classes(classIdx - 1).EndIdx = i - 1
                End If
            Next
        End If

        classes(nClasses - 1).EndIdx = sortedData.Length - 1
        For i As Integer = 0 To nClasses - 1
            classes(i).RefreshSquareDeviation()
        Next

        ' Optimize
        JenksOptimize(classes, sortedData, squaredValues, 0, classes.Length - 1, SDAM)
        ' Break range counts
        If breakCounts Is Nothing OrElse breakCounts.Length <> distinctValues.Length Then breakCounts = New Integer(classes.Length - 1) {}

        For i As Integer = 0 To classes.Length - 1
            breakCounts(i) = classes(i).Count
        Next

        ' Break upper bounds
        Dim result = New Double(classes.Length - 1) {}
        For i As Integer = 0 To classes.Length - 1
            result(i) = sortedData(classes(i).EndIdx)
        Next
        Return result
    End Function
    Private Sub JenksOptimize(ByVal classes As JenksBreak(), ByVal sortedData As Double(), ByVal squaredValues As Double(), ByVal leftIndex As Integer, ByVal rightIndex As Integer, ByVal SDAM As Double)
        Dim minValue As Double = 0R
        Dim proceed As Boolean = True
        Dim previousFromIndex As Integer = -1
        Dim previousToIndex As Integer = -1

        While proceed
            Dim fromIndex As Integer = 1
            Dim toIndex As Integer = -1
            Dim minSDCM As Double = Double.MaxValue
            Dim SDCM As Double

            For i As Integer = leftIndex To rightIndex - 1
                If classes(i).Count = 1 Then Continue For
                MakeShift(classes, sortedData, squaredValues, i, i + 1, 1)
                SDCM = GetSumSquaredDeviations(classes, leftIndex, rightIndex)

                If SDCM < minSDCM Then
                    fromIndex = i
                    toIndex = i + 1
                    minSDCM = SDCM
                End If

                MakeShift(classes, sortedData, squaredValues, i + 1, i, 1)
            Next

            For i As Integer = rightIndex To leftIndex + 1
                If classes(i).Count = 1 Then Continue For
                MakeShift(classes, sortedData, squaredValues, i, i - 1, 1)
                SDCM = GetSumSquaredDeviations(classes, leftIndex, rightIndex)

                If SDCM < minSDCM Then
                    fromIndex = i
                    toIndex = i - 1
                    minSDCM = SDCM
                End If

                MakeShift(classes, sortedData, squaredValues, i - 1, i, 1)
            Next

            If minSDCM = Double.MaxValue Then Exit While
            MakeShift(classes, sortedData, squaredValues, fromIndex, toIndex, 1)
            Dim GVF As Double = (SDAM - GetSumSquaredDeviations(classes)) / SDAM

            If GVF > minValue AndAlso previousFromIndex <> toIndex And previousToIndex <> fromIndex Then
                minValue = GVF
                proceed = True
            Else
                MakeShift(classes, sortedData, squaredValues, toIndex, fromIndex, 1)
                Dim lowerIndex As Integer, upperIndex As Integer

                If toIndex > fromIndex Then
                    lowerIndex = fromIndex
                    upperIndex = toIndex
                ElseIf toIndex < fromIndex Then
                    lowerIndex = toIndex
                    upperIndex = fromIndex
                Else
                End If

                If lowerIndex > leftIndex Then JenksOptimize(classes, sortedData, squaredValues, leftIndex, lowerIndex, SDAM)
                If upperIndex < rightIndex Then JenksOptimize(classes, sortedData, squaredValues, upperIndex, rightIndex, SDAM)
                proceed = False
            End If

            previousFromIndex = fromIndex
            previousToIndex = toIndex
        End While
    End Sub

    Private Sub MakeShift(ByVal classes As JenksBreak(), ByVal sortedData As Double(), ByVal squaredValues As Double(), ByVal currentClassIndex As Integer, ByVal targetId As Integer, ByVal Optional nShifts As Integer = 1)
        Dim dataIndex As Integer

        For i As Integer = 1 To nShifts

            If targetId < currentClassIndex Then
                If classes(currentClassIndex).StartIdx < sortedData.Length - 1 Then classes(currentClassIndex).StartIdx += 1
                If classes(targetId).EndIdx < sortedData.Length - 1 Then classes(targetId).EndIdx += 1
                dataIndex = classes(targetId).EndIdx
                If dataIndex = -1 Then Return
                classes(currentClassIndex).Sum -= sortedData(dataIndex)
                classes(currentClassIndex).SumofSquares -= squaredValues(dataIndex)
                classes(currentClassIndex).RefreshSquareDeviation()
                classes(targetId).Sum += sortedData(dataIndex)
                classes(targetId).SumofSquares += squaredValues(dataIndex)
                classes(targetId).RefreshSquareDeviation()
            Else
                If classes(currentClassIndex).EndIdx > 0 Then classes(currentClassIndex).EndIdx -= 1
                If classes(targetId).StartIdx > 0 Then classes(targetId).StartIdx -= 1
                dataIndex = classes(targetId).StartIdx
                If dataIndex = -1 Then Return
                classes(currentClassIndex).Sum -= sortedData(dataIndex)
                classes(currentClassIndex).SumofSquares -= squaredValues(dataIndex)
                classes(currentClassIndex).RefreshSquareDeviation()
                classes(targetId).Sum += sortedData(dataIndex)
                classes(targetId).SumofSquares += squaredValues(dataIndex)
                classes(targetId).RefreshSquareDeviation()
            End If
        Next
    End Sub

    Private Function GetSumSquaredDeviations(ByVal classes As JenksBreak(), ByVal Optional startIndex As Integer = -1, ByVal Optional endIndex As Integer = -1) As Double
        If startIndex < 0 Then startIndex = 0
        If endIndex <= 0 Then endIndex = classes.Length - 1
        Dim sum As Double = 0R

        For i As Integer = startIndex To endIndex
            sum += classes(i).SquareDeviation
        Next

        Return sum
    End Function

    Private Class JenksBreak
        Public Sub New()
            Sum = 0.0R
            SumofSquares = 0.0R
            SquareDeviation = 0R
            StartIdx = -1
            EndIdx = -1
        End Sub

        Public Property StartIdx As Integer
        Public Property EndIdx As Integer
        Public Property Average As Double
        Public Property Variance As Double
        Public Property SquareDeviation As Double
        Public Property SumofSquares As Double
        Public Property Sum As Double
        Public Property Count As Integer

        Public Sub RefreshSquareDeviation()
            Count = EndIdx - StartIdx + 1

            If Count <= 0 Then
                Average = 0R
                Variance = 0R
            ElseIf Count = 1 Then
                Average = Sum
                Variance = 0R
            Else
                Average = Sum / Count
                Variance = SumofSquares / Count - Math.Pow(Average, 2.0R)
                If Variance < 0R Then Variance = 0R
            End If

            SquareDeviation = Variance * Count
        End Sub
    End Class
End Module
