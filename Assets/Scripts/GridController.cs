using System;
using System.Collections.Generic;

public class GridController<T> where T : class, ITile {
	public interface IGridManager {
		T CreateTile(Coord coord);
		void RemoveTile(T tile);
		void MoveTile(T a,  Coord coord);
		void FreeTile(T a);
	}
		
	private readonly Coord[] _offsets = {new Coord(-1, 0), new Coord(0, 1), new Coord(1, 0), new Coord(0, -1)};
	private IGridManager _gridManager;
	public const int TypeCount = 4;
	public Coord Size;
	public T[] Grid;

	public T this[int x, int y] {
		get => Grid[x + y * Size.X];
		set => Grid[x + y * Size.X] = value;
	}

	public GridController(IGridManager gridManager, Coord size) {
		Size = size;
		_gridManager = gridManager;
			
		Grid = new T[Size.X * Size.Y];
		for (int i = 0; i < Grid.Length; i++) {
			var coord = new Coord(i % Size.X, i / Size.X);
			Grid[i] = _gridManager.CreateTile(coord);
		}
	}

	public void Shuffle()
    {
		List<T> copiedGrid = new(Grid);

		for (int i = 0; i < Grid.Length / 2; i++)
		{
			var a = copiedGrid[UnityEngine.Random.Range(0, copiedGrid.Count)];
			copiedGrid.Remove(a);
			var b = copiedGrid[UnityEngine.Random.Range(0, copiedGrid.Count)];
			copiedGrid.Remove(b);
			Swap(a, b);
		}
	}

	public bool Swap(T a, T b) {
		var aIndex = Array.IndexOf(Grid, a);
		var bIndex = Array.IndexOf(Grid, b);
		Grid[bIndex] = a;
		Grid[aIndex] = b;
		_gridManager.MoveTile(a, IndexToCoord(bIndex));
		_gridManager.MoveTile(b, IndexToCoord(aIndex));
		return true;
	}

	public void Fill() {
		for (int i = 0; i < Grid.Length; i++) {
			if (Grid[i] != null) continue;
			Coord coord = IndexToCoord(i);
			Coord spawnCoord = new(coord.X, Size.Y * 2);
			Grid[i] = _gridManager.CreateTile(spawnCoord);
			_gridManager.MoveTile(Grid[i], coord);
		}
	}

	public LinkedList<T> GetAdjacentTiles(T tile) {
		var index = Array.IndexOf(Grid, tile);
		if (index < 0) return null;
		Coord target = IndexToCoord(index);
		LinkedList<T> neighbours = new LinkedList<T>();
		foreach (Coord offset in _offsets) {
			Coord neighbour = target + offset;
			if (IsOutsideGrid(neighbour)) continue;
			neighbours.AddLast(this[neighbour.X, neighbour.Y]);
		}

		return neighbours;
	}

	public void Remove(T tile) {
		var index = Array.IndexOf(Grid, tile);
		Grid[index] = null;
		_gridManager.RemoveTile(tile);
	}

	public void Step() {
		for (int i = 0; i < Grid.Length; i++) {
			if (Grid[i] != null) continue;
			for (int j = i; j < Grid.Length; j += Size.X) {
				if (Grid[j] == null) continue;
				Grid[i] = Grid[j];
				Grid[j] = null;
				_gridManager.MoveTile(Grid[i], IndexToCoord(i));
				_gridManager.FreeTile(Grid[i]);
				break;
			}
		}
	}

	private bool IsOutsideGrid(Coord coord) {
		return coord.X < 0 || coord.X >= Size.X || coord.Y < 0 || coord.Y >= Size.Y;
	}

	private int CoordToIndex(Coord coord) {
		return coord.X + coord.X * Size.X;
	}

	private Coord IndexToCoord(int index) {
		return new Coord(index % Size.X, index / Size.X);
	}
}
