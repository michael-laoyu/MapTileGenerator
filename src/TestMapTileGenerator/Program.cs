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
            //监听控制台关闭事件；
            SetConsoleCtrlHandler(cancelHandler, true);

            Console.WriteLine("加载mapConfig.json...");
            MapConfig mapConfig = MapConfig.Load();
            generator = new TileGenerator(mapConfig);
            generator.TileLoaded += new EventHandler<TileCoord>((sender, tileCoord) =>
            {
                Console.WriteLine(string.Format("Tile : zoom ：  {0} x： {1} y ：{2}，已完成：{3}/{4}", tileCoord.Zoom, tileCoord.X,
                    tileCoord.Y, generator.SuccessTile, generator.TotalTile));
            });
            generator.Finished += new EventHandler<string>((sender, msg) =>
             {
                 Console.WriteLine("///////////////////////////////////////");
                 Console.WriteLine(msg);

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
        private static TileGenerator generator = null;

        #region 控制台关闭事件监听
        private delegate bool ControlCtrlDelegate(int CtrlType);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(CancelHandlerInvoker);

        private static bool CancelHandlerInvoker(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭 
                    if(generator != null)
                    {
                        generator.Close();
                    }
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭
                    if (generator != null)
                    {
                        generator.Close();
                    }
                    break;
            }
            Console.ReadLine();
            return false;
        }
        #endregion

    }
}
