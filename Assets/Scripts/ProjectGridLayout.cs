using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class ProjectGridLayout : LayoutGroup
{
    [SerializeField] private int columns = 2; // Number of columns
    [SerializeField] private float spacingX = 10f;
    [SerializeField] private float spacingY = 10f;
    [SerializeField] private float maxCellWidth = 200f; // Max width per cell

    private float cellWidth;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (rectChildren.Count == 0) return;

        float totalWidth = rectTransform.rect.width - padding.left - padding.right;
        cellWidth = Mathf.Min((totalWidth - (columns - 1) * spacingX) / columns, maxCellWidth);

        SetLayoutInputForAxis(totalWidth, totalWidth, -1, 0);
    }

    public override void CalculateLayoutInputVertical()
    {
        if (rectChildren.Count == 0) return;

        float totalHeight = 0;
        int rows = Mathf.CeilToInt((float)rectChildren.Count / columns);

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            float aspectRatio = GetAspectRatio(child);
            float cellHeight = cellWidth / aspectRatio; // Apply your formula

            if (i % columns == 0) totalHeight += cellHeight + spacingY;
        }

        SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
    }

    public override void SetLayoutHorizontal()
    {
        ArrangeCells();
    }

    public override void SetLayoutVertical()
    {
        ArrangeCells();
    }

    private void ArrangeCells()
    {
        if (rectChildren.Count == 0) return;

        float startX = padding.left;
        float startY = -padding.top;

        int row = 0, col = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            float aspectRatio = GetAspectRatio(child);
            float cellHeight = cellWidth / aspectRatio; // Apply your formula

            float posX = startX + col * (cellWidth + spacingX);
            float posY = startY - row * (cellHeight + spacingY);

            SetChildAlongAxis(child, 0, posX, cellWidth);
            SetChildAlongAxis(child, 1, posY, cellHeight);

            col++;
            if (col >= columns)
            {
                col = 0;
                row++;
            }
        }
    }

    private float GetAspectRatio(RectTransform rect)
    {
        Image img = rect.GetComponent<Image>();
        if (img != null && img.sprite != null)
        {
            return (float)img.sprite.texture.width / img.sprite.texture.height;
        }
        return 1f; // Default to square if no image
    }
}
