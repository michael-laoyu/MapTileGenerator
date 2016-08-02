using System;
using System.IO;
namespace MapTileGenerator.Core
{
    public interface ISourceProvider
    {
       Stream GetTile(TileCoord tileCoord);
        TileGrid TileGrid { get; }
    }
}
