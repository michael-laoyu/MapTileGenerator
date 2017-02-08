using MapTileGenerator;
using MapTileGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestMapTileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("加载mapConfig.json...");
            MapConfig mapConfig = MapConfig.Load();
            TileGenerator generator = new TileGenerator(mapConfig);
            generator.TileLoaded += new EventHandler<TileCoord>((sender, tileCoord) =>
            {
                Console.WriteLine(string.Format("Tile : zoom ：  {0} x： {1} y ：{2}，已完成：{3}/{4}", tileCoord.Zoom, tileCoord.X,
                    tileCoord.Y, generator.SuccessTileIndex, generator.TotalTile));
            });
            generator.Finished += new EventHandler((sender, e) =>
             {
                 Console.WriteLine(string.Format("完成：{0}/{1}，失败：{2}!", generator.SuccessTileIndex, generator.TotalTile,generator.FailTiles));

                 //if (generator.FailTiles > 0)
                 //{
                 //    Console.WriteLine("是否尝试重新下载失败的瓦片？[Y/N]");
                 //    string askYesNo = Console.ReadLine();
                 //    if (askYesNo.Trim().ToUpper() =="Y")
                 //    {
                 //        generator.RetryFails();
                 //    }
                 //}
             });

            Console.WriteLine("服务已启动......");
            generator.Start();
            Console.WriteLine("输入回车退出...");
            Console.ReadKey();
            generator.Close();
        }
    }
}
