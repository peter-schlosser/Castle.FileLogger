// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Test

using System.Threading.Tasks;

namespace Castle.Extensions.Logging.FileLogger.Test
{
    internal class ManualIntervalControl
    {
        private TaskCompletionSource<object> _pauseCompletionSource = new TaskCompletionSource<object>();
        private TaskCompletionSource<object> _resumeCompletionSource;

        public Task Pause => _pauseCompletionSource.Task;

        public void Resume()
        {
            _pauseCompletionSource = new TaskCompletionSource<object>();
            _resumeCompletionSource.SetResult(null);
        }

        public async Task IntervalAsync()
        {
            _resumeCompletionSource = new TaskCompletionSource<object>();
            _pauseCompletionSource.SetResult(null);

            await _resumeCompletionSource.Task;
        }
    }
}
