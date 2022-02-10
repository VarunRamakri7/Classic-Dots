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

                // Set next position for previous dot
                if (i > 0 && j > 0)
                {
                    dotGrid[i - 1][j - 1].SetNextPosition(dotGrid[i][j].GetPosition());
                }
            }
        }

        RenameGrid(); // Set all names
    }

    /// <summary>
    /// Fill grid with dots again
    /// </summary>
    public void RepopulateGrid()
    {
        //Debug.Log("Repopulating grid");

        // TODO: Make dots fall to make up for space
        MakeDotsFall();

        //RefillAllColumns();
        RenameGrid();
    }

    /// <summary>
    /// Move dots in this colomn down by one element until empty row
    /// </summary>
    /// <param name="row">Row with an empty cell</param>
    /// <param name="column">Column with an empty cell</param>
    public void MoveDotsDownByOne(int row, int column)
    {
        // Iterate through column and move dots down by one
        for (int i = row; i > 0; i--)
        {
            //MoveDot(i - 1, column, i, column);

            // Check occupation for cell below this cell
            if ((i+ 1) < gridSize[1] && !dotGrid[i + 1][column].IsOccupied())
            {
                MoveDot(i, column);
            }
        }

        RefillColumn(column);
    }

    /// <summary>
    /// Make the dots in the grid fall and fill the empty space
    /// Reference: https://stackoverflow.com/questions/55091008/fall-down-elements-in-2d-array
    /// TODO: Fix
    /// </summary>
    public void MakeDotsFall()
    {
        // Iterate through each column
        for (int j = 0; j < gridSize[1]; j++)
        {
            //List<GameObject> dotsColumn = new List<GameObject>();

            // Iterate through column in reverse
            for (int i = gridSize[0]; i > 0; i--)
            {
                // Check if the cell below is empty
                if ((i + 1) < gridSize[0] &&!dotGrid[i + 1][j].IsOccupied())
                {
                    MoveDot(i + 1, j, i, j); // Move dot down by one
                }
            }
        }
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
                    dotGrid[i][j].GetDot().name = "dot(" + i + "," + j + ")";
                }
            }
        }

    }

    #region Dot Related
    /// <summary>
    /// Removes the dot GameObject at the given index
    /// </summary>
    /// <param name="i">Column number</param>
    /// <param name="j">Row number</param>
    public void RemoveDot(int i, int j)
    {
        dotGrid[i][j].SetOccupied(false); // Set occupation of cell to false    
        Destroy(dotGrid[i][j].GetDot()); // Destory GameObject

        //MoveDotsDownByOne(i, j);

        //Debug.Log(string.Format("Removed: ({0}, {1})", i, j));
    }

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
