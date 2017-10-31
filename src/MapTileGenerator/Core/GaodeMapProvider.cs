using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 高德地图
    /// 
    /// 高德地图实际是WMTS规则，但是实际计算时，发现总是少一列，用TMS换算后计算就没有问题。因此用TMS规则下载。
    /// 请注意：因此离线瓦片存储是按TMS规则存储
    /// </summary>
    public class GaodeMapProvider : TmsSourceProvider
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
