using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firk.Core.Actions;

namespace Firk.Core
{
    /// <summary>
    ///     バッググラウンドで動作するスレッドを表します。
    /// </summary>
    public class BackgroundWorker : IDisposable
    {
        private readonly TimeSpan _interval;
        private CancellationTokenSource _cancellationTokenSource;

        public BackgroundWorker(TimeSpan interval, ICollection<BackgroundAction> backgroundActions)
        {
            _interval = interval;
            BackgroundActions = backgroundActions;
        }

        public ICollection<BackgroundAction> BackgroundActions { get; private set; }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // タスクを開始する。
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                    foreach (var backgroundAction in BackgroundActions)
                    {
                        backgroundAction.InvokeAsync();
                    }

                    Task.Delay(_interval).Wait();
                }
            },
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        #region Dispose

        // Flag: Has Dispose already been called?
        private bool disposed;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                _cancellationTokenSource.Cancel();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion
    }
}