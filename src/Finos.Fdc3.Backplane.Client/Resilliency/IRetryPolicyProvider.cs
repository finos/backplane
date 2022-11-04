/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Polly;
using System;

namespace Finos.Fdc3.Backplane.Client.Resilliency
{

    /// <summary>
    /// Resilliency
    /// </summary>
    public interface IRetryPolicyProvider
    {
        /// <summary>
        /// Builds an Polly.Retry.AsyncRetryPolicy that will wait and retry retryCount times
        /// calling onRetry on each retry with the raised exception, the current sleep duration,
        /// retry count, and context data. On each retry, the duration to wait is calculated
        /// by calling retryIntervalProvider with the current retry number (1 for first retry,
        /// 2 for second etc) and execution context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="retryCount"></param>
        /// <param name="retryIntervalProvider"></param>
        /// <returns></returns>
        IAsyncPolicy GetAsyncRetryPolicy<T>(int retryCount, Func<int, TimeSpan> retryIntervalProvider) where T : Exception;
    }
}