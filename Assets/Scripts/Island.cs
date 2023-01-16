using System.Collections;
using System.Collections.Generic;
using System;

public class Island : IEnumerable
{
    private List<GridTile> _tiles;

    public Island(int size)
    {
        _tiles = new(size);
    }

    public List<GridTile> GetItems() => _tiles;

    public int Size => _tiles.Count;

    public void AddItem(GridTile item)
    {
        _tiles.Add(item);
    }

    public void RemoveItem(GridTile item)
    {
        _tiles.Remove(item);
    }

    public IEnumerator GetEnumerator()
    {
        return new IslandIterator(this);
    }
}

public class IslandIterator : IEnumerator
{
    private Island _island;
    private int _position = -1;

    public IslandIterator(Island island)
    {
        _island = island;
    }

    public object Current => _island.GetItems()[_position];

    public bool MoveNext()
    {
        _position++;
        return _position < _island.GetItems().Count;
    }

    public void Reset()
    {
        _position = -1;
    }
}
