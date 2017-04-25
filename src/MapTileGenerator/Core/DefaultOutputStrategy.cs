using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 默认的瓦片输出方式：以文件方式存储；
    /// </summary>
    public class DefaultOutputStrategy : ITileOutputStrategy
    {
        protected string _rootPath;
        protected IDictionary<string, string> _zoomFolderDic = new Dictionary<string, string>();
        protected IDictionary<string,string> _zoomAndXFolderDic = new Dictionary<string, string>();

        public DefaultOutputStrategy()
        {
            
        }

        #region ITileOutputStrategy 成员

        public void Init(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void Write(Stream input, OutputTile outputTile)
        {
            string filePath = BuildTilePath(outputTile);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] buffer = new byte[8192];//8K
                int ret = input.Read(buffer, 0, buffer.Length);
                while (ret > 0)
                {
                    fs.Write(buffer, 0, ret);
                    ret = input.Read(buffer, 0, buffer.Length);
                }
            }
        }

        protected virtual string BuildTilePath(OutputTile outputTile)
        {
            string zoomFolder;
            if (!_zoomFolderDic.ContainsKey(outputTile.Zoom))
            {
                zoomFolder = Path.Combine(_rootPath, outputTile.Zoom);
                Directory.CreateDirectory(zoomFolder);
                _zoomFolderDic[outputTile.Zoom] = zoomFolder;
            }
            else
            {
                zoomFolder = _zoomFolderDic[outputTile.Zoom];
            }

            string zoomXFolder;
            string zoomXKey = outputTile.Zoom + "_" + outputTile.X;
            if (!_zoomAndXFolderDic.ContainsKey(zoomXKey))
            {
                zoomXFolder = Path.Combine(zoomFolder, outputTile.X);
                Directory.CreateDirectory(zoomXFolder);
                _zoomFolderDic[zoomXKey] = zoomXFolder;
            }
            else
            {
                zoomXFolder = _zoomFolderDic[zoomXKey];
            }

            return Path.Combine(zoomXFolder, outputTile.Y + ".png");
        }
        #endregion
    }
}
