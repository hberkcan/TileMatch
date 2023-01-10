using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridTile : MonoBehaviour, ITile, IPointerClickHandler {
	public int Type { get; set; }
	public Action<GridTile> Clicked;
	public event Action<int> UpdateIconEvent;
	public event Action UpdateLayerOrderEvent;

	public Island island;
	public int kek = -1;
		
	public void OnPointerClick(PointerEventData eventData) {
		Clicked?.Invoke(this);
	}

	public void UpdateIcon(int count)
    {
		UpdateIconEvent?.Invoke(count);
    }

	public void UpdateLayerOrder()
    {
		UpdateLayerOrderEvent?.Invoke();
    }
}
