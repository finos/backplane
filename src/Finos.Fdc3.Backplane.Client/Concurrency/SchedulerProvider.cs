/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using System.Reactive.Concurrency;

namespace Finos.Fdc3.Backplane.Client.Concurrency
{
    internal class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler TaskPool => TaskPoolScheduler.Default;
    }
}
