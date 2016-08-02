using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public  class DefaultTilePathBuilder : ITilePathBuilder
    {
        private string _rootPath;
        private IDictionary<int, string> _zoomFolds = new Dictionary<int, string>();
        public DefaultTilePathBuilder(string rootPath)
        {
            _rootPath = rootPath;
        }

        #region ITilePathBuilder 成员

        public string RootPath
        {
            get
            {
                return _rootPath;
            }
        }

        public string BuildZoomFold(int zoom , int offsetZoom)
        {
            string fold = Path.Combine(RootPath, (zoom + offsetZoom).ToString());
            if (!Directory.Exists(fold))
            {
                Directory.CreateDirectory(fold);
            }
            _zoomFolds.Add(zoom, fold);
            return fold;
        }

        public string BuildTilePath(TileCoord tileCoord)
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
            return Path.Combine(zoomFold, x + "_" + y + ".png");             
        }



        #endregion
    }
}
