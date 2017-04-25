using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    /// <summary>
    /// 以sqlite数据库方式保存瓦片,以mbtiles格式保存瓦片；
    /// mbtiles格式参考：https://github.com/mapbox/mbtiles-spec/
    ///                 http://blog.csdn.net/zfz1214/article/details/8880644
    /// </summary>
    public class SqliteOutputStrategy : ITileOutputStrategy
    {
        private string _rootPath;
        private string _sqliteConnectionString;
        public const string FILENAME = "tiles.db";

        public SqliteOutputStrategy()
        {
            
        }

        public void Init(string rootPath)
        {
            _rootPath = rootPath;
            string db = Path.Combine(rootPath, FILENAME);
            CreateDatabase(db);
        }

       

        private void CreateDatabase(string db)
        {
            _sqliteConnectionString = string.Format(
@"Data Source={0};Pooling=true;FailIfMissing=false;Version=3;UTF8Encoding=True;Journal Mode=Off;", db);

            if (!File.Exists(db))
            {
                //创建空库与空表，创建索引
                SQLiteConnection.CreateFile(db);
                var conn = new SQLiteConnection(_sqliteConnectionString);
                conn.Open();

                string sql = @"CREATE TABLE tiles (zoom_level TEXT, tile_column INTEGER, tile_row INTEGER, tile_data  BLOB);";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }


        public void Write(Stream input, OutputTile outputTile)
        {
            var sql = @"insert into tiles(zoom_level,tile_column,tile_row,tile_data) values(@zoom,@x,@y,@tileData);";
            int ret = 0;
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream())
            {
                ret = input.Read(buffer, 0, buffer.Length);
                while (ret > 0)
                {
                    ms.Write(buffer, 0, ret);
                    ret = input.Read(buffer, 0, 4096);
                }

                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    ret = conn.Execute(sql, new
                    {
                        zoom = outputTile.Zoom,
                        x = int.Parse(outputTile.X),
                        y = int.Parse(outputTile.Y),
                        tileData = ms.ToArray()
                    });
                }
            }
        }
    }
}
