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
using System.Reflection;

namespace FrameworkInterfaces.Undo.Actions
{
    /// <summary>
    /// Represents a property value change that can be undone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This action captures the old and new values of a property change
    /// and can restore the old value on undo.
    /// </para>
    /// <para>
    /// Supports merging of rapid changes to the same property (within a configurable
    /// time window) to avoid creating too many undo entries during typing.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class PropertyChangeAction : IUndoableAction
    {
        #region Fields

        private readonly object _target;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private object _newValue;
        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Time window in milliseconds for merging rapid changes.
        /// </summary>
        public static int MergeWindowMilliseconds = 500;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeAction"/> class.
        /// </summary>
        /// <param name="target">The object whose property changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="target"/> or <paramref name="propertyName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the property is not found on the target object.
        /// </exception>
        public PropertyChangeAction(object target, string propertyName, object oldValue, object newValue)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _oldValue = oldValue;
            _newValue = newValue;

            _propertyInfo = target.GetType().GetProperty(propertyName);
            if (_propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{target.GetType().Name}'", nameof(propertyName));
            }

            Timestamp = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                // Try to get a meaningful name from the target
                string targetName = null;
                if (_target is IElement element)
                {
                    targetName = element.DisplayName;
                }
                else if (_target is IMetaData metaData)
                {
                    targetName = metaData.Name;
                }

                if (!string.IsNullOrEmpty(targetName))
                {
                    return $"Change {_propertyName} on {targetName}";
                }

                return $"Change {_propertyName}";
            }
        }

        /// <inheritdoc/>
        public DateTime Timestamp { get; private set; }

        /// <inheritdoc/>
        public object Target => _target;

        /// <summary>
        /// Gets the name of the property that changed.
        /// </summary>
        public string PropertyName => _propertyName;

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        public object OldValue => _oldValue;

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        public object NewValue => _newValue;

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void Execute()
        {
            _propertyInfo.SetValue(_target, _newValue);
        }

        /// <inheritdoc/>
        public void Undo()
        {
            _propertyInfo.SetValue(_target, _oldValue);
        }

        /// <inheritdoc/>
        public bool CanMergeWith(IUndoableAction other)
        {
            // Only merge with other PropertyChangeActions
            if (!(other is PropertyChangeAction pca)) return false;

            // Must be same target and property
            if (!ReferenceEquals(pca._target, _target)) return false;
            if (pca._propertyName != _propertyName) return false;

            // Must be within the merge window
            var timeDiff = (pca.Timestamp - Timestamp).TotalMilliseconds;
            if (timeDiff < 0 || timeDiff > MergeWindowMilliseconds) return false;

            return true;
        }

        /// <inheritdoc/>
        public IUndoableAction MergeWith(IUndoableAction other)
        {
            if (!(other is PropertyChangeAction pca)) return this;

            // Keep old value from this action, new value from other action
            return new PropertyChangeAction(_target, _propertyName, _oldValue, pca._newValue)
            {
                Timestamp = pca.Timestamp // Use the later timestamp
            };
        }

        #endregion
    }
}
