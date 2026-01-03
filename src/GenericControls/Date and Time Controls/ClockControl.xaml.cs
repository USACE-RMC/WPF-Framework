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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// A WPF control for selecting time using a circular, analog-style clock face.
    /// Supports hour, minute, and second selection in 12-hour or 24-hour modes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class ClockControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClockControl"/> class and configures default layout and clock ticks.
        /// </summary>
        public static DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(DateTime), typeof(ClockControl), new UIPropertyMetadata(new DateTime(1980, 7, 30, 12, 0, 0), TimePropertyCallback));

        /// <summary>
        /// Event handler for time property dependencies.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TimePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(ClockControl))
                return;
            ClockControl thisControl = (ClockControl)d;
            // 
            if (e.NewValue == null)
                return;
            if (e.NewValue.GetType() != typeof(DateTime))
                return;
            thisControl.ClearSelected();
            thisControl.SelectCurrentTime((DateTime)e.NewValue);
        }

        /// <summary>
        /// indicates, Gets/sets the time displayed and edited by the control.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return (DateTime)this.GetValue(TimeProperty);
            }
            set
            {
                this.SetValue(TimeProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets or sets a value indicating whether the clock uses 24-hour format.
        /// </summary>
        public static DependencyProperty Is24HourProperty = DependencyProperty.Register(nameof(Is24Hour), typeof(bool), typeof(ClockControl), new FrameworkPropertyMetadata(false));
        public bool Is24Hour
        {
            get
            {
                return (bool)this.GetValue(Is24HourProperty);
            }
            set
            {
                this.SetValue(Is24HourProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets whether the control is currently in hour-selection mode.
        /// </summary>
        public static DependencyProperty IsHoursProperty = DependencyProperty.Register(nameof(IsHours), typeof(bool), typeof(ClockControl), new UIPropertyMetadata(true));
        public bool IsHours
        {
            get
            {
                return (bool)this.GetValue(IsHoursProperty);
            }
            set
            {
                this.SetValue(IsHoursProperty, value);
                ClearSelected();
                SelectCurrentTime(Time);
            }
        }

        /// <summary>
        /// indicates, Gets/sets whether the control is currently in minute-selection mode.
        /// </summary>
        public static DependencyProperty IsMinutesProperty = DependencyProperty.Register(nameof(IsMinutes), typeof(bool), typeof(ClockControl), new UIPropertyMetadata(false));
        public bool IsMinutes
        {
            get
            {
                return (bool)this.GetValue(IsMinutesProperty);
            }
            set
            {
                this.SetValue(IsMinutesProperty, value);
                ClearSelected();
                SelectCurrentTime(Time);
            }
        }

        /// <summary>
        /// indicates, Gets/sets whether the control is currently in second-selection mode.
        /// </summary>
        public static DependencyProperty IsSecondsProperty = DependencyProperty.Register(nameof(IsSeconds), typeof(bool), typeof(ClockControl), new UIPropertyMetadata(false));
        public bool IsSeconds
        {
            get
            {
                return (bool)this.GetValue(IsSecondsProperty);
            }
            set
            {
                this.SetValue(IsSecondsProperty, value);
                ClearSelected();
                SelectCurrentTime(Time);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the minutes property.
        /// </summary>
        public static DependencyProperty HasMinutesProperty = DependencyProperty.Register(nameof(HasMinutes), typeof(bool), typeof(ClockControl), new UIPropertyMetadata(true));
        public bool HasMinutes
        {
            get
            {
                return (bool)this.GetValue(HasMinutesProperty);
            }
            set
            {
                this.SetValue(HasMinutesProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the seconds property.
        /// </summary>
        public static DependencyProperty HasSecondsProperty = DependencyProperty.Register(nameof(HasSeconds), typeof(bool), typeof(ClockControl), new UIPropertyMetadata(false));
        public bool HasSeconds
        {
            get
            {
                return (bool)this.GetValue(HasSecondsProperty);
            }
            set
            {
                this.SetValue(HasSecondsProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the brush used to render the selected time color
        /// </summary>
        public static DependencyProperty SelectedTimeColorProperty = DependencyProperty.Register(nameof(SelectedTimeColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.Black));
        public SolidColorBrush SelectedTimeColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(SelectedTimeColorProperty);
            }
            set
            {
                this.SetValue(SelectedTimeColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the brush used for the time color
        /// </summary>
        public static DependencyProperty TimeColorProperty = DependencyProperty.Register(nameof(TimeColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.DarkGray));
        public SolidColorBrush TimeColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(TimeColorProperty);
            }
            set
            {
                this.SetValue(TimeColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the brush used to render the selected font size for the time.
        /// </summary>
        public static DependencyProperty SelectedTimeFontSizeProperty = DependencyProperty.Register(nameof(SelectedTimeFontSize), typeof(double), typeof(ClockControl), new UIPropertyMetadata(24d));
        public double SelectedTimeFontSize
        {
            get
            {
                return (double)this.GetValue(SelectedTimeFontSizeProperty);
            }
            set
            {
                this.SetValue(SelectedTimeFontSizeProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the brush used for the time font size.
        /// </summary>
        public static DependencyProperty TimeFontSizeProperty = DependencyProperty.Register(nameof(TimeFontSize), typeof(double), typeof(ClockControl), new UIPropertyMetadata(24d));
        public double TimeFontSize
        {
            get
            {
                return (double)this.GetValue(TimeFontSizeProperty);
            }
            set
            {
                this.SetValue(TimeFontSizeProperty, value);
            }
        }

        /// <summary>
        /// indicates, Gets/sets the brush used to render selected tick marks.
        /// </summary>
        public static DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 24, 24, 25))));
        public SolidColorBrush SelectedColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(SelectedColorProperty);
            }
            set
            {
                this.SetValue(SelectedColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, gets/sets the brush used for highlighting hover states.
        /// </summary>
        public static DependencyProperty HighlightColorProperty = DependencyProperty.Register(nameof(HighlightColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.LightGray));
        public SolidColorBrush HighlightColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(HighlightColorProperty);
            }
            set
            {
                this.SetValue(HighlightColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, get/set the brush used for coloring the face of the clock.
        /// </summary>
        public static DependencyProperty FaceColorProperty = DependencyProperty.Register(nameof(FaceColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 240, 240, 245))));
        public SolidColorBrush FaceColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(FaceColorProperty);
            }
            set
            {
                this.SetValue(FaceColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, Get/set the brush used for coloring the hands of the clock.
        /// </summary>
        public static DependencyProperty HandColorProperty = DependencyProperty.Register(nameof(HandColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.DarkGray));
        public SolidColorBrush HandColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(HandColorProperty);
            }
            set
            {
                this.SetValue(HandColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, get/set the brush used for the previewed hand color.
        /// </summary>
        public static DependencyProperty PreviewHandColorProperty = DependencyProperty.Register(nameof(PreviewHandColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.LightGray));
        public SolidColorBrush PreviewHandColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(PreviewHandColorProperty);
            }
            set
            {
                this.SetValue(PreviewHandColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, get/set the brush used for the font color.
        /// </summary>
        public static DependencyProperty FontColorProperty = DependencyProperty.Register(nameof(FontColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.Black));
        public SolidColorBrush FontColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(FontColorProperty);
            }
            set
            {
                this.SetValue(FontColorProperty, value);
            }
        }

        /// <summary>
        /// indicates, get/set the brush used to render selected font color.
        /// </summary>
        public static DependencyProperty SelectedFontColorProperty = DependencyProperty.Register(nameof(SelectedFontColor), typeof(SolidColorBrush), typeof(ClockControl), new UIPropertyMetadata(Brushes.White));
        public SolidColorBrush SelectedFontColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(SelectedFontColorProperty);
            }
            set
            {
                this.SetValue(SelectedFontColorProperty, value);
            }
        }

        private Style _clockMajorNumberStyle;
        private Style _clockMinorNumberStyle;
        private ClockToggle[] _hourBorders = new ClockToggle[0];
        private ClockToggle[] _minuteBorders = new ClockToggle[60];
        private ClockToggle[] _secondBorders = new ClockToggle[60];

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockControl"/> class.
        /// Sets up resources, intializes hour/minute/second ticks, and registers event handlers.
        /// </summary>
        public ClockControl()
        {

            // This call is required by the designer.
            this.InitializeComponent();
            // 
            // Add any initialization after the InitializeComponent() call.
            _clockMajorNumberStyle = (Style)this.FindResource("ClockMajorValueStyle");
            _clockMinorNumberStyle = (Style)this.FindResource("ClockMinorValueStyle");
            // Set up Hours
            InitializeClock();
            // 
            // Minute and second borders only need to be set once on initialization
            double transformRadius = 42d; // transform radius
                                          // Get and set the minute and second tick circles
            for (int i = 1; i <= 59; i++)
            {
                if (i % 5 != 0)
                {
                    _minuteBorders[i] = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMinor);
                    this.MinutesGrid.Children.Add(_minuteBorders[i]);
                    _secondBorders[i] = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMinor);
                    this.SecondsGrid.Children.Add(_secondBorders[i]);
                }
            }

            // larger toggles (5, 10, 15, etc.) need to be rendered after to make sure they show up on top of the minor minute toggles.
            // Adding to the grid after the minors have been added set the render order.
            for (int i = 0; i <= 55; i += 5)
            {
                _minuteBorders[i] = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMajor);
                this.MinutesGrid.Children.Add(_minuteBorders[i]);
                _secondBorders[i] = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMajor);
                this.SecondsGrid.Children.Add(_secondBorders[i]);
            }

            this.Loaded += ClockControl_Loaded;
        }

        /// <summary>
        /// Handles the control's <see cref="FrameworkElement.Loaded"/> event.
        /// Re-initializes clock and selects current time after layout is completed.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ClockControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeClock();
            SelectCurrentTime(Time);
        }

        /// <summary>
        /// Initializes the hour tick marks based on 12-hour or 24-hour configuration. 
        /// Clears existing elements and repositions tick controls.
        /// </summary>
        private void InitializeClock()
        {
            double transformRadius = 42d; // transform radius
                                          //
                                          // Get and set the hour tick circles
            _hourBorders = new ClockToggle[Is24Hour ? 24 : 12];
            for (int i = this.HoursGrid.Children.Count - 1; i >= 0; i -= 1)
            {
                if (this.HoursGrid.Children[i].GetType() == typeof(ClockToggle))
                    this.HoursGrid.Children.RemoveAt(i);
            }
            // Add hour tick circles
            double angle = 360d / _hourBorders.Count();
            if (Is24Hour)
            {
                for (int i = 1, loopTo = _hourBorders.Count() - 1; i <= loopTo; i += 2)
                {
                    _hourBorders[i] = GetBorder(i, 180d - i * angle, transformRadius, ClockToggle.State.HourMinor);
                    this.HoursGrid.Children.Add(_hourBorders[i]);
                }
                // In 24-hour mode, display 0 at the top (midnight)
                _hourBorders[0] = GetBorder(0, 180d, transformRadius, ClockToggle.State.HourMajor);
                this.HoursGrid.Children.Add(_hourBorders[0]);
                for (int i = 2, loopTo1 = _hourBorders.Count() - 1; i <= loopTo1; i += 2)
                {
                    _hourBorders[i] = GetBorder(i, 180d - i * angle, transformRadius, ClockToggle.State.HourMajor);
                    this.HoursGrid.Children.Add(_hourBorders[i]);
                }
            }
            else
            {
                _hourBorders[0] = GetBorder(_hourBorders.Count(), 180d, transformRadius, ClockToggle.State.HourMajor);
                this.HoursGrid.Children.Add(_hourBorders[0]);
                for (int i = 1, loopTo2 = _hourBorders.Count() - 1; i <= loopTo2; i++)
                {
                    _hourBorders[i] = GetBorder(i, 180d - i * angle, transformRadius, ClockToggle.State.HourMajor);
                    this.HoursGrid.Children.Add(_hourBorders[i]);
                }
            }
        }

        /// <summary>
        /// A circular toggle used with the <see cref="ClockControl"/> for selecting specific time units (hours, minutes, seconds).
        /// Displayes a number and highlights or selects on interaction.
        /// </summary>
        /// <param name="t">The numeric time value (e.g. 5, 10).</param>
        /// <param name="angleDegree">Angle on the clock face to position the toggle.</param>
        /// <param name="transformRadius">distance from the clock center to the toggle.</param>
        /// <param name="borderState">The visual and logical state of the toggle (e.g., minute major).</param>
        /// <returns>A configured <see cref="ClockToggle"/> element.</returns>
        private ClockToggle GetBorder(int t, double angleDegree, double transformRadius, ClockToggle.State borderState)
        {
            var b = new ClockToggle() { TimeValue = t, TimeState = borderState };
            if (borderState == ClockToggle.State.MinuteMinor | borderState == ClockToggle.State.SecondMinor | borderState == ClockToggle.State.HourMinor)
            {
                b.Style = _clockMinorNumberStyle;
            }
            else
            {
                b.Style = _clockMajorNumberStyle;
            }
            // 
            b.RenderTransform = new TranslateTransform(transformRadius * Math.Sin(angleDegree * Math.PI / 180d), transformRadius * Math.Cos(angleDegree * Math.PI / 180d));
            return b;
        }

        /// <summary>
        /// Handles clicks on the hour section.
        /// Sets mode to hour selection
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void HoursMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMinutes = false;
            IsSeconds = false;
            IsHours = true;
        }

        /// <summary>
        /// Handles clicks on the minutes section;
        /// Sets mode to minute selection.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void MinutesMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (HasMinutes == false)
                return;
            IsMinutes = true;
            IsSeconds = false;
            IsHours = false;
        }

        /// <summary>
        /// Handles clicks on the seconds section;
        /// sets mode to second selection
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void SecondsMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMinutes = false;
            IsSeconds = true;
            IsHours = false;
        }

        /// <summary>
        /// Toggles AM/PM by adding or subtracting 12 hours from the selected time.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void AmPmMouseUp(object sender, MouseButtonEventArgs e)
        {
            int hour = Time.Hour;
            if (hour >= 12)
                hour = hour - 12;
            else
                hour = hour + 12;
            Time = new DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second);
        }

        /// <summary>
        /// Handles promotion of selection from hours to minutes to seconds on left click. 
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void ClockFace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (IsHours)
                {
                    if (HasMinutes)
                    {
                        IsMinutes = true;
                        IsSeconds = false;
                        IsHours = false;
                    }
                }
                else if (IsMinutes)
                {
                    IsHours = false;
                    if (HasSeconds)
                    {
                        IsMinutes = false;
                        IsSeconds = true;
                    }
                }
                else if (IsSeconds)
                {
                    IsHours = false;
                    if (HasSeconds)
                    {
                        IsMinutes = false;
                        IsSeconds = true;
                    }
                }
            }
        }

        /// <summary>
        /// Selects or highlights the time value closest to the clicked position.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void ClockFace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int timeValue = GetTimeValue(e.GetPosition(this.ClockFace));
            ClearHighlighted();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetTimeValue(timeValue);
            }
            else
            {
                SetHighlighted(timeValue);
            }
        }

        /// <summary>
        /// Updates selection or highllight as the mouse moves over the clock face.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments</param>
        private void ClockFace_MouseMove(object sender, MouseEventArgs e)
        {
            int timeValue = GetTimeValue(e.GetPosition(this.ClockFace));
            ClearHighlighted();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetTimeValue(timeValue);
            }
            else
            {
                SetHighlighted(timeValue);
            }
            // UpdateForTimeValue(timeValue, e.LeftButton)
        }

        /// <summary>
        /// Converts a mouse position into a corresponding clock value (hour/minute/second)
        /// </summary>
        /// <param name="p">Index of a coordinate</param>
        /// <returns></returns>
        private int GetTimeValue(Point p)
        {
            double quadrantSize;
            if (IsHours)
            {
                quadrantSize = 1d / _hourBorders.Count(); // If(Is24Hour = True, 24, 12)
            }
            else // Must be minutes or seconds
            {
                quadrantSize = 1d / 60d;
            }
            // 
            double angleRadians = Math.Atan2(-50 + p.X, 50d - p.Y);
            double arc0To1 = angleRadians > 0d ? angleRadians / Math.PI / 2d : (2d + angleRadians / Math.PI) / 2d;
            int timeValue = (int)Math.Round(arc0To1 / quadrantSize);
            if (IsHours)
            {
                if (timeValue == _hourBorders.Count())
                    timeValue = 0;
            }
            else if (timeValue == 60) // Must be minutes or seconds
                timeValue = 0;
            return timeValue;
        }

        /// <summary>
        /// Updates the internal Time value based on the selected hour/minute/second.
        /// </summary>
        /// <param name="timeValue"></param>
        private void SetTimeValue(int timeValue)
        {
            if (IsHours)
            {
                // Convert from AM/PM to 24 hour value
                if (Is24Hour == false)
                {
                    int hour = Time.Hour;
                    if (timeValue >= 12)
                        timeValue = timeValue - 12;
                    if (hour >= 12)
                        timeValue = timeValue + 12;
                }
                Time = new DateTime(Time.Year, Time.Month, Time.Day, timeValue, Time.Minute, Time.Second);
            }
            else if (IsMinutes)
            {
                Time = new DateTime(Time.Year, Time.Month, Time.Day, Time.Hour, timeValue, Time.Second);
            }
            else if (IsSeconds)
            {
                Time = new DateTime(Time.Year, Time.Month, Time.Day, Time.Hour, Time.Minute, timeValue);
            }
        }

        /// <summary>
        /// Removes all highlight effects from tick marks on the clock face.
        /// </summary>
        private void ClearHighlighted()
        {
            if (IsHours)
            {
                for (int i = 0, loopTo = _hourBorders.Count() - 1; i <= loopTo; i++)
                    _hourBorders[i].IsHighlighted = false;
                this.PreviewHourHandLine.Visibility = Visibility.Hidden;
            }
            else if (IsMinutes)
            {
                for (int i = 0, loopTo1 = _minuteBorders.Count() - 1; i <= loopTo1; i++)
                    _minuteBorders[i].IsHighlighted = false;
                this.PreviewMinuteHandLine.Visibility = Visibility.Hidden;
            }
            else if (IsSeconds)
            {
                for (int i = 0, loopTo2 = _secondBorders.Count() - 1; i <= loopTo2; i++)
                    _secondBorders[i].IsHighlighted = false;
                this.PreviewSecondHandLine.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Visually highlights the tick mark for the current hover value.
        /// </summary>
        /// <param name="timeValue"></param>
        private void SetHighlighted(int timeValue)
        {
            if (IsHours)
            {
                if (_hourBorders[timeValue].IsSelected == false)
                    _hourBorders[timeValue].IsHighlighted = true;
                this.PreviewHourHandLine.Visibility = Visibility.Visible;
                this.PreviewHourHandLine.X2 = ((TranslateTransform)_hourBorders[timeValue].RenderTransform).X;
                this.PreviewHourHandLine.Y2 = ((TranslateTransform)_hourBorders[timeValue].RenderTransform).Y;
            }
            else if (IsMinutes)
            {
                if (_minuteBorders[timeValue].IsSelected == false)
                    _minuteBorders[timeValue].IsHighlighted = true;
                this.PreviewMinuteHandLine.Visibility = Visibility.Visible;
                this.PreviewMinuteHandLine.X2 = ((TranslateTransform)_minuteBorders[timeValue].RenderTransform).X;
                this.PreviewMinuteHandLine.Y2 = ((TranslateTransform)_minuteBorders[timeValue].RenderTransform).Y;
            }
            else if (IsSeconds)
            {
                if (_secondBorders[timeValue].IsSelected == false)
                    _secondBorders[timeValue].IsHighlighted = true;
                this.PreviewSecondHandLine.Visibility = Visibility.Visible;
                this.PreviewSecondHandLine.X2 = ((TranslateTransform)_secondBorders[timeValue].RenderTransform).X;
                this.PreviewSecondHandLine.Y2 = ((TranslateTransform)_secondBorders[timeValue].RenderTransform).Y;
            }
        }

        /// <summary>
        /// Deselects all tick marks of the active time unit (hour/minute/second)
        /// </summary>
        private void ClearSelected()
        {
            if (IsHours)
            {
                for (int i = 0, loopTo = _hourBorders.Count() - 1; i <= loopTo; i++)
                    _hourBorders[i].IsSelected = false;
            }
            else if (IsMinutes)
            {
                for (int i = 0, loopTo1 = _minuteBorders.Count() - 1; i <= loopTo1; i++)
                    _minuteBorders[i].IsSelected = false;
            }
            else if (IsSeconds)
            {
                for (int i = 0, loopTo2 = _secondBorders.Count() - 1; i <= loopTo2; i++)
                    _secondBorders[i].IsSelected = false;
            }
        }

        /// <summary>
        /// set the time and update UI selection accordingly.
        /// </summary>
        /// <param name="timeValue"></param>
        public void SelectTime(DateTime timeValue)
        {
            SelectCurrentTime(timeValue);
        }

        /// <summary>
        /// Updates the UI to reflect the selected DateTime (hour,minute,second).
        /// </summary>
        /// <param name="timeValue"></param>
        private void SelectCurrentTime(DateTime timeValue)
        {
            ClearHighlighted();
            if (IsHours)
            {
                int hour = timeValue.Hour;
                if (_hourBorders.Count() < 13)
                {
                    if (hour >= 12)
                        hour = hour - 12; // Else hour = hour + 12
                }
                _hourBorders[hour].IsSelected = true;
                this.HourHandLine.X2 = ((TranslateTransform)_hourBorders[hour].RenderTransform).X;
                this.HourHandLine.Y2 = ((TranslateTransform)_hourBorders[hour].RenderTransform).Y;
            }
            else if (IsMinutes)
            {
                _minuteBorders[timeValue.Minute].IsSelected = true;
                this.MinuteHandLine.X2 = ((TranslateTransform)_minuteBorders[timeValue.Minute].RenderTransform).X;
                this.MinuteHandLine.Y2 = ((TranslateTransform)_minuteBorders[timeValue.Minute].RenderTransform).Y;
            }
            else if (IsSeconds)
            {
                _secondBorders[timeValue.Second].IsSelected = true;
                this.SecondHandLine.X2 = ((TranslateTransform)_secondBorders[timeValue.Second].RenderTransform).X;
                this.SecondHandLine.Y2 = ((TranslateTransform)_secondBorders[timeValue.Second].RenderTransform).Y;
            }
            // 
            if (Is24Hour == false)
            {
                if (timeValue.Hour < 12)
                {
                    this.AMFace.Background = SelectedColor;
                    this.AMText.Foreground = SelectedFontColor;
                    this.PMFace.Background = FaceColor;
                    this.PMText.Foreground = FontColor;
                }
                else
                {
                    this.PMFace.Background = SelectedColor;
                    this.PMText.Foreground = SelectedFontColor;
                    this.AMFace.Background = FaceColor;
                    this.AMText.Foreground = FontColor;
                }
            }
        }

        /// <summary>
        /// Utility method to compute Euclidean distance between two points.
        /// </summary>
        /// <param name="x1">x coordinate of the first point.</param>
        /// <param name="y1">y coordinate of the first point.</param>
        /// <param name="x2">x coordinate of the second point.</param>
        /// <param name="y2">y coordinate of the second point.</param>
        /// <returns></returns>
        public static double LineMagnitude(double x1, double y1, double x2, double y2)
        {
            // There exists methods to approximate the square root that are much faster than math.sqrt(). 
            // May want to consider implementing. http: //blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html
            return Math.Sqrt(Math.Pow(x2 - x1, 2d) + Math.Pow(y2 - y1, 2d));
        }

        /// <summary>
        /// Focuses the control when mouse enters the clock face.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void ClockFace_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ClockFace.Focus();
        }

        /// <summary>
        /// Clears highlights when the mouse exits the clock face.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void ClockFace_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearHighlighted();
        }

        /// <summary>
        /// Highlights AM/PM toggle when hovered and it would switch period.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void AMPMBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border selectedBorder = (Border)sender;
            // 
            int hour = Time.Hour;
            if (hour >= 12 & selectedBorder.Equals(this.AMFace))
            {
                this.AMFace.Background = HighlightColor;
            }
            else if (hour < 12 & selectedBorder.Equals(this.PMFace))
            {
                this.PMFace.Background = HighlightColor;
            }

        }

        /// <summary>
        /// Restores default background when AM/PM toggle is no longer hovered.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void AMPMBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Border selectedBorder = (Border)sender;
            // 
            int hour = Time.Hour;
            if (hour >= 12 & selectedBorder.Equals(this.AMFace))
            {
                this.AMFace.Background = FaceColor;
            }
            else if (hour < 12 & selectedBorder.Equals(this.PMFace))
            {
                this.PMFace.Background = FaceColor;
            }
        }

        /// <summary>
        /// Switches between AM and PM when the user clicks on the toggle.
        /// </summary>
        /// <param name="sender">Object raising event.</param>
        /// <param name="e">Event arguments.</param>
        private void AMPMBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 
            Border selectedBorder = (Border)sender;

            int hour = Time.Hour;
            if (selectedBorder.Equals(this.AMFace) & hour >= 12)
            {
                hour = hour - 12;
                Time = new DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second);
            }
            else if (selectedBorder.Equals(this.PMFace) & hour < 12)
            {
                hour = hour + 12;
                Time = new DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second);
            }
        }

    }

    /// <summary>
    /// Represents the type of time element rendered on the clock face.
    /// </summary>
    public class ClockToggle : Border
    {

        public enum State
        {
            HourMajor,
            MinuteMajor,
            SecondMajor,
            HourMinor,
            MinuteMinor,
            SecondMinor
        }

        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ClockToggle), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets/sets whether the clock toggle is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// indicates, gets/sets whether the clock toggle is highlighted
        /// </summary>
        public static DependencyProperty IsHighlightedProperty = DependencyProperty.Register(nameof(IsHighlighted), typeof(bool), typeof(ClockToggle), new FrameworkPropertyMetadata(false));
        public bool IsHighlighted
        {
            get
            {
                return (bool)GetValue(IsHighlightedProperty);
            }
            set
            {
                SetValue(IsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// indicates brush, gets/sets the font color of the toggle.
        /// </summary>
        public static DependencyProperty FontColorProperty = DependencyProperty.Register(nameof(FontColor), typeof(SolidColorBrush), typeof(ClockToggle), new UIPropertyMetadata(Brushes.Black));
        public SolidColorBrush FontColor
        {
            get
            {
                return (SolidColorBrush)GetValue(FontColorProperty);
            }
            set
            {
                SetValue(FontColorProperty, value);
            }
        }

        /// <summary>
        /// indicates numeric time value, gets/sets numeric value of th etime unit this toggle represents.
        /// </summary>
        public static DependencyProperty TimeValueProperty = DependencyProperty.Register(nameof(TimeValue), typeof(int), typeof(ClockToggle), new UIPropertyMetadata(1));
        public int TimeValue
        {
            get
            {
                return (int)GetValue(TimeValueProperty);
            }
            set
            {
                SetValue(TimeValueProperty, value);
            }
        }

        /// <summary>
        /// Identifies the style/state of the clock toggle (e.g.,3,15,45).
        /// </summary>
        public static DependencyProperty TimeStateProp = DependencyProperty.Register(nameof(TimeState), typeof(State), typeof(ClockToggle), new UIPropertyMetadata(State.HourMajor, StateChangedCallback));

        /// <summary>
        /// Callback triggered when the TimeState property is changed. Removes content for minor ticks.
        /// </summary>
        /// <param name="d">The dependency object (should be a ClockToggle)</param>
        /// <param name="e">Change event arguments.</param>
        private static void StateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(ClockToggle))
                return;
            ClockToggle thisControl = (ClockToggle)d;
            // 
            if (e.NewValue == null)
                return;
            if (e.NewValue.GetType() != typeof(State))
                return;
            State newState = (State)e.NewValue;
            if (newState == State.MinuteMinor | newState == State.SecondMinor | newState == State.HourMinor)
            {
                thisControl.Child = null;
            }


        }

        /// <summary>
        /// Gets/sets the visual state of the toggle (e.g. HourMajor, MinuteMinor)
        /// </summary>
        public State TimeState
        {
            get
            {
                return (State)GetValue(TimeStateProp);
            }
            set
            {
                SetValue(TimeStateProp, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ClockToggle class, sets up bindings and default child visuals.
        /// </summary>
        public ClockToggle()
        {

            // Make it a circle
            IsHitTestVisible = false;
            SetBinding(CornerRadiusProperty, new Binding() { Path = new PropertyPath("ActualHeight"), Source = this });
            SetBinding(WidthProperty, new Binding() { Path = new PropertyPath("ActualHeight"), Source = this });
            // Create inner text
            var tBox = new TextBlock() { FontSize = 7d, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Center };
            tBox.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath(nameof(TimeValue)), Source = this });
            tBox.SetBinding(TextBlock.ForegroundProperty, new Binding() { Path = new PropertyPath("FontColor"), Source = this });
            // add text to circle
            Child = tBox;
        }

    }
}