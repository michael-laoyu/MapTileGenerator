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
    public class FailsProvider
    {
        private string _sqliteConnectionString;
        public const string FILENAME = "fails.db";
        public int Count = 0;

        public List<FailTile> Load(string file)
        {
            try
            {
                _sqliteConnectionString = string.Format(
@"Data Source={0};Pooling=true;FailIfMissing=false;Version=3;UTF8Encoding=True;Journal Mode=Off;", file);

                if (File.Exists(file))
                {
                    var sql = @"select ID,Zoom,X,Y from FailTile;";
                    var list = new List<FailTile>();
                    using (var conn = new SQLiteConnection(_sqliteConnectionString))
                    {
                        list = conn.Query<FailTile>(sql) as List<FailTile>;
                        Count = list.Count;
                        return list;
                    }
                }
                else
                {
                    //创建空库与空表，创建索引
                    SQLiteConnection.CreateFile(file);
                    var conn = new SQLiteConnection(_sqliteConnectionString);
                    conn.Open();

                    string sql = @"create table FailTile (
                                [Id] integer PRIMARY KEY autoincrement,-- 设置自增长主键
                                [Zoom] int,
                                [X]  double,
                                [Y] double 
                            );";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
            return null;
        }

        public void Insert(TileCoord tile)
        {
            try
            {
                var sql = @"insert into FailTile(Zoom,X,Y) values(@Zoom,@X,@Y);";
                int ret = 0;
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    ret = conn.Execute(sql, new
                    {
                        Zoom = tile.Zoom,
                        X = tile.X,
                        Y = tile.Y
                    });
                }
                if (ret > 0)
                {
                    System.Threading.Interlocked.Increment(ref Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete(FailTile failTile)
        {
            try
            {
                var sql = @"delete from FailTile where Id=@Id;";
                int ret = 0;
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    ret = conn.Execute(sql, new
                    {
                        Id = failTile.Id
                    });
                }
                if (ret > 0)
                {
                    System.Threading.Interlocked.Decrement(ref Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class FailTile
    {
        public FailTile() { }

        public FailTile(int id, int zoom, double x, double y)
        {
            Zoom = zoom;
            X = x;
            Y = y;
        }

        public TileCoord ConvertTo()
        {
            return new Core.TileCoord(Zoom, X, Y);
        }

        public int Id
        {
            get;
            set;
        }
        public int Zoom
        {
            get;
            set;
        }
        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
    }
}
