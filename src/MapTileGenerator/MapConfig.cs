using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MapTileGenerator.Core;

namespace MapTileGenerator
{
    public class MapConfig
    {
        private int[] _tileSize = new int[] { 256, 256 };
        private double[] _origin = null;
        private double[] _extent;

        [JsonProperty("resolutions")]
        public double[] Resolutions
        {
            get;
            set;
        }

        [JsonProperty("offsetZoom")]
        public int OffsetZoom
        {
            get;
            set;
        }

        [JsonProperty("runThreadCount")]
        public int RunThreadCount
        {
            get;
            set;
        }

        [JsonProperty("savePath")]
        public string SavePath
        {
            get;
            set;
        }

        [JsonProperty("tileSize")]
        public int[] TileSize
        {
            get
            {
                return _tileSize;
            }
            set
            {
                if (value != null && value.Length == 2)
                {
                    _tileSize = value;
                }
                else
                {
                    throw new ArgumentException("The TileSize must be array [length, width] ");
                }
            }
        }

        [JsonProperty("extent")]
        public double[] Extent
        {
            get
            {
                return _extent;
            }
            set
            {
                if (value != null && value.Length == 4)
                {
                    _extent = value;
                }
                else
                {
                    throw new ArgumentException("The Extent must be array [minx, miny,maxx,maxy] ");
                }
            }
        }

        [JsonProperty("origin")]
        public double[] Origin
        {
            get
            {
                if (_origin == null)
                {
                    if (Extent != null && Extent.Length == 4)
                    {
                        _origin = new double[] { Extent[0], Extent[1]};
                    }
                }
                return _origin;
            }
            set
            {
                if (value != null && value.Length == 2)
                {
                    _origin = value;
                }
                else
                {
                    throw new ArgumentException("The origin must be array [x, y] ");
                }
            }
        }

        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }

        [JsonProperty("output")]
        public string Output
        {
            get;
            set;
        }

        [JsonProperty("url")]
        public string Url
        {
            get;
            set;
        }

        [JsonProperty("urlParas")]
        public Dictionary<string,object> UrlParas
        {
            get;
            set;
        }

        [JsonProperty("lastTile")]
        public TileCoord LastTile;

        public static MapConfig Load()
        {
            MapConfig config = null;
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "mapConfig.json"),
                        FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string json = sr.ReadToEnd();
                config = JsonConvert.DeserializeObject<MapConfig>(json);  
            }

            //if (config.TileSize == null)
            //{
            //    config.TileSize = new int[] { 256, 256 };
            //}
            //if (config.Origin == null)
            //{
            //    config.Origin = new double[] { config.Extent[0], config.Extent[3] };
            //}
            if (string.IsNullOrEmpty(config.SavePath))
            {
                config.SavePath = Path.Combine(Environment.CurrentDirectory, "Tiles");
                Directory.CreateDirectory(config.SavePath);
            }
            if (string.IsNullOrEmpty(config.Output))
            {
                config.Output = "File";
            }
            return config;
        }

        public void Save()
        {
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "mapConfig.json"),
                        FileMode.OpenOrCreate, FileAccess.Write))
            {
                Console.WriteLine("....");
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                string config = JsonConvert.SerializeObject(this);
                sw.Write(config);
            }
        }
    }
}
