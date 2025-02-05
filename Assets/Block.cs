using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public string blockColor; // The block's color
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    public Vector2Int gridPosition; // Position in the grid
    private static BoardManager boardManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>(); // Store reference once
    }




    public void SetBlock(string color, Sprite sprite, Vector2Int position)
    {
        blockColor = color; // Set the block's color
        spriteRenderer.sprite = sprite; // Assign the correct sprite
        gridPosition = position; // Update the grid position
        spriteRenderer.color = Color.white; // Reset any highlight effects
    }


    private void OnMouseDown()
    {
        StartCoroutine(ClickEffect()); // Add animation effect

        BoardManager boardManager = FindObjectOfType<BoardManager>();
        if (boardManager != null)
        {
            boardManager.HandleBlockClick(this);
        }
    }

    private IEnumerator ClickEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Slightly larger size when clicked
        float duration = 0.1f; // Animation speed
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            yield return null;
        }

        transform.localScale = originalScale; // Ensure final reset
    }


}
