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
            if (config.TileSize == null)
            {
                config.TileSize = new int[] { 256, 256 };
            }
            var tileSize = new Size(config.TileSize);
            var extent = new Extent(config.Extent);
            Coordinate origin;
            if (config.Origin == null)
            {
                origin = extent.GetTopLeft();
            }
            else
            {
                origin = new Coordinate(config.Origin);
            }
            if (string.IsNullOrEmpty(config.SavePath))
            {
                config.SavePath = Path.Combine(Environment.CurrentDirectory, "Tiles");
            }
            string url = config.Url;
            //var paras = new Dictionary<string,object>();
            //paras.Add("FORMAT", config.UrlParas.Format);
            //paras.Add("VERSION", config.UrlParas.Version);
            //paras.Add("STYLES", config.UrlParas.Style);
            //paras.Add("LAYERS", config.UrlParas.Layers);
            //paras.Add("REQUEST", config.UrlParas.Request);
            //paras.Add("SRS", config.UrlParas.Srs);
            var paras = config.UrlParas;
            ISourceProvider source = null;
            switch (config.Type.ToUpper())
            {
                case "WMTS" :
                    var wmtsTileGrid = new WmtsTileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmtsSourceProvider(wmtsTileGrid, url, paras);
                    break;
                case "WMS":
                    var tileGrid = new TileGrid(config.Resolutions, extent, origin, tileSize);
                    source = new WmsSourceProvider(tileGrid, config.Url, paras);
                    break;
            }
            return source;
        }
    }
}
