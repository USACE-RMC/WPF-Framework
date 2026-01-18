/*
 * Unit tests for MathEditorControl functions in the NumericControls library.
 * Tests ApplyFunctionToSeries for Add, Subtract, Multiply, Divide and advanced functions.
 */

using Numerics.Data;
using NumericControls;
using Xunit;

namespace NumericControls.Tests.MathEditor
{
    /// <summary>
    /// Tests for <see cref="MathEditorControl.ApplyFunctionToSeries"/> method.
    /// </summary>
    public class MathEditorFunctionTests
    {
        private TimeSeries CreateTestSeries()
        {
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            for (int i = 0; i < 5; i++)
            {
                series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(i), (i + 1) * 10.0));
            }
            return series;
        }

        private TimeSeries CreateTestSeriesWithNaN()
        {
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 10.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), double.NaN));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 30.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(3), double.NaN));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(4), 50.0));
            return series;
        }

        #region Add Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Add_AllRows_AddsValueToAll()
        {
            // Arrange
            var series = CreateTestSeries();
            double addValue = 5.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, addValue, null);

            // Assert
            Assert.Equal(15.0, series[0].Value);
            Assert.Equal(25.0, series[1].Value);
            Assert.Equal(35.0, series[2].Value);
            Assert.Equal(45.0, series[3].Value);
            Assert.Equal(55.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Add_SpecificIndices_AddsValueToSelected()
        {
            // Arrange
            var series = CreateTestSeries();
            double addValue = 100.0;
            var indices = new List<int> { 1, 3 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, addValue, indices);

            // Assert
            Assert.Equal(10.0, series[0].Value);  // Unchanged
            Assert.Equal(120.0, series[1].Value); // Changed
            Assert.Equal(30.0, series[2].Value);  // Unchanged
            Assert.Equal(140.0, series[3].Value); // Changed
            Assert.Equal(50.0, series[4].Value);  // Unchanged
        }

        [Fact]
        public void ApplyFunctionToSeries_Add_NegativeValue_SubtractsEffectively()
        {
            // Arrange
            var series = CreateTestSeries();
            double addValue = -5.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, addValue, null);

            // Assert
            Assert.Equal(5.0, series[0].Value);
            Assert.Equal(15.0, series[1].Value);
            Assert.Equal(25.0, series[2].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Add_PreservesNaN()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double addValue = 5.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, addValue, null);

            // Assert
            Assert.Equal(15.0, series[0].Value);
            Assert.True(double.IsNaN(series[1].Value)); // NaN preserved
            Assert.Equal(35.0, series[2].Value);
            Assert.True(double.IsNaN(series[3].Value)); // NaN preserved
            Assert.Equal(55.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Add_Zero_NoChange()
        {
            // Arrange
            var series = CreateTestSeries();
            double addValue = 0.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, addValue, null);

            // Assert
            Assert.Equal(10.0, series[0].Value);
            Assert.Equal(20.0, series[1].Value);
        }

        #endregion

        #region Subtract Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Subtract_AllRows_SubtractsValueFromAll()
        {
            // Arrange
            var series = CreateTestSeries();
            double subtractValue = 5.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Subtract, subtractValue, null);

            // Assert
            Assert.Equal(5.0, series[0].Value);
            Assert.Equal(15.0, series[1].Value);
            Assert.Equal(25.0, series[2].Value);
            Assert.Equal(35.0, series[3].Value);
            Assert.Equal(45.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Subtract_SpecificIndices_SubtractsValueFromSelected()
        {
            // Arrange
            var series = CreateTestSeries();
            double subtractValue = 5.0;
            var indices = new List<int> { 0, 2, 4 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Subtract, subtractValue, indices);

            // Assert
            Assert.Equal(5.0, series[0].Value);   // Changed
            Assert.Equal(20.0, series[1].Value);  // Unchanged
            Assert.Equal(25.0, series[2].Value);  // Changed
            Assert.Equal(40.0, series[3].Value);  // Unchanged
            Assert.Equal(45.0, series[4].Value);  // Changed
        }

        [Fact]
        public void ApplyFunctionToSeries_Subtract_PreservesNaN()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double subtractValue = 5.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Subtract, subtractValue, null);

            // Assert
            Assert.Equal(5.0, series[0].Value);
            Assert.True(double.IsNaN(series[1].Value)); // NaN preserved
            Assert.Equal(25.0, series[2].Value);
        }

        #endregion

        #region Multiply Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Multiply_AllRows_MultipliesAllByValue()
        {
            // Arrange
            var series = CreateTestSeries();
            double multiplyValue = 2.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Multiply, multiplyValue, null);

            // Assert
            Assert.Equal(20.0, series[0].Value);
            Assert.Equal(40.0, series[1].Value);
            Assert.Equal(60.0, series[2].Value);
            Assert.Equal(80.0, series[3].Value);
            Assert.Equal(100.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Multiply_SpecificIndices_MultipliesSelected()
        {
            // Arrange
            var series = CreateTestSeries();
            double multiplyValue = 3.0;
            var indices = new List<int> { 1, 3 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Multiply, multiplyValue, indices);

            // Assert
            Assert.Equal(10.0, series[0].Value);  // Unchanged
            Assert.Equal(60.0, series[1].Value);  // Changed
            Assert.Equal(30.0, series[2].Value);  // Unchanged
            Assert.Equal(120.0, series[3].Value); // Changed
            Assert.Equal(50.0, series[4].Value);  // Unchanged
        }

        [Fact]
        public void ApplyFunctionToSeries_Multiply_ByZero_SetsToZero()
        {
            // Arrange
            var series = CreateTestSeries();
            double multiplyValue = 0.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Multiply, multiplyValue, null);

            // Assert
            Assert.Equal(0.0, series[0].Value);
            Assert.Equal(0.0, series[1].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Multiply_ByNegative_ReversesSign()
        {
            // Arrange
            var series = CreateTestSeries();
            double multiplyValue = -1.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Multiply, multiplyValue, null);

            // Assert
            Assert.Equal(-10.0, series[0].Value);
            Assert.Equal(-20.0, series[1].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Multiply_PreservesNaN()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double multiplyValue = 2.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Multiply, multiplyValue, null);

            // Assert
            Assert.Equal(20.0, series[0].Value);
            Assert.True(double.IsNaN(series[1].Value)); // NaN preserved
            Assert.Equal(60.0, series[2].Value);
        }

        #endregion

        #region Divide Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Divide_AllRows_DividesAllByValue()
        {
            // Arrange
            var series = CreateTestSeries();
            double divideValue = 2.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Divide, divideValue, null);

            // Assert
            Assert.Equal(5.0, series[0].Value);
            Assert.Equal(10.0, series[1].Value);
            Assert.Equal(15.0, series[2].Value);
            Assert.Equal(20.0, series[3].Value);
            Assert.Equal(25.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Divide_SpecificIndices_DividesSelected()
        {
            // Arrange
            var series = CreateTestSeries();
            double divideValue = 5.0;
            var indices = new List<int> { 0, 2, 4 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Divide, divideValue, indices);

            // Assert
            Assert.Equal(2.0, series[0].Value);   // Changed
            Assert.Equal(20.0, series[1].Value);  // Unchanged
            Assert.Equal(6.0, series[2].Value);   // Changed
            Assert.Equal(40.0, series[3].Value);  // Unchanged
            Assert.Equal(10.0, series[4].Value);  // Changed
        }

        [Fact]
        public void ApplyFunctionToSeries_Divide_PreservesNaN()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double divideValue = 2.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Divide, divideValue, null);

            // Assert
            Assert.Equal(5.0, series[0].Value);
            Assert.True(double.IsNaN(series[1].Value)); // NaN preserved
            Assert.Equal(15.0, series[2].Value);
        }

        #endregion

        #region Exponentiate Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Exponentiate_AllRows_RaisesToPower()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 2.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 3.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 4.0));
            double power = 2.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Exponentiate, power, null);

            // Assert
            Assert.Equal(4.0, series[0].Value);
            Assert.Equal(9.0, series[1].Value);
            Assert.Equal(16.0, series[2].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Exponentiate_PowerOfZero_ReturnsOne()
        {
            // Arrange
            var series = CreateTestSeries();
            double power = 0.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Exponentiate, power, null);

            // Assert
            Assert.Equal(1.0, series[0].Value);
            Assert.Equal(1.0, series[1].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Exponentiate_SquareRoot()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 4.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 9.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 16.0));
            double power = 0.5;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Exponentiate, power, null);

            // Assert
            Assert.Equal(2.0, series[0].Value, 5);
            Assert.Equal(3.0, series[1].Value, 5);
            Assert.Equal(4.0, series[2].Value, 5);
        }

        [Fact]
        public void ApplyFunctionToSeries_Exponentiate_SpecificIndices_RaisesSelected()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 2.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 3.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 4.0));
            double power = 2.0;
            var indices = new List<int> { 0, 2 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Exponentiate, power, indices);

            // Assert
            Assert.Equal(4.0, series[0].Value);  // Changed
            Assert.Equal(3.0, series[1].Value);  // Unchanged
            Assert.Equal(16.0, series[2].Value); // Changed
        }

        #endregion

        #region Logarithm Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Logarithm_Base10_AllRows()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 10.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 100.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 1000.0));
            double logBase = 10.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Logarithm, logBase, null);

            // Assert
            Assert.Equal(1.0, series[0].Value, 5);
            Assert.Equal(2.0, series[1].Value, 5);
            Assert.Equal(3.0, series[2].Value, 5);
        }

        [Fact]
        public void ApplyFunctionToSeries_Logarithm_NaturalLog()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, Math.E));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), Math.E * Math.E));
            double logBase = Math.E;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Logarithm, logBase, null);

            // Assert
            Assert.Equal(1.0, series[0].Value, 5);
            Assert.Equal(2.0, series[1].Value, 5);
        }

        [Fact]
        public void ApplyFunctionToSeries_Logarithm_SpecificIndices()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 10.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 100.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 1000.0));
            double logBase = 10.0;
            var indices = new List<int> { 1 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Logarithm, logBase, indices);

            // Assert
            Assert.Equal(10.0, series[0].Value);  // Unchanged
            Assert.Equal(2.0, series[1].Value, 5); // Changed
            Assert.Equal(1000.0, series[2].Value); // Unchanged
        }

        #endregion

        #region Inverse Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Inverse_AllRows_TakesInverse()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 2.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 4.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 5.0));

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Inverse, 0, null);

            // Assert
            Assert.Equal(0.5, series[0].Value);
            Assert.Equal(0.25, series[1].Value);
            Assert.Equal(0.2, series[2].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Inverse_SpecificIndices()
        {
            // Arrange
            var series = new TimeSeries(TimeInterval.Irregular);
            var baseDate = new DateTime(2024, 1, 1);
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate, 2.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(1), 4.0));
            series.Add(new SeriesOrdinate<DateTime, double>(baseDate.AddDays(2), 5.0));
            var indices = new List<int> { 0, 2 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Inverse, 0, indices);

            // Assert
            Assert.Equal(0.5, series[0].Value);  // Changed
            Assert.Equal(4.0, series[1].Value);  // Unchanged
            Assert.Equal(0.2, series[2].Value);  // Changed
        }

        #endregion

        #region Replace Function Tests

        [Fact]
        public void ApplyFunctionToSeries_Replace_AllRows_ReplacesMissingWithValue()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double replaceValue = 0.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Replace, replaceValue, null);

            // Assert
            Assert.Equal(10.0, series[0].Value);  // Unchanged (not NaN)
            Assert.Equal(0.0, series[1].Value);   // Replaced (was NaN)
            Assert.Equal(30.0, series[2].Value);  // Unchanged (not NaN)
            Assert.Equal(0.0, series[3].Value);   // Replaced (was NaN)
            Assert.Equal(50.0, series[4].Value);  // Unchanged (not NaN)
        }

        [Fact]
        public void ApplyFunctionToSeries_Replace_SpecificIndices_ReplacesMissingInSelected()
        {
            // Arrange
            var series = CreateTestSeriesWithNaN();
            double replaceValue = -999.0;
            var indices = new List<int> { 1 }; // Only replace at index 1

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Replace, replaceValue, indices);

            // Assert
            Assert.Equal(10.0, series[0].Value);
            Assert.Equal(-999.0, series[1].Value); // Replaced (was NaN)
            Assert.Equal(30.0, series[2].Value);
            Assert.True(double.IsNaN(series[3].Value)); // Not in indices, still NaN
            Assert.Equal(50.0, series[4].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_Replace_NoMissingData_NoChange()
        {
            // Arrange
            var series = CreateTestSeries();
            double replaceValue = 0.0;

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Replace, replaceValue, null);

            // Assert - All values should remain unchanged
            Assert.Equal(10.0, series[0].Value);
            Assert.Equal(20.0, series[1].Value);
            Assert.Equal(30.0, series[2].Value);
            Assert.Equal(40.0, series[3].Value);
            Assert.Equal(50.0, series[4].Value);
        }

        #endregion

        #region Null Series Tests

        [Fact]
        public void ApplyFunctionToSeries_NullSeries_DoesNotThrow()
        {
            // Act & Assert - should not throw
            var exception = Record.Exception(() =>
                MathEditorControl.ApplyFunctionToSeries(null, MathFunctionType.Add, 5.0, null));
            Assert.Null(exception);
        }

        #endregion

        #region Empty Indices Tests

        [Fact]
        public void ApplyFunctionToSeries_EmptyIndices_AppliesToAll()
        {
            // Arrange
            var series = CreateTestSeries();
            var emptyIndices = new List<int>();

            // Act
            MathEditorControl.ApplyFunctionToSeries(series, MathFunctionType.Add, 5.0, emptyIndices);

            // Assert - should apply to all when indices list is empty
            Assert.Equal(15.0, series[0].Value);
            Assert.Equal(25.0, series[1].Value);
        }

        [Fact]
        public void ApplyFunctionToSeries_AllIndices_AppliesLikeNull()
        {
            // Arrange
            var series1 = CreateTestSeries();
            var series2 = CreateTestSeries();
            var allIndices = new List<int> { 0, 1, 2, 3, 4 };

            // Act
            MathEditorControl.ApplyFunctionToSeries(series1, MathFunctionType.Multiply, 2.0, null);
            MathEditorControl.ApplyFunctionToSeries(series2, MathFunctionType.Multiply, 2.0, allIndices);

            // Assert - both should have same result
            for (int i = 0; i < series1.Count; i++)
            {
                Assert.Equal(series1[i].Value, series2[i].Value);
            }
        }

        #endregion

        #region TimeSeriesTable HasOperand Tests

        [Fact]
        public void HasOperand_Add_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Add));
        }

        [Fact]
        public void HasOperand_Subtract_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Subtract));
        }

        [Fact]
        public void HasOperand_Multiply_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Multiply));
        }

        [Fact]
        public void HasOperand_Divide_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Divide));
        }

        [Fact]
        public void HasOperand_Logarithm_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Logarithm));
        }

        [Fact]
        public void HasOperand_Exponentiate_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Exponentiate));
        }

        [Fact]
        public void HasOperand_Replace_ReturnsTrue()
        {
            Assert.True(TimeSeriesTable.HasOperand(MathFunctionType.Replace));
        }

        [Fact]
        public void HasOperand_Inverse_ReturnsFalse()
        {
            Assert.False(TimeSeriesTable.HasOperand(MathFunctionType.Inverse));
        }

        [Fact]
        public void HasOperand_Interpolate_ReturnsFalse()
        {
            Assert.False(TimeSeriesTable.HasOperand(MathFunctionType.Interpolate));
        }

        #endregion
    }
}
