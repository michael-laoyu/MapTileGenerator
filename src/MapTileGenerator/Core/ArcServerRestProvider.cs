using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class ArcServerRestProvider : WmtsSourceProvider
    {
        private int offsetZoom;

        public ArcServerRestProvider(MapConfig config) : 
                base(config)
        {
            this.offsetZoom = config.OffsetZoom;
        }

        public override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            url = url.Replace("{z}", (offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{y}", tileCoord.Y.ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            return url;
        }     
    }
}
