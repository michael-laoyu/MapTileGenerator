using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    interface ITilePathBuilder
    {
        string RootPath
        {
            get;
        }
        string BuildZoomFold(int zoom, int offsetZoom);
        string BuildTilePath(TileCoord tileCoord);

    }
}
