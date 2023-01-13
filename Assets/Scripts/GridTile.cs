using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridTile : MonoBehaviour, ITile, IPointerClickHandler {
	public int Type { get; set; }
	public Action<GridTile> Clicked;
	private Island _island;

    private RenderController renderController;

    private void Awake()
    {
        renderController = GetComponent<RenderController>();
    }

    private void OnEnable()
    {
        GridManager.BoardUpdated += GridManager_BoardUpdated;
    }

    private void GridManager_BoardUpdated()
    {
        UpdateIcon();
    }

    public void OnPointerClick(PointerEventData eventData) {
		Clicked?.Invoke(this);
	}

	public void SetIsland(Island island)
    {
		_island = island;
    }

    public Island GetIsland => _island;

    public void UpdateLayerOrder()
    {
        renderController.UpdateLayerOrder();
    }

    public void UpdateIcon()
    {
        if(_island != null)
        {
            renderController.UpdateIcon(_island.Size);
            return;
        }

        renderController.UpdateIcon(0);
    }

    private void OnDestroy()
    {
        GridManager.BoardUpdated -= GridManager_BoardUpdated;
    }
}
