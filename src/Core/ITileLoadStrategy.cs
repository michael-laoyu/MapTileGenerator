using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    interface ITileLoadStrategy
    {
        Stream GetTile(string url);
    }
}
