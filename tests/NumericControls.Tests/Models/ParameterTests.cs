/*
 * Unit tests for Parameter in the NumericControls library.
 * Tests value changes and validation state.
 */

using NumericControls;
using Xunit;

namespace NumericControls.Tests.Models
{
    /// <summary>
    /// Tests for <see cref="Parameter"/>.
    /// </summary>
    public class ParameterTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsName()
        {
            // Arrange & Act
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert
            Assert.Equal("Mean", parameter.Name);
        }

        [Fact]
        public void Constructor_SetsDisplayName()
        {
            // Arrange & Act
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert
            Assert.Equal("Mean (mu)", parameter.DisplayName);
        }

        [Fact]
        public void Constructor_SetsValue()
        {
            // Arrange & Act
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert
            Assert.Equal(100.0, parameter.Value);
        }

        [Fact]
        public void Constructor_SetsIsValidToTrue()
        {
            // Arrange & Act
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert
            Assert.True(parameter.IsValid);
        }

        [Fact]
        public void Constructor_SetsErrorMessageToNull()
        {
            // Arrange & Act
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert
            Assert.Null(parameter.ErrorMessage);
        }

        [Fact]
        public void Constructor_WithZeroValue_SetsCorrectly()
        {
            // Arrange & Act
            var parameter = new Parameter("Scale", "Scale", 0.0);

            // Assert
            Assert.Equal(0.0, parameter.Value);
        }

        [Fact]
        public void Constructor_WithNegativeValue_SetsCorrectly()
        {
            // Arrange & Act
            var parameter = new Parameter("Shift", "Shift", -50.0);

            // Assert
            Assert.Equal(-50.0, parameter.Value);
        }

        [Fact]
        public void Constructor_WithNaN_SetsCorrectly()
        {
            // Arrange & Act
            var parameter = new Parameter("Test", "Test", double.NaN);

            // Assert
            Assert.True(double.IsNaN(parameter.Value));
        }

        [Fact]
        public void Constructor_WithInfinity_SetsCorrectly()
        {
            // Arrange & Act
            var parameter = new Parameter("Test", "Test", double.PositiveInfinity);

            // Assert
            Assert.True(double.IsPositiveInfinity(parameter.Value));
        }

        #endregion

        #region Value Property Tests

        [Fact]
        public void Value_SetNewValue_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = 150.0;

