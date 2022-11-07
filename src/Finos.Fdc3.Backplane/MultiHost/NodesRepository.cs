/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Repository of member nodes
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

        public void AddNode(Uri nodeUri)
        {
            AddRemoveNode(nodeUri, false);
        }

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
                if(isRemove)
                {
                   _value= _value.Remove(node);
                }
                else 
                {
                    if(!_value.Contains(node))
                      _value=  _value.Add(node);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            _logger.LogDebug($"Member nodes repo:{node} {(isRemove? "removed" : "added")}");
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

        public void Dispose()
        {
            Dispose(disposeManagedObjects: true);
            GC.SuppressFinalize(this);
        }
    }
}
