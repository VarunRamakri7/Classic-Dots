using System.Collections.Generic;
using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager gameManager;

    [SerializeField]
    private S_DotGridManager gridManager;
    [SerializeField]
    private S_ConnectDots connectionManager;

    private Camera mainCamera;
    private List<int[]> dotsIndices;
    private List<string> dotNames;
    private bool squareMade;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }

        mainCamera = Camera.main; // Get main camera
        dotsIndices = new List<int[]>(); // Initialize List of indices
        dotNames = new List<string>(); // // Initialize list of names
        squareMade = false;
    }

    private void Update()
    {
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
                    int[] index = gridManager.GetIndexOfDot(hit.transform.gameObject.name);
                    //Debug.Log(string.Format("Index: ({0}, {1})", index[0], index[1]));

                    dotsIndices.Add(index); // Add dot index to list
                    dotNames.Add(hit.transform.gameObject.name); // Add name to list

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

            if (Physics.Raycast(ray, out hit) && !squareMade)
            {
                // Check if a dot is hit and see if this is already part of the line unless it's a square
                if (hit.transform.gameObject.tag == "dot" && (!dotNames.Contains(hit.transform.gameObject.name) || IsSquare(hit.transform.gameObject.name)))
                {
                    // Debug.Log("Drag Hit: " + hit.transform.gameObject.name);

                    // Compare colors of dots
                    Color hitDotColor = hit.transform.gameObject.GetComponent<Renderer>().material.color; // Get color of new dot
                    Color lineColor = gridManager.GetDotColor(dotsIndices[0][0], dotsIndices[0][1]); // Get color of first dot

                    if (lineColor.Equals(hitDotColor))
                    {
                        //Debug.Log("Same Color");

                        // Add unique indices
                        int[] index = gridManager.GetIndexOfDot(hit.transform.gameObject.name);
                        //Debug.Log(string.Format("Index: ({0}, {1}) is in list: {2}", index[0], index[1], dotsIndices.Contains(index)));

                        if (CanConnectToCurrentDot(index)) // Check if new dot can connect to old dot
                        {
                            //Debug.Log("Can connect");
                            dotsIndices.Add(index); // Add dot index to list
                            dotNames.Add(hit.transform.gameObject.name); // Add name to list

                            connectionManager.SetPoint(dotsIndices.Count - 1, gridManager.GetDotAt(index[0], index[1]).transform.position); // Set new dot as last point
                            connectionManager.AddPoint(mousePos); // Add mouse position to line

                            squareMade = IsSquare(hit.transform.gameObject.name);
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
            if (connectionManager != null)
            {
                connectionManager.EmptyLine(); // Erase Line
            }

            // Destroy dots added to list
            if (dotsIndices.Count > 1)
            {
                //Debug.Log("Removing dots");

                // Remove last element from lists if square is made
                if (squareMade)
                {
                    // Remove repeated entry
                    dotsIndices.RemoveAt(dotsIndices.Count - 1);
                    dotNames.RemoveAt(dotNames.Count - 1);
                }

                // Iterate through indices and remove all connected dots in reverse order
                Color dotsColor = gridManager.GetDotColor(dotsIndices[0][0], dotsIndices[0][1]);
                for (int i = 0; i < dotsIndices.Count; i++)
                {
                    gridManager.RemoveDot(dotsIndices[i][0], dotsIndices[i][1]); // Remove dot
                }
                gridManager.RepopulateGrid(squareMade, dotsColor); // Spawn new dots
            }

            dotsIndices = new List<int[]>(); // Empty dots indices
            dotNames = new List<string>(); // Empty names

            squareMade = false; // Reset to false
        }
    }

    /// <summary>
    /// Check if the new dot is above, below, or beside the current dot and if a square has already been made
    /// </summary>
    /// <param name="index">Index of new dot</param>
    /// <returns>True if the new dot is in a valid position</returns>
    public bool CanConnectToCurrentDot(int[] index)
    {
        bool canConnect = false;

        int[] currentIndex = dotsIndices[dotsIndices.Count - 1];

        if (!squareMade &&
            (index[0] == (currentIndex[0] - 1)) && (index[1] == currentIndex[1]) || // Above
            (index[0] == (currentIndex[0] + 1)) && (index[1] == currentIndex[1]) || // Below
            (index[0] == (currentIndex[0])) && (index[1] == currentIndex[1] - 1) || // Left
            (index[0] == (currentIndex[0])) && (index[1] == currentIndex[1] + 1)) // Right
        {
            //Debug.Log("Can Connect");
            canConnect = true;
        }

        return canConnect;
    }

    /// <summary>
    /// Check if the user has made a square
    /// </summary>
    /// <param name="name">Name of the new dot</param>
    /// <returns>True or false depending on whether a square has been made</returns>
    public bool IsSquare(string name)
    {
        bool isSquare = false;

        if (dotNames[0].Equals(name) && dotNames.Count >= 4)
        {
            //Debug.Log("Square made");
            isSquare = true;
        }

        return isSquare;
    }
}
