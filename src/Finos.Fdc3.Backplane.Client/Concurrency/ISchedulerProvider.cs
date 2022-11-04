/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using System.Reactive.Concurrency;

namespace Finos.Fdc3.Backplane.Client.Concurrency
{
    /// <summary>
    /// Scheduler Provider
    /// </summary>
    public interface ISchedulerProvider
    {
        /// <summary>
        /// TaskPool Scheduler
        /// </summary>
        IScheduler TaskPool { get; }
    }

}
