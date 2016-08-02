using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class TileWmsSourceProvider : MapTileGenerator.Core.ISourceProvider
    {
        private TileGrid _tileGrid = null;
        private string _url = null;
        private Dictionary<string, object> _paras = new Dictionary<string, object>();
        private ITileLoadStrategy _tileLoad = new HttpTileLoadStrategy();

        public TileWmsSourceProvider(TileGrid tileGrid, string url, Dictionary<string,object> paras)
        {
            _tileGrid = tileGrid;
            _url = url;
            if (paras != null && paras.Count > 0)
            {
                paras.ToList().ForEach(keyValue =>
                {
                    _paras.Add(keyValue.Key,keyValue.Value);
                });
            }
        }

        public TileGrid TileGrid
        {
            get
            {
                return _tileGrid;
            }
        }

        public Stream GetTile(TileCoord tileCoord)
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
    }
}
