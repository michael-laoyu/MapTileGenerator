using System.Collections.Generic;

namespace MapTileGenerator.Core
{
    public interface ITileGrid
    {
        List<Extent> TileRanges { get; }
        Size TileSize { get; }
        double TotalTile { get; }

        Extent GetTileExtent(TileCoord tileCoord);
    }
}