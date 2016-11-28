using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class SourceProviderFactory
    {
        public static ISourceProvider CreateSourceProvider(MapConfig config)
        {
            var tileSize = new Size(config.TileSize);
            var extent = new Extent(config.Extent);
            Coordinate origin = origin = new Coordinate(config.Origin);          
            string url = config.Url;
            var paras = config.UrlParas;
            ISourceProvider source = null;
            switch (config.Type.ToUpper())
            {
                case "BAIDU":
                    var tileGrid = new TmsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new BaiduMapProvider(tileGrid, config.Url, paras);
                    break;
                case "WMTS" :
                    var wmtsTileGrid = new WmtsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmtsSourceProvider(wmtsTileGrid, url, paras);
                    break;
                case "WMS":
                    tileGrid = new TmsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmsSourceProvider(tileGrid, config.Url, paras);
                    break;
                case "ARCSERVERREST":
                    tileGrid = new WmtsTileGrid(config.Resolutions, extent, origin, tileSize);//new TileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new ArcServerRestProvider(tileGrid, config.Url, paras);
                    break;
            }
            return source;
        }
    }
}
