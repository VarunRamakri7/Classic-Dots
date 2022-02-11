using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DropDot : MonoBehaviour
{
    private Vector3 nextPos;
    private bool canDrop;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        nextPos = new Vector3();
        canDrop = false;
        speed = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (canDrop && Vector3.Distance(this.transform.position, nextPos) < 0)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, nextPos, speed * Time.deltaTime);
        }
    }

    #region Getters and Setters

    public void SetNextPosition(Vector3 position)
    {
        nextPos = position;
    }

    public void SetCanDrop(bool status)
    {
        canDrop = status;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    public Vector3 GetNextPosition()
    {
        return nextPos;
    }

    public bool CanDrop()
    {
        return canDrop;
    }

    public float GetSpeed()
    {
        return speed;
    }
    #endregion
}
