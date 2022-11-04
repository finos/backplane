/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using NLog;
using System;
using System.Threading;

namespace Finos.Fdc3.Backplane
{
    public class SingleInstance : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Mutex _mutex;
        private bool _hasHandle;

        public bool IsAlreadyRunning => !_hasHandle;

        public SingleInstance()
        {
            _mutex = new Mutex(false, $"{GetMutexId()}", out _hasHandle);
            TryAcquire(TimeSpan.FromSeconds(5));
        }

        protected void TryAcquire(TimeSpan timeout)
        {
            try
            {
                _hasHandle = _mutex.WaitOne(timeout);
            }
            catch (AbandonedMutexException abandonedMutexException)
            {
                Logger.Warn($"Previous instance did not cleanly exit: \n{abandonedMutexException}");
                _hasHandle = true;
            }
        }

        private static string GetMutexId()
        {
            return $"Global\\{{{new Guid("9683584a-9c5e-4926-a2af-4f31829bdd2c")}}}";
        }

        public void Dispose()
        {
            if (_mutex != null)
            {
                Logger.Info("Disposing mutex");

                if (_hasHandle)
                {
                    _mutex.ReleaseMutex();
                }

                _hasHandle = false;
                _mutex.Close();
            }

            Logger.Info("Finished disposing single instance");
        }
    }
}