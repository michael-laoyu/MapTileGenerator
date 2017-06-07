using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class TencentMapProvider : WmsSourceProvider
    {
        public TencentMapProvider(MapConfig config) : base(config)
        {
            offsetZoom = config.OffsetZoom;
        }

        private int offsetZoom;

        public override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            url = url.Replace("{z}", (offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            url = url.Replace("{y}", tileCoord.Y.ToString());
            return url;
        }

    }
}
