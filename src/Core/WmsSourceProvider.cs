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
        protected TileGrid _tileGrid = null;
        protected string _url = null;
        protected Dictionary<string, object> _paras = null;
        protected ITileLoadStrategy _tileLoad = new HttpTileLoadStrategy();

        public WmsSourceProvider(TileGrid tileGrid, string url, Dictionary<string,object> paras)
        {
            _tileGrid = tileGrid;
            _url = url;
            _paras = paras;
        }

        public TileGrid TileGrid
        {
            get
            {
                return _tileGrid;
            }
        }

        public virtual Stream GetTile(TileCoord tileCoord)
        {
            string url = GetRequestUrl(tileCoord);
            return _tileLoad.GetTile(url);
        }

        protected virtual string GetRequestUrl(TileCoord tileCoord)
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

        public virtual void EnumerateTileRange(Action<int> getZoomCallback, Action<TileCoord> getTileCallback)
        {
            List<Extent> fullTileRange = _tileGrid.TileRanges;
            for (int z = 0; z < fullTileRange.Count; z++)
            {
                getZoomCallback(z);
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
    }
}
