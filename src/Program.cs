using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapTileGenerator.Core;

namespace MapTileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("加载mapConfig.json...");
            MapConfig mapConfig = MapConfig.Load();
            TileGenerator generator = new Core.TileGenerator(mapConfig);
            generator.Start();
            Console.WriteLine("服务已启动......");
            Console.WriteLine("输入回车退出...");
            Console.ReadLine();
            generator.Close();

        }
    }
}
