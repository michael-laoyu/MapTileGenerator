using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 一个较通用的，基于WMTS XYZ的瓦片请求
    /// </summary>
    public class WmtsXyzTileSourceProvider : WmtsSourceProvider
    {

        public WmtsXyzTileSourceProvider(MapConfig config) : 
                base(config)
        {
            this._offsetZoom = config.OffsetZoom;
        }

        public override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            url = url.Replace("{z}", (_offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{y}", tileCoord.Y.ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            return url;
        }
    }
}
