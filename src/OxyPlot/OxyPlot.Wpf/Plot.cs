// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plot.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a WPF control that displays a plot with support for declarative series, axes, and annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable annotations
#nullable disable warnings

namespace OxyPlot.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using OxyPlot.Legends;

    /// <summary>
    /// Represents a WPF control that displays a plot with support for declarative series, axes, and annotations.
    /// </summary>
    /// <remarks>
    /// This control extends <see cref="PlotView"/> to add ObservableCollections for Series, Axes, and Annotations,
    /// enabling declarative XAML-based plot construction with data binding support.
    /// </remarks>
    [ContentProperty("Series")]
    [TemplatePart(Name = PartGrid, Type = typeof(Grid))]
    public partial class Plot : PlotView, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes on this Plot or on any item in its
        /// Annotations, Series, or Axes collections.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For Plot-level properties, the property name is the DP name (e.g., "Title", "SubtitleColor").
        /// For collection structure changes, the property name is the collection name ("Annotations", "Series", "Axes").
        /// For item-level property changes, the property name is prefixed (e.g., "Annotation.Color", "Series.Title").
        /// </para>
        /// <para>
        /// This event is suppressed when <see cref="SuppressPropertyChanged"/> is true.
        /// </para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets whether <see cref="PropertyChanged"/> events are suppressed.
        /// When true, no PropertyChanged events fire from this Plot instance,
        /// including relayed item and collection change events.
        /// </summary>
        /// <remarks>
        /// Use this to suppress events during bulk operations such as deserialization,
        /// theme application, or programmatic series population where the undo system
        /// should not record individual changes.
        /// </remarks>
        public bool SuppressPropertyChanged { get; set; }

        /// <summary>
        /// Occurs when an axis is replaced via <see cref="ReplaceAxis"/>,
        /// typically due to an axis type change (e.g., Linear to Logarithmic).
        /// The first parameter is the old axis, the second is the new axis.
        /// </summary>
        public event Action<Axis, Axis>? AxisReplaced;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event if not suppressed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (SuppressPropertyChanged) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="Subtitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
            nameof(Subtitle),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register(
            nameof(TitleColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="SubtitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleColorProperty = DependencyProperty.Register(
            nameof(SubtitleColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="PlotAreaBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBackgroundProperty = DependencyProperty.Register(
            nameof(PlotAreaBackground),
            typeof(Brush),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="PlotAreaBorderColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBorderColorProperty = DependencyProperty.Register(
            nameof(PlotAreaBorderColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(Colors.Black, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="PlotAreaBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBorderThicknessProperty = DependencyProperty.Register(
            nameof(PlotAreaBorderThickness),
            typeof(Thickness),
            typeof(Plot),
            new PropertyMetadata(new Thickness(1), AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="IsLegendVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLegendVisibleProperty = DependencyProperty.Register(
            nameof(IsLegendVisible),
            typeof(bool),
            typeof(Plot),
            new PropertyMetadata(true, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            nameof(TextColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(Colors.Black, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontProperty = DependencyProperty.Register(
            nameof(TitleFont),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(
            nameof(TitleFontSize),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(18.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontWeightProperty = DependencyProperty.Register(
            nameof(TitleFontWeight),
            typeof(FontWeight),
            typeof(Plot),
            new PropertyMetadata(FontWeights.Bold, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TitlePadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitlePaddingProperty = DependencyProperty.Register(
            nameof(TitlePadding),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(6.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="SubtitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontProperty = DependencyProperty.Register(
            nameof(SubtitleFont),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="SubtitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontSizeProperty = DependencyProperty.Register(
            nameof(SubtitleFontSize),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(14.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="SubtitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontWeightProperty = DependencyProperty.Register(
            nameof(SubtitleFontWeight),
            typeof(FontWeight),
            typeof(Plot),
            new PropertyMetadata(FontWeights.Normal, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="DefaultPlotCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultPlotCursorProperty = DependencyProperty.Register(
            nameof(DefaultPlotCursor),
            typeof(System.Windows.Input.Cursor),
            typeof(Plot),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LegendBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty = DependencyProperty.Register(
            nameof(LegendBackground),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Undefined, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendBorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBorderProperty = DependencyProperty.Register(
            nameof(LegendBorder),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Undefined, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBorderThicknessProperty = DependencyProperty.Register(
            nameof(LegendBorderThickness),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(1.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendItemAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemAlignmentProperty = DependencyProperty.Register(
            nameof(LegendItemAlignment),
            typeof(System.Windows.HorizontalAlignment),
            typeof(Plot),
            new PropertyMetadata(System.Windows.HorizontalAlignment.Left, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendItemOrder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemOrderProperty = DependencyProperty.Register(
            nameof(LegendItemOrder),
            typeof(OxyPlot.Legends.LegendItemOrder),
            typeof(Plot),
            new PropertyMetadata(OxyPlot.Legends.LegendItemOrder.Normal, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendItemSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemSpacingProperty = DependencyProperty.Register(
            nameof(LegendItemSpacing),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(24.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendLineSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendLineSpacingProperty = DependencyProperty.Register(
            nameof(LegendLineSpacing),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(0.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMarginProperty = DependencyProperty.Register(
            nameof(LegendMargin),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(8.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendMaxHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMaxHeightProperty = DependencyProperty.Register(
            nameof(LegendMaxHeight),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(double.NaN, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendMaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMaxWidthProperty = DependencyProperty.Register(
            nameof(LegendMaxWidth),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(double.NaN, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendOrientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendOrientationProperty = DependencyProperty.Register(
            nameof(LegendOrientation),
            typeof(OxyPlot.Legends.LegendOrientation),
            typeof(Plot),
            new PropertyMetadata(OxyPlot.Legends.LegendOrientation.Vertical, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPaddingProperty = DependencyProperty.Register(
            nameof(LegendPadding),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(8.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPlacementProperty = DependencyProperty.Register(
            nameof(LegendPlacement),
            typeof(OxyPlot.Legends.LegendPlacement),
            typeof(Plot),
            new PropertyMetadata(OxyPlot.Legends.LegendPlacement.Inside, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty = DependencyProperty.Register(
            nameof(LegendPosition),
            typeof(OxyPlot.Legends.LegendPosition),
            typeof(Plot),
            new PropertyMetadata(OxyPlot.Legends.LegendPosition.RightTop, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendSymbolLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolLengthProperty = DependencyProperty.Register(
            nameof(LegendSymbolLength),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(16.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendSymbolMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolMarginProperty = DependencyProperty.Register(
            nameof(LegendSymbolMargin),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(4.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendSymbolPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolPlacementProperty = DependencyProperty.Register(
            nameof(LegendSymbolPlacement),
            typeof(OxyPlot.Legends.LegendSymbolPlacement),
            typeof(Plot),
            new PropertyMetadata(OxyPlot.Legends.LegendSymbolPlacement.Left, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTextColorProperty = DependencyProperty.Register(
            nameof(LegendTextColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleProperty = DependencyProperty.Register(
            nameof(LegendTitle),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleColorProperty = DependencyProperty.Register(
            nameof(LegendTitleColor),
            typeof(Color),
            typeof(Plot),
            new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontProperty = DependencyProperty.Register(
            nameof(LegendTitleFont),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontSizeProperty = DependencyProperty.Register(
            nameof(LegendTitleFontSize),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(double.NaN, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontWeightProperty = DependencyProperty.Register(
            nameof(LegendTitleFontWeight),
            typeof(FontWeight),
            typeof(Plot),
            new PropertyMetadata(FontWeights.Bold, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendColumnSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendColumnSpacingProperty = DependencyProperty.Register(
            nameof(LegendColumnSpacing),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(8.0, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontProperty = DependencyProperty.Register(
            nameof(LegendFont),
            typeof(string),
            typeof(Plot),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty = DependencyProperty.Register(
            nameof(LegendFontSize),
            typeof(double),
            typeof(Plot),
            new PropertyMetadata(double.NaN, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="LegendFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty = DependencyProperty.Register(
            nameof(LegendFontWeight),
            typeof(FontWeight),
            typeof(Plot),
            new PropertyMetadata(FontWeights.Normal, AppearanceChanged));

        /// <summary>
        /// The series collection.
        /// </summary>
        private readonly ObservableCollection<Series> series;

        /// <summary>
        /// The axes collection.
        /// </summary>
        private readonly ObservableCollection<Axis> axes;

        /// <summary>
        /// The annotations collection.
        /// </summary>
        private readonly ObservableCollection<Annotation> annotations;

        /// <summary>
        /// Shadow list tracking Series items subscribed to PropertyChanged.
        /// Required to unsubscribe on Reset (Clear), which does not provide OldItems.
        /// </summary>
        private readonly List<Series> subscribedSeries = new List<Series>();

        /// <summary>
        /// Shadow list tracking Axis items subscribed to PropertyChanged.
        /// </summary>
        private readonly List<Axis> subscribedAxes = new List<Axis>();

        /// <summary>
        /// Shadow list tracking Annotation items subscribed to PropertyChanged.
        /// </summary>
        private readonly List<Annotation> subscribedAnnotations = new List<Annotation>();

        /// <summary>
        /// The internal plot model.
        /// </summary>
        private readonly PlotModel internalModel;

        /// <summary>
        /// When true, the next <see cref="InvalidatePlot"/> call will re-synchronize
        /// all WPF wrapper properties to the internal OxyPlot model. Set by any mutation
        /// (DP changes, collection changes). Cleared after synchronization completes.
        /// This avoids expensive re-synchronization during zoom/pan, where only
        /// internal axis ranges change and the WPF wrappers are untouched.
        /// </summary>
        internal bool _needsSynchronization = true;

        /// <summary>
        /// Initializes static members of the <see cref="Plot"/> class.
        /// </summary>
        static Plot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Plot), new FrameworkPropertyMetadata(typeof(PlotViewBase)));
            PaddingProperty.OverrideMetadata(typeof(Plot), new FrameworkPropertyMetadata(new Thickness(8), AppearanceChanged));
            BackgroundProperty.OverrideMetadata(typeof(Plot), new FrameworkPropertyMetadata(null, AppearanceChanged));
            BorderBrushProperty.OverrideMetadata(typeof(Plot), new FrameworkPropertyMetadata(null, AppearanceChanged));
            BorderThicknessProperty.OverrideMetadata(typeof(Plot), new FrameworkPropertyMetadata(new Thickness(0), AppearanceChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plot"/> class.
        /// </summary>
        public Plot()
        {
            this.series = new ObservableCollection<Series>();
            this.axes = new ObservableCollection<Axis>();
            this.annotations = new ObservableCollection<Annotation>();

            this.series.CollectionChanged += this.OnSeriesChanged;
            this.axes.CollectionChanged += this.OnAxesChanged;
            this.annotations.CollectionChanged += this.OnAnnotationsChanged;

            this.internalModel = new PlotModel();
            ((IPlotModel)this.internalModel).AttachPlotView(this);
        }

        /// <summary>
        /// Gets the series collection.
        /// </summary>
        public ObservableCollection<Series> Series => this.series;

        /// <summary>
        /// Gets the axes collection.
        /// </summary>
        public ObservableCollection<Axis> Axes => this.axes;

        /// <summary>
        /// Gets the annotations collection.
        /// </summary>
        public ObservableCollection<Annotation> Annotations => this.annotations;

        /// <summary>
        /// Gets the actual model.
        /// </summary>
        public override PlotModel ActualModel => this.internalModel;

        /// <summary>
        /// Overlay canvas for backward compatibility when using the DrawingVisual backend.
        /// Provides a transparent canvas that toolbar and other code can add overlay elements to.
        /// </summary>
        private Canvas _overlayCanvas;

        /// <summary>
        /// Gets the canvas used for rendering.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When using the Canvas backend, returns the actual rendering canvas (identical to previous behavior).
        /// When using the DrawingVisual backend, returns a transparent overlay canvas that is
        /// automatically inserted into the grid. This allows toolbar code to continue adding
        /// overlay elements (leader lines, text editing controls) without modification.
        /// </para>
        /// </remarks>
        public Canvas canvas
        {
            get
            {
                if (this.plotPresenter is Canvas c)
                {
                    return c;
                }

                // DrawingVisual backend: return overlay canvas for toolbar compatibility.
                if (this._overlayCanvas == null)
                {
                    this._overlayCanvas = new Canvas { IsHitTestVisible = false };
                    if (this.grid != null)
                    {
                        var idx = this.grid.Children.IndexOf(this.plotPresenter);
                        this.grid.Children.Insert(idx + 1, this._overlayCanvas);
                    }
                }

                return this._overlayCanvas;
            }
        }

        /// <summary>
        /// Gets the grid used for layout.
        /// </summary>
        /// <remarks>
        /// This property is provided for backward compatibility with code that accesses the grid directly.
        /// </remarks>
        public new Grid grid => base.grid;

        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <remarks>
        /// This property is provided for backward compatibility with code that accesses the render context directly.
        /// </remarks>
        public IRenderContext RenderContext => this.renderContext;

        /// <summary>
        /// Hit-tests rendered text elements at the given point.
        /// </summary>
        /// <param name="point">The point in plot coordinates.</param>
        /// <returns>
        /// For the Canvas backend: delegates to <c>Canvas.InputHitTest</c> (returns a
        /// <see cref="System.Windows.Controls.TextBlock"/>, <see cref="System.Windows.Shapes.Path"/>, or null).
        /// For the DrawingVisual backend: queries recorded text positions (returns a
        /// <see cref="DrawingVisualRenderContext.TextHitResult"/> or null).
        /// </returns>
        /// <remarks>
        /// This method provides backend-agnostic text hit-testing for the OxyPlotToolbar.
        /// Callers should check the return type to determine the backend and extract the text content.
        /// </remarks>
        public object HitTestRenderedText(Point point)
        {
            if (this.plotPresenter is Canvas c)
            {
                return c.InputHitTest(point);
            }

            if (this.renderContext is DrawingVisualRenderContext dvrc)
            {
                return dvrc.HitTestText(new ScreenPoint(point.X, point.Y));
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the plot title.
        /// </summary>
        public string Title
        {
            get => (string)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the plot subtitle.
        /// </summary>
        public string Subtitle
        {
            get => (string)this.GetValue(SubtitleProperty);
            set => this.SetValue(SubtitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the title color.
        /// </summary>
        public Color TitleColor
        {
            get => (Color)this.GetValue(TitleColorProperty);
            set => this.SetValue(TitleColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the subtitle color.
        /// </summary>
        public Color SubtitleColor
        {
            get => (Color)this.GetValue(SubtitleColorProperty);
            set => this.SetValue(SubtitleColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the plot area background brush.
        /// </summary>
        public Brush PlotAreaBackground
        {
            get => (Brush)this.GetValue(PlotAreaBackgroundProperty);
            set => this.SetValue(PlotAreaBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the plot area border color.
        /// </summary>
        public Color PlotAreaBorderColor
        {
            get => (Color)this.GetValue(PlotAreaBorderColorProperty);
            set => this.SetValue(PlotAreaBorderColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the plot area border thickness.
        /// </summary>
        public Thickness PlotAreaBorderThickness
        {
            get => (Thickness)this.GetValue(PlotAreaBorderThicknessProperty);
            set => this.SetValue(PlotAreaBorderThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the legend is visible.
        /// </summary>
        public bool IsLegendVisible
        {
            get => (bool)this.GetValue(IsLegendVisibleProperty);
            set => this.SetValue(IsLegendVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets the default text color.
        /// </summary>
        public Color TextColor
        {
            get => (Color)this.GetValue(TextColorProperty);
            set => this.SetValue(TextColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the title font.
        /// </summary>
        public string TitleFont
        {
            get => (string)this.GetValue(TitleFontProperty);
            set => this.SetValue(TitleFontProperty, value);
        }

        /// <summary>
        /// Gets or sets the title font size.
        /// </summary>
        public double TitleFontSize
        {
            get => (double)this.GetValue(TitleFontSizeProperty);
            set => this.SetValue(TitleFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the title font weight.
        /// </summary>
        public FontWeight TitleFontWeight
        {
            get => (FontWeight)this.GetValue(TitleFontWeightProperty);
            set => this.SetValue(TitleFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the title padding.
        /// </summary>
        public double TitlePadding
        {
            get => (double)this.GetValue(TitlePaddingProperty);
            set => this.SetValue(TitlePaddingProperty, value);
        }

        /// <summary>
        /// Gets or sets the subtitle font.
        /// </summary>
        public string SubtitleFont
        {
            get => (string)this.GetValue(SubtitleFontProperty);
            set => this.SetValue(SubtitleFontProperty, value);
        }

        /// <summary>
        /// Gets or sets the subtitle font size.
        /// </summary>
        public double SubtitleFontSize
        {
            get => (double)this.GetValue(SubtitleFontSizeProperty);
            set => this.SetValue(SubtitleFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the subtitle font weight.
        /// </summary>
        public FontWeight SubtitleFontWeight
        {
            get => (FontWeight)this.GetValue(SubtitleFontWeightProperty);
            set => this.SetValue(SubtitleFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the default plot cursor.
        /// </summary>
        public System.Windows.Input.Cursor DefaultPlotCursor
        {
            get => (System.Windows.Input.Cursor)this.GetValue(DefaultPlotCursorProperty);
            set => this.SetValue(DefaultPlotCursorProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend background color.
        /// </summary>
        public Color LegendBackground
        {
            get => (Color)this.GetValue(LegendBackgroundProperty);
            set => this.SetValue(LegendBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend border color.
        /// </summary>
        public Color LegendBorder
        {
            get => (Color)this.GetValue(LegendBorderProperty);
            set => this.SetValue(LegendBorderProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend border thickness.
        /// </summary>
        public double LegendBorderThickness
        {
            get => (double)this.GetValue(LegendBorderThicknessProperty);
            set => this.SetValue(LegendBorderThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend item alignment.
        /// </summary>
        public System.Windows.HorizontalAlignment LegendItemAlignment
        {
            get => (System.Windows.HorizontalAlignment)this.GetValue(LegendItemAlignmentProperty);
            set => this.SetValue(LegendItemAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend item order.
        /// </summary>
        public OxyPlot.Legends.LegendItemOrder LegendItemOrder
        {
            get => (OxyPlot.Legends.LegendItemOrder)this.GetValue(LegendItemOrderProperty);
            set => this.SetValue(LegendItemOrderProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend item spacing.
        /// </summary>
        public double LegendItemSpacing
        {
            get => (double)this.GetValue(LegendItemSpacingProperty);
            set => this.SetValue(LegendItemSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend line spacing.
        /// </summary>
        public double LegendLineSpacing
        {
            get => (double)this.GetValue(LegendLineSpacingProperty);
            set => this.SetValue(LegendLineSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend margin.
        /// </summary>
        public double LegendMargin
        {
            get => (double)this.GetValue(LegendMarginProperty);
            set => this.SetValue(LegendMarginProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend maximum height.
        /// </summary>
        public double LegendMaxHeight
        {
            get => (double)this.GetValue(LegendMaxHeightProperty);
            set => this.SetValue(LegendMaxHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend maximum width.
        /// </summary>
        public double LegendMaxWidth
        {
            get => (double)this.GetValue(LegendMaxWidthProperty);
            set => this.SetValue(LegendMaxWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend orientation.
        /// </summary>
        public OxyPlot.Legends.LegendOrientation LegendOrientation
        {
            get => (OxyPlot.Legends.LegendOrientation)this.GetValue(LegendOrientationProperty);
            set => this.SetValue(LegendOrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend padding.
        /// </summary>
        public double LegendPadding
        {
            get => (double)this.GetValue(LegendPaddingProperty);
            set => this.SetValue(LegendPaddingProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend placement.
        /// </summary>
        public OxyPlot.Legends.LegendPlacement LegendPlacement
        {
            get => (OxyPlot.Legends.LegendPlacement)this.GetValue(LegendPlacementProperty);
            set => this.SetValue(LegendPlacementProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend position.
        /// </summary>
        public OxyPlot.Legends.LegendPosition LegendPosition
        {
            get => (OxyPlot.Legends.LegendPosition)this.GetValue(LegendPositionProperty);
            set => this.SetValue(LegendPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend symbol length.
        /// </summary>
        public double LegendSymbolLength
        {
            get => (double)this.GetValue(LegendSymbolLengthProperty);
            set => this.SetValue(LegendSymbolLengthProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend symbol margin.
        /// </summary>
        public double LegendSymbolMargin
        {
            get => (double)this.GetValue(LegendSymbolMarginProperty);
            set => this.SetValue(LegendSymbolMarginProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend symbol placement.
        /// </summary>
        public OxyPlot.Legends.LegendSymbolPlacement LegendSymbolPlacement
        {
            get => (OxyPlot.Legends.LegendSymbolPlacement)this.GetValue(LegendSymbolPlacementProperty);
            set => this.SetValue(LegendSymbolPlacementProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend text color.
        /// </summary>
        public Color LegendTextColor
        {
            get => (Color)this.GetValue(LegendTextColorProperty);
            set => this.SetValue(LegendTextColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend title.
        /// </summary>
        public string LegendTitle
        {
            get => (string)this.GetValue(LegendTitleProperty);
            set => this.SetValue(LegendTitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend title color.
        /// </summary>
        public Color LegendTitleColor
        {
            get => (Color)this.GetValue(LegendTitleColorProperty);
            set => this.SetValue(LegendTitleColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend title font.
        /// </summary>
        public string LegendTitleFont
        {
            get => (string)this.GetValue(LegendTitleFontProperty);
            set => this.SetValue(LegendTitleFontProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend title font size.
        /// </summary>
        public double LegendTitleFontSize
        {
            get => (double)this.GetValue(LegendTitleFontSizeProperty);
            set => this.SetValue(LegendTitleFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend title font weight.
        /// </summary>
        public FontWeight LegendTitleFontWeight
        {
            get => (FontWeight)this.GetValue(LegendTitleFontWeightProperty);
            set => this.SetValue(LegendTitleFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend column spacing.
        /// </summary>
        public double LegendColumnSpacing
        {
            get => (double)this.GetValue(LegendColumnSpacingProperty);
            set => this.SetValue(LegendColumnSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend font.
        /// </summary>
        public string LegendFont
        {
            get => (string)this.GetValue(LegendFontProperty);
            set => this.SetValue(LegendFontProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend font size.
        /// </summary>
        public double LegendFontSize
        {
            get => (double)this.GetValue(LegendFontSizeProperty);
            set => this.SetValue(LegendFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend font weight.
        /// </summary>
        public FontWeight LegendFontWeight
        {
            get => (FontWeight)this.GetValue(LegendFontWeightProperty);
            set => this.SetValue(LegendFontWeightProperty, value);
        }

        /// <summary>
        /// Gets an enumerator for logical child elements.
        /// </summary>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                foreach (var annotation in this.Annotations)
                {
                    yield return annotation;
                }

                foreach (var axis in this.Axes)
                {
                    yield return axis;
                }

                foreach (var s in this.Series)
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Invalidates the plot and updates the model.
        /// </summary>
        /// <param name="updateData">Whether to update data.</param>
        public override void InvalidatePlot(bool updateData = true)
        {
            if (this._needsSynchronization || updateData)
            {
                this.SynchronizeProperties();
                this.SynchronizeSeries();
                this.SynchronizeAxes();
                this.SynchronizeAnnotations();
                this._needsSynchronization = false;
            }

            base.InvalidatePlot(updateData);
        }

        /// <summary>
        /// Called when visual appearance changes.
        /// </summary>
        /// <param name="d">The dependency object that changed.</param>
        /// <param name="e">The event arguments containing property change information.</param>
        private static void AppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var plot = (Plot)d;
            if (plot.SuppressPropertyChanged) return;
            plot._needsSynchronization = true;
            plot.InvalidatePlot(false);
            plot.OnPropertyChanged(e.Property.Name);
        }

        /// <summary>
        /// Called when the series collection changes. Subscribes/unsubscribes to item
        /// PropertyChanged events and fires Plot-level INPC with property name "Series".
        /// </summary>
        private void OnSeriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateItemSubscriptions(e, this.subscribedSeries);
            this.SyncLogicalTree(e);
            this._needsSynchronization = true;
            if (this.SuppressPropertyChanged) return;
            this.InvalidatePlot();
            this.OnPropertyChanged("Series");
        }

        /// <summary>
        /// Called when the axes collection changes. Subscribes/unsubscribes to item
        /// PropertyChanged events and fires Plot-level INPC with property name "Axes".
        /// </summary>
        private void OnAxesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateItemSubscriptions(e, this.subscribedAxes);
            this.SyncLogicalTree(e);
            this._needsSynchronization = true;
            if (this.SuppressPropertyChanged) return;
            this.InvalidatePlot();
            this.OnPropertyChanged("Axes");
        }

        /// <summary>
        /// Called when the annotations collection changes. Subscribes/unsubscribes to item
        /// PropertyChanged events and fires Plot-level INPC with property name "Annotations".
        /// </summary>
        private void OnAnnotationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateItemSubscriptions(e, this.subscribedAnnotations);
            this.SyncLogicalTree(e);
            this._needsSynchronization = true;
            if (this.SuppressPropertyChanged) return;
            this.InvalidatePlot();
            this.OnPropertyChanged("Annotations");
        }

        /// <summary>
        /// Notifies subscribers that annotation properties have been modified in bulk.
        /// Call after annotation modifications performed with
        /// <see cref="Annotation.SuppressPropertyChanged"/> enabled, so that undo bridges
        /// can rebuild their shadow state to match the current property values.
        /// </summary>
        public void NotifyAnnotationsModified()
        {
            this.OnPropertyChanged("Annotations");
        }

        /// <summary>
        /// Replaces an axis in the <see cref="Axes"/> collection, preserving its position,
        /// and fires the <see cref="AxisReplaced"/> event.
        /// </summary>
        /// <param name="oldAxis">The axis to remove.</param>
        /// <param name="newAxis">The axis to insert at the same position.</param>
        /// <remarks>
        /// Use this method instead of manual Remove/Add when changing axis types so that
        /// the undo system can record the replacement as a single undoable action.
        /// </remarks>
        public void ReplaceAxis(Axis oldAxis, Axis newAxis)
        {
            int index = this.Axes.IndexOf(oldAxis);
            if (index < 0) return;
            this.Axes.RemoveAt(index);
            this.Axes.Insert(index, newAxis);
            this.AxisReplaced?.Invoke(oldAxis, newAxis);
        }

        /// <summary>
        /// Updates PropertyChanged subscriptions for items in a collection based on the change event.
        /// On Reset (Clear), unsubscribes from all tracked items and clears the shadow list.
        /// On Add/Remove/Replace, subscribes to new items and unsubscribes from old items.
        /// </summary>
        /// <typeparam name="T">The item type, which must implement INotifyPropertyChanged.</typeparam>
        /// <param name="e">The collection change event args.</param>
        /// <param name="subscribedItems">The shadow list tracking subscribed items.</param>
        private void UpdateItemSubscriptions<T>(NotifyCollectionChangedEventArgs e, List<T> subscribedItems)
            where T : INotifyPropertyChanged
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in subscribedItems)
                {
                    item.PropertyChanged -= this.OnCollectionItemPropertyChanged;
                }

                subscribedItems.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (T item in e.OldItems)
                    {
                        item.PropertyChanged -= this.OnCollectionItemPropertyChanged;
                        subscribedItems.Remove(item);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (T item in e.NewItems)
                    {
                        item.PropertyChanged += this.OnCollectionItemPropertyChanged;
                        subscribedItems.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Relays PropertyChanged events from collection items through the Plot's own
        /// PropertyChanged event. The property name is prefixed with the item type
        /// (e.g., "Annotation.Color", "Series.Title", "Axis.Minimum").
        /// </summary>
        private void OnCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this._needsSynchronization = true;

            string prefix;
            if (sender is Annotation)
            {
                prefix = "Annotation";
            }
            else if (sender is Series)
            {
                prefix = "Series";
            }
            else if (sender is Axis)
            {
                prefix = "Axis";
            }
            else
            {
                prefix = "Item";
            }

            this.OnPropertyChanged(prefix + "." + e.PropertyName);
        }

        /// <summary>
        /// Synchronizes the logical tree with collection changes.
        /// </summary>
        private void SyncLogicalTree(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this.AddLogicalChild(item);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    this.RemoveLogicalChild(item);
                }
            }
        }

        /// <summary>
        /// Synchronizes the plot properties to the internal model.
        /// </summary>
        private void SynchronizeProperties()
        {
            var m = this.internalModel;

            m.Title = this.Title;
            m.Subtitle = this.Subtitle;
            m.TitleColor = this.TitleColor.ToOxyColor();
            m.SubtitleColor = this.SubtitleColor.ToOxyColor();
            m.TitleFont = this.TitleFont;
            m.TitleFontSize = this.TitleFontSize;
            m.TitleFontWeight = this.TitleFontWeight.ToOpenTypeWeight();
            m.TitlePadding = this.TitlePadding;
            m.SubtitleFont = this.SubtitleFont;
            m.SubtitleFontSize = this.SubtitleFontSize;
            m.SubtitleFontWeight = this.SubtitleFontWeight.ToOpenTypeWeight();
            m.TextColor = this.TextColor.ToOxyColor();
            m.Background = this.Background.ToOxyColor();
            m.PlotAreaBackground = this.PlotAreaBackground.ToOxyColor();
            m.PlotAreaBorderColor = this.PlotAreaBorderColor.ToOxyColor();
            m.PlotAreaBorderThickness = this.PlotAreaBorderThickness.ToOxyThickness();
            m.Padding = this.Padding.ToOxyThickness();
            m.IsLegendVisible = this.IsLegendVisible;

            // Synchronize legend
            this.SynchronizeLegend();
        }

        /// <summary>
        /// Synchronizes the legend properties to the internal model.
        /// </summary>
        private void SynchronizeLegend()
        {
            var m = this.internalModel;

            // Get or create the default legend
            var legend = m.Legends.FirstOrDefault() as Legend;
            if (legend == null)
            {
                legend = new Legend();
                m.Legends.Add(legend);
            }

            // Synchronize legend properties
            legend.IsLegendVisible = this.IsLegendVisible;
            legend.LegendBackground = this.LegendBackground.ToOxyColor();
            legend.LegendBorder = this.LegendBorder.ToOxyColor();
            legend.LegendBorderThickness = this.LegendBorderThickness;
            legend.LegendItemAlignment = this.LegendItemAlignment.ToHorizontalAlignment();
            legend.LegendItemOrder = this.LegendItemOrder;
            legend.LegendItemSpacing = this.LegendItemSpacing;
            legend.LegendLineSpacing = this.LegendLineSpacing;
            legend.LegendMargin = this.LegendMargin;
            legend.LegendMaxHeight = this.LegendMaxHeight;
            legend.LegendMaxWidth = this.LegendMaxWidth;
            legend.LegendOrientation = this.LegendOrientation;
            legend.LegendPadding = this.LegendPadding;
            legend.LegendPlacement = this.LegendPlacement;
            legend.LegendPosition = this.LegendPosition;
            legend.LegendSymbolLength = this.LegendSymbolLength;
            legend.LegendSymbolMargin = this.LegendSymbolMargin;
            legend.LegendSymbolPlacement = this.LegendSymbolPlacement;
            legend.LegendTextColor = this.LegendTextColor.ToOxyColor();
            legend.LegendTitle = this.LegendTitle;
            legend.LegendTitleColor = this.LegendTitleColor.ToOxyColor();
            legend.LegendTitleFont = this.LegendTitleFont;
            legend.LegendTitleFontSize = this.LegendTitleFontSize;
            legend.LegendTitleFontWeight = this.LegendTitleFontWeight.ToOpenTypeWeight();
            legend.LegendColumnSpacing = this.LegendColumnSpacing;
            legend.LegendFont = this.LegendFont;
            legend.LegendFontSize = this.LegendFontSize;
            legend.LegendFontWeight = this.LegendFontWeight.ToOpenTypeWeight();
        }

        /// <summary>
        /// Synchronizes the series collection to the internal model.
        /// </summary>
        private void SynchronizeSeries()
        {
            this.internalModel.Series.Clear();
            foreach (var s in this.Series)
            {
                var model = s.CreateModel();
                if (model != null)
                {
                    this.internalModel.Series.Add(model);
                }
            }
        }

        /// <summary>
        /// Synchronizes the axes collection to the internal model.
        /// </summary>
        private void SynchronizeAxes()
        {
            this.internalModel.Axes.Clear();
            foreach (var a in this.Axes)
            {
                var model = a.CreateModel();
                if (model != null)
                {
                    this.internalModel.Axes.Add(model);
                }
            }
        }

        /// <summary>
        /// Synchronizes the annotations collection to the internal model.
        /// </summary>
        private void SynchronizeAnnotations()
        {
            this.internalModel.Annotations.Clear();
            foreach (var a in this.Annotations)
            {
                var model = a.CreateModel();
                if (model != null)
                {
                    this.internalModel.Annotations.Add(model);
                }
            }
        }
    }
}
