using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class WmtsTileGrid : TileGrid
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
                var extent = new Extent(topLeft.X, topLeft.Y, rightBottom.X, (-1 * rightBottom.Y-2));
                result.Add(extent);
                _tileTotal += (extent.MaxX - extent.MinX + 1) * (extent.MaxY - extent.MinY + 1);
            }
            return result;
        }

        //protected override Coordinate GetTileCoordByXYAndZoom(int zoom, Coordinate coord, bool reverseIntersectionPolicy)
        //{
        //    var adjustX = reverseIntersectionPolicy ? 0.5 : 0.0;
        //    var adjustY = reverseIntersectionPolicy ? 0.0 : 0.5;
        //    var resolution = this._resolutions[zoom];
        //    var xFromOrigin = Math.Floor((coord.X - this._origin.X) / resolution + adjustX);
        //    var yFromOrigin = Math.Floor((coord.Y - this._origin.Y) / resolution + adjustY);
        //    var tileCoordX = xFromOrigin / this._tileSize.Width;
        //    var tileCoordY = yFromOrigin / this._tileSize.Height;
        //    if (reverseIntersectionPolicy)
        //    {
        //        tileCoordX = Math.Ceiling(tileCoordX) - 1;
        //        tileCoordY = Math.Ceiling(tileCoordY) - 1;
        //    }
        //    else
        //    {
        //        tileCoordX = Math.Floor(tileCoordX);
        //        tileCoordY = Math.Floor(tileCoordY);
        //    }
        //    return new Coordinate(tileCoordX, tileCoordY);
        //}

    }
}
