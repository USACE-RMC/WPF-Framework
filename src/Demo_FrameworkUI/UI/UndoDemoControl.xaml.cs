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
using System.Windows;
using System.Windows.Controls;
using Demo_FrameworkUI.Project.Undo_Demo;
using FrameworkInterfaces.Undo;

namespace Demo_FrameworkUI.UI
{
    /// <summary>
    /// A demo user control that showcases the undo/redo functionality with interactive UI elements and stack visualization.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class UndoDemoControl : UserControl
    {
        private UndoDemoElement _element;
        private bool _isUpdatingUI = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoDemoControl"/> class.
        /// </summary>
        public UndoDemoControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element being edited.
        /// </summary>
        public UndoDemoElement Element
        {
            get => _element;
            set
            {
                _element = value;
                if (_element != null)
                {
                    _element.PropertyChanged += Element_PropertyChanged;
                    RefreshUI();
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the element to refresh the UI.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The property changed event arguments.</param>
        private void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!_isUpdatingUI)
            {
                RefreshUI();
            }
        }

        /// <summary>
        /// Refreshes the UI to reflect the current element state.
        /// </summary>
        private void RefreshUI()
        {
            if (_element == null) return;

            _isUpdatingUI = true;
            try
            {
                NameTextBox.Text = _element.Name ?? string.Empty;
                CustomValueTextBox.Text = _element.CustomValue ?? string.Empty;
                NumericValueTextBox.Text = _element.NumericValue.ToString();
                BooleanValueCheckBox.IsChecked = _element.BooleanValue;
                DateValuePicker.SelectedDate = _element.DateValue;

                RefreshUndoRedoStacks();
                UpdateButtonStates();
            }
            finally
            {
                _isUpdatingUI = false;
            }
        }

        /// <summary>
        /// Updates the undo/redo stack displays.
        /// </summary>
        private void RefreshUndoRedoStacks()
        {
            UndoStackListBox.Items.Clear();
            RedoStackListBox.Items.Clear();

            if (_element?.UndoManager == null) return;

            var undoStack = _element.UndoManager.UndoStack;
            var redoStack = _element.UndoManager.RedoStack;

            foreach (var action in undoStack)
            {
                UndoStackListBox.Items.Add(action.Description);
            }

            foreach (var action in redoStack)
            {
                RedoStackListBox.Items.Add(action.Description);
            }
        }

        /// <summary>
        /// Updates the enabled state of the undo/redo buttons.
        /// </summary>
        private void UpdateButtonStates()
        {
            UndoButton.IsEnabled = _element?.UndoManager?.CanUndo == true;
            RedoButton.IsEnabled = _element?.UndoManager?.CanRedo == true;
        }

        /// <summary>
        /// Handles text property changes for the name and custom value text boxes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The text changed event arguments.</param>
        private void Property_Changed(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingUI || _element == null) return;

            var textBox = sender as TextBox;
            if (textBox == NameTextBox)
            {
                _element.Name = textBox.Text;
            }
            else if (textBox == CustomValueTextBox)
            {
                _element.CustomValue = textBox.Text;
            }

            RefreshUndoRedoStacks();
            UpdateButtonStates();
        }

        /// <summary>
        /// Handles numeric property changes for the numeric value text box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The text changed event arguments.</param>
        private void NumericProperty_Changed(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingUI || _element == null) return;

            if (int.TryParse(NumericValueTextBox.Text, out int value))
            {
                _element.NumericValue = value;
                RefreshUndoRedoStacks();
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Handles boolean property changes for the checkbox.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void BooleanProperty_Changed(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingUI || _element == null) return;

            _element.BooleanValue = BooleanValueCheckBox.IsChecked == true;
            RefreshUndoRedoStacks();
            UpdateButtonStates();
        }

        /// <summary>
        /// Handles date property changes for the date picker.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void DateProperty_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingUI || _element == null) return;

            if (DateValuePicker.SelectedDate.HasValue)
            {
                _element.DateValue = DateValuePicker.SelectedDate.Value;
                RefreshUndoRedoStacks();
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Handles the undo button click to undo the last change.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_element?.UndoManager?.CanUndo == true)
            {
                _element.UndoManager.Undo();
                RefreshUI();
            }
        }

        /// <summary>
        /// Handles the redo button click to redo the last undone change.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_element?.UndoManager?.CanRedo == true)
            {
                _element.UndoManager.Redo();
                RefreshUI();
            }
        }

        /// <summary>
        /// Handles the batch update button click to perform multiple changes in a single transaction.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void BatchUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_element == null) return;

            // Demonstrate batch update - all changes will be undone together
            _element.PerformBatchUpdate(
                "Batch Updated Value",
                999,
                !_element.BooleanValue
            );

            RefreshUI();
        }

        /// <summary>
        /// Handles the clear history button click to clear the undo/redo history.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _element?.UndoManager?.Clear();
            RefreshUndoRedoStacks();
            UpdateButtonStates();
        }
    }
}
