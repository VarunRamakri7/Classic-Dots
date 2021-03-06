using System.Collections;
using UnityEngine;

public class S_Dot
{
    public enum DotColors { RED, GREEN, BLUE }; // Possible colors of dots

    private GameObject dot; // GameObject representing a dot
    private Vector3 cellPos; // Position of dot in the grid
    private Vector3 nextPos; // Position for dot to fall to
    private bool isOccupied = false; // True if this cell is occupied in the grid
    private int color; // Color of dot
    private float speed = 1.0f; // Dropping speed

    /// <summary>
    /// Default Constructor
    /// </summary>
    public S_Dot()
    {
        dot = null;
        cellPos = new Vector3();
        nextPos = new Vector3();
        isOccupied = false;
        SetColor(1);
    }

    /// <summary>
    /// Instance constructor
    /// </summary>
    /// <param name="_dot"></param>
    /// <param name="position"></param>
    /// <param name="_color"></param>
    /// <param name="status"></param>
    public S_Dot(GameObject _dot, Vector3 position, int _color, bool status)
    {
        dot = _dot;
        cellPos = position;
        isOccupied = status;
        SetColor(_color);
    }

    public void SetDotColor(Color _color)
    {
        if (dot != null && dot.GetComponent<Renderer>() != null)
        {
            //Debug.Log("Setting color: " + (DotColors)color);
            dot.GetComponent<Renderer>().material.color = _color;
        }
    }

    public IEnumerator DropDot()
    {
        while (dot.transform.position != nextPos)
        {
            dot.transform.position = Vector3.MoveTowards(dot.transform.position, nextPos, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    #region Getters and Setters
    public void SetDot(GameObject _dot)
    {
        if (_dot != null)
        {
            dot = _dot;
            dot.transform.position = cellPos;
        }
    }

    public GameObject GetDot()
    {
        return dot;
    }

    public void SetPosition(Vector3 position)
    {
        cellPos = position;

        if (dot != null && dot.tag == "dot")
        {
            dot.transform.position = cellPos;
        }
    }

    public Vector3 GetPosition()
    {
        return cellPos;
    }

    public void SetNextPosition(Vector3 position)
    {
        nextPos = position;
    }

    public Vector3 GetNextPosition()
    {
        return nextPos;
    }

    public void SetColor(int _color)
    {
        color = _color;

        switch(color)
        {
            case 0: // Red
                SetDotColor(new Color(1.0f, 0.65f, 0.65f, 1.0f));
                break;
            case 1: // Green
                SetDotColor(new Color(0.65f, 1.0f, 0.65f, 1.0f));
                break;
            case 2: // Blue
                SetDotColor(new Color(0.65f, 0.65f, 1.0f, 1.0f));
                break;
        }
    }

    public int GetColor()
    {
        return color;
    }

    public void SetOccupied(bool status)
    {
        isOccupied = status;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }
    #endregion
}
