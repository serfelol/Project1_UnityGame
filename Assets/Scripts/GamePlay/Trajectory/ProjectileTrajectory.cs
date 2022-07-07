using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3 initialPosition;
    private int lineDetail = 20;
    private float timeBetweenPoints = 0.05f;
    int lookingRight = 1;
    float angle;
    float forceBeingApplied;
    public LayerMask checkLayer;

    #region Properties
    public Vector3 InitialPosition
    {
        set { initialPosition = value; }
    }
    public float Angle
    {
        set { angle = value; }
    }
    public float ForceBeingApplied
    {
        set { forceBeingApplied = value; }
    }
    #endregion


    private void Start()
    {
        if(!(lineRenderer = GetComponent<LineRenderer>()))
        {
            Debug.Log("LineRenderer component not found on trajectory line");
        }

        transform.position = initialPosition;
    }

    private void Update()
    {
        lineRenderer.positionCount = lineDetail;
        
        if (transform.parent.transform.localScale.x >= 0) lookingRight = 1;
        else lookingRight = -1;

        Vector3[] trajectoryPoints = new Vector3[lineDetail];
        for(int i = 0; i < lineDetail; i++)
        {
            float timeStep = timeBetweenPoints * i;
            Vector3 newPoint = transform.position;
            newPoint.x += forceBeingApplied * Mathf.Cos(angle * Mathf.Deg2Rad) * timeStep * lookingRight;
            newPoint.y += forceBeingApplied * Mathf.Sin(angle * Mathf.Deg2Rad) * timeStep + Physics2D.gravity.y * Mathf.Pow(timeStep, 2) / 2; 
            
            trajectoryPoints[i] = newPoint;
            
            if(Physics2D.OverlapCircle(newPoint, 0.1f, checkLayer) != null)
            {
                lineRenderer.positionCount = i + 1;
                break;
            }

        }

        lineRenderer.SetPositions(trajectoryPoints);
    }
}
