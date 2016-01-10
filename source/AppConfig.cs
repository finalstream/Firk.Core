using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Firk.Core
{
    public class AppConfig : IAppConfig
    {
        public string AppVersion { get; set; }
        public int SchemaVersion { get; set; }
        public Rect WindowBounds { get; set; }


        public virtual void UpdateSchemaVersion(int version)
        {
            SchemaVersion = version;
        }

        public virtual void Update<T>(T config) where T: IAppConfig
        {
            this.AppVersion = config.AppVersion;
            this.SchemaVersion = config.SchemaVersion;
            this.WindowBounds = config.WindowBounds;
        }
    }
}
