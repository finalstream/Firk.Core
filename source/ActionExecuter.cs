﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Firk.Core.Actions;
using NLog;

namespace Firk.Core
{
    /// <summary>
    ///     アクションを実行するものを表します。
    /// </summary>
    public class ActionExecuter<T> : IDisposable
    {
        private readonly IDisposable _handle;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly ISubject<IGeneralAction<T>> _subject;

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="param">アクションの引数</param>
        public ActionExecuter(T param)
        {
            _subject = new Subject<IGeneralAction<T>>();

            _handle = _subject.ObserveOn(_scheduler)
                .Subscribe(x =>
                {
                    {
                        try
                        {
                            x.Invoke(param);
                        }
                        catch (TaskCanceledException)
                        {
                            // タスク実行中に終了したら発生するので捨てる。
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex, string.Format("Execute Action Failed. Action:{0}", x.GetType()));
                            OnExecuteFailed(ex);
                        }
                    }
                });
        }

        public void Post(IGeneralAction<T> action)
        {
            _subject.OnNext(action);
        }

        #region ExecuteFailedイベント

        public event EventHandler<Exception> ExecuteFailed;

        protected virtual void OnExecuteFailed(Exception ex)
        {
            var handler = ExecuteFailed;
            if (handler != null)
            {
                handler(this, ex);
            }
        }

        #endregion

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
                _subject.OnCompleted();
                _handle.Dispose();
                _scheduler.Dispose();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion
    }
}