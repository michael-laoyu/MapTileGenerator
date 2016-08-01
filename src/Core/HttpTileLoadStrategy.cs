using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class HttpTileLoadStrategy : ITileLoadStrategy
    {

        #region ITileLoadStrategy 成员

        public Stream GetTile(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            WebResponse response = request.GetResponse();
            return response.GetResponseStream();
        }

        #endregion
    }
}
