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

    // Make grid of dots
    public void MakeGrid()
    {
        // Iterate through all rows and columns and instantiate a dot in each cell
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {                
                Vector3 position = new Vector3(startPos.x + (i * offset), startPos.y, startPos.z + (j * offset));
                GameObject dot = Instantiate(prefab, position, Quaternion.identity);
                dot.transform.parent = gridParent.transform;

                dotGrid[i][j] = new S_Dot(dot, position, true);
            }
        }
    }

    public List<S_Dot> GetEmptyCells()
    {
        List<S_Dot> emptyDots = new List<S_Dot>();

        // Iterate through grid to find  all empty locations
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                if (dotGrid[i][j].IsOccupied())
                {
                    emptyDots.Add(dotGrid[i][j]);
                }
            }
        }

        return emptyDots;
    }

    public S_Dot GetDot(int i, int j)
    {
        return dotGrid[i][j];
    }
}
