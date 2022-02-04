﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ConnectDots : MonoBehaviour
{
    public S_ConnectDots connectDots;

    //[SerializeField]
    //private S_GameManager gameManager;
    [SerializeField]
    private S_DotGridManager gridManager;

    private LineRenderer lineRenderer;
    private List<Vector3> positions; // Positions for line renderer

    private void Start()
    {
        if(connectDots == null)
        {
            connectDots = this;
        }

        // Get line renderer and set defaults
        lineRenderer = this.gameObject.GetComponent<LineRenderer>(); // Add line renderer to GameObject
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.useWorldSpace = true;

        positions = new List<Vector3>();
    }

    /// <summary>
    /// Draw line between points
    /// </summary>
    public void DrawLine()
    {
        lineRenderer.positionCount = positions.Count; // Set position count
        lineRenderer.SetPositions(positions.ToArray()); // Set positions
    }

    /// <summary>
    /// Add position for Line renderer
    /// </summary>
    /// <param name="point">Point to add to Line</param>
    public void AddPoint(Vector3 point)
    {
        // Add positions for line
        positions.Add(point);

        DrawLine();
    }

    /// <summary>
    /// Set the point at the given index to the given position
    /// </summary>
    /// <param name="index">Index to change</param>
    /// <param name="point">New position</param>
    public void SetPoint(int index, Vector3 point)
    {
        positions[index] = point;
    }

    /// <summary>
    /// Get the point at the specified index
    /// </summary>
    /// <param name="index">Index to get</param>
    /// <returns></returns>
    public Vector3 GetPoint(int index)
    {
        return positions[index];
    }

    /// <summary>
    /// Get number of points in line
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        return positions.Count;
    }

    public void EmptyLine()
    {
        positions = new List<Vector3>();
        lineRenderer.positionCount = 0; // Set position count to 0
    }

    /// <summary>
    /// Set color of line rendered
    /// </summary>
    /// <param name="color">New color of line renderer</param>
    public void SetLineColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}