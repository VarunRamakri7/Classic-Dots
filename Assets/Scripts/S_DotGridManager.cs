using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DotGridManager : MonoBehaviour
{
    public static S_DotGridManager dotGridManager;

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject gridParent;
    [SerializeField]
    private Vector3 startPos;
    [SerializeField]
    private int[] gridSize;
    [SerializeField]
    private float offset = 2.5f;

    private char[] trimChar = { 'd', 'o', 't', '(', ')' };
    private S_Dot[][] dotGrid;

    private void Start()
    {
        if (dotGridManager == null)
        {
            dotGridManager = this;
        }

        // Initialize dots grid
        dotGrid = new S_Dot[gridSize[0]][];
        for (int i = 0; i < gridSize[0]; i++)
        {
            dotGrid[i] = new S_Dot[gridSize[1]]; // Initialize empty array
            for (int j = 0; j < gridSize[1]; j++)
            {
                dotGrid[i][j] = new S_Dot(); // Add empty dot
            }
        }

        FillGrid();
    }

    /// <summary>
    /// Fill grid with dots
    /// </summary>
    public void FillGrid()
    {
        // Iterate through all rows and columns and instantiate a dot in each cell
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                Vector3 position = new Vector3(startPos.x + (i * offset), startPos.y, startPos.z + (j * offset)); // Set position while accounting for offset
                dotGrid[i][j] = MakeNewDot(position); // Make new dot
                dotGrid[i][j].SetNextPosition(position); // Initialize next position as current position
            }
        }

        RenameGrid(); // Set all names
        SetNextPositions(); // Set next positions
    }

    /// <summary>
    /// Set the next positions of all the dots
    /// </summary>
    public void SetNextPositions()
    {
        for (int j = 0; j < gridSize[1]; j++)
        {
            for (int i = 0; i < gridSize[0]; i++)
            {
                int nextIndex = i + 1;

                if (nextIndex < gridSize[0])
                {
                    dotGrid[i][j].SetNextPosition(dotGrid[nextIndex][j].GetPosition());
                }
            }
        }
    }

    /// <summary>
    /// Fill grid with dots again
    /// </summary>
    public void RepopulateGrid(bool isSquare, Color squareColor)
    {
        //Debug.Log("Repopulating grid");
        //Debug.Log("Square Made: " + isSquare);

        // Check if square has been made
        if (isSquare)
        {
            //Debug.Log("Square color: " + squareColor);
            DestroyAllDotsOfColor(squareColor);
        }

        MakeDotsFall(); // Dots fall to empty space
        RefillAllColumns(); // Refill free space
        RenameGrid(); // Rename grid
        SetNextPositions(); // Reset next positions
    }

    /// <summary>
    /// Make the dots in the grid fall and fill the empty space
    /// </summary>
    public void MakeDotsFall()
    {
        // Iterate through each column
        for (int j = 0; j < gridSize[1]; j++)
        {
            if (ColumnHasEmptyCell(j))
            {
                //Debug.Log("Empty Cell in: " + j);

                List<GameObject> dotsColumn = new List<GameObject>();

                // Get all dots in column
                for (int i = 0; i < gridSize[0]; i++)
                {
                    // Make temporary dot
                    // GameObject temp = MakeNewDot(dotGrid[i][j].GetPosition()).GetDot();

                    if (dotGrid[i][j].IsOccupied())
                    {
                        //Debug.Log(string.Format("Taking at: ({0}, {1})", i, j));

                        dotsColumn.Add(dotGrid[i][j].GetDot());
                        dotGrid[i][j].SetOccupied(false);
                    }
                }

                dotsColumn.Reverse(); // Reverse the list of dots

                // Drop all dots in list
                /*foreach(GameObject dot in dotsColumn)
                {
                    int[] dotIndex = GetIndexOfDot(dot.name);

                    StartCoroutine(WaitForDotDrops(dotIndex[0], dotIndex[1]));
                }*/

                // Refill column with dots
                int index = 0;
                for (int i = gridSize[0] - 1; i >= (gridSize[0] - dotsColumn.Count); i--)
                {
                    //Debug.Log(string.Format("Adding at: ({0}, {1})", i, j));

                    dotGrid[i][j].SetDot(dotsColumn[index++]);
                    dotGrid[i][j].SetOccupied(true);
                }
            }
        }
    }


    /// <summary>
    /// Check if the column has an empty cell
    /// </summary>
    /// <param name="column">Column to check</param>
    /// <returns>True if there's at least one empty cell, false if zero empty space</returns>
    public bool ColumnHasEmptyCell(int column)
    {
        bool hasSpace = false;

         for (int i = 0; i < gridSize[0]; i++)
        {
            if (!dotGrid[i][column].IsOccupied())
            {
                hasSpace = true;
            }
        }

        return hasSpace;
    }

    /// <summary>
    /// Refill all columns
    /// </summary>
    public void RefillAllColumns()
    {
        // Iterate through all columns and refill them
        for (int j = 0; j < gridSize[1]; j++)
        {
            RefillColumn(j);
        }
    }

    /// <summary>
    /// Refill empty cells in this column
    /// </summary>
    /// <param name="column">Column with an empty cell</param>
    public void RefillColumn(int column)
    {
        // Iterate through column and refill empty cells
        for (int i = 0; i < gridSize[0]; i++)
        {
            if (!dotGrid[i][column].IsOccupied())
            {
                dotGrid[i][column] = MakeNewDot(dotGrid[i][column].GetPosition());
            }
        }
    }

    /// <summary>
    /// Destroy all dots of the given color
    /// </summary>
    /// <param name="color">Color of dots to destroy</param>
    public void DestroyAllDotsOfColor(Color color)
    {
        //Debug.Log("Destroying all dots of color: " + color);

        // Iterate through all rows
        for (int i = 0; i < gridSize[0]; i++)
        {
            // Iterate through all columns
            for (int j = 0; j < gridSize[1]; j++)
            {
                Color temp = GetDotColor(i, j); // Get color of current dot

                //Debug.Log("Color match: " + temp.Equals(color));

                // Check if color of dot matches color of square
                if (dotGrid[i][j].IsOccupied() && temp.Equals(color))
                {
                    //Debug.Log(string.Format("Removing: ({0}, {1})", i, j));

                    RemoveDot(i, j); // Remove dot if colors match
                }
            }
        }
    }

    /// <summary>
    /// Rename all dots in the grid
    /// </summary>
    public void RenameGrid()
    {
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                if (dotGrid[i][j].GetDot() != null)
                {
                    dotGrid[i][j].GetDot().name = "dot(" + i + "," + j + ")"; // Add index to name in correct format
                }
            }
        }

    }

    /// <summary>
    /// Coroutine to wait for dot dropping coroutine to finish
    /// </summary>
    /// <param name="row">Row number of dot to drop</param>
    /// <param name="column">Column number of dot to drop</param>
    /// <returns></returns>
    public IEnumerator WaitForDotDrops(int row, int column)
    {
        Debug.Log(string.Format("Dropping: ({0}, {1})", row, column));

        yield return StartCoroutine(dotGrid[row][column].DropDot());
    }

    /// <summary>
    /// Coroutine that waits for seconds
    /// </summary>
    /// <param name="seconds">Seconds to wait for</param>
    /// <returns></returns>
    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    #region Dot Related
    /// <summary>
    /// Make a new dot at this position
    /// </summary>
    /// <param name="position">Position of this dot</param>
    /// <returns></returns>
    public S_Dot MakeNewDot(Vector3 position)
    {
        GameObject dot = Instantiate(prefab, position, Quaternion.identity); // Instantiate the prefab
        dot.transform.parent = gridParent.transform; // Set parent
        int color = Random.Range(0, 3); // Set color

        return new S_Dot(dot, position, color, true);
    }

    /// <summary>
    /// Move dot from (i,j) to (u,v).
    /// </summary>
    /// <param name="i">Current column number</param>
    /// <param name="j">Current row number</param>
    /// <param name="u">Next column number</param>
    /// <param name="v">Next row number</param>
    public void MoveDot(int i, int j, int u, int v)
    {
        if ((i >= 0 && i < gridSize[0]) && (j >= 0 && j < gridSize[1]) && (u >= 0 && u < gridSize[0]) && (v >= 0 && v < gridSize[1]) && (i != u))
        {
            Debug.Log(string.Format("Moving from: ({0}, {1}) to ({2}, {3})", i, j, u, v));
    
            //StartCoroutine(AnimateDot(dotGrid[i][j].GetDot(), dotGrid[u][v].GetPosition()));

            dotGrid[u][v].SetDot(dotGrid[i][j].GetDot()); // Copy over GameObject
            dotGrid[u][v].SetColor(dotGrid[i][j].GetColor()); // Copy overcolor
            dotGrid[u][v].SetOccupied(true); // Set occupation status

            dotGrid[i][j].SetDot(new GameObject());
            dotGrid[i][j].SetOccupied(false);
        }
    }

    /// <summary>
    /// Removes the dot GameObject at the given index
    /// </summary>
    /// <param name="i">Column number</param>
    /// <param name="j">Row number</param>
    public void RemoveDot(int i, int j)
    {
        dotGrid[i][j].SetOccupied(false); // Set occupation of cell to false
        Destroy(dotGrid[i][j].GetDot()); // Destory GameObject

        //Debug.Log(string.Format("Removed: ({0}, {1})", i, j));
    }

    /// <summary>
    /// Get the dot at the given location
    /// </summary>
    /// <param name="row">Row number of dot to get</param>
    /// <param name="column">Column number of dot to get</param>
    /// <returns></returns>
    public GameObject GetDotAt(int row, int column)
    {
        //if ((row >= 0 && row < gridSize[0]) && (column >= 0 && column < gridSize[1]))
        return dotGrid[row][column].GetDot();
    }

    /// <summary>
    /// Get the color of the dot at the given index
    /// </summary>
    /// <param name="row">Row of dot</param>
    /// <param name="column">Column of dot</param>
    /// <returns></returns>
    public Color GetDotColor(int row, int column)
    {
        return GetDotAt(row, column).GetComponent<Renderer>().material.color;
    }

    /// <summary>
    /// Get index of the dot from its name
    /// </summary>
    /// <param name="dotName">Name of dot</param>
    /// <returns>Index of dot</returns>
    public int[] GetIndexOfDot(string dotName)
    {
        string trimmed = dotName.Trim(trimChar); // Remove all extra chars except ','
        //Debug.Log("After trim: " + trimmed);
        string[] split = trimmed.Split(',');
        int[] index = { int.Parse(split[0]), int.Parse(split[1]) }; // Get numeric value from chars
        //Debug.Log("Got Index: (" + index[0] + ", " + index[1] + ")");

        return index;
    }

    #endregion
}
