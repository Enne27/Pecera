using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;
using System.Threading;
public class SteeringAgent : MonoBehaviour {
    [Header("General Configuration")]
    public Transform target;
    public float maxVelocity;
    public float maxSpeed;
    public float maxForce;
    Vector3 velocity = Vector3.zero;
    public float mass;

    [Header("Arrive")]
    public float slowingRadius = 1.0f;

    [Header("Wander")]
    public float wanderTimer = 0.0f;
    public float wanderEvaluationTime = 5.0f;
    public float wanderCircleDistance;
    public float maxWanderAngle;
    public float wanderCircleRadius;
    public Vector3 wanderPosition;

    [Header("Collision Avoidance")]
    public float maxSeeAhead = 2.0f;
    public float maxAvoidForce = 10.0f;

    private void Update() {
        //Force Application on Velocity
       // velocity = Vector3.ClampMagnitude(velocity + Flee(target.position) + ObstacleAvoidance(), maxSpeed);
        velocity = Vector3.ClampMagnitude(velocity + Wander() + ObstacleAvoidance(), maxSpeed);

        //velocity = velocity + Seek(target.position);

        //Velocity Application on position
        transform.position = transform.position + velocity * Time.deltaTime;
    }

    Vector3 Seek(Vector3 target) {
        //Seek
        Vector3 desiredVelocity = (target - transform.position).normalized * maxVelocity;
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering / mass;
        return steering;
    }

    Vector3 Flee(Vector3 target) {
        return Seek(target) * -1;
    }

    Vector3 Arrive(Vector3 target) {
        Vector3 steering;

        Vector3 desiredVelocity = target - transform.position;
        float distance = desiredVelocity.magnitude;
        if (distance < slowingRadius) {
            //Dentro del aro de ralentización
            desiredVelocity = desiredVelocity.normalized * maxVelocity * (distance / slowingRadius);
            steering = desiredVelocity - velocity;
        } else {
            //Fuera del aro de ralentización
            steering = Seek(target);
        }

        return steering;
    }

    Vector3 Wander() {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderEvaluationTime) {
            wanderTimer = 0;
            UpdateTargetPosForWandering();
        }
        return Seek(wanderPosition);
    }

    Vector3 ObstacleAvoidance() {
        Vector3 steering = Vector3.zero;
        Vector3 ahead = transform.position + velocity.normalized * maxSeeAhead;
        Vector3 ahead2 = ahead / 2;
        SteeringObstacle mostThreatening = FindMostThreateningObstacle(ahead, ahead2);

        if (mostThreatening != null) {
            steering.x = ahead.x - mostThreatening.transform.position.x;
            steering.y = ahead.y - mostThreatening.transform.position.y;
            steering = steering.normalized * maxAvoidForce;

        } else {
            steering *= 0;
        }

        return steering;
    }

    /*private function collisionAvoidance() :Vector3D {
	ahead = ...; // calculate the ahead vector 
	ahead2 = ...; // calculate the ahead2 vector 
	var mostThreatening :Obstacle = findMostThreateningObstacle();
	var avoidance :Vector3D = new Vector3D(0, 0, 0);
	if (mostThreatening != null) {
		avoidance.x = ahead.x - mostThreatening.center.x;
		avoidance.y = ahead.y - mostThreatening.center.y;
		avoidance.normalize();
		avoidance.scaleBy(MAX_AVOID_FORCE);
	} else {
		avoidance.scaleBy(0); // nullify the avoidance force 
	}
	return avoidance;
}
*/


    void UpdateTargetPosForWandering() {
        Vector2 position = transform.position;
        Vector2 targetPos = target.position;
        Vector2 desiredMovement = targetPos - position;
        Vector2 desiredDirection = desiredMovement.normalized;

        Vector2 circleCenter = position + desiredDirection * wanderCircleDistance;
        float randomAngleToAdd = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);
        float wanderAngle = randomAngleToAdd;

        Vector2 displacementVector = Vector2.up * wanderCircleRadius;


        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.forward);
        Vector2 randomlyRotatedDisplacementVector = rotation * displacementVector;

        Vector2 newTargetPos = circleCenter + randomlyRotatedDisplacementVector;
        if (target) target.position = newTargetPos;
        wanderPosition = newTargetPos;
    }


    SteeringObstacle FindMostThreateningObstacle(Vector3 ahead, Vector3 ahead2) {
        SteeringObstacle mostThreatening = null;

        for (int i = 0; i < SteeringObstacleManager.instance.obstacles.Count; i++) {
            SteeringObstacle obstacle = SteeringObstacleManager.instance.obstacles[i];
            bool collision = LineIntersectsCircle(ahead, ahead / 2, obstacle);
            if (collision && (mostThreatening == null
                || Vector3.Distance(transform.position, obstacle.transform.position)
                    < Vector3.Distance(transform.position, mostThreatening.transform.position))) {

                mostThreatening = obstacle;
            }
        }

        return mostThreatening;
    }

    private bool LineIntersectsCircle(Vector3 ahead, Vector3 ahead2, SteeringObstacle obstacle) {
        // the property "center" of the obstacle is a Vector3D. 
        return Vector3.Distance(obstacle.transform.position, ahead) <= obstacle.GetRadius() || Vector3.Distance(obstacle.transform.position, ahead2) <= obstacle.GetRadius();
    }


}
