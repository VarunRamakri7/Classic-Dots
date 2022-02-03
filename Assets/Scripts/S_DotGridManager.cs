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
            dotGrid[i] = new S_Dot[gridSize[1]];
            for (int j = 0; j < gridSize[1]; j++)
            {
                dotGrid[i][j] = new S_Dot();
            }
        }
    }

    /// <summary>
    /// Make grid of dots
    /// </summary>
    public void MakeGrid()
    {
        // Iterate through all rows and columns and instantiate a dot in each cell
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                // Make new dot
                Vector3 position = new Vector3(startPos.x + (i * offset), startPos.y, startPos.z + (j * offset)); // Set position while accounting for offset
                GameObject dot = Instantiate(prefab, position, Quaternion.identity); // Instantiate the prefab
                dot.name = "dot(" + i + "," + j + ")";
                dot.transform.parent = gridParent.transform; // Set parent
                int color = Random.Range(0, 3); // Set color

                dotGrid[i][j] = new S_Dot(dot, position, color, true); // Add dot to grid
            }
        }
    }

    /// <summary>
    ///  Get a boolean grid where value reflects ooccupation status
    /// </summary>
    /// <returns>2D Array of booleans</returns>
    public bool[][] GetEmptyCells()
    {
        bool[][] indices = new bool[gridSize[0]][];

        // Iterate through grid to find all empty locations
        for (int i = 0; i < gridSize[0]; i++)
        {
            indices[i] = new bool[gridSize[1]];
            for (int j = 0; j < gridSize[1]; j++)
            {
                if (dotGrid[i][j].IsOccupied())
                {
                    indices[i][j] = true;
                }
            }
        }

        return indices;
    }

    /// <summary>
    /// Fill grid with dots again
    /// </summary>
    public void RepopulateGrid()
    {
        bool[][] filledDots = GetEmptyCells();

        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                if (!filledDots[i][j])
                {
                    //Debug.Log(string.Format("Empty at: ({0}, {1})", i, j));

                    MoveDotsDownByOne(i, j);
                }
            }
        }

        // Iterate through top row and refill
        /*for (int j = 0; j < gridSize[1]; j++)
        {
            if (!dotGrid[0][j].IsOccupied())
            {
                GameObject dot = Instantiate(prefab, dotGrid[0][j].GetPosition(), Quaternion.identity); // Instantiate the prefab
                dot.name = "dot(0," + j + ")";
                dot.transform.parent = gridParent.transform; // Set parent
                int color = Random.Range(0, 3); // Set color

                dotGrid[0][j] = new S_Dot(dot, dotGrid[0][j].GetPosition(), color, true); // Add dot to grid

            }
        }*/
    }

    /// <summary>
    /// Move dots in this colomn down by one element until row
    /// </summary>
    /// <param name="row">Row with an empty cell</param>
    /// <param name="column">Column with an empty cell</param>
    public void MoveDotsDownByOne(int row, int column)
    {
        // Iterate through column and move dots down by one
        for (int i = row; i > 0; i--)
        {
            MoveDot(i - 1, column, i, column);
        }
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
        //Debug.Log(string.Format("Moving from: ({0}, {1}) to ({2}, {3})", i, j, u, v));

        dotGrid[u][v].SetDot(dotGrid[i][j].GetDot()); // Copy over GameObject
        dotGrid[u][v].SetColor(dotGrid[i][j].GetColor()); // Copy overcolor
        dotGrid[u][v].SetOccupied(true); // Set occupation status
        dotGrid[i][j].SetDot(new GameObject());
        dotGrid[i][j].SetOccupied(false);
    }

    /// <summary>
    /// Removes the dot GameObject at the given index
    /// </summary>
    /// <param name="i">Column number</param>
    /// <param name="j">Row number</param>
    public void RemoveDot(int i, int j)
    {
        Destroy(dotGrid[i][j].GetDot()); // Destory GameObject
        dotGrid[i][j].SetOccupied(false); // Set occupation of cell to false
    }

    /// <summary>
    /// Get dot at given index
    /// </summary>
    /// <param name="i">Row number</param>
    /// <param name="j">Column number</param>
    /// <returns>Returns dot at given location</returns>
    public S_Dot GetDot(int i, int j)
    {
        return dotGrid[i][j];
    }
}
