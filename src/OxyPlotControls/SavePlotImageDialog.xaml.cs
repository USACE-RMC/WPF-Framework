/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GenericControls;
using OxyPlot.Wpf;

namespace OxyPlotControls
{
    /// <summary>
    /// A dialog window for saving OxyPlot charts as image files.
    /// Provides a live plot preview that scales with the selected export dimensions,
    /// and inline controls for file type, directory, and file name.
    /// Supports PNG, PDF, and SVG formats with customizable dimensions.
    /// </summary>
    public partial class SavePlotImageDialog : MetroDialogWindow
    {
        private Plot _sourcePlot;
        private DispatcherTimer _resizeDebounceTimer;
        private bool _isInitialized;
        private bool _useReportTheme = true;
        private static string _lastUsedFolderPath = "";
        private double _desiredPreviewWidth;
        private double _desiredPreviewHeight;

        // Layout constants for dialog sizing calculations.
        // These estimate the non-preview space (controls, chrome, margins) so the dialog
        // is sized appropriately. The preview itself uses explicit dimensions and does not
        // depend on these constants being pixel-perfect.
        private const double ControlsPanelHeight = 190;
        private const double DialogChromeHeight = 50;
        private const double DialogHorizontalPadding = 36;
        private const double MinPreviewWidth = 280;
        private const double MinPreviewHeight = 180;
        private const double ScreenUsageFraction = 0.95;

        /// <summary>
        /// Initializes a new instance of the <see cref="SavePlotImageDialog"/> class.
        /// </summary>
        /// <param name="thePlot">The OxyPlot Plot control to save as an image.</param>
        public SavePlotImageDialog(Plot thePlot)
        {
            InitializeComponent();

            _sourcePlot = thePlot;

            // Set theme-aware window icon
            SetThemeAwareIcon();

            _resizeDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _resizeDebounceTimer.Tick += ResizeDebounceTimer_Tick;

            // Compute correct initial size from the default combo selection before the window is shown,
            // so WindowStartupLocation="CenterOwner" centers at the right size with no flash/shift.
            ParseDimensionsFromComboBox();
            if (TryGetExportDimensions(out int initWidth, out int initHeight))
            {
                var (w, h, pw, ph) = ComputeDialogSize(initWidth, initHeight);
                Width = w;
                Height = h;
                _desiredPreviewWidth = pw;
                _desiredPreviewHeight = ph;
            }

            ContentRendered += SavePlotImageDialog_ContentRendered;
            Closing += SavePlotImageDialog_Closing;
        }

        /// <summary>
        /// Handles the ContentRendered event to initialize the dialog with default values and render the first preview.
        /// </summary>
        private void SavePlotImageDialog_ContentRendered(object? sender, EventArgs e)
        {
            // Set tooltips showing current plot dimensions
            WidthTextBox.ToolTip = "Current Plot Width is " + ((int)_sourcePlot.ActualWidth).ToString() + " px";
            HeightTextBox.ToolTip = "Current Plot Height is " + ((int)_sourcePlot.ActualHeight).ToString() + " px";

            // Initialize folder path: last used > Pictures > Documents
            if (!string.IsNullOrEmpty(_lastUsedFolderPath) && Directory.Exists(_lastUsedFolderPath))
                FolderPathControl.Text = _lastUsedFolderPath;
            else
            {
                string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                FolderPathControl.Text = Directory.Exists(picturesPath) ? picturesPath
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            // Initialize file name
            FileNameTextBox.Text = "plot";

            // Populate existing file names for duplicate detection
            UpdateExistingFileNames();

            _isInitialized = true;

            // Defer first preview render until layout is complete
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(UpdatePreview));
        }

        /// <summary>
        /// Handles the Closing event to activate the owner window and persist the folder path.
        /// </summary>
        private void SavePlotImageDialog_Closing(object? sender, CancelEventArgs e)
        {
            // Stop the resize-debounce timer so a pending Tick doesn't fire after the
            // visual tree has been torn down and UpdatePreview() accesses disposed elements.
            _resizeDebounceTimer.Stop();
            _resizeDebounceTimer.Tick -= ResizeDebounceTimer_Tick;

            if (!string.IsNullOrEmpty(FolderPathControl.Text))
                _lastUsedFolderPath = FolderPathControl.Text;

            if (Owner != null) Owner.Activate();
        }

