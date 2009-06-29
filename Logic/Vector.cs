using System;

namespace ifpfc.Logic
{
	public struct Vector
	{
		public static readonly Vector Zero = new Vector(0.0, 0.0);
		public double x, y;

		public Vector(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return string.Format("{0:F3}, {1:F3}, L:{2:F2}", x, y, Len());
		}

		public double PolarAngle
		{
			get { return Math.Atan2(y, x); }
		}

		public double Len()
		{
			return Math.Sqrt(Len2());
		}

		public double Len2()
		{
			return x*x + y*y;
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			return new Vector(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vector operator +(Vector v1, Vector v2)
		{
			return new Vector(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vector operator *(Vector v, double f)
		{
			return new Vector(v.x*f, v.y*f);
		}

		public static Vector operator *(double f, Vector v)
		{
			return v*f;
		}

		public Vector Mul(double f)
		{
			return new Vector(x*f, y*f);
		}

		public Vector Sub(Vector vector)
		{
			return new Vector(x - vector.x, y - vector.y);
		}

		public Vector Norm()
		{
			var len = Len();
			return Math.Abs(len) < 1e-10 ? Zero : new Vector(x / len, y / len);
		}

		public bool Equals(Vector other)
		{
			return other.x == x && other.y == y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(Vector)) return false;
			return Equals((Vector) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode()*397) ^ y.GetHashCode();
			}
		}
	}
}