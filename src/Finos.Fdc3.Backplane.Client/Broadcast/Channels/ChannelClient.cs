/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Broadcast.Channels
{
    internal class ChannelClient : IChannel
    {
        private readonly IClientMiddleware _clientMiddleware;

        public ChannelClient(IClientMiddleware clientMiddleware, Channel channelDto)
        {
            _clientMiddleware = clientMiddleware;
            Id = channelDto.Id;
            Type = channelDto.Type;
            DisplayMetadata = channelDto.DisplayMetadata;
        }

        public string Id { get; }

        public string Type { get; }

        public DisplayMetadata DisplayMetadata { get; }

        public async Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, CancellationToken ct = default)
        {
            return await _clientMiddleware.AddContextListenerAsync(contextType, handler, Id, ct);
        }

        public async Task BroadcastAsync(IContext context, CancellationToken ct = default)
        {
            await _clientMiddleware.BroadcastAsync(context, Id, ct);
        }

        public async Task<IContext> GetCurrentContextAsync(string contextype = null, CancellationToken ct = default)
        {
            IContext context = await _clientMiddleware.GetCurrentContextAsync(Id, ct);
            if (contextype == null || contextype == context?.Type)
            {
                return context;
            }
            return null;

        }

        public override string ToString()
        {
            return $"{Id}-{Type}-{DisplayMetadata?.Name}";
        }
    }
}

