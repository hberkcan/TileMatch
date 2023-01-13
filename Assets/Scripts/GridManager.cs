using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
//using Zenject;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System;
using System.Collections;

public class GridManager : MonoBehaviour, GridController<GridTile>.IGridManager
{
	[SerializeField] private GameData gameData;
	//public int SizeX, SizeY;
	//public float CellSize;

	[SerializeField] private GridTile[] Prefabs;
	private GridController<GridTile> _gridController;
	private GridTile _selected;

	private HashSet<GridTile> checkedGridTiles;
	public HashSet<Island> Islands = new(16);

	public static event Action BoardUpdated;

	//[Inject]
	//public void Inject(GridController<GridTile> gridController) {
	//	_gridController = gridController;
	//}

	private void Awake()
	{
		_gridController = new GridController<GridTile>(this, new Coord(gameData.SizeX, gameData.SizeY));
		checkedGridTiles = new HashSet<GridTile>(gameData.SizeX * gameData.SizeY);
	}

	private void Start()
	{
		//StartCoroutine(
		//        UpdateBoardCoroutine());

		DetectIslandsOnBoard();
		BoardUpdated?.Invoke();
	}

	public GridTile CreateTile(Coord coord)
	{
		var random = Random.Range(0, gameData.ColorCount);
		GridTile tile = Instantiate(Prefabs[random], Coord2Position(coord), Quaternion.identity, transform);
		//tile.transform.localScale = Vector3.zero;
		tile.Type = random;
		//tile.transform.DOScale(Vector3.one, .25f);
		tile.Clicked = TileClicked;
		tile.UpdateLayerOrder();
		return tile;
	}

	public void MoveTile(GridTile tile, Coord coord)
	{
		tile.transform.DOLocalMove(Coord2Position(coord), .25f).OnComplete(() =>
		{
			tile.transform.DOPunchPosition(Vector3.up * .4f, .2f);
			tile.UpdateLayerOrder();
		});

		Island island = tile.GetIsland;
		if (island != null)
        {
			DiscardIsland(tile);
			return;
        }

		checkedGridTiles.Remove(tile);
	}

	public void RemoveTile(GridTile tile)
	{
		tile.transform.DOScale(0, .25f).OnComplete(() => Destroy(tile.gameObject));
	}

	public void UncheckTile(GridTile tile)
    {

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

	[Button]
	public void TestShuffle()
    {
        foreach (GridTile gridTile in _gridController.Grid)
        {
			gridTile.SetIsland(null);
        }
		Islands.Clear();
        ShuffleBoard();
    }

	private Vector3 Coord2Position(Coord coord)
	{
		var offset = new Vector3((gameData.SizeX - 1) * gameData.CellSize * .5f, (gameData.SizeY - 1) * gameData.CellSize * .5f, 0);
		return new Vector3(coord.X * gameData.CellSize, coord.Y * gameData.CellSize, 0) - offset;
	}

	private void TileClicked(GridTile tile)
	{
		_selected = tile;

		if(_selected.GetIsland == null)
        {
			_selected = null;
			return;
        }

        DestroyIsland(_selected.GetIsland);

		_gridController.Step();
		_gridController.Fill();
		_selected = null;

        //StartCoroutine(
        //        UpdateBoardCoroutine());

		DetectIslandsOnBoard();
		BoardUpdated?.Invoke();
	}

	private HashSet<GridTile> FindIsland(GridTile gridTile, ref HashSet<GridTile> tilesInIsland)
	{
		foreach (GridTile t in _gridController.GetAdjacentTiles(gridTile))
		{
			if (t.Type == gridTile.Type && tilesInIsland.Add(t))
			{
				tilesInIsland.Add(gridTile);

                if (t.GetIsland != null && checkedGridTiles.Contains(t))
                {
					//AddTileToIsland(gridTile, t.GetIsland);

					//foreach (GridTile g in t.GetIsland)
					//{
					//    tilesInIsland.Add(g);
					//}

					//newIsland = false;
					tilesInIsland.UnionWith(t.GetIsland.GetItems());
					//continue;
					DiscardIsland(t);
                }

                FindIsland(t, ref tilesInIsland);
			}
		}

		return tilesInIsland;
	}

	private void DetectIslandsOnBoard()
    {
		HashSet<GridTile> refSet = new(gameData.SizeX * gameData.SizeY);

		foreach (GridTile gridTile in _gridController.Grid)
		{
			if (!checkedGridTiles.Add(gridTile))
				continue;

			refSet.Clear();
			var tilesInIsland = FindIsland(gridTile, ref refSet);

   //         if (!newIsland)
   //         {
			//	//foreach (GridTile gt in tilesInIsland)
			//	//{
			//	//	checkedGridTiles.Add(gt);
			//	//}

			//	//refSet.Clear();
			//	//gridTile.UpdateLayerOrder();
			//	//gridTile.UpdateIcon();
			//	continue;
			//}

			if (tilesInIsland.Count > 1)
			{
				Island island = CreateIsland(tilesInIsland.Count);

				foreach (GridTile gt in tilesInIsland)
                {
					AddTileToIsland(gt, island);
					//gt.UpdateIcon();
                }

				//refSet.Clear();
			}

            //gridTile.UpdateLayerOrder();
            //gridTile.UpdateIcon();
        }

        StartCoroutine(
                UpdateBoardCoroutine());
    }

	private Island CreateIsland(int size)
    {
		Island island = new(size);
		Islands.Add(island);
		return island;
    }

	private void DestroyIsland(Island island)
    {
		int size = island.Size;
		for (int i = size - 1; i >= 0; i--)
		{
			GridTile gridTile = island.GetItems()[i];
			checkedGridTiles.Remove(gridTile);
			_gridController.Remove(gridTile);
		}

		Islands.Remove(island);
    }

	private void AddTileToIsland(GridTile tile, Island island)
    {
		island.AddItem(tile);
		tile.SetIsland(island);
		checkedGridTiles.Add(tile);
    }

	private void DiscardIsland(GridTile tile)
    {
		//Island island = tile.GetIsland;
		//island.RemoveItem(tile);
		//tile.SetIsland(null);
		//checkedGridTiles.Remove(tile);

		//if(island.GetSize() == 1)
		//      {
		//	GridTile gt = island.GetItems()[0];
		//	gt.SetIsland(null);
		//	checkedGridTiles.Remove(gt);

		//	islands.Remove(island);
		//      }

		Island island = tile.GetIsland;

		foreach(GridTile t in island)
        {
			t.SetIsland(null);
			checkedGridTiles.Remove(t);
			//t.UpdateIcon();
        }

		Islands.Remove(island);
    }

	public IEnumerator UpdateBoardCoroutine()
    {
		yield return new WaitForSeconds(.25f);
		//DetectIslandsOnBoard();
		//BoardUpdated?.Invoke();
		//foreach (GridTile gridTile in _gridController.Grid)
		//{
		//    gridTile.UpdateLayerOrder();
		//    gridTile.UpdateIcon();
		//}
		CheckDeadlock();
    }

	private void CheckDeadlock()
    {
		if (Islands.Count == 0)
			ShuffleBoard();
    }

	private void ShuffleBoard()
    {
		_gridController.Shuffle();
		checkedGridTiles.Clear();
        DetectIslandsOnBoard();
		BoardUpdated?.Invoke();
	}
}
