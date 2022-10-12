using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int columns = 0;
    public int rows = 0;
    public float squareGaps = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOfset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSqaures = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquarePositions();
    }

    private void SpawnGridSquares()
    {
        // 0, 1, 2, 3, 4
        // 5, 6, 7, 8, 9

        int squareIndex = 0;
        
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            {
                _gridSqaures.Add(Instantiate(gridSquare) as GameObject);
                _gridSqaures[_gridSqaures.Count - 1].transform.SetParent(this.transform);
                _gridSqaures[_gridSqaures.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSqaures[_gridSqaures.Count - 1].GetComponent<GridSquare>().SetImage(squareIndex % 2 == 0);
                squareIndex++;
            }
        }
    }

    private void SetGridSquarePositions()
    {
        int columnNum = 0;
        int rowNum = 0;
        Vector2 squareGapNum = new Vector2(0.0f, 0.0f);
        bool rowMoved = false;

        var squareRect = _gridSqaures[0].GetComponent<RectTransform>();

        _offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOfset;
        _offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOfset;

        foreach (var square in _gridSqaures)
        {
            if (columnNum + 1 > columns)
            {
                squareGapNum.x = 0;
                // go to next column
                columnNum = 0;
                rowNum++;
                rowMoved = false;
            }

            var positionXOffset = _offset.x * columnNum + (squareGapNum.x * squareGaps);
            var positionYOffset = _offset.y * rowNum + (squareGapNum.y * squareGaps);

            if (columnNum > 0 && columnNum % 3 == 0)
            {
                squareGapNum.x++;
                positionXOffset += squareGaps;
            }

            if (rowNum > 0 && rowNum % 3 == 0 && !rowMoved)
            {
                rowMoved = true;
                squareGapNum.y++;
                positionYOffset += squareGaps;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + positionXOffset, startPosition.y - positionYOffset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + positionXOffset, startPosition.y - positionYOffset, 0.0f);
            columnNum++;
        }

    }
}
