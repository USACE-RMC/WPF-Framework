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
        private readonly object? _oldValue;
        private object? _newValue;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _syncLock = new object();

        private static int _mergeWindowMilliseconds = 500;

        /// <summary>
        /// Gets or sets the time window in milliseconds for merging rapid changes to the same property.
        /// Must be greater than or equal to zero. Default is 500 ms.
        /// </summary>
        public static int MergeWindowMilliseconds
        {
            get => _mergeWindowMilliseconds;
            set => _mergeWindowMilliseconds = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "MergeWindowMilliseconds must be >= 0.");
        }

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
        public PropertyChangeAction(object target, string propertyName, object? oldValue, object? newValue)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _oldValue = oldValue;
            _newValue = newValue;

            var propertyInfo = target.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{target.GetType().Name}'", nameof(propertyName));
            }
            _propertyInfo = propertyInfo;

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
                string? targetName = null;
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
        public object? OldValue => _oldValue;

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        public object? NewValue => _newValue;

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void Execute()
        {
            lock (_syncLock)
            {
                _propertyInfo.SetValue(_target, _newValue);
            }
        }

        /// <inheritdoc/>
        public void Undo()
        {
            lock (_syncLock)
            {
                _propertyInfo.SetValue(_target, _oldValue);
            }
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
