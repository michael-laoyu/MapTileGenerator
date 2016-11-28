using System;
using System.IO;
namespace MapTileGenerator.Core
{
    public interface ISourceProvider
    {
       Stream GetTile(TileCoord tileCoord);
       void EnumerateTileRange(Action<int> getZoomCallback, Action<TileCoord> getTileCallback);
        TmsTileGrid TileGrid { get; }

        ITilePathBuilder GetTilePathBuilder(string rootPath);
    }
}
