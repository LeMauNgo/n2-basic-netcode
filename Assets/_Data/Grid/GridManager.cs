using com.cyborgAssets.inspectorButtonPro;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : SaiSingleton<GridManager>
{
    [SerializeField] protected Transform startPoint;
    [SerializeField] protected GridCell cellPrefab;
    [SerializeField] protected Vector2 size = new(22, 7);
    [SerializeField] protected List<GridCell> createdCells = new();

    protected override void Start()
    {
        base.Start();
        this.cellPrefab.gameObject.SetActive(false);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGridCell();
        this.LoadStartPoint();
    }

    protected virtual void LoadStartPoint()
    {
        if (this.startPoint != null) return;
        this.startPoint = transform.Find("StartPoint");
        Debug.LogWarning(transform.name + ": LoadStartPoint", gameObject);
    }

    protected virtual void LoadGridCell()
    {
        if (this.cellPrefab != null) return;
        this.cellPrefab = GetComponentInChildren<GridCell>();
        this.cellPrefab.SetActive(false);
        Debug.LogWarning(transform.name + ": LoadGridCell", gameObject);
    }

    [ProButton]
    protected virtual void CreateGrid()
    {
        this.ClearGrid();
        createdCells.Clear();

        Vector3 startPosition = startPoint.position;
        Vector3 rowOffset = new Vector3(0, -1, 0); // Giảm Y mỗi khi xuống hàng mới
        Vector3 colOffset = new Vector3(1, 0, 0); // Mỗi ô cách nhau đúng 1 đơn vị theo X

        for (int row = 0; row < size.y; row++)
        {
            Vector3 rowStartPos = startPosition + rowOffset * row;

            for (int col = 0; col < size.x; col++)
            {
                Vector3 cellPosition = rowStartPos + colOffset * col;
                GridCell newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                newCell.gameObject.SetActive(true);
                createdCells.Add(newCell);
            }
        }
    }

    [ProButton]
    protected virtual void ClearGrid()
    {
        foreach (var cell in createdCells)
        {
            if (cell != null)
            {
                DestroyImmediate(cell.gameObject);
            }
        }
        createdCells.Clear();
    }
}
