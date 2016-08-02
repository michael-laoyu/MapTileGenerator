using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class TileGenerator : IDisposable
    {
        private int _threadCount = 1;
        private string _savePath;
        private int _offsetZoom = 0;
        private bool _isStop = false;
        private ITilePathBuilder _tilePathBuilder = null;
        private ISourceProvider _source = null;
        private BlockingCollection<TileCoord> _blockingQueue = null;
        private CancellationTokenSource _tokenSource = null;

        public EventHandler<TileCoord> TileLoaded;

        public TileGenerator(MapConfig config)
        {
            if (config.TileSize == null)
            {
                config.TileSize = new int[] { 256, 256 };
            }
            var tileSize = new Size(config.TileSize);
            var extent = new Extent(config.Extent);
            Coordinate origin;
            if (config.Origin == null)
            {
                origin = extent.GetTopLeft();
            }
            else
            {
                origin = new Coordinate(config.Origin);
            }
            if (string.IsNullOrEmpty(config.SavePath))
            {
                config.SavePath = Path.Combine(Environment.CurrentDirectory, "Tiles");
            }

            var tileGrid = new TileGrid(config.Resolutions, extent, origin, tileSize);
            var paras = new Dictionary<string,object>();
            paras.Add("FORMAT", config.Paras.Format);
            paras.Add("VERSION", config.Paras.Version);
            paras.Add("STYLES", config.Paras.Style);
            paras.Add("LAYERS", config.Paras.Layers);
            paras.Add("REQUEST", config.Paras.Request);
            paras.Add("SRS", config.Paras.Srs);

            _source = new TileWmsSourceProvider(tileGrid, config.Url, paras);
            _threadCount = config.RunThreadCount;
            _savePath = config.SavePath;
            _offsetZoom = config.OffsetZoom;
            _tilePathBuilder = new DefaultTilePathBuilder(_savePath);
        }

        public ISourceProvider Source
        {
            get
            {
                return _source;
            }
        }

        protected virtual void OnTileLoaded(TileCoord tileCoord)
        {
            if (this.TileLoaded != null)
            {
                TileLoaded(this, tileCoord);
            }
        }

        public void Start()
        {
            if (!_isStop)
            {
                _blockingQueue = new BlockingCollection<TileCoord>(_threadCount);
                _tokenSource = new CancellationTokenSource();
                var token = _tokenSource.Token;
                var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning);

                for (int i = 0; i < _threadCount; i++)
                {
                    Task workThread = taskFactory.StartNew(() =>
                    {
                        while (!_isStop)
                        {
                            token.ThrowIfCancellationRequested();

                            TileCoord tileCoord = null;
                            if(_blockingQueue.TryTake(out tileCoord, 100))
                            //tileCoord = _blockingQueue.Take();
                            //if (tileCoord != null)
                            {
                                using (Stream stream = _source.GetTile(tileCoord))
                                {
                                    string filePath = _tilePathBuilder.BuildTilePath(tileCoord);
                                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                    {
                                        int ret = 0;
                                        byte[] buffer = new byte[8192];//8K
                                        ret = stream.Read(buffer, 0, buffer.Length);
                                        while (ret > 0)
                                        {
                                            fs.Write(buffer, 0, ret);
                                            ret = stream.Read(buffer, 0, buffer.Length);
                                        }
                                        OnTileLoaded(tileCoord);
                                    }
                                }
                            }
                            //else
                            //{
                            //    token.WaitHandle.WaitOne(100);
                            //}
                        }
                    }, token);
                }

                List<Extent> fullTileRange = _source.TileGrid.TileRanges;
                for (int z = 0; z < fullTileRange.Count; z++)
                {
                   _tilePathBuilder.BuildZoomFold(z, _offsetZoom);

                    for (double x = fullTileRange[z].MinX; x <= fullTileRange[z].MaxX; ++x)
                    {
                        for (double y = fullTileRange[z].MinY; y <= fullTileRange[z].MaxY; ++y)
                        {
                            var tile = new TileCoord(z, x, y);
                            //_blockingQueue.TryAdd(tile, 500);
                            _blockingQueue.Add(tile);
                        }
                    }
                }
            }
        }

        public void Close()
        {
            try
            {
                _isStop = true;
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                }
                if (_blockingQueue != null)
                {
                    _blockingQueue.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Close();
        }

        #endregion

       
    }
}
