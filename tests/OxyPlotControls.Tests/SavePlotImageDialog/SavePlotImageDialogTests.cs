/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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

using System.IO;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlotControls;
using Xunit;

namespace OxyPlotControls.Tests.SavePlotImageDialog;

/// <summary>
/// Unit tests for the SavePlotImageDialog.
/// Tests construction, preset parsing, custom mode, and file name validation.
/// </summary>
public class SavePlotImageDialogTests
{
    /// <summary>
    /// Creates a Plot control with a simple PlotModel for testing.
    /// </summary>
    private static Plot CreateTestPlot()
    {
        var plot = new Plot();
        var model = new PlotModel { Title = "Test Plot" };
        model.Series.Add(new OxyPlot.Series.LineSeries());
        plot.Model = model;
        return plot;
    }

    #region Construction Tests

    /// <summary>
    /// Verifies that the dialog can be constructed without throwing an exception.
    /// </summary>
    [StaFact]
    public void Constructor_WithValidPlot_DoesNotThrow()
    {
        // Arrange
        var plot = CreateTestPlot();

        // Act & Assert
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);
        Assert.NotNull(dialog);
    }

    /// <summary>
    /// Verifies that the dialog title is set correctly.
    /// </summary>
    [StaFact]
    public void Constructor_WithValidPlot_TitleIsSetCorrectly()
    {
        // Arrange
        var plot = CreateTestPlot();

        // Act
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Assert
        Assert.Equal("Save Plot Image", dialog.Title);
    }

    /// <summary>
    /// Verifies that the dialog initial size is computed from the default preset (600x480) and
    /// falls within the expected bounds. The exact dialog size depends on
    /// <see cref="System.Windows.SystemParameters.WorkArea"/>, which varies by display/DPI and
    /// between interactive and headless (CI) environments, so the assertion uses tolerant ranges
    /// rather than exact pixel values.
    /// </summary>
    [StaFact]
    public void Constructor_WithValidPlot_HasCorrectInitialSize()
    {
        // Arrange
        var plot = CreateTestPlot();

        // Act
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Assert — initial size computed from default 600x480 preset:
        //   width  = 600 + 36 (padding)                       ≈ 636 on a typical display
        //   height = 480 + 190 (controls) + 50 (chrome)       ≈ 720 on a typical display
        // Both are clamped to at least MinWidth/MinHeight and at most WorkArea * ScreenUsageFraction,
        // so CI's smaller WorkArea can produce slightly smaller values.
        Assert.InRange(dialog.Width, dialog.MinWidth, 800);
        Assert.InRange(dialog.Height, dialog.MinHeight, 900);
    }

    /// <summary>
    /// Verifies that the dialog minimum size constraints are set.
    /// </summary>
    [StaFact]
    public void Constructor_WithValidPlot_HasCorrectMinimumSize()
    {
        // Arrange
        var plot = CreateTestPlot();

        // Act
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Assert
        Assert.Equal(500, dialog.MinWidth);
        Assert.Equal(450, dialog.MinHeight);
    }

    #endregion

    #region Image Size Preset Tests

    /// <summary>
    /// Verifies that the ImageSizeComboBox has all 10 preset items plus Custom.
    /// </summary>
    [StaFact]
    public void ImageSizeComboBox_HasCorrectItemCount()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("ImageSizeComboBox");

        // Assert
        Assert.NotNull(comboBox);
        Assert.Equal(10, comboBox.Items.Count); // 9 presets + Custom
    }

    /// <summary>
    /// Verifies that the first preset is selected by default.
    /// </summary>
    [StaFact]
    public void ImageSizeComboBox_DefaultSelectionIsFirstPreset()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("ImageSizeComboBox");

        // Assert
        Assert.NotNull(comboBox);
        Assert.Equal(0, comboBox.SelectedIndex);
    }

    /// <summary>
    /// Verifies that the last item in the ComboBox is "Custom".
    /// </summary>
    [StaFact]
    public void ImageSizeComboBox_LastItemIsCustom()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("ImageSizeComboBox");
        var lastItem = (System.Windows.Controls.ComboBoxItem)comboBox!.Items[comboBox.Items.Count - 1];

        // Assert
        Assert.Equal("Custom", lastItem.Content.ToString());
    }

    /// <summary>
    /// Verifies that preset items contain expected dimension format.
    /// </summary>
    [StaFact]
    public void ImageSizeComboBox_PresetsContainExpectedFormat()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("ImageSizeComboBox");

        // Act & Assert - verify first preset is "600 x 480 (5:4)"
        var firstItem = (System.Windows.Controls.ComboBoxItem)comboBox!.Items[0];
        Assert.Equal("600 x 480 (5:4)", firstItem.Content.ToString());

        // Verify 1920x1080 preset exists
        var fullHdItem = (System.Windows.Controls.ComboBoxItem)comboBox.Items[5];
        Assert.Equal("1920 x 1080 (16:9)", fullHdItem.Content.ToString());
    }

    #endregion

    #region Custom Dimensions Panel Tests

    /// <summary>
    /// Verifies that the custom dimensions panel is collapsed by default.
    /// </summary>
    [StaFact]
    public void CustomDimensionsPanel_IsCollapsedByDefault()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var panel = (System.Windows.Controls.Grid)dialog.FindName("CustomDimensionsPanel");

        // Assert
        Assert.NotNull(panel);
        Assert.Equal(Visibility.Collapsed, panel.Visibility);
    }

    /// <summary>
    /// Verifies that width and height textboxes are disabled by default (non-custom mode).
    /// </summary>
    [StaFact]
    public void DimensionTextBoxes_AreDisabledByDefault()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var widthBox = (GenericControls.NumericTextBox)dialog.FindName("WidthTextBox");
        var heightBox = (GenericControls.NumericTextBox)dialog.FindName("HeightTextBox");

        // Assert
        Assert.NotNull(widthBox);
        Assert.NotNull(heightBox);
        Assert.False(widthBox.IsEnabled);
        Assert.False(heightBox.IsEnabled);
    }

    #endregion

    #region File Type Tests

    /// <summary>
    /// Verifies that the FileTypeComboBox has PNG, PDF, and SVG options.
    /// </summary>
    [StaFact]
    public void FileTypeComboBox_HasThreeOptions()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("FileTypeComboBox");

        // Assert
        Assert.NotNull(comboBox);
        Assert.Equal(3, comboBox.Items.Count);
    }

    /// <summary>
    /// Verifies that PNG is the default selected file type.
    /// </summary>
    [StaFact]
    public void FileTypeComboBox_DefaultIsPng()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("FileTypeComboBox");
        var selectedItem = (System.Windows.Controls.ComboBoxItem)comboBox!.SelectedItem;

        // Assert
        Assert.Equal(0, comboBox.SelectedIndex);
        Assert.Equal("PNG (.png)", selectedItem.Content.ToString());
    }

    /// <summary>
    /// Verifies that all file type options have correct display text.
    /// </summary>
    [StaFact]
    public void FileTypeComboBox_HasCorrectDisplayText()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);
        var comboBox = (System.Windows.Controls.ComboBox)dialog.FindName("FileTypeComboBox");

        // Act & Assert
        var pngItem = (System.Windows.Controls.ComboBoxItem)comboBox!.Items[0];
        var pdfItem = (System.Windows.Controls.ComboBoxItem)comboBox.Items[1];
        var svgItem = (System.Windows.Controls.ComboBoxItem)comboBox.Items[2];

        Assert.Equal("PNG (.png)", pngItem.Content.ToString());
        Assert.Equal("PDF (.pdf)", pdfItem.Content.ToString());
        Assert.Equal("SVG (.svg)", svgItem.Content.ToString());
    }

    #endregion

    #region File Name Validation Tests

    /// <summary>
    /// Verifies that the NameTextBox is configured to not allow blank names.
    /// </summary>
    [StaFact]
    public void FileNameTextBox_CannotBeBlank()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var nameBox = (GenericControls.NameTextBox)dialog.FindName("FileNameTextBox");

        // Assert
        Assert.NotNull(nameBox);
        Assert.False(nameBox.CanBeBlank);
    }

    /// <summary>
    /// Verifies that the NameTextBox character limit is 200.
    /// </summary>
    [StaFact]
    public void FileNameTextBox_CharacterLimitIs200()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var nameBox = (GenericControls.NameTextBox)dialog.FindName("FileNameTextBox");

        // Assert
        Assert.NotNull(nameBox);
        Assert.Equal(200, nameBox.CharacterLimit);
    }

    /// <summary>
    /// Verifies that setting InvalidStrings marks a matching name as invalid.
    /// </summary>
    [StaFact]
    public void FileNameTextBox_DuplicateName_IsInvalid()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);
        var nameBox = (GenericControls.NameTextBox)dialog.FindName("FileNameTextBox");

        // Act
        nameBox!.InvalidStrings = new[] { "existing_file", "another_file" };
        nameBox.Text = "existing_file";

        // Assert
        Assert.False(nameBox.IsValid);
    }

    /// <summary>
    /// Verifies that a unique name is valid when InvalidStrings are set.
    /// </summary>
    [StaFact]
    public void FileNameTextBox_UniqueName_IsValid()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);
        var nameBox = (GenericControls.NameTextBox)dialog.FindName("FileNameTextBox");

        // Act
        nameBox!.InvalidStrings = new[] { "existing_file", "another_file" };
        nameBox.Text = "new_unique_name";

        // Assert
        Assert.True(nameBox.IsValid);
    }

    #endregion

    #region Preview Container Tests

    /// <summary>
    /// Verifies that the preview container and image control exist.
    /// </summary>
    [StaFact]
    public void PreviewContainer_Exists()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var container = (System.Windows.Controls.Grid)dialog.FindName("PreviewContainer");
        var image = (System.Windows.Controls.Image)dialog.FindName("PreviewImage");

        // Assert
        Assert.NotNull(container);
        Assert.NotNull(image);
    }

    /// <summary>
    /// Verifies that the preview image uses high quality bitmap scaling.
    /// </summary>
    [StaFact]
    public void PreviewImage_UsesHighQualityScaling()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var image = (System.Windows.Controls.Image)dialog.FindName("PreviewImage");

        // Assert
        Assert.NotNull(image);
        Assert.Equal(System.Windows.Media.BitmapScalingMode.HighQuality,
                     System.Windows.Media.RenderOptions.GetBitmapScalingMode(image));
    }

    #endregion

    #region Button Tests

    /// <summary>
    /// Verifies that the Save button exists and is the default button.
    /// </summary>
    [StaFact]
    public void SaveButton_IsDefault()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var saveButton = (System.Windows.Controls.Button)dialog.FindName("SaveButton");

        // Assert
        Assert.NotNull(saveButton);
        Assert.True(saveButton.IsDefault);
        Assert.Equal("Save", saveButton.Content.ToString());
    }

    /// <summary>
    /// Verifies that the Cancel button exists and is the cancel button.
    /// </summary>
    [StaFact]
    public void CancelButton_IsCancel()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var cancelButton = (System.Windows.Controls.Button)dialog.FindName("CancelButton");

        // Assert
        Assert.NotNull(cancelButton);
        Assert.True(cancelButton.IsCancel);
        Assert.Equal("Cancel", cancelButton.Content.ToString());
    }

    /// <summary>
    /// Verifies that both buttons have consistent dimensions matching NameDialog pattern.
    /// </summary>
    [StaFact]
    public void Buttons_HaveConsistentDimensions()
    {
        // Arrange
        var plot = CreateTestPlot();
        var dialog = new OxyPlotControls.SavePlotImageDialog(plot);

        // Act
        var saveButton = (System.Windows.Controls.Button)dialog.FindName("SaveButton");
        var cancelButton = (System.Windows.Controls.Button)dialog.FindName("CancelButton");

        // Assert - both should be 75x24 matching NameDialog pattern
        Assert.Equal(75, saveButton!.Width);
        Assert.Equal(24, saveButton.Height);
        Assert.Equal(75, cancelButton!.Width);
        Assert.Equal(24, cancelButton.Height);
    }

    #endregion

    #region Export Integration Tests

    /// <summary>
    /// Verifies that a PNG file can be exported to a temporary directory.
    /// </summary>
    [StaFact]
    public void Export_Png_CreatesFile()
    {
        // Arrange
        var plot = CreateTestPlot();
        string tempDir = Path.Combine(Path.GetTempPath(), "SavePlotImageDialogTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            string filePath = Path.Combine(tempDir, "test_export.png");

            // Act
            plot.SaveBitmap(filePath, 600, 480, OxyColors.White);

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.True(new FileInfo(filePath).Length > 0);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// Verifies that an SVG file can be exported to a temporary directory.
    /// </summary>
    [StaFact]
    public void Export_Svg_CreatesFile()
    {
        // Arrange
        var plot = CreateTestPlot();
        string tempDir = Path.Combine(Path.GetTempPath(), "SavePlotImageDialogTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            string filePath = Path.Combine(tempDir, "test_export.svg");

            // Act
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                OxyPlot.SvgExporter.Export(plot.Model, fs, 600, 480, true);
            }

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.True(new FileInfo(filePath).Length > 0);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// Verifies that a PDF file can be exported to a temporary directory.
    /// </summary>
    [StaFact]
    public void Export_Pdf_CreatesFile()
    {
        // Arrange
        var plot = CreateTestPlot();
        string tempDir = Path.Combine(Path.GetTempPath(), "SavePlotImageDialogTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            string filePath = Path.Combine(tempDir, "test_export.pdf");

            // Act
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
#pragma warning disable CS0618
                OxyPlot.PdfExporter.Export(plot.Model, fs, 600, 480);
#pragma warning restore CS0618
            }

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.True(new FileInfo(filePath).Length > 0);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    #endregion
}
