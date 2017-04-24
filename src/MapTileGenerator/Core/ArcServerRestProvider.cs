using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class ArcServerRestProvider : WmsSourceProvider
    {
        private int offsetZoom;

        public ArcServerRestProvider(ITileGrid tileGrid, string url, Dictionary<string, object> paras,int offsetZoom) : 
                base(tileGrid, url, paras)
        {
            this.offsetZoom = offsetZoom;
        }

        protected override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            url = url.Replace("{z}", (offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{y}", tileCoord.Y.ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            return url;
        }

        public override ITilePathBuilder GetTilePathBuilder(string rootPath)
        {
            return new ArcServerRestTilePathBuilder(rootPath);
        }

        
    }
}
