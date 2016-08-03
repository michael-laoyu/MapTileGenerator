using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class WmtsSourceProvider : WmsSourceProvider
    {
        public WmtsSourceProvider(TileGrid tileGrid, string url, Dictionary<string, object> paras)
            :base(tileGrid,url,paras)
        {

        }

        protected override string GetRequestUrl(TileCoord tileCoord)
        {
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
            paras.Add("TileMatrix",tileCoord.Zoom+1);
            paras.Add("TileRow", tileCoord.Y);
            paras.Add("TileCol", tileCoord.X);
            foreach (KeyValuePair<string, object> item in paras)
            {
                url += (item.Key + "=" + item.Value.ToString() + "&");
            }
            return url;
        }
    }
}
