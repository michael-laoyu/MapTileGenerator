using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// geoserver wms或其他标准的WMS服务
    /// </summary>
    public class WmsSourceProvider : TmsSourceProvider
    {

        public WmsSourceProvider(MapConfig config)
            : base(config)
        {
          
        }


        public override string GetRequestUrl(TileCoord tileCoord)
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
            //Console.WriteLine(string.Format("x:{0},y:{1},zoom:{2},bbox:{3},url:{4}",tileCoord.X,tileCoord.Y,
            //    tileCoord.Zoom,tileExtent,url));
            return url;
        }
    }
}
