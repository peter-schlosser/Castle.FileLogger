// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
using System;

namespace Castle.Extensions.Logging.FileLogger
{
    public struct CastleLogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
    }
}
