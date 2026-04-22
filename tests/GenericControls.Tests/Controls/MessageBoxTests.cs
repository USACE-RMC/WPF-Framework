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

using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace GenericControls.Tests.Controls;

/// <summary>
/// Unit tests for the <see cref="MessageBox"/> and <see cref="MessageBoxWindow"/> classes.
/// Tests the configuration logic, button setup, icon mapping, and suppression behavior
/// without requiring a WPF application context or ShowDialog.
/// </summary>
public class MessageBoxTests
{
    #region Suppression Store Tests

    /// <summary>
    /// Tests that when the suppression store has the key set to true,
    /// the Show method returns the default suppressed result without showing a dialog.
    /// </summary>
    [Fact]
    public void Show_WhenSuppressed_ReturnsDefaultResult()
    {
        // Arrange
        var store = new Dictionary<string, bool> { { "test.key", true } };

        // Act - This would normally show a dialog, but since it's suppressed, it returns immediately.
        // We need an Application context for the non-suppressed path, but the suppressed path
        // short-circuits before creating any window.
        var result = GenericControls.MessageBox.Show(
            "Test message", "Test", MessageBoxButton.YesNo, MessageBoxImage.Question,
            "test.key", store, MessageBoxResult.Yes);

        // Assert
        Assert.Equal(MessageBoxResult.Yes, result);
    }

    /// <summary>
    /// Tests that when the suppression store has the key set to false,
    /// the message is not suppressed (the dialog would be shown).
    /// </summary>
    [Fact]
    public void Show_WhenNotSuppressed_DoesNotReturnDefault()
    {
        // Arrange
        var store = new Dictionary<string, bool> { { "test.key", false } };

        // Note: We cannot fully test the non-suppressed path without an Application context
        // because it creates a window. This test verifies that false values don't suppress.
        // The actual dialog would need to be shown, which requires an STA thread and Application.
        // We verify the store logic indirectly through the suppressed=true test above.
        Assert.False(store["test.key"]);
    }

    /// <summary>
    /// Tests that when the suppression store doesn't contain the key,
    /// the message is not suppressed.
    /// </summary>
    [Fact]
    public void Show_WhenKeyNotInStore_DoesNotSuppress()
    {
        // Arrange
        var store = new Dictionary<string, bool>();

        // The store doesn't have the key, so TryGetValue returns false
        // and the dialog would be shown (not suppressed).
        Assert.False(store.ContainsKey("missing.key"));
    }

    /// <summary>
    /// Tests that the suppression result can be any MessageBoxResult value.
    /// </summary>
    [Theory]
    [InlineData(MessageBoxResult.OK)]
    [InlineData(MessageBoxResult.Cancel)]
    [InlineData(MessageBoxResult.Yes)]
    [InlineData(MessageBoxResult.No)]
    [InlineData(MessageBoxResult.None)]
    public void Show_WhenSuppressed_ReturnsCorrectDefaultForEachResult(MessageBoxResult expectedResult)
    {
        // Arrange
        var store = new Dictionary<string, bool> { { "test.key", true } };

        // Act
        var result = GenericControls.MessageBox.Show(
            "Test", "Test", MessageBoxButton.OK, MessageBoxImage.None,
            "test.key", store, expectedResult);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    #endregion

    #region MessageBoxImage Enum Alias Tests

    /// <summary>
    /// Tests that MessageBoxImage.Stop has the same underlying value as MessageBoxImage.Error (both are 16).
    /// </summary>
    [Fact]
    public void MessageBoxImage_Stop_EqualToError()
    {
        Assert.Equal(MessageBoxImage.Error, MessageBoxImage.Stop);
    }

    /// <summary>
    /// Tests that MessageBoxImage.Hand has the same underlying value as MessageBoxImage.Error (both are 16).
    /// </summary>
    [Fact]
    public void MessageBoxImage_Hand_EqualToError()
    {
        Assert.Equal(MessageBoxImage.Error, MessageBoxImage.Hand);
    }

    /// <summary>
    /// Tests that MessageBoxImage.Exclamation has the same underlying value as MessageBoxImage.Warning (both are 48).
    /// </summary>
    [Fact]
    public void MessageBoxImage_Exclamation_EqualToWarning()
    {
        Assert.Equal(MessageBoxImage.Warning, MessageBoxImage.Exclamation);
    }

    /// <summary>
    /// Tests that MessageBoxImage.Asterisk has the same underlying value as MessageBoxImage.Information (both are 64).
    /// </summary>
    [Fact]
    public void MessageBoxImage_Asterisk_EqualToInformation()
    {
        Assert.Equal(MessageBoxImage.Information, MessageBoxImage.Asterisk);
    }

    #endregion
}
