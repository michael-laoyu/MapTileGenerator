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
    class SqliteAndBase64OutputStrategy : ITileOutputStrategy
    {
        private string _rootPath;
        private string _sqliteConnectionString;
        public const string FILENAME = "tiles.db";

        public SqliteAndBase64OutputStrategy()
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
                Directory.CreateDirectory(Path.GetDirectoryName(db));
                //创建空库与空表，创建索引
                SQLiteConnection.CreateFile(db);
                var conn = new SQLiteConnection(_sqliteConnectionString);
                conn.Open();

                //创建索引；
                string sql = @"CREATE TABLE tiles (zoom_level TEXT, tile_column INTEGER, tile_row INTEGER, tile_data  CLOB);";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();

                //创建索引；
                sql = @"--DROP INDEX idx_tiles;
CREATE INDEX idx_tiles
  ON tiles
  (zoom_level, tile_row, tile_column);";
                command = new SQLiteCommand(sql, conn);
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
                byte[] tobase64 = ms.ToArray();
                StringBuilder sb = new StringBuilder();
                sb.Append("data:image/png;base64,").Append(Convert.ToBase64String(tobase64, 0, tobase64.Length));               

                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    ret = conn.Execute(sql, new
                    {
                        zoom = outputTile.Zoom,
                        x = int.Parse(outputTile.X),
                        y = int.Parse(outputTile.Y),
                        tileData = sb.ToString()
                    });
                }
            }
        }
    }
}
