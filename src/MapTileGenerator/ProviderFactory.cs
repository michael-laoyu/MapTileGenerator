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
                case "gaode":
                    source = new GaodeMapProvider(config);
                    break;
                case "tencent":
                    source = new TencentMapProvider(config);
                    break;
                case "tms":
                    source = new TmsSourceProvider(config);
                    break;
                case "wmts" :
                    source = new WmtsSourceProvider(config);
                    break;
                case "wmtsxyz":
                    source = new WmtsXyzTileSourceProvider(config);
                    break;
                case "wms":
                    source = new WmsSourceProvider(config);
                    break;
                case "arcserverrest":
                    source = new ArcServerRestProvider(config);
                    break;
                case "arcserverlocaltile":
                    source = new ArcServerLocalTileProvider(config);
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
                case "sqliteandbase64":
                    result = new SqliteAndBase64OutputStrategy();
                    break;
                default:
                    result = new DefaultOutputStrategy();
                    break;
            }
            return result;
        }
    }
}
