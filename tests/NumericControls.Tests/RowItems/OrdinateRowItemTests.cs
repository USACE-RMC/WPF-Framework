/*
 * Unit tests for OrdinateRowItem in the NumericControls library.
 * Tests X/Y validation, ordering constraints, and min/max bounds.
 */

using System.Collections.ObjectModel;
using Numerics.Data;
using NumericControls;
using Xunit;

namespace NumericControls.Tests.RowItems
{
    /// <summary>
    /// Tests for <see cref="OrdinateRowItem"/>.
    /// </summary>
    public class OrdinateRowItemTests
    {
        private ObservableCollection<object> CreateParentList()
        {
            return new ObservableCollection<object>();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var list = CreateParentList();
            double xVal = 10.0;
            double yVal = 20.0;

            // Act
            var item = new OrdinateRowItem(xVal, yVal, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Assert
            Assert.Equal(xVal, item.X);
            Assert.Equal(yVal, item.Y);
            Assert.Equal(0, item.MinXValue);
            Assert.Equal(100, item.MaxXValue);
            Assert.Equal(0, item.MinYValue);
            Assert.Equal(100, item.MaxYValue);
        }

        [Fact]
        public void Constructor_WithStrictOrdering_SetsIsStrictFlags()
        {
            // Arrange
            var list = CreateParentList();

            // Act
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, true, true, SortOrder.Ascending, SortOrder.Ascending);
            list.Add(item);

            // Assert
            Assert.True(item.IsStrictX);
            Assert.True(item.IsStrictY);
        }

        [Fact]
        public void Constructor_WithSortOrder_SetsSortOrderProperties()
        {
            // Arrange
            var list = CreateParentList();

            // Act
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.Descending);
            list.Add(item);

            // Assert
            Assert.Equal(SortOrder.Ascending, item.XOrder);
            Assert.Equal(SortOrder.Descending, item.YOrder);
        }

        #endregion

        #region X Property Tests

