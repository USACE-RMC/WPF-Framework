using System.ComponentModel;
using System.Reflection;
using System.Windows;
using GenericControls;
using Numerics.Data;
using NumericControls;
using Xunit;

namespace NumericControls.Tests
{
    /// <summary>
    /// Tests for <see cref="TimeSeriesTable"/>.
    /// </summary>
    public class TimeSeriesTableTests
    {
        /// <summary>
        /// Tracks whether the generic control resource dictionary has been loaded for this test process.
        /// </summary>
        private static bool _resourcesLoaded;

        /// <summary>
        /// Ensures a WPF application exists for control construction.
        /// </summary>
        private static void EnsureApplication()
        {
            var application = Application.Current ?? new Application();

            if (_resourcesLoaded) return;

            application.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/GenericControls;component/Themes/GenericControlsTheme.xaml", UriKind.Relative)
            });
            _resourcesLoaded = true;
        }

        /// <summary>
        /// Creates a sample irregular time series.
        /// </summary>
        /// <returns>A sample time series with distinct values.</returns>
        private static TimeSeries CreateSampleSeries()
        {
            var start = new DateTime(2020, 1, 1);
            var series = new TimeSeries(TimeInterval.Irregular);
            series.Add(new SeriesOrdinate<DateTime, double>(start, 10d));
            series.Add(new SeriesOrdinate<DateTime, double>(start.AddDays(1), 30d));
            series.Add(new SeriesOrdinate<DateTime, double>(start.AddDays(2), 20d));
            return series;
        }

        /// <summary>
        /// Verifies that preview delete maps a visually sorted row index to the correct backing row.
        /// </summary>
        [StaFact]
        public void PreviewDeleteRows_AfterValueSort_RemovesVisualRowFromBackingSeries()
        {
            EnsureApplication();
            var series = CreateSampleSeries();
            var table = new TimeSeriesTable { Series = series };
            var grid = Assert.IsType<ValidationDataGrid>(table.FindName("TimeSeriesDataGrid"));

            grid.Items.SortDescriptions.Clear();
            grid.Items.SortDescriptions.Add(new SortDescription(nameof(TimeSeriesRowItem.Value), ListSortDirection.Descending));
            grid.Items.Refresh();

            Assert.Equal(30d, ((TimeSeriesRowItem)grid.Items[0]).Value);

            var deleteMethod = typeof(TimeSeriesTable).GetMethod(
                "TimeSeriesDataGrid_PreviewDeleteRows",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(deleteMethod);

            object?[] arguments = { new List<int> { 0 }, false };
            deleteMethod.Invoke(table, arguments);

            var values = Enumerable.Range(0, series.Count).Select(i => series[i].Value).ToList();
            Assert.True((bool)arguments[1]!);
            Assert.Equal(2, series.Count);
            Assert.DoesNotContain(30d, values);
        }
    }
}
