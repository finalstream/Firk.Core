using System;
using System.IO;
using System.Reflection;
using FinalstreamCommons.Systems;
using FinalstreamCommons.Utils;
using Newtonsoft.Json;
using NLog;

namespace Firk.Core
{
    /// <summary>
    ///     アプリケーションのクライアントを表します。
    /// </summary>
    public abstract class AppClient : IAppClient
    {
        private readonly Assembly _executingAssembly;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected readonly AssemblyInfoData ExecutingAssemblyInfo;

        /// <summary>
        ///     新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="executingAssembly"></param>
        protected AppClient(Assembly executingAssembly)
        {
            _executingAssembly = executingAssembly;
            ExecutingAssemblyInfo = new AssemblyInfoData(_executingAssembly);
        }

        public IAppConfig AppConfig { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public abstract void Dispose();

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
        ///     終了処理を行います。
        /// </summary>
        public void Finish()
        {
            FinalizeCore();
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
    }
}