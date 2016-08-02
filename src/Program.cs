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
            var tileIndex = 0;
            generator.TileLoaded += new EventHandler<TileCoord>((sender, tileCoord) =>
            {
                ++tileIndex;
                Console.WriteLine(string.Format("Tile : zoom ：  {0} x： {1} y ：{2}，已完成：{3}/{4}",tileCoord.Zoom,tileCoord.X,
                    tileCoord.Y, tileIndex, generator.Source.TileGrid.TileTotal));
            });
            Console.WriteLine("服务已启动......");
            Console.WriteLine("输入回车退出...");

            generator.Start();
            Console.ReadLine();
            generator.Close();

        }
    }
}
