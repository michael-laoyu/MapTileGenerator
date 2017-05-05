using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class WmsSourceProvider : ISourceProvider
    {
        protected ITileGrid _tileGrid = null;
        protected string _url = null;
        protected Dictionary<string, object> _paras = null;

        public WmsSourceProvider(MapConfig config)
        {
            var tileSize = new Size(config.TileSize);
            var extent = new Extent(config.Extent);
            Coordinate origin = origin = new Coordinate(config.Origin);

            _tileGrid = CreateTileGrid(config.Resolutions, extent, origin, tileSize);
            _url = config.Url;
            _paras = config.UrlParas;
        }

        public ITileGrid TileGrid
        {
            get
            {
                return _tileGrid;
            }
        }

        public virtual string GetRequestUrl(TileCoord tileCoord)
        {
            Extent tileExtent = _tileGrid.GetTileExtent(tileCoord);
            string url = this._url;
            if (!url.EndsWith("?"))
            {
                url += "?";
            }
            var paras = new Dictionary<string, object>();
            _paras.ToList().ForEach(keyValue =>
            {
                paras.Add(keyValue.Key, keyValue.Value);
            });

            paras["TRANSPARENT"] = true;
            paras.Add("WIDTH", this._tileGrid.TileSize.Width);
            paras.Add("HEIGHT", this._tileGrid.TileSize.Height);
            paras.Add("BBOX", tileExtent);
            foreach (KeyValuePair<string, object> item in paras)
            {
                url += (item.Key + "=" + item.Value.ToString() + "&");
            }
            return url;
        }

        public virtual void EnumerateTileRange(TileCoord beginTile/*为了实现续载功能，从上次失败的点开始继续*/,
                                    Action<TileCoord> getTileCallback)
        {
            List<Extent> fullTileRange = _tileGrid.TileRanges;

            int minZoom = 0, index = 0;
            double minX = fullTileRange[minZoom].MinX,minY = fullTileRange[minZoom].MinY;
            if (beginTile != null)
            {
                minZoom = beginTile.Zoom;//从失败的那一级别开始下载。
                minX = beginTile.X;
                minY = beginTile.Y;
            }

            
            for (double x = minX; x <= fullTileRange[minZoom].MaxX; ++x)           
            {
                for (double y = minY; y <= fullTileRange[minZoom].MaxY; ++y)               
                {
                    ++index;
                    var tile = new TileCoord(minZoom, x, y,index);
                    getTileCallback(tile);
                }
            }

            for (int z = minZoom+1; z < fullTileRange.Count; z++)
            {
                //for (double x = minX; x <= fullTileRange[z].MaxX; ++x)
                for (double x = fullTileRange[z].MinX; x <= fullTileRange[z].MaxX; ++x)
                {
                    //for (double y = minY; y <= fullTileRange[z].MaxY; ++y)
                    for (double y = fullTileRange[z].MinY; y <= fullTileRange[z].MaxY; ++y)
                    {
                        ++index;
                        var tile = new TileCoord(z, x, y,index);
                        getTileCallback(tile);
                    }
                }
            }
        }

        public virtual OutputTile GetOutputTile(TileCoord input, int zoomOffset)
        {
            double x = input.X,
                        y = input.Y;
            if (input.X < 0)
            {
                x = Math.Abs(input.X);
            }
            if (input.Y < 0)
            {
                y = Math.Abs(input.Y);
            }
            return new OutputTile((input.Zoom + zoomOffset).ToString(), x.ToString(), y.ToString());
        }

        protected virtual ITileGrid CreateTileGrid(double[] resolutions, Extent extent, Coordinate origin, Size tileSize)
        {
            return new TmsTileGrid(resolutions, extent, origin, tileSize);
        }
    }
}
