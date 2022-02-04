﻿using System.Collections;
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
        // Iterate through matrix
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                // Check if this is an empty cell
                if (!dotGrid[i][j].IsOccupied())
                {
                    //Debug.Log(string.Format("Empty at: ({0}, {1})", i, j));

                    MoveDotsDownByOne(i, j); // Move all dots above this cell down by one
                }
            }
        }

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
    /// Move dots in this column to lowest cell
    /// </summary>
    /// <param name="column">Column to check</param>
    public void MoveDotsToLowest(int column)
    {
        // Iterate through column and move dots down to lowest
        for (int i = gridSize[1] - 1; i >= 0; i--)
        {
            int row = GetLowestEmptyRowInColumn(column);
            Debug.Log(string.Format("Lowest: ({0}, {1})", i, column));

            // Check if current row is above lowest low
            if (i > row)
            {
                MoveDot(i, column, row, column);
            }
        }

        RefillColumn(column);
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
                dotGrid[i][j].GetDot().name = "dot(" + i + "," + j + ")";
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

        //Debug.Log(string.Format("Lowest: ({0}, {1})", lowest, column));

        return lowest;
    }

    #region Dot Related
    /// <summary>
    /// Removes the dot GameObject at the given index
    /// </summary>
    /// <param name="i">Column number</param>
    /// <param name="j">Row number</param>
    /*public void RemoveDot(int i, int j)
    {
        MakeDotsFall(i, j);

        //Destroy(dotGrid[i][j].GetDot()); // Destory GameObject
        //dotGrid[i][j].SetOccupied(false); // Set occupation of cell to false    
    }*/

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
        //Debug.Log(string.Format("Moving from: ({0}, {1}) to ({2}, {3})", i, j, u, v));

        dotGrid[u][v].SetDot(dotGrid[i][j].GetDot()); // Copy over GameObject
        dotGrid[u][v].SetColor(dotGrid[i][j].GetColor()); // Copy overcolor
        dotGrid[u][v].SetOccupied(true); // Set occupation status

        dotGrid[i][j].SetDot(new GameObject());
        dotGrid[i][j].SetOccupied(false);
    }

    /// <summary>
    /// Make all dots above (i,j) fall down by one place
    /// </summary>
    /// <param name="row">Empty cell row number</param>
    /// <param name="column">Empty cell column number</param>
    public void MakeDotsFall(int row, int column)
    {
        dotGrid[row][column].SetOccupied(false); // Set occupation of cell to false    

        // Make dots in this column fall down to the lowest row
        int lowest = GetLowestEmptyRowInColumn(column); // Get lowest index
        for (int i = row; i > 0; i--)
        {
            StartCoroutine(AnimateDot(dotGrid[i - 1][column].GetDot(), dotGrid[lowest][column].GetPosition())); // Make dot fall to lowest
            lowest = i - 1; // Reset lowest
        }

        Destroy(dotGrid[row][column].GetDot()); // Destroy gameobject at empty cell
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

            // Move dot to new location
            while (dot != null && Vector3.Distance(dot.transform.position, destination) > 0.0f)
            {
                //Debug.Log("Animating");
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

    #endregion
}
