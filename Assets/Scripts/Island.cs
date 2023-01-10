using System.Collections;
using System.Collections.Generic;

public class Island : IEnumerable
{
    List<GridTile> _tiles = new(4);

    public List<GridTile> getItems()
    {
        return _tiles;
    }

    public void AddItem(GridTile item)
    {
        this._tiles.Add(item);
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

    public object Current => _island.getItems()[_position];

    public bool MoveNext()
    {
        _position++;
        return _position < _island.getItems().Count;
    }

    public void Reset()
    {
        _position = -1;
    }
}
