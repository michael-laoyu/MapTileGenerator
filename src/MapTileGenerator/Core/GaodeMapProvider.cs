using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 高德地图
    /// </summary>
    public class GaodeMapProvider : WmsSourceProvider
    {

        public GaodeMapProvider(MapConfig config) : base(config)
        {

        }

        public override string GetRequestUrl(TileCoord tileCoord)
        {
            //http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + x + "&y=" + y + "&z=" + z
            string url = this._url;          
            url = url.Replace("{z}", (_offsetZoom + tileCoord.Zoom).ToString());
            url = url.Replace("{x}", tileCoord.X.ToString());
            url = url.Replace("{y}", (-1 * tileCoord.Y - 1).ToString()); 
            return url;
        }
    }
}