        /// <summary>
        /// Creates and sets a theme-aware vector icon for the dialog window.
        /// Uses the EnvironmentWindowText color so the icon adapts to light/dark themes.
        /// </summary>
        private void SetThemeAwareIcon()
        {
            try
            {
                var iconBrush = FindResource("EnvironmentWindowText") as SolidColorBrush ?? Brushes.Black;

                // Camera/save icon: simple camera shape matching the toolbar CameraIcon
                var geometry = Geometry.Parse(
                    "F0 M 5.40625 1.828125 C 5.2435365 1.828125 5.1126068 1.9997109 5.03125 2.140625 " +
                    "C 4.7248392 2.671344 4.09375 3.734375 4.09375 3.734375 L 2.4375 3.734375 " +
                    "C 1.5394159 3.734375 0 5.1192214 0 6.265625 L 0 12.015625 " +
                    "C 0 13.322471 1.2946973 14.734375 2.40625 14.734375 L 7.96875 14.734375 " +
                    "L 13.53125 14.734375 C 14.642803 14.734375 16 13.322471 16 12.015625 " +
                    "L 16 6.265625 C 16 5.1192214 14.398084 3.734375 13.5 3.734375 " +
                    "L 11.84375 3.734375 C 11.84375 3.734375 11.212661 2.671344 10.90625 2.140625 " +
                    "C 10.82489 1.9997109 10.693963 1.828125 10.53125 1.828125 " +
                    "L 7.96875 1.828125 L 5.40625 1.828125 z " +
                    "M 8 4.8203125 C 10.421762 4.8203125 12.400391 6.7989415 12.400391 9.2207031 " +
                    "C 12.400391 11.642465 10.421762 13.619141 8 13.619141 " +
                    "C 5.5782383 13.619141 3.5996094 11.642465 3.5996094 9.2207031 " +
                    "C 3.5996094 6.7989415 5.5782383 4.8203125 8 4.8203125 z " +
                    "M 8 6.2207031 C 6.3348545 6.2207031 5 7.5555576 5 9.2207031 " +
                    "C 5 10.885849 6.3348545 12.220703 8 12.220703 " +
                    "C 9.6651455 12.220703 11 10.885849 11 9.2207031 " +
                    "C 11 7.5555576 9.6651455 6.2207031 8 6.2207031 z");

                var drawing = new GeometryDrawing(iconBrush, null, geometry);
                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();
                Icon = drawingImage;
            }
            catch
            {
                // Fall back gracefully - no icon if resource resolution fails
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ImageSizeComboBox to update width and height fields
        /// and resize the dialog.
        /// </summary>
        private void ImageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (WidthTextBox == null || HeightTextBox == null) return;
            if (ImageSizeComboBox.SelectedIndex < 0) return;

            bool isCustom = ImageSizeComboBox.SelectedIndex == ImageSizeComboBox.Items.Count - 1;
            CustomDimensionsPanel.Visibility = isCustom ? Visibility.Visible : Visibility.Collapsed;
            WidthTextBox.IsEnabled = isCustom;
            HeightTextBox.IsEnabled = isCustom;

            if (isCustom)
            {
                WidthTextBox.Text = ((int)_sourcePlot.ActualWidth).ToString();
                HeightTextBox.Text = ((int)_sourcePlot.ActualHeight).ToString();
            }
            else
            {
                ParseDimensionsFromComboBox();
            }

            UpdateDialogSize();
            UpdatePreview();
        }

        /// <summary>
        /// Parses width and height values from the currently selected ComboBox preset item.
        /// </summary>
        private void ParseDimensionsFromComboBox()
        {
            if (ImageSizeComboBox.SelectedIndex < 0) return;
            if (ImageSizeComboBox.SelectedIndex == ImageSizeComboBox.Items.Count - 1) return;

            var size = ((ComboBoxItem)ImageSizeComboBox.SelectedItem).Content.ToString()!;
            var sizes = size.Split(' ');
            if (sizes.Length >= 3)
            {
                WidthTextBox.Text = sizes[0];
                HeightTextBox.Text = sizes[2];
            }
        }

        /// <summary>
        /// Handles dimension text changes with debouncing.
        /// </summary>
        private void DimensionTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _resizeDebounceTimer.Stop();
            _resizeDebounceTimer.Start();
        }

        /// <summary>
        /// Handles the Checked/Unchecked events of the ReportThemeCheckBox to toggle the report theme for preview and export.
        /// </summary>
        private void ReportThemeCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            _useReportTheme = ReportThemeCheckBox.IsChecked == true;
            if (_isInitialized)
                UpdatePreview();
        }

