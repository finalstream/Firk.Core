using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
