using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// ArcServer Rest瓦片请求
    /// </summary>
    public class ArcServerRestProvider : WmtsXyzTileSourceProvider
    {
        public ArcServerRestProvider(MapConfig config) : 
                base(config)
        {
        }       
    }
}
