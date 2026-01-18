using Xunit;
using FrameworkInterfaces;
using FrameworkInterfaces.Messaging;
using System.Windows.Media;
using System.IO;

namespace FrameworkInterfaces.Tests.Messaging
{
    public class MessengerTests
    {
        private Messenger GetFreshMessenger()
        {
            var messenger = Messenger.GetInstance();
            messenger.Clear();
            return messenger;
        }

        #region Singleton Tests

        [Fact]
        public void GetInstance_ReturnsSameInstance()
        {
            var instance1 = Messenger.GetInstance();
            var instance2 = Messenger.GetInstance();

            Assert.Same(instance1, instance2);
        }

        #endregion

        #region Add Message Tests

        [Fact]
        public void Add_SingleMessage_AddsToCollection()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message = new BasicMessageItem(MessageType.Error, "Test error", source, "Collection", "Name", code: "TST-001");

            messenger.Add(message);

            var allMessages = messenger.AllMessageItems();
            Assert.Single(allMessages);
            Assert.Same(message, allMessages[0]);
        }

        [Fact]
        public void Add_NullMessage_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.Add((IMessageItem)null!));
        }

        [Fact]
        public void Add_MessageWithNullSource_DoesNotAdd()
        {
            var messenger = GetFreshMessenger();
            var message = new BasicMessageItem { Source = null, Code = "TST-001" };

            messenger.Add(message);

            Assert.Empty(messenger.AllMessageItems());
        }

        [Fact]
        public void Add_DuplicateMessage_IgnoresDuplicate()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message1 = new BasicMessageItem(MessageType.Error, "Error 1", source, "Collection", "Name", code: "TST-001");
            var message2 = new BasicMessageItem(MessageType.Error, "Error 2", source, "Collection", "Name", code: "TST-001");

            messenger.Add(message1);
            messenger.Add(message2);

            var allMessages = messenger.AllMessageItems();
            Assert.Single(allMessages);
            Assert.Equal("Error 1", allMessages[0].Description);
        }

        [Fact]
        public void Add_EventMessage_AutoIncrementsCode()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var event1 = new BasicMessageItem(MessageType.Event, "Event 1", source, "Collection", "Name", code: "EVT-001");
            var event2 = new BasicMessageItem(MessageType.Event, "Event 2", source, "Collection", "Name", code: "EVT-001");

            messenger.Add(event1);
            messenger.Add(event2);

            var allMessages = messenger.AllMessageItems();
            Assert.Equal(2, allMessages.Count);
            Assert.Equal("EVT-001", event1.Code);
            Assert.Equal("EVT-0011", event2.Code);
        }

        [Fact]
        public void Add_RaisesMessagesAddedEvent()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message = new BasicMessageItem(MessageType.Warning, "Warning", source, "Collection", "Name", code: "WRN-001");
            IMessageItem[]? addedMessages = null;

            messenger.MessagesAdded += msgs => addedMessages = msgs;
            messenger.Add(message);

            Assert.NotNull(addedMessages);
            Assert.Single(addedMessages);
            Assert.Same(message, addedMessages[0]);
        }

        [Fact]
        public void Add_MultipleMessages_AddsAll()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var messages = new[]
            {
                new BasicMessageItem(MessageType.Error, "Error 1", source, "Collection", "Name", code: "ERR-001"),
                new BasicMessageItem(MessageType.Warning, "Warning 1", source, "Collection", "Name", code: "WRN-001"),
                new BasicMessageItem(MessageType.Message, "Message 1", source, "Collection", "Name", code: "MSG-001")
            };

            messenger.Add(messages);

            Assert.Equal(3, messenger.AllMessageItems().Count);
        }

        [Fact]
        public void Add_NullEnumerable_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.Add((IEnumerable<IMessageItem>)null!));
        }

        #endregion

        #region Remove Message Tests

        [Fact]
        public void Remove_ExistingMessage_ReturnsTrue()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message = new BasicMessageItem(MessageType.Error, "Error", source, "Collection", "Name", code: "ERR-001");
            messenger.Add(message);

            var result = messenger.Remove(message);

            Assert.True(result);
            Assert.Empty(messenger.AllMessageItems());
        }

        [Fact]
        public void Remove_NonExistentMessage_ReturnsFalse()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message = new BasicMessageItem(MessageType.Error, "Error", source, "Collection", "Name", code: "ERR-001");

            var result = messenger.Remove(message);

            Assert.False(result);
        }

        [Fact]
        public void Remove_NullMessage_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.Remove(null!));
        }

        [Fact]
        public void Remove_MessageWithNullSource_ReturnsFalse()
        {
            var messenger = GetFreshMessenger();
            var message = new BasicMessageItem { Source = null };

            Assert.False(messenger.Remove(message));
        }

        [Fact]
        public void Remove_RaisesMessagesRemovedEvent()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            var message = new BasicMessageItem(MessageType.Error, "Error", source, "Collection", "Name", code: "ERR-001");
            messenger.Add(message);
            IMessageItem[]? removedMessages = null;

            messenger.MessagesRemoved += msgs => removedMessages = msgs;
            messenger.Remove(message);

            Assert.NotNull(removedMessages);
            Assert.Single(removedMessages);
        }

        #endregion

        #region Clear Tests

        [Fact]
        public void Clear_RemovesAllMessages()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            messenger.Add(new BasicMessageItem(MessageType.Error, "Error 1", source, "Collection", "Name", code: "ERR-001"));
            messenger.Add(new BasicMessageItem(MessageType.Error, "Error 2", source, "Collection", "Name", code: "ERR-002"));

            messenger.Clear();

            Assert.Empty(messenger.AllMessageItems());
        }

        [Fact]
        public void Clear_WithSource_RemovesOnlySourceMessages()
        {
            var messenger = GetFreshMessenger();
            var source1 = new object();
            var source2 = new object();
            messenger.Add(new BasicMessageItem(MessageType.Error, "Error 1", source1, "Collection", "Name", code: "ERR-001"));
            messenger.Add(new BasicMessageItem(MessageType.Error, "Error 2", source2, "Collection", "Name", code: "ERR-002"));

            messenger.Clear(source1);

            var allMessages = messenger.AllMessageItems();
            Assert.Single(allMessages);
            Assert.Equal("Error 2", allMessages[0].Description);
        }

        [Fact]
        public void Clear_WithNullSource_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.Clear(null!));
        }

        [Fact]
        public void Clear_WithNonExistentSource_DoesNotThrow()
        {
            var messenger = GetFreshMessenger();
            var source = new object();

            var exception = Record.Exception(() => messenger.Clear(source));

            Assert.Null(exception);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ShowErrors_DefaultTrue()
        {
            var messenger = Messenger.GetInstance();
            Assert.True(messenger.ShowErrors);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShowErrors_SetAndGet(bool value)
        {
            var messenger = Messenger.GetInstance();
            messenger.ShowErrors = value;
            Assert.Equal(value, messenger.ShowErrors);
        }

        [Fact]
        public void ShowErrors_WhenChanged_RaisesPropertyChanged()
        {
            var messenger = Messenger.GetInstance();
            var originalValue = messenger.ShowErrors;
            var propertyChanged = false;
            messenger.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Messenger.ShowErrors))
                    propertyChanged = true;
            };

            messenger.ShowErrors = !originalValue;

            Assert.True(propertyChanged);
            messenger.ShowErrors = originalValue;
        }

        [Fact]
        public void ShowWarnings_DefaultTrue()
        {
            var messenger = Messenger.GetInstance();
            Assert.True(messenger.ShowWarnings);
        }

        [Fact]
        public void ShowMessages_DefaultTrue()
        {
            var messenger = Messenger.GetInstance();
            Assert.True(messenger.ShowMessages);
        }

        [Fact]
        public void ShowEvents_DefaultTrue()
        {
            var messenger = Messenger.GetInstance();
            Assert.True(messenger.ShowEvents);
        }

        [Fact]
        public void ErrorColor_DefaultRed()
        {
            var messenger = Messenger.GetInstance();
            Assert.Equal(Colors.Red, messenger.ErrorColor.Color);
        }

        [Fact]
        public void WarningColor_DefaultDarkOrange()
        {
            var messenger = Messenger.GetInstance();
            Assert.Equal(Colors.DarkOrange, messenger.WarningColor.Color);
        }

        [Fact]
        public void MessageColor_DefaultBlue()
        {
            var messenger = Messenger.GetInstance();
            Assert.Equal(Colors.Blue, messenger.MessageColor.Color);
        }

        [Fact]
        public void EventColor_DefaultBlack()
        {
            var messenger = Messenger.GetInstance();
            Assert.Equal(Colors.Black, messenger.EventColor.Color);
        }

        [Fact]
        public void ErrorColor_SetNull_DoesNotChange()
        {
            var messenger = Messenger.GetInstance();
            var originalColor = messenger.ErrorColor;

            messenger.ErrorColor = null!;

            Assert.Same(originalColor, messenger.ErrorColor);
        }

        [Fact]
        public void TextFileName_SetAndGet()
        {
            var messenger = Messenger.GetInstance();
            var testPath = @"C:\test\messages.txt";

            messenger.TextFileName = testPath;

            Assert.Equal(testPath, messenger.TextFileName);
        }

        [Fact]
        public void TextFileName_SetNull_DefaultsToEmpty()
        {
            var messenger = Messenger.GetInstance();
            messenger.TextFileName = "test.txt";

            messenger.TextFileName = null!;

            Assert.Equal(string.Empty, messenger.TextFileName);
        }

        [Fact]
        public void WriteToFile_SetAndGet()
        {
            var messenger = Messenger.GetInstance();

            messenger.WriteToFile = true;
            Assert.True(messenger.WriteToFile);

            messenger.WriteToFile = false;
            Assert.False(messenger.WriteToFile);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ErrorBeep_SetAndGet(bool value)
        {
            var messenger = Messenger.GetInstance();
            messenger.ErrorBeep = value;
            Assert.Equal(value, messenger.ErrorBeep);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WarningBeep_SetAndGet(bool value)
        {
            var messenger = Messenger.GetInstance();
            messenger.WarningBeep = value;
            Assert.Equal(value, messenger.WarningBeep);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MessageBeep_SetAndGet(bool value)
        {
            var messenger = Messenger.GetInstance();
            messenger.MessageBeep = value;
            Assert.Equal(value, messenger.MessageBeep);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EventBeep_SetAndGet(bool value)
        {
            var messenger = Messenger.GetInstance();
            messenger.EventBeep = value;
            Assert.Equal(value, messenger.EventBeep);
        }

        #endregion

        #region Export Tests

        [Fact]
        public void ExportToTextFile_NullFileName_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.ExportToTextFile(null!));
        }

        [Fact]
        public void ExportToTextFile_EmptyFileName_ThrowsArgumentNullException()
        {
            var messenger = GetFreshMessenger();

            Assert.Throws<ArgumentNullException>(() => messenger.ExportToTextFile(string.Empty));
        }

        [Fact]
        public void ExportToTextFile_WritesMessages()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            messenger.Add(new BasicMessageItem(MessageType.Error, "Test error message", source, "Collection", "Element", code: "ERR-001"));

            var tempFile = Path.Combine(Path.GetTempPath(), $"messenger_test_{Guid.NewGuid()}.txt");
            try
            {
                messenger.ExportToTextFile(tempFile);

                Assert.True(File.Exists(tempFile));
                var content = File.ReadAllText(tempFile);
                Assert.Contains("Test error message", content);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void ExportToTextFile_CreatesDirectoryIfNotExists()
        {
            var messenger = GetFreshMessenger();
            var source = new object();
            messenger.Add(new BasicMessageItem(MessageType.Message, "Test message", source, "Collection", "Element", code: "MSG-001"));

            var tempDir = Path.Combine(Path.GetTempPath(), $"messenger_test_{Guid.NewGuid()}");
            var tempFile = Path.Combine(tempDir, "messages.txt");
            try
            {
                messenger.ExportToTextFile(tempFile);

                Assert.True(Directory.Exists(tempDir));
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        #endregion
    }
}
