using MapTileGenerator.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator
{
    public class ProviderFactory
    {
        public static ISourceProvider CreateSourceProvider(MapConfig config)
        {
            ISourceProvider source = null;      
            switch (config.Type.ToLower())
            {
                case "baidu":
                    source = new BaiduMapProvider(config);
                    break;
                case "wmts" :
                    source = new WmtsSourceProvider(config);
                    break;
                case "wms":
                    source = new WmsSourceProvider(config);
                    break;
                case "arcserverrest":
                    source = new ArcServerRestProvider(config);
                    break;
            }
            return source;
        }

        public static ITileOutputStrategy CreateOutputStrategy(MapConfig config)
        {
            ITileOutputStrategy result = null;
           switch (config.Output.ToLower())
            {
                case "file":
                    result = new DefaultOutputStrategy();
                    break;
                case "sqlite":
                    result = new SqliteOutputStrategy();
                    break;
                default:
                    result = new DefaultOutputStrategy();
                    break;
            }
            return result;
        }
    }
}
