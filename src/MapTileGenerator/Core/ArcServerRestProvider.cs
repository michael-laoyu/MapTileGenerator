using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class ArcServerRestProvider : WmsSourceProvider
    {
        public ArcServerRestProvider(TmsTileGrid tileGrid, string url, Dictionary<string, object> paras) : 
                base(tileGrid, url, paras)
        {

        }

        protected override string GetRequestUrl(TileCoord tileCoord)
        {
            string url = this._url;
            url = string.Format(url, tileCoord.Zoom, Math.Abs(tileCoord.Y), Math.Abs(tileCoord.X));
            return url;
        }

        public override ITilePathBuilder GetTilePathBuilder(string rootPath)
        {
            return new ArcServerRestTilePathBuilder(rootPath);
        }

        
    }
}
