using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class WmtsTileGrid : TmsTileGrid
    {
        public WmtsTileGrid(double[] resolutions, Extent extent, Coordinate origin, Size tileSize)
            : base(resolutions,extent,origin,tileSize)
        {

        }

        protected override List<Extent> CalculateTileRanges()
        {
            List<Extent> result = new List<Extent>();
            for (int i = 0, len = _resolutions.Length; i < len; i++)
            {
                var topLeft = GetTileCoordByXYAndZoom(i, this._extent.GetTopLeft(), false);
                var rightBottom = GetTileCoordByXYAndZoom(i, this._extent.GetRightBottom(), true);
                var extent = new Extent(topLeft.X, (-1 * topLeft.Y -1), rightBottom.X, (-1 * rightBottom.Y-1));
                result.Add(extent);
                _totalTile += (extent.MaxX - extent.MinX + 1) * (extent.MaxY - extent.MinY + 1);
            }
            return result;
        }
    }
}
