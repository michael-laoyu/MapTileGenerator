using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        [JsonProperty("wmsUrl")]
        public string Url
        {
            get;
            set;
        }

        [JsonProperty("wmsParas")]
        public WmsParas Paras
        {
            get;
            set;
        }

        public static MapConfig Load()
        {
            MapConfig instance = null;
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "mapConfig.json"),
                        FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string json = sr.ReadToEnd();
                instance = JsonConvert.DeserializeObject<MapConfig>(json);  
            }
            return instance;
        }

        public class WmsParas
        {
            [JsonProperty("FORMAT")]
            public string Format
            {
                get;
                set;
            }

            [JsonProperty("VERSION")]
            public string Version
            {
                get;
                set;
            }

            [JsonProperty("STYLES")]
            public string Style
            {
                get;
                set;
            }
            [JsonProperty("LAYERS")]
            public string Layers
            {
                get;
                set;
            }
            [JsonProperty("REQUEST")]
            public string Request
            {
                get;
                set;
            }

            [JsonProperty("SRS")]
            public string Srs
            {
                get;
                set;
            }
        }
    }
}
