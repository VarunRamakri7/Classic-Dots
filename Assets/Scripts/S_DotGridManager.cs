using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        //MakeDotsInAllColumnsFallToLowest();
        RenameGrid();
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

        RefillColumn(column);
    }

    /// <summary>
    /// Move dots in all rows down to lowest
    /// TODO: Fix
    /// </summary>
    public void MakeDotsInAllColumnsFallToLowest()
    {
        // Iterate through in reverse
        for (int j = 0; j < gridSize[1]; j++)
        {
            int lowest = GetLowestEmptyRowInColumn(j); // Get the lowest empty cell
            if (lowest > 0)
            {
                for (int i = GetLowestFilledRowInColumn(j); i >= 0; i--)
                {
                    if (i < gridSize[0])
                    {
                        MoveDot(/*i, j, */lowest, j, i, j); // Move dot
                        lowest--; // Decrease lowest cell
                    }
                }
            }

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

    /// <summary>
    /// Get the lowest empty cell in the given column
    /// </summary>
    /// <param name="column">Column number to check</param>
    /// <returns></returns>
    public int GetLowestEmptyRowInColumn(int column)
    {
        int lowest = 0;

        // Iterate through each row in the column
        for (int i = 0; i < gridSize[0]; i++)
        {
            // Check occupation status
            if (!dotGrid[i][column].IsOccupied())
            {
                lowest++; // Increment if it is unoccupied
            }
        }

        //Debug.Log(string.Format("Lowest empty: ({0}, {1})", lowest, column));

        return lowest;
    }

    /// <summary>
    /// Get the lowest occupied cell in the given column
    /// </summary>
    /// <param name="column">Column number to check</param>
    /// <returns></returns>
    public int GetLowestFilledRowInColumn(int column)
    {
        int lowest = 0;

        // Iterate through each row in the column
        for (int i = 0; i < gridSize[0]; i++)
        {
            // Check occupation status
            if (dotGrid[i][column].IsOccupied())
            {
                lowest++; // Increment if it is occupied
            }
        }

        //Debug.Log(string.Format("Lowest occupied: ({0}, {1})", lowest, column));

        return lowest;
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

        if (i > 1)
        {
            StartCoroutine(AnimateDot(dotGrid[i - 1][j].GetDot(), dotGrid[i][j].GetPosition()));
        }

        //RefillColumn(j);
        //MoveDotsDownByOne(i, j);        
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
            //Debug.Log(string.Format("Moving from: ({0}, {1}) to ({2}, {3})", i, j, u, v));
    
            //StartCoroutine(AnimateDot(dotGrid[i][j].GetDot(), dotGrid[u][v].GetPosition()));

            dotGrid[u][v].SetDot(dotGrid[i][j].GetDot()); // Copy over GameObject
            dotGrid[u][v].SetColor(dotGrid[i][j].GetColor()); // Copy overcolor
            dotGrid[u][v].SetOccupied(true); // Set occupation status

            dotGrid[i][j].SetDot(new GameObject());
            dotGrid[i][j].SetOccupied(false);
        }
    }

    /// <summary>
    /// Animate the dot to move from it's current position to the destination
    /// </summary>
    /// <param name="dot">Dot to move</param>
    /// <param name="destination">Destination position</param>
    /// <returns></returns>
    public IEnumerator AnimateDot(GameObject dot, Vector3 destination)
    {
        if (dot != null) // Check if dot exists
        {
            float totalMovementTime = 2.0f; // Total time of animation
            float currentMovementTime = 0.0f; // Amount of time that has passed
            Vector3 curPos = dot.transform.position;

            Debug.Log("Dropping: " + dot.name);

            // Move dot to new location
            while (dot != null && Vector3.Distance(dot.transform.position, destination) > 0.0f)
            {
                currentMovementTime += Time.deltaTime;
                dot.transform.position = Vector3.Lerp(curPos, destination, currentMovementTime / totalMovementTime);
                yield return new WaitForSeconds(0.001f);
            }
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
        int[] index = { (int)char.GetNumericValue(trimmed[0]), (int)char.GetNumericValue(trimmed[2]) }; // Get numeric value from chars
        // Debug.Log("After trim: " + trimmed);
        // Debug.Log("Index: (" + index[0] + ", " + index[1] + ")");

        return index;
    }

    #endregion
}
