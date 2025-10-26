using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringObstacle : MonoBehaviour {

    public float radius;
    private void OnEnable()
    {
        SteeringObstacleManager.instance.RegisterObstacle(this);
    }

    private void OnDisable() {
        if (SteeringObstacleManager.instance == null) return;
        SteeringObstacleManager.instance.UnRegisterObstacle(this);
    }

    public float GetRadius() {
        return radius = transform.lossyScale.x;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
