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
        private ITileOutputStrategy _outputStrategy = null;
        private ITileLoadStrategy _tileLoadStrategy = new HttpTileLoadStrategy();
        private ISourceProvider _source = null;
        private FailTilesOutputStrategy _failsStrategy = new FailTilesOutputStrategy();
        private QueueTaskWorker<TileCoordWrap> _worker = null;
        private MapConfig _mapConfig = null;
        private LimitedQueue<TileCoord> _lastTiles = null;

        public EventHandler<TileCoord> TileLoaded;
        public EventHandler<string> Finished;

        public TileGenerator(MapConfig config)
        {
            _mapConfig = config;
            _source = ProviderFactory.CreateSourceProvider(config);
            _outputStrategy = ProviderFactory.CreateOutputStrategy(config);
            _outputStrategy.Init(config.SavePath);
            _worker = new Core.QueueTaskWorker<TileCoordWrap>(config.RunThreadCount, GetTile, true);
            _totalTile = _source.TileGrid.TotalTile;
            _lastTiles = new Core.LimitedQueue<Core.TileCoord>(config.RunThreadCount);
            _successTileIndex = config.Result.SuccessTiles;
            _currTileIndex = config.Result.LastTileIndex;
        }

        private int _successTileIndex = 0;
        private int _failRetrySuccessIndex = 0;
        private int _failRetryIndex = 0;
        private int _currTileIndex = 0;
        public int SuccessTile
        {
            get
            {
                return _successTileIndex;
            }
        }

        private double _totalTile;
        public double TotalTile
        {
            get
            {
                return _totalTile;
            }
        }

        public int FailTiles
        {
            get
            {
                return _failsStrategy.Count;
            }
        }


        public void Start()
        {
            if (_worker != null)
            {
                _worker.Start();
            }
            //重试失败的瓦片；
            TryDoFails();

            //从最后一次的瓦片开始下载瓦片
            _source.EnumerateTileRange(_mapConfig.Result.LastTile,               
               (tile) =>
               {
                   _worker.TryQueue(new TileCoordWrap
                   {
                       Tile = tile,
                       OnSuccess = (tile1)=> {
                           Interlocked.Increment(ref _successTileIndex);
                           Task.Run(() =>
                           {
                               OnTileLoaded(tile1);
                           });                           
                       },
                       OnFailed = (tile1,ex) =>
                       {
                           _failsStrategy.Insert(tile1);
                           Console.WriteLine(string.Format("Tile : zoom ：  {0} x： {1} y ：{2}，有异常：{3}", tile1.Zoom, tile1.X,
                                                    tile1.Y,ex.Message ));
                       },
                       OnFinally = (tile2) =>
                       {
                           lock (this)
                           {
                               if (_currTileIndex >= TotalTile)
                               {
                                   Task.Run(() =>
                                   {
                                       var msg = string.Format("{瓦片下载完成... 完成情况：{0}/{1}，失败：{2}!", _successTileIndex, TotalTile, FailTiles);
                                       OnFinished(msg);
                                   });
                               }
                           }
                       }
                   });
               });
        }

        public void RetryFails()
        {
            _currTileIndex = 0;
            TryDoFails();
        }

        public void Close()
        {
            if (_lastTiles.Count > 0)
            {
                var min = _lastTiles.Min(tile => tile.Index);
                var minTile = _lastTiles.Where(tile => tile.Index == min).ToList();

                if(minTile!=null && minTile.Count>0)
                {                    
                    _mapConfig.Result.LastTile = minTile[0];
                }                                
            }
            _mapConfig.Result.SuccessTiles = SuccessTile;            
            _mapConfig.Result.FailTiles = FailTiles;
            _mapConfig.Result.LastTileIndex = _mapConfig.Result.LastTile != null ? _mapConfig.Result.LastTile.Index : _currTileIndex;

            _mapConfig.Save();

            if (_worker != null)
            {
                _worker.Close();
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Close();
        }

        #endregion

        protected virtual void OnTileLoaded(TileCoord tileCoord)
        {            
            if (this.TileLoaded != null)
            {
                TileLoaded(this, tileCoord);
            }
        }

        protected virtual void OnFinished(string msg)
        {
            this.Finished?.Invoke(this, msg);
        }

        protected virtual void GetTile(TileCoordWrap tileWrap)
        {
            try
            {                
                string url = _source.GetRequestUrl(tileWrap.Tile);
                Interlocked.Increment(ref _currTileIndex);
                lock (_lastTiles)
                {
                    _lastTiles.Enqueue(tileWrap.Tile);//记录最后一次Tile，下次执行时继续。
                }
                using (Stream stream = _tileLoadStrategy.GetTile(url,_mapConfig.Timeout))
                {
                    _outputStrategy.Write(stream, _source.GetOutputTile(tileWrap.Tile, _mapConfig.OffsetZoom));
                    tileWrap.OnSuccess.Invoke(tileWrap.Tile);                    
                }
            }
            catch (Exception ex)
            {
                tileWrap.OnFailed.Invoke(tileWrap.Tile, ex);
            }
            finally
            {
                tileWrap.OnFinally.Invoke(tileWrap.Tile);
            }
        }

        private void TryDoFails()
        {
            string failsDb = Path.Combine(_mapConfig.SavePath, FailTilesOutputStrategy.FILENAME);
            var failTiles = _failsStrategy.Load(failsDb);
            if (failTiles != null)
            {
                foreach (TileCoord failTile in failTiles)
                {
                    _worker.TryQueue(new TileCoordWrap
                    {
                        Tile = failTile,
                        OnSuccess = (tile) =>
                        {
                            _failsStrategy.Delete(tile);
                            Interlocked.Increment(ref _failRetrySuccessIndex);
                            Task.Run(() =>
                            {
                                OnTileLoaded(tile);
                            });
                        },
                        OnFailed = (tile1,ex)=> {
                            Console.WriteLine(string.Format("Tile : zoom ：  {0} x： {1} y ：{2}，有异常：{3}", tile1.Zoom, tile1.X,
                                                   tile1.Y, ex.Message));
                        },////重试错误瓦片再次发生错误，不用记录；
                        OnFinally = (tile2) =>
                        {
                            lock (this)
                            {
                                Interlocked.Increment(ref _failRetrySuccessIndex);
                                Interlocked.Increment(ref _currTileIndex);
                                if (_failRetryIndex >= FailTiles)
                                {
                                    var msg = string.Format("失败瓦片重试完成...完成情况：{0}/{1}，失败：{2}!", _failRetrySuccessIndex, FailTiles, FailTiles- _failRetryIndex);
                                    OnFinished(msg);
                                }
                            }
                        }
                    });
                }
            }
        }
    }

    public class TileCoordWrap
    {
        public TileCoord Tile;
        public Action<TileCoord> OnSuccess = null;
        public Action<TileCoord,Exception> OnFailed = null;
        public Action<TileCoord> OnFinally = null;
    }
}
