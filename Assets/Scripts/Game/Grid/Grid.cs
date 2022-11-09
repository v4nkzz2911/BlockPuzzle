using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;

    private void OnDisable()
    {
        GameEvent.CheckIfShapeCanbePlaced -= CheckIfShapeCanBePlaced;
    }

    private void OnEnable()
    {
        GameEvent.CheckIfShapeCanbePlaced += CheckIfShapeCanBePlaced;
    }
    private void SpawGridSquares()
    {
        int square_index = 0;

        for (var row =0; row < rows; row++)
        {
            for(var column = 0; column<columns; column++)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(square_index % 2 == 0);

                //_gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().Index = square_index;
                square_index++;
            }    
        }    
    }    

    private void SetGridSquarePositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                //to next col
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);
            
            if (column_number > 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }    

            if(row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0.0f);
            
            column_number++;
        }

    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
                //gridSquare.ActivateSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null)
            return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }

            }

            if (shapeLeft == 0)
            {
                GameEvent.RequestNewShapes();
            }
            else
            {
                GameEvent.SetShapeInactive();
            }
            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvent.MoveShapeToStartPosition();
        }
        
    }
    
    


    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //col
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
            
        }
        

        //row
        for (int row = 0; row < 8; row++)
        {
            List<int> data = new List<int>(8);
            for (var index = 0; index < 8; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }



        var completedLine= CheckIfSquaresAreCompleted(lines);

        if (completedLine > 2)
        {
            //bonus
        }

        //score
        var totalScore = 10 * completedLine;
        GameEvent.AddScore(totalScore);

        CheckIfPlayerLost();
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;

        
        foreach (var line in data)
        {
            
            

            //var lineCompleted = false;
            var n = 0;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == true)
                {
                    n++;

                }
                if (n==8)
                {
                    completedLines.Add(line);
                }
            }     
        }
        Debug.Log(completedLines.Count);
        foreach (var line in completedLines)
        {
            ouputArray(line);
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                completed = true;
            }
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
                
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CreateGrid()
    {
        SpawGridSquares();
        SetGridSquarePositions();

    }    
    // Start is called before the first frame update
   
    void ouputArray(int[] a)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < a.Length; i++)
        {
            sb.Append(a[i]);
            sb.Append(" ");
        }
        Debug.Log(sb.ToString());
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;
        for (var index =0; index< shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            var isShapeOnStartPos = shapeStorage.shapeList[index].IsOnStartPosition();
            if (isShapeActive && isShapeOnStartPos)
                if (CheckIdShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]))
                {
                    shapeStorage.shapeList[index]?.ActivateShape();
                    validShapes++;
                }
        }
        if (validShapes == 0)
        {
            GameEvent.GameOver(false);
            Debug.Log("GAME OVER");
        }
    }

    private bool CheckIdShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        //All indexes of filled up squares

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        Debug.Log("Check Shape");

        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                    Debug.Log("Index Check: " + squareIndex);
                }
                squareIndex++;
                
            }
        }

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Number of Filled up Square are not the same as the original shape have");
        }

        var squareList = GetAllSquareCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied == true)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }

    private List<int[]> GetAllSquareCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 8)
        {
            var rowData = new List<int>();
            for (var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (int column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns - 1) >= 8)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }

        }
        return squareList;
    }
    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
