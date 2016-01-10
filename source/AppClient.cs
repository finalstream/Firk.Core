using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Forms;
using FinalstreamCommons.Systems;
using FinalstreamCommons.Utils;
using Firk.Core.Actions;
using Firk.Core.Properties;
using Newtonsoft.Json;
using NLog;

namespace Firk.Core
{

    public abstract class AppClient<T> : AppClient where T : AppConfig, new()
    {
        protected AppClient(Assembly executingAssembly) : base(executingAssembly)
        {
            _appConfig = new T();
        }

        private T _appConfig;

        public new T AppConfig
        {
            get
            {
                return _appConfig;
            }
        }

        /// <summary>
        ///     初期化を行います。
        /// </summary>
        public new void Initialize()
        {
            var appConfigFilePath = GetConfigPath();
            if (File.Exists(appConfigFilePath)) AppConfig.Update(LoadConfig<T>(appConfigFilePath));
            AppConfig.AppVersion = ExecutingAssemblyInfo.FileVersion;

            base.Initialize();
        }

        public new void Finish()
        {
            base.Finish();
            SaveConfig(GetConfigPath(),AppConfig);
        }

        public abstract string GetConfigPath();
    }

    /// <summary>
    ///     アプリケーションのクライアントを表します。
    /// </summary>
    public abstract class AppClient : IAppClient
    {
        private readonly Assembly _executingAssembly;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected readonly AssemblyInfoData ExecutingAssemblyInfo;
        protected BackgroundWorker BackgroundWorker;

        /// <summary>
        ///     新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="executingAssembly"></param>
        protected AppClient(Assembly executingAssembly)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            _executingAssembly = executingAssembly;
            ExecutingAssemblyInfo = new AssemblyInfoData(_executingAssembly);
        }

        /// <summary>
        /// ハンドルされない例外を検出した場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error(e.ExceptionObject as Exception, "Catch UnhandledException.");

            MessageBox.Show(Resources.MessageUnknownError, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Environment.Exit(1);
        }

        public IAppConfig AppConfig { get; protected set; }
        public bool IsInitialized { get; protected set; }

        /// <summary>
        ///     初期化を行います。
        /// </summary>
        public void Initialize()
        {
            _log.Info("Start Application: {0} {1} {2}",
                ExecutingAssemblyInfo.Product,
                ExecutingAssemblyInfo.Version,
                ApplicationUtils.IsAssemblyDebugBuild(_executingAssembly) ? "Debug" : "Release");
            InitializeCore();
        }

        /// <summary>
        /// バックグラウンドワーカーをリセットします。
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="backgroundActions"></param>
        public void ResetBackgroundWorker(TimeSpan interval, BackgroundAction[] backgroundActions)
        {
            if (BackgroundWorker != null) BackgroundWorker.Dispose();

            BackgroundWorker = new BackgroundWorker(interval, backgroundActions);
            BackgroundWorker.Start();
        }

        /// <summary>
        ///     終了処理を行います。
        /// </summary>
        public void Finish()
        {
            FinalizeCore();
        }

        /// <summary>
        /// アプリバージョンを取得します。
        /// </summary>
        /// <returns></returns>
        public virtual string GetAppVersion()
        {
            return ExecutingAssemblyInfo.FileVersion;
        }

        /// <summary>
        ///     設定をファイルからロードします。
        /// </summary>
        protected T LoadConfig<T>(string configFilePath) where T : IAppConfig, new()
        {
            if (!File.Exists(configFilePath)) return new T();
            return JsonConvert.DeserializeObject<T>(
                File.ReadAllText(configFilePath));
        }

        /// <summary>
        ///     設定をファイルに保存します。
        /// </summary>
        protected void SaveConfig<T>(string configFilePath, T config) where T : IAppConfig, new()
        {
            if (configFilePath == null) return;
            Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        /// <summary>
        ///     初期化を行います。
        /// </summary>
        protected abstract void InitializeCore();

        /// <summary>
        ///     ファイナライズを行います。
        /// </summary>
        protected abstract void FinalizeCore();

        #region ExceptionThrowedイベント

        public event EventHandler<Exception> ExceptionThrowed;

        protected virtual void OnExceptionThrowed(Exception ex)
        {
            var handler = ExceptionThrowed;
            if (handler != null)
            {
                handler(this, ex);
            }
        }

        #endregion

        #region Dispose

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public virtual void Dispose()
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
                if(BackgroundWorker != null) BackgroundWorker.Dispose();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion
    }
}