using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    public class UpdateDownloadResultTests
    {
        #region Property Tests

        [Fact]
        public void Success_CanBeSetAndRetrieved()
        {
            var result = new UpdateDownloadResult { Success = true };

            Assert.True(result.Success);
        }

        [Fact]
        public void FilePath_CanBeSetAndRetrieved()
        {
            var path = "/tmp/update.zip";
            var result = new UpdateDownloadResult { FilePath = path };

            Assert.Equal(path, result.FilePath);
        }

        [Fact]
        public void Error_CanBeSetAndRetrieved()
        {
            var exception = new InvalidOperationException("Test error");
            var result = new UpdateDownloadResult { Error = exception };

            Assert.Same(exception, result.Error);
        }

        [Fact]
        public void WasCancelled_CanBeSetAndRetrieved()
        {
            var result = new UpdateDownloadResult { WasCancelled = true };

            Assert.True(result.WasCancelled);
        }

        [Fact]
        public void BytesDownloaded_CanBeSetAndRetrieved()
        {
            var result = new UpdateDownloadResult { BytesDownloaded = 1024 * 1024 };

            Assert.Equal(1024 * 1024, result.BytesDownloaded);
        }

        [Fact]
        public void Update_CanBeSetAndRetrieved()
        {
            var update = new UpdateInfo { Name = "Test Update" };
            var result = new UpdateDownloadResult { Update = update };

            Assert.Same(update, result.Update);
        }

        #endregion

        #region Successful Factory Method Tests

        [Fact]
        public void Successful_SetsSuccessToTrue()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024);

            Assert.True(result.Success);
        }

        [Fact]
        public void Successful_SetsFilePath()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024);

            Assert.Equal("/tmp/update.zip", result.FilePath);
        }

        [Fact]
        public void Successful_SetsUpdate()
        {
            var update = new UpdateInfo { Name = "Test" };
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024);

            Assert.Same(update, result.Update);
        }

        [Fact]
        public void Successful_SetsBytesDownloaded()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024 * 1024);

            Assert.Equal(1024 * 1024, result.BytesDownloaded);
        }

        [Fact]
        public void Successful_DoesNotSetError()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024);

            Assert.Null(result.Error);
        }

        [Fact]
        public void Successful_DoesNotSetWasCancelled()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/update.zip", update, 1024);

            Assert.False(result.WasCancelled);
        }

        #endregion

        #region Failed Factory Method Tests

        [Fact]
        public void Failed_SetsSuccessToFalse()
        {
            var error = new Exception("Test");
            var result = UpdateDownloadResult.Failed(error);

            Assert.False(result.Success);
        }

        [Fact]
        public void Failed_SetsError()
        {
            var error = new InvalidOperationException("Test error");
            var result = UpdateDownloadResult.Failed(error);

            Assert.Same(error, result.Error);
        }

        [Fact]
        public void Failed_DoesNotSetFilePath()
        {
            var error = new Exception("Test");
            var result = UpdateDownloadResult.Failed(error);

            Assert.Null(result.FilePath);
        }

        [Fact]
        public void Failed_DoesNotSetWasCancelled()
        {
            var error = new Exception("Test");
            var result = UpdateDownloadResult.Failed(error);

            Assert.False(result.WasCancelled);
        }

        [Fact]
        public void Failed_DoesNotSetUpdate()
        {
            var error = new Exception("Test");
            var result = UpdateDownloadResult.Failed(error);

            Assert.Null(result.Update);
        }

        [Fact]
        public void Failed_WithHttpRequestException_PreservesExceptionType()
        {
            var error = new System.Net.Http.HttpRequestException("Network error");
            var result = UpdateDownloadResult.Failed(error);

            Assert.IsType<System.Net.Http.HttpRequestException>(result.Error);
        }

        #endregion

        #region Cancelled Factory Method Tests

        [Fact]
        public void Cancelled_SetsSuccessToFalse()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.False(result.Success);
        }

        [Fact]
        public void Cancelled_SetsWasCancelledToTrue()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.True(result.WasCancelled);
        }

        [Fact]
        public void Cancelled_WithZeroBytesDownloaded_DefaultsToZero()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.Equal(0, result.BytesDownloaded);
        }

        [Fact]
        public void Cancelled_WithBytesDownloaded_SetsBytesDownloaded()
        {
            var result = UpdateDownloadResult.Cancelled(bytesDownloaded: 5000);

            Assert.Equal(5000, result.BytesDownloaded);
        }

        [Fact]
        public void Cancelled_DoesNotSetError()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.Null(result.Error);
        }

        [Fact]
        public void Cancelled_DoesNotSetFilePath()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.Null(result.FilePath);
        }

        [Fact]
        public void Cancelled_DoesNotSetUpdate()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.Null(result.Update);
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void NewInstance_HasFalseSuccess()
        {
            var result = new UpdateDownloadResult();

            Assert.False(result.Success);
        }

        [Fact]
        public void NewInstance_HasNullFilePath()
        {
            var result = new UpdateDownloadResult();

            Assert.Null(result.FilePath);
        }

        [Fact]
        public void NewInstance_HasNullError()
        {
            var result = new UpdateDownloadResult();

            Assert.Null(result.Error);
        }

        [Fact]
        public void NewInstance_HasFalseWasCancelled()
        {
            var result = new UpdateDownloadResult();

            Assert.False(result.WasCancelled);
        }

        [Fact]
        public void NewInstance_HasZeroBytesDownloaded()
        {
            var result = new UpdateDownloadResult();

            Assert.Equal(0, result.BytesDownloaded);
        }

        [Fact]
        public void NewInstance_HasNullUpdate()
        {
            var result = new UpdateDownloadResult();

            Assert.Null(result.Update);
        }

        #endregion

        #region Combination Tests

        [Fact]
        public void SuccessfulResult_IsNotCancelledOrFailed()
        {
            var update = new UpdateInfo();
            var result = UpdateDownloadResult.Successful("/tmp/file.zip", update, 1024);

            Assert.True(result.Success);
            Assert.False(result.WasCancelled);
            Assert.Null(result.Error);
        }

        [Fact]
        public void FailedResult_IsNotSuccessfulOrCancelled()
        {
            var result = UpdateDownloadResult.Failed(new Exception());

            Assert.False(result.Success);
            Assert.False(result.WasCancelled);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void CancelledResult_IsNotSuccessfulButHasNoError()
        {
            var result = UpdateDownloadResult.Cancelled();

            Assert.False(result.Success);
            Assert.True(result.WasCancelled);
            Assert.Null(result.Error);
        }

        #endregion
    }
}
