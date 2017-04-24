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
            var tileSize = new Size(config.TileSize);
            var extent = new Extent(config.Extent);
            Coordinate origin = origin = new Coordinate(config.Origin);          
            string url = config.Url;
            var paras = config.UrlParas;
            ISourceProvider source = null;
            ITileGrid tileGrid = null;
            switch (config.Type.ToLower())
            {
                case "baidu":
                    tileGrid = new TmsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new BaiduMapProvider(tileGrid, config.Url, paras,config.OffsetZoom);
                    break;
                case "wmts" :
                    tileGrid = new WmtsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmtsSourceProvider(tileGrid, url, paras);
                    break;
                case "wms":
                    tileGrid = new TmsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmsSourceProvider(tileGrid, config.Url, paras);
                    break;
                case "arcserverrest":
                    tileGrid = new WmtsTileGrid(config.Resolutions, extent, origin, tileSize);//new TileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new ArcServerRestProvider(tileGrid, config.Url, paras, config.OffsetZoom);
                    break;
            }
            return source;
        }
    }
}
