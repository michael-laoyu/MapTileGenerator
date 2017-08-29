using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// geoserver wmts或其他标准的WMTS（例如：天地图）
    /// </summary>
    public class WmtsSourceProvider : TmsSourceProvider
    {
        public WmtsSourceProvider(MapConfig config)
            :base(config)
        {

        }

        public override string GetRequestUrl(TileCoord tileCoord)
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

        protected override ITileGrid CreateTileGrid(double[] resolutions, Extent extent, Coordinate origin, Size tileSize)
        {
            return new WmtsTileGrid(resolutions, extent, origin, tileSize);
        }

    }
}
