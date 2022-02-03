using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager gameManager;

    [SerializeField]
    private S_DotGridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }

        gridManager.MakeGrid(); // Make dots grid
    }

    private void Update()
    {
        CheckMouseHit();

        if (Input.GetKeyDown(KeyCode.R))
        {
            gridManager.RepopulateGrid();
        }
    }

    private void CheckMouseHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Select stage    
                if (hit.transform.gameObject.tag == "dot")
                {
                    Debug.Log("Hit: " + hit.transform.gameObject.name);

                    // Get index of dot
                    string dotName = hit.transform.gameObject.name;
                    //Debug.Log("Name: " + dotName);

                    char[] trimChar = { 'd', 'o', 't', '(', ')' };
                    string trimmed = dotName.Trim(trimChar); // Remove all extra chars except ','
                    //Debug.Log("After trim: " + trimmed);

                    int[] index = { (int)char.GetNumericValue(trimmed[0]), (int)char.GetNumericValue(trimmed[2]) }; // Get numeric value from chars
                    //Debug.Log("Index: (" + index[0] + ", " + index[1] + ")");

                    gridManager.RemoveDot(index[0], index[1]); // Remove dot at location
                }
            }
        }
    }
}
