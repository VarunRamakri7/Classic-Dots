using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Dot
{
    private GameObject dot;
    private Vector3 cellPos;
    private bool isOccupied;

    public S_Dot()
    {
        dot = new GameObject();
        cellPos = new Vector3();
        isOccupied = false;
    }

    public S_Dot(GameObject _dot, Vector3 position, bool status)
    {
        dot = _dot;
        cellPos = position;
        isOccupied = status;
    }

    #region Getters and Setters
    public void SetDot(GameObject _dot)
    {
        dot = _dot;
    }

    public GameObject GetDot()
    {
        return dot;
    }

    public void SetPosition(Vector3 position)
    {
        cellPos = position;
    }

    public Vector3 GetTransform()
    {
        return cellPos;
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
