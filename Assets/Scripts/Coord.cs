using System;

[Serializable]
public struct Coord
{
	public int X;
	public int Y;

	public Coord(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static Coord operator +(Coord a, Coord b)
	{
		return new Coord(a.X + b.X, a.Y + b.Y);
	}

	public static Coord operator -(Coord a, Coord b)
	{
		return new Coord(a.X - b.X, a.Y - b.Y);
	}

	public override string ToString()
	{
		return $"{X},{Y}";
	}
}