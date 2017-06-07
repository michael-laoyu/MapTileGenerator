using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class GaodeMapProvider : WmsSourceProvider
    {
        private int offsetZoom;

        public GaodeMapProvider(MapConfig config) : base(config)
        {
            offsetZoom = config.OffsetZoom;
        }

        public override string GetRequestUrl(TileCoord tileCoord)
        {
            //http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + x + "&y=" + y + "&z=" + z
            string url = this._url;          
            url = url.Replace("{z}", (offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            url = url.Replace("{y}", (-1 * tileCoord.Y - 1).ToString()); 
            return url;
        }
    }
}
