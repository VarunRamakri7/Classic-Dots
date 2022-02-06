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
        lineRenderer.startWidth = 0.25f;
        lineRenderer.endWidth = 0.25f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false; // Disable line renderer

        positions = new List<Vector3>();
    }

    /// <summary>
    /// Draw line between points
    /// </summary>
    public void DrawLine()
    {
        lineRenderer.positionCount = positions.Count; // Set position count
        lineRenderer.SetPositions(positions.ToArray()); // Set positions
        lineRenderer.enabled = true;
    }

    /// <summary>
    /// Add position for Line renderer
    /// </summary>
    /// <param name="point">Point to add to Line</param>
    public void AddPoint(Vector3 point)
    {
        if (!positions.Contains(point) || point.Equals(positions[0]))
        {
            point.y = 0.2f; // Flatten point
            positions.Add(point); // Add new position to list
            DrawLine(); // Draw line
        }
    }

    /// <summary>
    /// Set the point at the given index to the given position
    /// </summary>
    /// <param name="index">Index to change</param>
    /// <param name="point">New position</param>
    public void SetPoint(int index, Vector3 point)
    {
        if (index < positions.Count)
        {
            point.y = 0.2f; // Flatten point
            positions[index] = point;

            DrawLine();
        }
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

    /// <summary>
    /// Empty the line and positions
    /// </summary>
    public void EmptyLine()
    {
        lineRenderer.enabled = false;
        positions = new List<Vector3>(); // Reset positions
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

    /// <summary>
    /// Return color of line
    /// </summary>
    /// <returns>Color of line</returns>
    public Color GetLineColor()
    {
        return lineRenderer.endColor;
    }
}
