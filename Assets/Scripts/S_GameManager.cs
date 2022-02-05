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
    private List<int[]> dotsIndices;
    private bool squareMade;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }

        dotsIndices = new List<int[]>();
        squareMade = false;
    }

    private void Update()
    {
        CheckKeyPress();

        CheckMousePress();
        //CheckMouseHold();
    }

    /// <summary>
    /// Check Mouse right click
    /// </summary>
    private void CheckMousePress()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if a dot is hit
                if (hit.transform.gameObject.tag == "dot")
                {
                    //Debug.Log("Hit: " + hit.transform.gameObject.name);

                    // Get index of dot
                    string dotName = hit.transform.gameObject.name;
                    //Debug.Log("Name: " + dotName);

                    string trimmed = dotName.Trim(trimChar); // Remove all extra chars except ','
                    //Debug.Log("After trim: " + trimmed);

                    int[] index = { (int)char.GetNumericValue(trimmed[0]), (int)char.GetNumericValue(trimmed[2]) }; // Get numeric value from chars
                    //Debug.Log("Index: (" + index[0] + ", " + index[1] + ")");

                    // TODO: Check color of new dot

                    // Add unique indices
                    if (!dotsIndices.Contains(index))
                    {
                        //Debug.Log(string.Format("Adding: ({0}, {1})", index[0], index[1]));
                        dotsIndices.Add(index); // Add dot index to list
                    }
                    // TODO: Checkk if square is made

                    connectionManager.SetLineColor(hit.transform.gameObject.GetComponent<Renderer>().material.color); // Change line color

                    // Add position to line
                    Vector3 point = hit.transform.position;
                    point.y = 0.2f;
                    connectionManager.AddPoint(point);
                }
            }
        }
    }

    /// <summary>
    /// Check if mouse is being held
    /// </summary>
    public void CheckMouseHold()
    {
        if (Input.GetMouseButton(0))
        {
            // Change last position to mouse position
            connectionManager.SetPoint(connectionManager.GetLength(), Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    /// <summary>
    /// Check if mouse is released
    /// </summary>
    public void CheckMouseRelease()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Destroy dots added to list and erase line
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
            connectionManager.EmptyLine(); // Erase line

            // Iterate through indices and remove all connected dots
            for(int i = 0; i < dotsIndices.Count; i++)
            {
                gridManager.RemoveDot(dotsIndices[i][0], dotsIndices[i][1]); // Remove all dots
            }
            dotsIndices = new List<int[]>(); // Reset list
        }

        // Spawn new dots
        if (Input.GetKeyDown(KeyCode.R))
        {
            gridManager.RepopulateGrid();
        }
    }
}