        [Fact]
        public void X_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.X)) propertyChanged = true; };

            // Act
            item.X = 15.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(15.0, item.X);
        }

        [Fact]
        public void X_SetSameValue_DoesNotNotifyPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.X)) propertyChanged = true; };

            // Act
            item.X = 5.0;

            // Assert
            Assert.False(propertyChanged);
        }

        #endregion

        #region Y Property Tests

        [Fact]
        public void Y_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.Y)) propertyChanged = true; };

            // Act
            item.Y = 25.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(25.0, item.Y);
        }

        [Fact]
        public void Y_SetNaN_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.Y)) propertyChanged = true; };

            // Act
            item.Y = double.NaN;

            // Assert
            Assert.True(propertyChanged);
            Assert.True(double.IsNaN(item.Y));
        }

        [Fact]
        public void Y_SetSameValue_DoesNotNotifyPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.Y)) propertyChanged = true; };

            // Act
            item.Y = 10.0;

            // Assert
            Assert.False(propertyChanged);
        }

        #endregion

        #region MinMax Bounds Tests

        [Fact]
        public void MinXValue_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.MinXValue)) propertyChanged = true; };

            // Act
            item.MinXValue = -10.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(-10.0, item.MinXValue);
        }

        [Fact]
        public void MaxXValue_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.MaxXValue)) propertyChanged = true; };

            // Act
            item.MaxXValue = 200.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(200.0, item.MaxXValue);
        }

        [Fact]
        public void MinYValue_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.MinYValue)) propertyChanged = true; };

            // Act
            item.MinYValue = -50.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(-50.0, item.MinYValue);
        }

        [Fact]
        public void MaxYValue_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.MaxYValue)) propertyChanged = true; };

            // Act
            item.MaxYValue = 500.0;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(500.0, item.MaxYValue);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void XValue_BelowMinimum_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 10, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act - X is 5.0 but minimum is 10
            item.ForceValidation();

            // Assert
            Assert.True(item.RuleMap["X"].HasError);
        }

        [Fact]
        public void XValue_AboveMaximum_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(150.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            item.ForceValidation();

            // Assert
            Assert.True(item.RuleMap["X"].HasError);
        }

        [Fact]
        public void YValue_BelowMinimum_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(50.0, -5.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            item.ForceValidation();

            // Assert
            Assert.True(item.RuleMap["Y"].HasError);
        }

        [Fact]
        public void YValue_AboveMaximum_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(50.0, 150.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            item.ForceValidation();

            // Assert
            Assert.True(item.RuleMap["Y"].HasError);
        }

        [Fact]
        public void ValidValues_NoValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(50.0, 50.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            item.ForceValidation();

            // Assert
            Assert.False(item.RuleMap["X"].HasError);
            Assert.False(item.RuleMap["Y"].HasError);
        }

        #endregion

        #region Ordering Tests

        [Fact]
        public void AscendingXOrder_ValuesInOrder_NoValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(10.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            var item2 = new OrdinateRowItem(20.0, 20.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            var item3 = new OrdinateRowItem(30.0, 30.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            list.Add(item1);
            list.Add(item2);
            list.Add(item3);

            // Act
            item1.ForceValidation();
            item2.ForceValidation();
            item3.ForceValidation();

            // Assert
            Assert.False(item1.RuleMap["X"].HasError);
            Assert.False(item2.RuleMap["X"].HasError);
            Assert.False(item3.RuleMap["X"].HasError);
        }

        [Fact]
        public void AscendingXOrder_ValuesOutOfOrder_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(10.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            var item2 = new OrdinateRowItem(5.0, 20.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            list.Add(item1);
            list.Add(item2);

            // Act
            item2.ForceValidation();

            // Assert
            Assert.True(item2.RuleMap["X"].HasError);
        }

        [Fact]
        public void DescendingXOrder_ValuesInOrder_NoValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(30.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Descending, SortOrder.None);
            var item2 = new OrdinateRowItem(20.0, 20.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Descending, SortOrder.None);
            var item3 = new OrdinateRowItem(10.0, 30.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Descending, SortOrder.None);
            list.Add(item1);
            list.Add(item2);
            list.Add(item3);

            // Act
            item1.ForceValidation();
            item2.ForceValidation();
            item3.ForceValidation();

            // Assert
            Assert.False(item1.RuleMap["X"].HasError);
            Assert.False(item2.RuleMap["X"].HasError);
            Assert.False(item3.RuleMap["X"].HasError);
        }

        [Fact]
        public void AscendingYOrder_ValuesInOrder_NoValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(10.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.Ascending);
            var item2 = new OrdinateRowItem(20.0, 20.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.Ascending);
            list.Add(item1);
            list.Add(item2);

            // Act
            item1.ForceValidation();
            item2.ForceValidation();

            // Assert
            Assert.False(item1.RuleMap["Y"].HasError);
            Assert.False(item2.RuleMap["Y"].HasError);
        }

        [Fact]
        public void StrictAscendingX_EqualValues_HasValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(10.0, 10.0, "X", "Y", list, 0, 100, 0, 100, true, false, SortOrder.Ascending, SortOrder.None);
            var item2 = new OrdinateRowItem(10.0, 20.0, "X", "Y", list, 0, 100, 0, 100, true, false, SortOrder.Ascending, SortOrder.None);
            list.Add(item1);
            list.Add(item2);

            // Act
            item2.ForceValidation();

            // Assert
            Assert.True(item2.RuleMap["X"].HasError);
        }

        [Fact]
        public void NonStrictAscendingX_EqualValues_NoValidationError()
        {
            // Arrange
            var list = CreateParentList();
            var item1 = new OrdinateRowItem(10.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            var item2 = new OrdinateRowItem(10.0, 20.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.Ascending, SortOrder.None);
            list.Add(item1);
            list.Add(item2);

            // Act
            item2.ForceValidation();

            // Assert
            Assert.False(item2.RuleMap["X"].HasError);
        }

        #endregion

        #region GetOrdinate Tests

        [Fact]
        public void GetOrdinate_ReturnsOrdinateWithCorrectValues()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(15.0, 25.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var ordinate = item.GetOrdinate();

            // Assert
            Assert.Equal(15.0, ordinate.X);
            Assert.Equal(25.0, ordinate.Y);
        }

        #endregion

        #region PropertyDisplayName Tests

        [Fact]
        public void PropertyDisplayName_X_ReturnsXColumnHeader()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "CustomX", "CustomY", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var displayName = item.PropertyDisplayName("X");

            // Assert
            Assert.Equal("CustomX", displayName);
        }

        [Fact]
        public void PropertyDisplayName_Y_ReturnsYColumnHeader()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "CustomX", "CustomY", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var displayName = item.PropertyDisplayName("Y");

            // Assert
            Assert.Equal("CustomY", displayName);
        }

        [Fact]
        public void PropertyDisplayName_OtherProperty_ReturnsPropertyName()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "CustomX", "CustomY", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var displayName = item.PropertyDisplayName("SomeOtherProperty");

            // Assert
            Assert.Equal("SomeOtherProperty", displayName);
        }

        #endregion

        #region IsGridDisplayable Tests

        [Fact]
        public void IsGridDisplayable_X_ReturnsTrue()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var isDisplayable = item.IsGridDisplayable("X");

            // Assert
            Assert.True(isDisplayable);
        }

        [Fact]
        public void IsGridDisplayable_Y_ReturnsTrue()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var isDisplayable = item.IsGridDisplayable("Y");

            // Assert
            Assert.True(isDisplayable);
        }

        [Fact]
        public void IsGridDisplayable_OtherProperty_ReturnsFalse()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);

            // Act
            var isDisplayable = item.IsGridDisplayable("MinXValue");

            // Assert
            Assert.False(isDisplayable);
        }

        #endregion

        #region RaisePropertyChanged Tests

        [Fact]
        public void RaisePropertyChanged_NotifiesAllProperties()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => propertyChanged = true;

            // Act
            item.RaisePropertyChanged();

            // Assert
            Assert.True(propertyChanged);
        }

        #endregion

        #region IsStrict Property Tests

        [Fact]
        public void IsStrictX_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.IsStrictX)) propertyChanged = true; };

            // Act
            item.IsStrictX = true;

            // Assert
            Assert.True(propertyChanged);
            Assert.True(item.IsStrictX);
        }

        [Fact]
        public void IsStrictY_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.IsStrictY)) propertyChanged = true; };

            // Act
            item.IsStrictY = true;

            // Assert
            Assert.True(propertyChanged);
            Assert.True(item.IsStrictY);
        }

        #endregion

        #region SortOrder Property Tests

        [Fact]
        public void XOrder_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.XOrder)) propertyChanged = true; };

            // Act
            item.XOrder = SortOrder.Ascending;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(SortOrder.Ascending, item.XOrder);
        }

        [Fact]
        public void YOrder_SetValue_NotifiesPropertyChanged()
        {
            // Arrange
            var list = CreateParentList();
            var item = new OrdinateRowItem(5.0, 10.0, "X", "Y", list, 0, 100, 0, 100, false, false, SortOrder.None, SortOrder.None);
            list.Add(item);
            bool propertyChanged = false;
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(item.YOrder)) propertyChanged = true; };

            // Act
            item.YOrder = SortOrder.Descending;

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal(SortOrder.Descending, item.YOrder);
        }

        #endregion
    }
}
