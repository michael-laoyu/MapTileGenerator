using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 以sqlite数据库方式保存瓦片
    /// </summary>
    public class SqliteOutputStrategy : ITileOutputStrategy
    {
        protected string _rootPath;
        public SqliteOutputStrategy(string rootPath)
        {
            _rootPath = rootPath;
        }

        public string RootPath
        {
            get
            {
                return _rootPath;
            }
        }

        public void Write(Stream input, OutputTile outputTile)
        {
            throw new NotImplementedException();
        }
    }
}
