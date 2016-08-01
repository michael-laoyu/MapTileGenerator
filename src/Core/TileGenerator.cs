using System;
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
        private TileWmsSourceProvider _source = null;
        private int _threadCount = 1;
        private string _savePath;
        private int _offsetZoom = 0;
        private bool _isStop = false;
        private List<WorThread> _workThreads = null;
        private List<ManualResetEvent> _waitOnes = null;
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
        }

        protected virtual void OnTileLoaded(TileCoord tileCoord)
        {
            if (this.TileLoaded != null)
            {
                TileLoaded(this, tileCoord);
            }
        }

        private Coordinate GetTileCoord()
        {
            List<Extent> fullTileRange = _source.TileGrid.TileRanges;
            int zIndex = 0;
            double xIndex = fullTileRange[zIndex].MinX;
            double yIndex = fullTileRange[zIndex].MinY;
            return null;
        }

        public void Start()
        {
            if (!_isStop)
            {
                _workThreads = new List<WorThread>(_threadCount);
                _waitOnes = new List<ManualResetEvent>(_threadCount);
                for (int i = 0; i < _threadCount; i++)
                {
                    ManualResetEvent waitOne = new ManualResetEvent(false);
                    WorThread workThread = new WorThread(waitOne, (tileCoord )=>
                    {
                        using (Stream stream = _source.GetTile(tileCoord))
                        {
                             string filePath ;
                            string z= (tileCoord.Zoom + _offsetZoom).ToString(),
                                x=tileCoord.X.ToString(),
                                y=tileCoord.Y.ToString();

                            filePath = Path.Combine(_savePath, z);
                            if(tileCoord.X <0)
                            {
                                x= "M" + Math.Abs(tileCoord.X).ToString();
                            }
                            if(tileCoord.Y<0)
                            {
                                y= "M" + Math.Abs(tileCoord.Y).ToString();
                            }
                            filePath = Path.Combine(filePath, x + "_" + y + ".png");
                            using(FileStream fs = new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.ReadWrite))
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
                    });
                    _workThreads.Add(workThread);
                    _waitOnes.Add(waitOne);
                    workThread.Start();
                }

                List<Extent> fullTileRange = _source.TileGrid.TileRanges;
                TileCoord tile;
                int workThreadIndex = 0;
                for (int z = 0; z < fullTileRange.Count; z++)
                {
                    string filePath = Path.Combine(_savePath, (z+_offsetZoom).ToString());
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    for (double x = fullTileRange[z].MinX; x <= fullTileRange[z].MaxX; ++x)
                    {
                        for (double y = fullTileRange[z].MinY; y <= fullTileRange[z].MaxY; ++y)
                        {
                            tile = new TileCoord(z, x, y);
                            workThreadIndex = workThreadIndex % _threadCount;
                            _workThreads[workThreadIndex].Enqueue(tile);
                            ++workThreadIndex;
                        }
                    }
                }
            }
        }

        public void Close()
        {
            try
            {
                if (_workThreads != null && _workThreads.Count > 0)
                {
                    foreach (WorThread item in _workThreads)
                    {
                        item.Dispose();
                    }
                    _workThreads.Clear();
                    _workThreads = null;
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

        public class WorThread : IDisposable
        {
            private Thread _thread = null;
            private bool _isStop = false;
            private ManualResetEvent _waitOne = null;
            private Queue<TileCoord> _tileQueue = new Queue<TileCoord>();
            private Action<TileCoord> _callback = null;
            public WorThread(ManualResetEvent waitOne,Action<TileCoord> callback)
            {
                _waitOne = waitOne;
                _callback = callback;
                _thread = new Thread(() =>
                {
                    while (!_isStop)
                    {
                        TileCoord tileCoor = Dequeue();
                        if (tileCoor != null)
                        {
                            _callback(tileCoor);
                        }
                        else
                        {
                            _waitOne.Reset();
                            _waitOne.WaitOne();
                        }
                    }
                });
                _thread.IsBackground = true;
            }

            protected virtual TileCoord Dequeue()
            {
                lock (_tileQueue)
                {
                    if (_tileQueue.Count > 0)
                    {
                        return _tileQueue.Dequeue();
                    }
                }
                return null;
            }

            public void Enqueue(TileCoord tileCoord)
            {
                lock (_tileQueue)
                {
                    _tileQueue.Enqueue(tileCoord);
                    _waitOne.Set();
                }
            }

            public void Start()
            {
                try
                {
                    _thread.Start();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            #region IDisposable 成员

            public void Dispose()
            {
                try
                {
                    _isStop = true;
                    if (_thread != null)
                    {
                        _thread.Join(10);
                        //_thread.Abort();
                        _thread = null;
                    }
                    _waitOne.Close();
                    GC.SuppressFinalize(this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            #endregion
        }
    }
}
