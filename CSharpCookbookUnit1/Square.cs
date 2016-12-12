using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpCookbookUnit1
{
    public class Square : IComparable<Square>
    {
        public Square() { }
        public Square(int height, int width)
        {
            this.Height = height;
            this.Width = width;
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public int CompareTo(Square other)
        {
            long area1 = this.Height * this.Width;
            long area2 = other.Height * other.Width;
            if (area1 == area2)
                return 0;
            else if (area1 > area2)
                return 1;
            else if (area1 < area2)
                return -1;
            else
                return -1;
        }

        public override int GetHashCode()
        {
            return this.Height.GetHashCode() | this.Width.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Square square = obj as Square;
            if (square != null)
            {
                return this.Height == square.Height && this.Width == square.Width;
            }

            return false;
        }

        public int CompareTo(object obj)
        {
            Square square = obj as Square;
            if (square != null)
                return CompareTo(square);
            throw
            new ArgumentException(
            "Both objects being compared must be of type Square.");
        }

        public override string ToString()
        {
            return string.Format("Height: {0} Width: {1}", this.Height, this.Width);
        }

        public static bool operator ==(Square x, Square y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Square x, Square y)
        {
            return !(x == y);
        }
        public static bool operator <(Square x, Square y)
        {
            return (x.CompareTo(y) < 0);
        }
        public static bool operator >(Square x, Square y)
        {
            return (x.CompareTo(y) > 0);
        }
    }
}
