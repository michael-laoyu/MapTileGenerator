using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public  class BaiduTilePathBuilder : DefaultTilePathBuilder
    {
        public BaiduTilePathBuilder(string rootPath)
            :base(rootPath)
        {
            
        }

        #region ITilePathBuilder 成员

        public override string BuildTilePath(TileCoord tileCoord)
        {
            string x = tileCoord.X.ToString(), 
                        y=tileCoord.Y.ToString();
            if (tileCoord.X < 0)
            {
                x = "M" + Math.Abs(tileCoord.X).ToString();
            }
            if (tileCoord.Y < 0)
            {
                y = "M" + Math.Abs(tileCoord.Y).ToString();
            }
            string zoomFold = _zoomFolds[tileCoord.Zoom];
            string filePath = Path.Combine(zoomFold, x);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            return Path.Combine(filePath,  y + ".png");             
        }

        #endregion
    }
}
