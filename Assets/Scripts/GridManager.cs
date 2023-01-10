using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
//using Zenject;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System;

public class GridManager : MonoBehaviour, GridController<GridTile>.IGridManager
{
	public int SizeX, SizeY;
	public float CellSize;

	[SerializeField] private GridTile[] Prefabs;
	private GridController<GridTile> _gridController;
	private GridTile _selected;

	private HashSet<GridTile> checkedGridTiles;
	private int island = 0;

	//[Inject]
	//public void Inject(GridController<GridTile> gridController) {
	//	_gridController = gridController;
	//}

	public void Awake()
	{
		_gridController = new GridController<GridTile>(this, new Coord(SizeX, SizeY));
		checkedGridTiles = new HashSet<GridTile>(SizeX * SizeY);
	}

	private void Start()
	{
		UUBaby();
    }

	public GridTile CreateTile(Coord coord)
	{
		var random = Random.Range(0, 6);
		GridTile tile = Instantiate(Prefabs[random], Coord2Position(coord), Quaternion.identity, transform);
		//tile.transform.localScale = Vector3.zero;
		tile.Type = random;
		//tile.transform.DOScale(Vector3.one, .25f);
		tile.Clicked = TileClicked;
		return tile;
	}

	public void MoveTile(GridTile tile, Coord coord)
	{
		tile.transform.DOLocalMove(Coord2Position(coord), .25f).OnComplete(() => tile.transform.DOPunchPosition(Vector3.up * .4f, .2f));
	}

	public void RemoveTile(GridTile tile)
	{
		tile.transform.DOScale(0, .25f).OnComplete(() => Destroy(tile.gameObject));
	}

	[Button]
	public void TestFill()
	{
		_gridController.Fill();
	}

	[Button]
	public void TestRemoval()
	{
		GridTile[] tiles = GetComponentsInChildren<GridTile>();
		GridTile target = tiles[Random.Range(0, tiles.Length)];
		_gridController.Remove(target);
		_gridController.Step();
	}

	private Vector3 Coord2Position(Coord coord)
	{
		var offset = new Vector3((SizeX - 1) * CellSize * .5f, (SizeY - 1) * CellSize * .5f, 0);
		return new Vector3(coord.X * CellSize, coord.Y * CellSize, 0) - offset;
	}

	private void TileClicked(GridTile tile)
	{
		//if (_selected == null) {
		//	_selected = tile;
		//	return;
		//}

		//if (tile == _selected) {
		//	_selected = null;
		//	return;
		//}

		//_selected = _gridController.Swap(_selected, tile) ? null : tile;
		//HashSet<GridTile> tilesInIsland = new HashSet<GridTile>(SizeX * SizeY);
		_selected = tile;
		//FindIsland(_selected, ref tilesInIsland);
		//Debug.Log(tilesInIsland.Count);
		Debug.Log(_selected.name);
        HashSet<GridTile> kek = new HashSet<GridTile>(SizeX * SizeY);

        foreach (GridTile t in FindIsland(_selected, ref kek))
        {
            _gridController.Remove(t);
        };

        kek.Clear();
        _gridController.Step();
        _gridController.Fill();
        _selected = null;
		checkedGridTiles.Clear();
		UUBaby();
    }

	private HashSet<GridTile> FindIsland(GridTile gridTile, ref HashSet<GridTile> tilesInIsland)
	{
		foreach (GridTile t in _gridController.GetAdjacentTiles(gridTile))
		{
			if (t.Type == gridTile.Type && tilesInIsland.Add(t))
			{
				t.kek = island;
				FindIsland(t, ref tilesInIsland);
			}
		}

		return tilesInIsland;
	}

	private void UUBaby()
    {
		HashSet<GridTile> tilesInIsland = new HashSet<GridTile>(SizeX * SizeY);

		foreach (GridTile gt in _gridController.Grid)
		{
			gt.UpdateLayerOrder();

			if (checkedGridTiles.Contains(gt))
				continue;

			HashSet<GridTile> lol = FindIsland(gt, ref tilesInIsland);

			if (lol.Count > 0)
			{
				island++;
				Island ýsland = new();

				foreach (GridTile lele in lol)
				{
					lele.UpdateIcon(lol.Count);
					lele.island = ýsland;
				}

				checkedGridTiles.UnionWith(lol);
				tilesInIsland.Clear();
			}

			checkedGridTiles.Add(gt);
		}
	}
}
