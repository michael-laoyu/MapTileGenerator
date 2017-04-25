using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 瓦片输出的策略
    /// </summary>
    public interface ITileOutputStrategy
    {

        void Init(string rootPath);

        void Write(Stream input,OutputTile outputTile);
    }

    public class OutputTile
    {
        public OutputTile() { }

        public OutputTile(string zoom,string x,string y)
        {
            this.Zoom = zoom;
            this.X = x;
            this.Y = y;
        }

        public string Zoom;
        public string X;
        public string Y;
    }
}
