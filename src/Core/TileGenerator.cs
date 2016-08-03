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
            _source = SourceProviderFactory.CreateSourceProvider(config);
            _threadCount = config.RunThreadCount;
            if (string.IsNullOrEmpty(config.SavePath))
            {
                config.SavePath = Path.Combine(Environment.CurrentDirectory, "Tiles");
            }
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
                            if (_blockingQueue.TryTake(out tileCoord, 100))
                            //tileCoord = _blockingQueue.Take();
                            //if (tileCoord != null)
                            {
                                try
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
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            //else
                            //{
                            //    token.WaitHandle.WaitOne(100);
                            //}
                        }
                    }, token);
                }

                _source.EnumerateTileRange(
                    (zoom) =>
                    {
                        _tilePathBuilder.BuildZoomFold(zoom, _offsetZoom);
                    },
                   (tile) =>
                   {
                       _blockingQueue.Add(tile);
                   });
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
