using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Firk.Core
{
    public abstract class AppConfig : IAppConfig
    {
        public string AppVersion { get; set; }
        public int SchemaVersion { get; set; }
        public Rect WindowBounds { get; set; }


        public virtual void UpdateSchemaVersion(int version)
        {
            SchemaVersion = version;
        }

        public virtual void Update<T>(T config) where T: AppConfig
        {
            this.AppVersion = config.AppVersion;
            this.SchemaVersion = config.SchemaVersion;
            this.WindowBounds = config.WindowBounds;
            UpdateCore(config);
        }

        protected abstract void UpdateCore<T>(T config);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
