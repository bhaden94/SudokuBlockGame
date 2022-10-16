using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squareGaps = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOfset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSqaures = new List<GameObject>();
    private LineIndicator _lineIndicator;

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquarePositions();
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
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

                _gridSqaures[_gridSqaures.Count - 1].GetComponent<GridSquare>().SquareIndex = squareIndex;
                _gridSqaures[_gridSqaures.Count - 1].transform.SetParent(this.transform);
                _gridSqaures[_gridSqaures.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSqaures[_gridSqaures.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(squareIndex) % 2 == 0);
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

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSqaures)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                //gridSquare.ActivateSquare();
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return; // there is no selected shape

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var i in squareIndexes)
            {
                _gridSqaures[i].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        // loop through columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        // loop through rows
        for (var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var i = 0; i < 9; i++)
            {
                data.Add(_lineIndicator.lineData[row, i]);
            }

            lines.Add(data.ToArray());
        }

        // loop through nested squares
        for (var square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (var i = 0; i < 9; i++)
            {
                data.Add(_lineIndicator.squareData[square, i]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if (completedLines > 2)
        {
            //TODO: play bonus animation
        }

        var totalScores = 10 * completedLines;
        GameEvents.AddScores(totalScores);
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSqaures[squareIndex].GetComponent<GridSquare>();
                if (!comp.SquareOccupied)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSqaures[squareIndex].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSqaures[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }
}