            // Assert
            Assert.Equal(150.0, parameter.Value);
        }

        [Fact]
        public void Value_SetNewValue_RaisesPropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.Value))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.Value = 150.0;

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Value_SetSameValue_DoesNotRaisePropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.Value))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.Value = 100.0;

            // Assert
            Assert.False(propertyChangedRaised);
        }

        [Fact]
        public void Value_SetToZero_UpdatesCorrectly()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = 0.0;

            // Assert
            Assert.Equal(0.0, parameter.Value);
        }

        [Fact]
        public void Value_SetToNegative_UpdatesCorrectly()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = -50.0;

            // Assert
            Assert.Equal(-50.0, parameter.Value);
        }

        [Fact]
        public void Value_SetToNaN_UpdatesCorrectly()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = double.NaN;

            // Assert
            Assert.True(double.IsNaN(parameter.Value));
        }

        [Fact]
        public void Value_SetToMaxValue_UpdatesCorrectly()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = double.MaxValue;

            // Assert
            Assert.Equal(double.MaxValue, parameter.Value);
        }

        [Fact]
        public void Value_SetToMinValue_UpdatesCorrectly()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.Value = double.MinValue;

            // Assert
            Assert.Equal(double.MinValue, parameter.Value);
        }

        #endregion

        #region IsValid Property Tests

        [Fact]
        public void IsValid_SetToFalse_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.IsValid = false;

            // Assert
            Assert.False(parameter.IsValid);
        }

        [Fact]
        public void IsValid_SetToFalse_RaisesPropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.IsValid))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.IsValid = false;

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void IsValid_SetSameValue_DoesNotRaisePropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.IsValid))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.IsValid = true; // Same as initial value

            // Assert
            Assert.False(propertyChangedRaised);
        }

        [Fact]
        public void IsValid_SetToTrue_AfterFalse_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            parameter.IsValid = false;

            // Act
            parameter.IsValid = true;

            // Assert
            Assert.True(parameter.IsValid);
        }

        #endregion

        #region ErrorMessage Property Tests

        [Fact]
        public void ErrorMessage_SetMessage_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            string errorMessage = "Value must be positive";

            // Act
            parameter.ErrorMessage = errorMessage;

            // Assert
            Assert.Equal(errorMessage, parameter.ErrorMessage);
        }

        [Fact]
        public void ErrorMessage_SetMessage_RaisesPropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.ErrorMessage))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.ErrorMessage = "Error occurred";

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void ErrorMessage_SetSameMessage_DoesNotRaisePropertyChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            parameter.ErrorMessage = "Error";
            bool propertyChangedRaised = false;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.ErrorMessage))
                    propertyChangedRaised = true;
            };

            // Act
            parameter.ErrorMessage = "Error"; // Same value

            // Assert
            Assert.False(propertyChangedRaised);
        }

        [Fact]
        public void ErrorMessage_SetToNull_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            parameter.ErrorMessage = "Some error";

            // Act
            parameter.ErrorMessage = null;

            // Assert
            Assert.Null(parameter.ErrorMessage);
        }

        [Fact]
        public void ErrorMessage_SetToEmptyString_UpdatesValue()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            parameter.ErrorMessage = "Some error";

            // Act
            parameter.ErrorMessage = "";

            // Assert
            Assert.Equal("", parameter.ErrorMessage);
        }

        #endregion

        #region Combined Validation Tests

        [Fact]
        public void SetInvalidState_UpdatesBothIsValidAndErrorMessage()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act
            parameter.IsValid = false;
            parameter.ErrorMessage = "Value must be greater than 0";

            // Assert
            Assert.False(parameter.IsValid);
            Assert.Equal("Value must be greater than 0", parameter.ErrorMessage);
        }

        [Fact]
        public void ClearInvalidState_ResetsBothIsValidAndErrorMessage()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            parameter.IsValid = false;
            parameter.ErrorMessage = "Error";

            // Act
            parameter.IsValid = true;
            parameter.ErrorMessage = null;

            // Assert
            Assert.True(parameter.IsValid);
            Assert.Null(parameter.ErrorMessage);
        }

        #endregion

        #region PropertyChanged Event Tests

        [Fact]
        public void PropertyChanged_ValueChange_RaisesOnce()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            int changeCount = 0;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.Value))
                    changeCount++;
            };

            // Act
            parameter.Value = 150.0;

            // Assert
            Assert.Equal(1, changeCount);
        }

        [Fact]
        public void PropertyChanged_MultipleValueChanges_RaisesMultipleTimes()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);
            int changeCount = 0;
            parameter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(parameter.Value))
                    changeCount++;
            };

            // Act
            parameter.Value = 150.0;
            parameter.Value = 200.0;
            parameter.Value = 250.0;

            // Assert
            Assert.Equal(3, changeCount);
        }

        [Fact]
        public void PropertyChanged_NoSubscriber_DoesNotThrow()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean", 100.0);

            // Act & Assert - should not throw
            var exception = Record.Exception(() => parameter.Value = 150.0);
            Assert.Null(exception);
        }

        #endregion

        #region Immutable Properties Tests

        [Fact]
        public void Name_IsReadOnly_CannotBeChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert - Name has no setter, this test verifies the design
            Assert.Equal("Mean", parameter.Name);
        }

        [Fact]
        public void DisplayName_IsReadOnly_CannotBeChanged()
        {
            // Arrange
            var parameter = new Parameter("Mean", "Mean (mu)", 100.0);

            // Assert - DisplayName has no setter, this test verifies the design
            Assert.Equal("Mean (mu)", parameter.DisplayName);
        }

        #endregion

        #region Common Distribution Parameter Tests

        [Fact]
        public void NormalDistribution_MeanParameter()
        {
            // Arrange & Act
            var meanParam = new Parameter("Mean", "mu (Mean)", 0.0);

            // Assert
            Assert.Equal("Mean", meanParam.Name);
            Assert.Equal("mu (Mean)", meanParam.DisplayName);
            Assert.Equal(0.0, meanParam.Value);
        }

        [Fact]
        public void NormalDistribution_StdDevParameter()
        {
            // Arrange & Act
            var stdDevParam = new Parameter("StdDev", "sigma (Std Dev)", 1.0);

            // Assert
            Assert.Equal("StdDev", stdDevParam.Name);
            Assert.Equal("sigma (Std Dev)", stdDevParam.DisplayName);
            Assert.Equal(1.0, stdDevParam.Value);
        }

        [Fact]
        public void UniformDistribution_MinParameter()
        {
            // Arrange & Act
            var minParam = new Parameter("Min", "Minimum", 0.0);

            // Assert
            Assert.Equal("Min", minParam.Name);
            Assert.Equal("Minimum", minParam.DisplayName);
        }

        [Fact]
        public void UniformDistribution_MaxParameter()
        {
            // Arrange & Act
            var maxParam = new Parameter("Max", "Maximum", 100.0);

            // Assert
            Assert.Equal("Max", maxParam.Name);
            Assert.Equal("Maximum", maxParam.DisplayName);
        }

        [Fact]
        public void TriangularDistribution_ModeParameter()
        {
            // Arrange & Act
            var modeParam = new Parameter("Mode", "Mode (Most Likely)", 50.0);

            // Assert
            Assert.Equal("Mode", modeParam.Name);
            Assert.Equal("Mode (Most Likely)", modeParam.DisplayName);
            Assert.Equal(50.0, modeParam.Value);
        }

        #endregion
    }
}
