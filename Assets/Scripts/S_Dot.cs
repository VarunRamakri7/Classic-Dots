using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Dot
{
    public enum DotColors { RED, GREEN, BLUE };

    private GameObject dot;
    //private int[] index;
    private Vector3 cellPos;
    private bool isOccupied;
    private int color;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public S_Dot()
    {
        dot = new GameObject();
        cellPos = new Vector3();
        isOccupied = false;
        //index = new int[2]{ -1, -1};
        SetColor(1);
    }

    /// <summary>
    /// Instance constructor
    /// </summary>
    /// <param name="_dot"></param>
    /// <param name="position"></param>
    /// <param name="_color"></param>
    /// <param name="status"></param>
    public S_Dot(GameObject _dot, /*int[] _index,*/ Vector3 position, int _color, bool status)
    {
        dot = _dot;
        //index = _index;
        cellPos = position;
        isOccupied = status;
        SetColor(_color);
    }

    public void SetDotColor(Color _color)
    {
        if (dot.GetComponent<Renderer>() != null)
        {
            //Debug.Log("Setting color: " + (DotColors)color);
            dot.GetComponent<Renderer>().material.color = _color;
        }
    }

    public void MoveDotSlowly()
    {
        while (dot.transform.position.z < cellPos.z)
        {
            dot.transform.Translate(-Vector3.forward * 2.0f * Time.deltaTime);
        }
    }

    #region Getters and Setters
    public void SetDot(GameObject _dot)
    {
        dot = _dot;
        dot.transform.position = cellPos;
        //MoveDotSlowly();
    }

    public GameObject GetDot()
    {
        return dot;
    }

    /*public void SetIndex(int x, int y)
    {
        index = new int[2] { x, y };
    }

    public int[] GetIndex()
    {
        return index;
    }*/

    public void SetPosition(Vector3 position)
    {
        cellPos = position;
    }

    public Vector3 GetPosition()
    {
        return cellPos;
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
