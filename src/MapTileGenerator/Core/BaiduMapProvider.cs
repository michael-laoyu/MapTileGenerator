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
        public BaiduMapProvider(ITileGrid tileGrid, string url, Dictionary<string, object> paras,int offsetZoom) : base(tileGrid, url, paras)
        {
            this.offsetZoom = offsetZoom;
        }

        public override string GetRequestUrl(TileCoord tileCoord)
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
            url = url.Replace("{z}", (offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{x}", (offsetZoom + tileCoord.X).ToString());
            url = url.Replace("{y}", (offsetZoom + tileCoord.Y).ToString());
            //url += "&x=" + tileCoord.X.ToString();
            //url += "&y=" + tileCoord.Y.ToString();
            //url += "&z=" + (offsetZoom + tileCoord.Zoom).ToString();
            //url = string.Format(url, tileCoord.Zoom, Math.Abs(tileCoord.Y), Math.Abs(tileCoord.X));
            return url;
        }

        public override OutputTile GetOutputTile(TileCoord input, int zoomOffset)
        {
            string x = input.X.ToString(),
                       y = input.Y.ToString();
            if (input.X < 0)
            {
                x = "M" + Math.Abs(input.X).ToString();
            }
            if (input.Y < 0)
            {
                y = "M" + Math.Abs(input.Y).ToString();
            }
            return new OutputTile((input.Zoom + zoomOffset).ToString(), x.ToString(), y.ToString());
        }
    }
}
