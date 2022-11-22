/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Member nodes repository.
    /// This is updated with latest live nodes through health check.
    /// </summary>
    public class NodesRepository : INodesRepository, IDisposable
    {
        private readonly ILogger<NodesRepository> _logger;
        private readonly ReaderWriterLockSlim _lock;
        private Immutable­Array<Uri> _value;
        private bool _isDisposed;

        public NodesRepository(ILogger<NodesRepository> logger)
        {
            _lock = new ReaderWriterLockSlim();
            _value = ImmutableArray.Create<Uri>();
            _logger = logger;
        }

        /// <summary>
        /// List of nodes uri running on other host under same user.
        /// Broadcast context propagation happens over nodes in this list only.
        /// </summary>
        public IEnumerable<Uri> MemberNodes
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _value;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Add a node to repository.For example dead node comes alive, it adds itself as member node
        /// </summary>
        /// <param name="nodeUri"></param>
        public void AddNode(Uri nodeUri)
        {
            AddRemoveNode(nodeUri, false);
        }

        /// <summary>
        /// Remove node from repository. For example a dead/non-responding node.
        /// </summary>
        /// <param name="nodeUri"></param>
        public void RemoveNode(Uri nodeUri)
        {
            AddRemoveNode(nodeUri, true);
        }

        private void AddRemoveNode(Uri node, bool isRemove)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }
            _lock.EnterWriteLock();
            try
            {
                if (isRemove)
                {
                    _value = _value.Remove(node);
                }
                else
                {
                    if (!_value.Contains(node))
                    {
                        _value = _value.Add(node);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            _logger.LogDebug($"Member nodes repo:{node} {(isRemove ? "removed" : "added")}");
        }



        protected virtual void Dispose(bool disposeManagedObjects)
        {
            if (!_isDisposed)
            {
                if (disposeManagedObjects)
                {
                    _lock.Dispose();
                }
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(disposeManagedObjects: true);
            GC.SuppressFinalize(this);
        }
    }
}
