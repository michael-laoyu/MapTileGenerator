using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class Extent
    {
        public Extent(double[] extent)
        {
            if (extent == null || extent.Length < 4)
            {
                throw new ArgumentException("extent must be array [minx,miny,maxx,maxy]");
            }
            MinX = extent[0];
            MinY = extent[1];
            MaxX = extent[2];
            MaxY = extent[3];
        }

        public Extent(Coordinate leftBottom, Coordinate topRight)
        {
            if (leftBottom != null && topRight != null)
            {
                MinX = leftBottom.X;
                MinY = leftBottom.Y;
                MaxX = topRight.X;
                MaxY = topRight.Y;
            }
        }

        public Coordinate GetLeftBottom()
        {
            return new Coordinate(MinX, MinY);
        }

        public Coordinate GetTopRight()
        {
            return new Coordinate(MaxX, MaxY);
        }

        public Coordinate GetTopLeft()
        {
            return new Coordinate(MinX, MaxY);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", MinX, MinY, MaxX, MaxY);
        }

        public double MinX;
        public double MinY;
        public double MaxX;
        public double MaxY;

    }

    public class Size
    {
        public Size(int[] size)
        {
            if (size == null || size.Length < 2)
            {
                throw new ArgumentException("extent must be array [width,height]");
            }
            Width = size[0];
            Height = size[1];
        }
        public int Width;
        public int Height;
    }

    public class Coordinate
    {
        public Coordinate(double[] coord)
        {
            if (coord == null || coord.Length < 2)
            {
                throw new ArgumentException("extent must be array [x,y]");
            }
            X = coord[0];
            Y = coord[1];
        }

        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;

    }

    public class TileCoord
    {
        public TileCoord(int zoom, double x, double y)
        {
            Zoom = zoom;
            X = x;
            Y = y;
        }

        public int Zoom;
        public double X;
        public double Y;
    }
}
