/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.API
{

    internal class Listener : IListener
    {
        private readonly Func<CancellationToken, Task> _unsubscribe;

        public Listener(Func<CancellationToken, Task> unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public async Task UnsubscribeAsync(CancellationToken ct = default)
        {
            await _unsubscribe(ct);

        }
    }
}