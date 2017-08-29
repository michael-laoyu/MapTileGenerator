using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 腾讯地图
    /// </summary>
    public class TencentMapProvider : TmsSourceProvider
    {
        public TencentMapProvider(MapConfig config) : base(config)
        {
        }

    }
}
