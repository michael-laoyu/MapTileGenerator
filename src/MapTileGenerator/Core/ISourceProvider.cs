using System;
using System.IO;
namespace MapTileGenerator.Core
{
    public interface ISourceProvider
    {
        string GetRequestUrl(TileCoord tileCoord);
       void EnumerateTileRange(TileCoord beginTile, Action<TileCoord> getTileCallback);
        ITileGrid TileGrid { get; }
        OutputTile GetOutputTile(TileCoord input, int zoomOffset);
    }
}
