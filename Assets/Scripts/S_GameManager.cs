using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager gameManager;

    [SerializeField]
    private S_DotGridManager gridManager;
    [SerializeField]
    private S_ConnectDots connectionManager;

    private char[] trimChar = { 'd', 'o', 't', '(', ')' };
    private Camera mainCamera;
    private List<int[]> dotsIndices;
    private string lastDotName;
    private bool squareMade;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }

        mainCamera = Camera.main; // Get main camera
        dotsIndices = new List<int[]>(); // Initialize array of indices
        lastDotName = "";
        squareMade = false;
    }

    private void Update()
    {
        CheckKeyPress();

        CheckMousePress();
        CheckMouseHold();
        CheckMouseRelease();
    }

    /// <summary>
    /// Check Mouse right click
    /// </summary>
    private void CheckMousePress()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if a dot is hit
                if (hit.transform.gameObject.tag == "dot")
                {
                    //Debug.Log("Hit: " + hit.transform.gameObject.name);
                    
                    // Add unique indices
                    int[] index = GetIndexOfDot(hit.transform.gameObject.name);
                    if (!dotsIndices.Contains(index))
                    {
                        dotsIndices.Add(index); // Add dot index to list
                        lastDotName = hit.transform.gameObject.name;
                    }

                    connectionManager.SetLineColor(hit.transform.gameObject.GetComponent<Renderer>().material.color); // Change line color
                    connectionManager.AddPoint(gridManager.GetDotAt(index[0], index[1]).transform.position); // Add position of GameObject to line
                }
            }
        }
    }

    /// <summary>
    /// Check if mouse is being held
    /// </summary>
    public void CheckMouseHold()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition); // Get mouse position

        if (Input.GetMouseButton(0))
        {
            // Add mouse position to line if this is the first press
            if (connectionManager.GetLength() == 1)
            {
                connectionManager.AddPoint(mousePos); // Add mouse position as new point
            }

            connectionManager.SetPoint(dotsIndices.Count, mousePos); // Change end position to mouse position

            // Check if line hits another dot
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if a dot is hit
                if (hit.transform.gameObject.tag == "dot")
                {
                    // Debug.Log("Drag Hit: " + hit.transform.gameObject.name);

                    // Compare colors of dots
                    Color hitDotColor = hit.transform.gameObject.GetComponent<Renderer>().material.color; // Get color of new dot
                    Color lineColor = gridManager.GetDotColor(dotsIndices[0][0], dotsIndices[0][1]); // Get color of first dot
                    if (lineColor.Equals(hitDotColor) && lastDotName != hit.transform.gameObject.name)
                    {
                        Debug.Log("Same Color");

                        // Add unique indices
                        int[] index = GetIndexOfDot(hit.transform.gameObject.name);
                        if (CanConnectToCurrentDot(index)) // Check if new dot can connect to new dot
                        {
                            Debug.Log("Can connect");

                            if (!dotsIndices.Contains(index))
                            {
                                //Debug.Log(string.Format("Adding: ({0}, {1})", index[0], index[1]));
                                dotsIndices.Add(index); // Add dot index to list
                                lastDotName = hit.transform.gameObject.name; // Update last dot name
                            }

                            connectionManager.SetPoint(dotsIndices.Count, gridManager.GetDotAt(index[0], index[1]).transform.position); // Set last point as new dot

                            connectionManager.AddPoint(mousePos); // Add mouse position to line
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if mouse is released
    /// </summary>
    public void CheckMouseRelease()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // TODO: Destroy dots added to list and erase line

            connectionManager.EmptyLine(); // Erase Line
            dotsIndices = new List<int[]>(); // Empty dots indices
        }
    }

    /// <summary>
    /// Check keyboard press
    /// </summary>
    public void CheckKeyPress()
    {
        // Destroy dots
        if(Input.GetKeyDown(KeyCode.D))
        {
            if (dotsIndices.Count > 1)
            {
                connectionManager.EmptyLine(); // Erase line

                // Iterate through indices and remove all connected dots
                for (int i = 0; i < dotsIndices.Count; i++)
                {
                    gridManager.RemoveDot(dotsIndices[i][0], dotsIndices[i][1]); // Remove all dots
                }
                dotsIndices = new List<int[]>(); // Reset list
            }
        }

        // Spawn new dots
        if (Input.GetKeyDown(KeyCode.R))
        {
            gridManager.RepopulateGrid();
        }
    }

    /// <summary>
    /// Get index of the dot from its name
    /// </summary>
    /// <param name="dotName">Name of dot</param>
    /// <returns>Index of dot</returns>
    public int[] GetIndexOfDot(string dotName)
    {
        string trimmed = dotName.Trim(trimChar); // Remove all extra chars except ','
        // Debug.Log("After trim: " + trimmed);

        int[] index = { (int)char.GetNumericValue(trimmed[0]), (int)char.GetNumericValue(trimmed[2]) }; // Get numeric value from chars
        // Debug.Log("Index: (" + index[0] + ", " + index[1] + ")");

        return index;
    }

    /// <summary>
    /// Check if the new dot is above, below, or beside the current dot
    /// </summary>
    /// <param name="index">Index of new dot</param>
    /// <returns>True if the new dot is in a valid position</returns>
    public bool CanConnectToCurrentDot(int[] index)
    {
        bool canConnect = false;

        int[] currentIndex = dotsIndices[dotsIndices.Count - 1];

        if ((index[0] == (currentIndex[0] - 1)) && (index[1] == currentIndex[1]) || // Above
            (index[0] == (currentIndex[0] + 1)) && (index[1] == currentIndex[1]) || // Below
            (index[0] == (currentIndex[0])) && (index[1] == currentIndex[1] - 1) || // Left
            (index[0] == (currentIndex[0])) && (index[1] == currentIndex[1] + 1)) // Right
        {
            //Debug.Log("Can Connect");
            canConnect = true;
        }

        return canConnect;
    }
}
