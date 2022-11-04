/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Finos.Fdc3.Backplane.Core.MultiHost
{
    /// <summary>
    /// This service will keep member backplane nodes updated in background and 
    /// add resiliency to cluster service discovery service.
    /// </summary>
    public class NodesRepository : INodesRepository, IDisposable
    {
        private readonly INodesDiscoveryClient _clusterService;
        private readonly IConfiguration _config;
        private readonly ILogger<NodesRepository> _logger;

        public NodesRepository(INodesDiscoveryClient clusterService, IConfiguration config, ILogger<NodesRepository> logger)
        {
            _lock_obj = new ReaderWriterLockSlim();
            _value = new List<Node>();
            _clusterService = clusterService;
            _config = config;
            _logger = logger;
        }

        private readonly ReaderWriterLockSlim _lock_obj;

        private readonly List<Node> _value;
        private bool _already_disposed;

        public IReadOnlyList<Node> MemberNodes
        {
            get
            {
                _lock_obj.EnterReadLock();
                try
                {
                    return _value;
                }
                finally
                {
                    _lock_obj.ExitReadLock();
                }
            }
        }

        public Uri CurrentNodeUri { get; set; }

        public bool AddOrUpdateActiveNode(Uri value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _lock_obj.EnterWriteLock();
            try
            {
                Node nodeToBeActivated = _value.FirstOrDefault(x => x.Uri.AbsoluteUri == value.AbsoluteUri);
                if (nodeToBeActivated == null)
                {
                    _value.Add(new Node() { Uri = value, IsActive = true });
                    _logger.LogInformation($"{value.AbsoluteUri} activated url added to local cache.");
                }
                else
                {
                    if (!nodeToBeActivated.IsActive)
                    {
                        nodeToBeActivated.IsActive = true;
                        _logger.LogInformation($"{value.AbsoluteUri} url activated in local cache.");
                    }
                }
            }
            finally
            {
                _lock_obj.ExitWriteLock();
            }
            return true;
        }

        public bool AddOrUpdateDeactiveNode(Uri value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _lock_obj.EnterWriteLock();
            try
            {
                Node itemToBeRemoved = _value.FirstOrDefault(x => x.Uri.AbsoluteUri == value.AbsoluteUri);
                if (itemToBeRemoved != null)
                {
                    itemToBeRemoved.IsActive = false;
                    _logger.LogInformation($"{value.AbsoluteUri} url de-activated in local cache.");
                }
                else
                {
                    _value.Add(new Node() { Uri = value, IsActive = false });
                    _logger.LogInformation($"{value.AbsoluteUri} de-activated url added to local cache.");
                }
            }
            finally
            {
                _lock_obj.ExitWriteLock();
            }
            return true;
        }

        protected virtual void Dispose(bool disposeManagedObjects)
        {
            if (!_already_disposed)
            {
                if (disposeManagedObjects)
                {
                    _lock_obj.Dispose();
                }
                _already_disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposeManagedObjects: true);
            GC.SuppressFinalize(this);
        }
    }
}
