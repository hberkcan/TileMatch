using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    private GridTile gridTile;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private IconContainer iconContainer;
    [SerializeField] private int A, B, C;

    private void Awake()
    {
        gridTile = GetComponent<GridTile>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        gridTile.UpdateIconEvent += GridTile_UpdateIconEvent;
        gridTile.UpdateLayerOrderEvent += GridTile_UpdateLayerOrderEvent;
    }

    private void GridTile_UpdateLayerOrderEvent()
    {
        spriteRenderer.sortingOrder = (int)transform.position.y;
    }

    private void GridTile_UpdateIconEvent(int count)
    {
        int i = 0;

        if (count <= A)
        {
            i = 0;
        }
        else if (A < count && count <= B)
        {
            i = 1;
        }
        else if (B < count && count <= C)
        {
            i = 2;
        }
        else if (C < count)
        {
            i = 3;
        }

        spriteRenderer.sprite = iconContainer.sprites[i];
    }
}
