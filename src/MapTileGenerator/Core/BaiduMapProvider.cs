using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class BaiduMapProvider : WmsSourceProvider
    {
        private int offsetZoom;
        public BaiduMapProvider(TmsTileGrid tileGrid, string url, Dictionary<string, object> paras,int offsetZoom) : base(tileGrid, url, paras)
        {
            this.offsetZoom = offsetZoom;
        }

        protected override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            string x = tileCoord.X.ToString();
            string y = tileCoord.Y.ToString();
            if(tileCoord.X<0)
            {
                x = "M" + Math.Abs(tileCoord.X);
            }
            if(tileCoord.Y<0)
            {
                y = "M" + Math.Abs(tileCoord.Y);
            }
            url += "&x=" + tileCoord.X.ToString();
            url += "&y=" + tileCoord.Y.ToString();
            url += "&z=" + (offsetZoom + tileCoord.Zoom).ToString();
            //url = string.Format(url, tileCoord.Zoom, Math.Abs(tileCoord.Y), Math.Abs(tileCoord.X));
            return url;
        }

        public override ITilePathBuilder GetTilePathBuilder(string rootPath)
        {
            return new BaiduTilePathBuilder(rootPath);
        }
    }
}
