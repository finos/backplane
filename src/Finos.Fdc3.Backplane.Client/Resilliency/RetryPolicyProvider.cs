/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Polly;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Resilliency
{
    internal class RetryPolicyProvider : IRetryPolicyProvider
    {
        public IAsyncPolicy GetAsyncRetryPolicy<T>(int retryCount, Func<int, TimeSpan> retryIntervalProvider) where T : Exception
        {
            return Policy.Handle<T>().WaitAndRetryAsync(retryCount, retryIntervalProvider);
        }
    }
}
