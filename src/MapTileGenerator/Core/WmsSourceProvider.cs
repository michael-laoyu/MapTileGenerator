using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class WmsSourceProvider : ISourceProvider
    {
        protected ITileGrid _tileGrid = null;
        protected string _url = null;
        protected Dictionary<string, object> _paras = null;

        public WmsSourceProvider(ITileGrid tileGrid, string url, Dictionary<string,object> paras)
        {
            _tileGrid = tileGrid;
            _url = url;
            _paras = paras;
        }

        public ITileGrid TileGrid
        {
            get
            {
                return _tileGrid;
            }
        }

        public virtual string GetRequestUrl(TileCoord tileCoord)
        {
            Extent tileExtent = _tileGrid.GetTileExtent(tileCoord);
            string url = this._url;
            if (!url.EndsWith("?"))
            {
                url += "?";
            }
            var paras = new Dictionary<string, object>();
            _paras.ToList().ForEach(keyValue =>
            {
                paras.Add(keyValue.Key, keyValue.Value);
            });

            paras["TRANSPARENT"] = true;
            paras.Add("WIDTH", this._tileGrid.TileSize.Width);
            paras.Add("HEIGHT", this._tileGrid.TileSize.Height);
            paras.Add("BBOX", tileExtent);
            foreach (KeyValuePair<string, object> item in paras)
            {
                url += (item.Key + "=" + item.Value.ToString() + "&");
            }
            return url;
        }

        public virtual void EnumerateTileRange(TileCoord lastTile/*为了实现续载功能，从上次失败的点开始继续*/,
                                    Action<TileCoord> getTileCallback)
        {
            int minZoom = 0;
            if (lastTile != null)
            {
                minZoom = lastTile.Zoom;//从失败的那一级别开始下载。
            }
            List<Extent> fullTileRange = _tileGrid.TileRanges;
            for (int z = minZoom; z < fullTileRange.Count; z++)
            {
                for (double x = fullTileRange[z].MinX; x <= fullTileRange[z].MaxX; ++x)
                {
                    for (double y = fullTileRange[z].MinY; y <= fullTileRange[z].MaxY; ++y)
                    {
                        var tile = new TileCoord(z, x, y);
                        getTileCallback(tile);
                    }
                }
            }
        }

        public virtual OutputTile GetOutputTile(TileCoord input, int zoomOffset)
        {
            double x = input.X,
                        y = input.Y;
            if (input.X < 0)
            {
                x = Math.Abs(input.X);
            }
            if (input.Y < 0)
            {
                y = Math.Abs(input.Y);
            }
            return new OutputTile((input.Zoom + zoomOffset).ToString(), x.ToString(), y.ToString());
        }
    }
}