        /// <summary>
        /// Fires after the debounce delay to update the dialog size and preview.
        /// </summary>
        private void ResizeDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _resizeDebounceTimer.Stop();
            UpdateDialogSize();
            UpdatePreview();
        }

        /// <summary>
        /// Computes the optimal dialog dimensions for the given export size.
        /// Used both for initial sizing (before the window is shown) and subsequent resizes.
        /// </summary>
        private static (double dialogWidth, double dialogHeight, double previewWidth, double previewHeight) ComputeDialogSize(int exportWidth, int exportHeight)
        {
            // Get available screen space
            var workArea = SystemParameters.WorkArea;
            double maxDialogWidth = workArea.Width * ScreenUsageFraction;
            double maxDialogHeight = workArea.Height * ScreenUsageFraction;

            // Compute available preview space
            double availablePreviewWidth = maxDialogWidth - DialogHorizontalPadding;
            double availablePreviewHeight = maxDialogHeight - ControlsPanelHeight - DialogChromeHeight;

            // Scale to fit — never upscale beyond the export dimensions
            double scaleX = availablePreviewWidth / exportWidth;
            double scaleY = availablePreviewHeight / exportHeight;
            double scale = Math.Min(scaleX, scaleY);
            if (scale > 1.0) scale = 1.0;

            double previewWidth = Math.Max(exportWidth * scale, MinPreviewWidth);
            double previewHeight = Math.Max(exportHeight * scale, MinPreviewHeight);

            // Compute dialog dimensions
            double dialogWidth = previewWidth + DialogHorizontalPadding;
            double dialogHeight = previewHeight + ControlsPanelHeight + DialogChromeHeight;

            // Enforce min/max
            dialogWidth = Math.Clamp(dialogWidth, 500, maxDialogWidth);
            dialogHeight = Math.Clamp(dialogHeight, 450, maxDialogHeight);

            return (dialogWidth, dialogHeight, previewWidth, previewHeight);
        }

        /// <summary>
        /// Resizes the dialog to reflect the selected export dimensions while maintaining aspect ratio.
        /// The dialog is capped at 95% of the screen working area.
        /// </summary>
        private void UpdateDialogSize()
        {
            if (!_isInitialized) return;
            if (!TryGetExportDimensions(out int exportWidth, out int exportHeight)) return;

            var (dialogWidth, dialogHeight, previewWidth, previewHeight) = ComputeDialogSize(exportWidth, exportHeight);
            Width = dialogWidth;
            Height = dialogHeight;
            _desiredPreviewWidth = previewWidth;
            _desiredPreviewHeight = previewHeight;

            // Re-center on owner, clamped to the work area
            if (Owner != null)
            {
                var workArea = SystemParameters.WorkArea;
                double newLeft = Owner.Left + (Owner.ActualWidth - Width) / 2;
                double newTop = Owner.Top + (Owner.ActualHeight - Height) / 2;

                // Clamp to work area so the title bar stays accessible
                Left = Math.Max(workArea.Left, Math.Min(newLeft, workArea.Right - Width));
                Top = Math.Max(workArea.Top, Math.Min(newTop, workArea.Bottom - Height));
            }
        }

        /// <summary>
        /// Renders the plot preview at the appropriate size within the preview container.
        /// Uses OxyPlot's PngExporter to generate a WYSIWYG bitmap preview.
        /// </summary>
        private void UpdatePreview()
        {
            if (!_isInitialized) return;
            if (_sourcePlot?.ActualModel == null) return;
            if (!TryGetExportDimensions(out int exportWidth, out int exportHeight)) return;

            if (_desiredPreviewWidth <= 0 || _desiredPreviewHeight <= 0) return;

            // Render at exactly the desired preview dimensions so the preview
            // is always the correct size, regardless of how much space the container has.
            int previewWidth = Math.Max((int)_desiredPreviewWidth, 100);
            int previewHeight = Math.Max((int)_desiredPreviewHeight, 100);

            // Set explicit dimensions on the Image — with Stretch="None" and Center alignment,
            // the preview is always exactly the right size even if the container is slightly larger.
            PreviewImage.Width = previewWidth;
            PreviewImage.Height = previewHeight;

            try
            {
                if (_useReportTheme)
                {
                    OxyPlotThemeManager.WithThemedModel(_sourcePlot.ActualModel, OxyPlotThemeManager.ReportTheme, model =>
                    {
                        var exporter = new PngExporter
                        {
                            Width = previewWidth,
                            Height = previewHeight,
                            Background = model.Background
                        };
                        PreviewImage.Source = exporter.ExportToBitmap(model);
                    });
                }
                else
                {
                    var exporter = new PngExporter
                    {
                        Width = previewWidth,
                        Height = previewHeight,
                        Background = _sourcePlot.ActualModel.Background
                    };
                    PreviewImage.Source = exporter.ExportToBitmap(_sourcePlot.ActualModel);
                }
            }
            catch (Exception)
            {
                // Silently handle preview render failures
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the preview container to update the preview.
        /// </summary>
        private void PreviewContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isInitialized) return;
            UpdatePreview();
        }

        /// <summary>
        /// Tries to parse the current width and height values from the dimension controls.
        /// </summary>
        private bool TryGetExportDimensions(out int width, out int height)
        {
            width = 0;
            height = 0;

            if (!WidthTextBox.ValueIsValid || !HeightTextBox.ValueIsValid) return false;

            width = WidthTextBox.GetValueAsInteger();
            height = HeightTextBox.GetValueAsInteger();

            return width > 0 && height > 0;
        }

        /// <summary>
        /// Gets the file extension for the currently selected file type.
        /// </summary>
        private string GetSelectedFileExtension()
        {
            return FileTypeComboBox.SelectedIndex switch
            {
                0 => ".png",
                1 => ".pdf",
                2 => ".svg",
                _ => ".png"
            };
        }

        /// <summary>
        /// Handles the Save button click event to validate inputs and export the plot.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourcePlot == null) return;
            if (_sourcePlot.ActualModel == null)
            {
                GenericControls.MessageBox.Show("The plot has no model to export.", "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate folder
            string folderPath = FolderPathControl.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                GenericControls.MessageBox.Show("The specified directory does not exist. Please select a valid folder.", "Invalid Folder", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate file name
            if (!FileNameTextBox.IsValid)
            {
                var errors = FileNameTextBox.GetErrorMessages();
                string errorMessage = errors.Count > 0 ? string.Join("\n", errors) : "File name is not valid.";
                GenericControls.MessageBox.Show(errorMessage, "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fileName = FileNameTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(fileName))
            {
                GenericControls.MessageBox.Show("Please enter a file name.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate dimensions
            if (!TryGetExportDimensions(out int imageWidth, out int imageHeight))
            {
                GenericControls.MessageBox.Show("Image dimensions are not valid. Width and height must be between 50 and 10,000 pixels.", "Invalid Dimensions", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Construct full file path
            string extension = GetSelectedFileExtension();
            string saveFile = Path.Combine(folderPath, fileName + extension);

            // Export
            try
            {
                if (_useReportTheme)
                {
                    OxyPlotThemeManager.WithThemedModel(_sourcePlot.ActualModel, OxyPlotThemeManager.ReportTheme, model =>
                    {
                        ExportModel(model, saveFile, extension, imageWidth, imageHeight);
                    });
                }
                else
                {
                    ExportModel(_sourcePlot.ActualModel, saveFile, extension, imageWidth, imageHeight);
                }
            }
            catch (Exception ex)
            {
                GenericControls.MessageBox.Show("Error occurred attempting to save the image file: " + ex.Message, "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _lastUsedFolderPath = folderPath;
            Close();
        }

        /// <summary>
        /// Exports the given PlotModel to the specified file using the appropriate exporter.
        /// </summary>
        private void ExportModel(OxyPlot.PlotModel model, string filePath, string extension, int width, int height)
        {
            switch (extension)
            {
                case ".png":
                    var exporter = new PngExporter { Width = width, Height = height, Background = model.Background };
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        exporter.Export(model, fs);
                    }
                    break;
                case ".svg":
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        OxyPlot.Wpf.WpfSvgExporter.Export(model, fs, width, height, true);
                    }
                    break;
                case ".pdf":
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        OxyPlot.Wpf.WpfPdfExporter.Export(model, fs, width, height);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the Cancel button click event to close the dialog without saving.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the FileTypeComboBox to refresh the existing file names list.
        /// </summary>
        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            UpdateExistingFileNames();
        }

        /// <summary>
        /// Handles the TextChanged event of the FolderPathControl to refresh the existing file names list.
        /// </summary>
        private void FolderPathControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            UpdateExistingFileNames();
        }

        /// <summary>
        /// Updates the NameTextBox InvalidStrings with existing file names (without extension)
        /// in the selected directory that match the selected file type.
        /// </summary>
        private void UpdateExistingFileNames()
        {
            string folder = FolderPathControl.Text?.Trim() ?? "";
            if (!Directory.Exists(folder))
            {
                FileNameTextBox.InvalidStrings = Array.Empty<string>();
                return;
            }

            try
            {
                string extension = GetSelectedFileExtension();
                var existingNames = Directory.GetFiles(folder, "*" + extension)
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToArray();
                FileNameTextBox.InvalidStrings = existingNames;
            }
            catch (Exception)
            {
                FileNameTextBox.InvalidStrings = Array.Empty<string>();
            }
        }
    }
}
