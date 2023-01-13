using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private IconContainer iconContainer;
    [SerializeField] private GameData gameData;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateLayerOrder()
    {
        spriteRenderer.sortingOrder = (int)transform.position.y;
    }

    public void UpdateIcon(int islandSize)
    {
        int i = 0;

        if (islandSize <= gameData.FirstIconThreshold)
        {
            i = 0;
        }
        else if (gameData.FirstIconThreshold < islandSize && islandSize <= gameData.SecondIconThreshold)
        {
            i = 1;
        }
        else if (gameData.SecondIconThreshold < islandSize && islandSize <= gameData.ThirdIconThreshold)
        {
            i = 2;
        }
        else if (gameData.ThirdIconThreshold < islandSize)
        {
            i = 3;
        }

        spriteRenderer.sprite = iconContainer.Sprites[i];
    }
}
