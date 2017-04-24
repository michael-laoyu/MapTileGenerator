using System;
using System.IO;
namespace MapTileGenerator.Core
{
    public interface ISourceProvider
    {
       Stream GetTile(TileCoord tileCoord);
       void EnumerateTileRange(TileCoord lastTile, Action<int> getZoomCallback, Action<TileCoord> getTileCallback);
        ITileGrid TileGrid { get; }

        ITilePathBuilder GetTilePathBuilder(string rootPath);
    }
}
