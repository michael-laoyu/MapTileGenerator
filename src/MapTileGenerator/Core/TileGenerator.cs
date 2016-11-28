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
        private int _offsetZoom = 0;
        private bool _isStop = false;
        private ITilePathBuilder _tilePathBuilder = null;
        private ISourceProvider _source = null;
        private QueueTaskWorker<TileCoord> _worker = null;

        public EventHandler<TileCoord> TileLoaded;

        public TileGenerator(MapConfig config)
        {
            _source = SourceProviderFactory.CreateSourceProvider(config);
            _threadCount = config.RunThreadCount;
            _offsetZoom = config.OffsetZoom;
            _tilePathBuilder = _source.GetTilePathBuilder(config.SavePath);
            _worker = new Core.QueueTaskWorker<Core.TileCoord>(config.RunThreadCount, ToDo, true);
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
            if(_worker !=null)
            {
                _worker.Start();
            }
            _source.EnumerateTileRange(
                (zoom) =>
                {
                    _tilePathBuilder.BuildZoomFold(zoom, _offsetZoom);
                },
               (tile) =>
               {
                   _worker.TryQueue(tile);
               });
        }

        private void ToDo(TileCoord tileCoord)
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

        public void Close()
        {
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

       
    }
}
