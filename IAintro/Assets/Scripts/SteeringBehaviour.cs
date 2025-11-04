using System;
using System.Collections.Generic;
using UnityEngine;


public class SteeringBehaviour : MonoBehaviour
{
    #region VARS
    [Header("FishEntry")] public FishEntry.FishType fishType; 

    #region STATE   
    public enum FishStates { SEEK, FLEE, WANDER, ARRIVE }
    [SerializeField] private FishStates currentState = FishStates.WANDER;
    #endregion

    [Header("General Values")]
    [SerializeField] float maxVelocity = 2f;
    [SerializeField] float mass = 1f;
    float maxForce = 2.0f;

    [SerializeField] public Transform target;
    Vector3 velocity;
    Vector3 steeringForce;

    public float MaxVelocity => maxVelocity; // propiedad pública de solo lectura
    
    [Header("Flee Settings")]
    [SerializeField] private float baseMaxVelocity = 1.5f; // velocidad normal de los peces
    [SerializeField] private float fleeMultiplier = 2f;

    [Header("Arrive Settings")]
    [SerializeField] float slowingRadius = 1.5f;

    [Header("Wander Settings")]
    [SerializeField] float wanderTimer = 0.0f;
    [SerializeField] float wanderEvaluationTime = 3.0f;
    [SerializeField] float wanderCircleRadius = 1.5f;
    [SerializeField] float wanderCircleDistance = 1.5f;
    [SerializeField] float maxWanderAngle = 30f;
    Vector3 wanderPosition;

    [Header("Collision Avoidance")]
    [SerializeField] float maxSeeAhead = 2.0f;
    [SerializeField] float maxAvoidForce = 10.0f;
    [SerializeField] Rigidbody2D rb;

    #endregion

    #region GETTERS_SETTERS
    public void ChangeBehaviourState(FishStates newState)
    {
        currentState = newState;
    }

    public FishStates GetState()
    {
        return currentState;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != FishStates.WANDER)
            return;

        // Reflejar la dirección actual de movimiento según la normal del impacto
        Vector3 reflectDir = Vector3.Reflect(velocity.normalized, collision.contacts[0].normal).normalized;

        // Ajustar la velocidad y forzar un cambio de dirección
        velocity = reflectDir * maxVelocity;

        // Actualizar el objetivo de wander para seguir en esa dirección reflejada
        wanderPosition = transform.position + reflectDir * wanderCircleDistance;

        // Reiniciar el temporizador para evitar cambio inmediato
        wanderTimer = 0f;

        // Pequeño desplazamiento para evitar quedarse pegado
        //transform.position += reflectDir * 0.1f;
    }

    void FixedUpdate()
    {
        maxVelocity = baseMaxVelocity;
        switch (currentState)
        {
            case FishStates.SEEK:
                // Actualizar la velocidad con el steering
                //velocity = Vector3.ClampMagnitude(velocity + Seek(target.position) + ObstacleAvoidance(), maxVelocity);
                velocity = MovementBehaviour(() => Seek(target.position) /*+ Arrive(target.position)*/);
                break;
            case FishStates.FLEE:
                // Actualizar la velocidad con el steering
                //velocity = Vector3.ClampMagnitude(velocity + Flee(), maxVelocity);
                maxVelocity = baseMaxVelocity * fleeMultiplier; // más rápido al huir
                velocity = MovementBehaviour(() => Flee());
                break;
            case FishStates.WANDER:
                //velocity = Vector3.ClampMagnitude(velocity + Wander(), maxVelocity);
                Vector3 desired_velocity = MovementBehaviour(() => Wander());
                velocity = Vector3.Lerp(velocity, desired_velocity, Time.deltaTime * 2f);
                //velocity = MovementBehaviour(() => Wander());
                break;
            case FishStates.ARRIVE:
                // velocity = Vector3.ClampMagnitude(velocity + Arrive(target.position), maxVelocity);
                velocity = MovementBehaviour(() => Arrive(target.position));
                break;
        }

        // Aplicar velocidad
       /* transform.position += velocity * Time.fixedDeltaTime
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);*/
        rb.MovePosition(rb.position + (Vector2)velocity * Time.fixedDeltaTime);

        // Orientarlo, en lugar de hacer rotaciones que quedan extrañas cuando hay un giro grande en el wander.
        if (velocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.fixedDeltaTime);
        }
    }


    #region BEHAVIOUR_METHODS
    /// <summary>
    /// Seguir al vector indicado.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector3 Seek(Vector3 target)
    {
        // Calcular desired_velocity hacia el target (el vector de dirección, básicamente)
        Vector3 desired_velocity = (target - transform.position).normalized * maxVelocity;

        // steering = desired - current (fuerza de viraje)
        steeringForce = desired_velocity - velocity;
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce); // Truncate, evitar que no pueda superar el máximo de longitud.
        steeringForce = steeringForce / mass;
        Debug.DrawLine(transform.position, transform.position + desired_velocity, Color.yellow);
        return steeringForce;
    }

    /// <summary>
    /// Huir del vector indicado.
    /// </summary>
    /// <returns></returns>
    private Vector3 Flee()
    {
        return Seek(target.position) * -1;
    }

    /// <summary>
    /// Cuando entramos en el radio especificado, empezar a frenar.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector3 Arrive(Vector3 target)
    {
        Vector3 steering;

        Vector3 desiredVelocity = target - transform.position;
        float distance = desiredVelocity.magnitude;
        if (distance < slowingRadius)
        {
            //Dentro del aro de ralentización
            desiredVelocity = desiredVelocity.normalized * maxVelocity * (distance / slowingRadius);
            steering = desiredVelocity - velocity;
        }
        else
        {
            //Fuera del aro de ralentización
            steering = Seek(target);
        }
        return steering;
    }

    /// <summary>
    /// Hacer rondas.
    /// </summary>
    /// <returns></returns>
    private Vector3 Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderEvaluationTime)
        {
            wanderTimer = 0f;
            UpdateWanderTarget(); // Calcula un nuevo destino aleatorio
        }

        // Calcular la fuerza deseada hacia el punto de wander
        Vector3 desiredVelocity = (wanderPosition - transform.position).normalized * maxVelocity;

        // Steering = desired - current velocity
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering /= mass;

        // Sumar el steering de colisión (ya lo haces en MovementBehaviour)
        // pero dejamos aquí la línea de depuración
        Debug.DrawLine(transform.position, wanderPosition, Color.cyan);

        return steering;
    }

    /// <summary>
    /// Método para no repetir el comportamiento del cambio en el vector velocity. Recibe la función necesaria y 
    /// </summary>
    /// <param name="steeringFunction"></param>
    /// <returns></returns>
    private Vector3 MovementBehaviour(Func<Vector3> steeringFunction) // Func<Vector3> es que esa función devuelve un vector3
    {
        Debug.DrawLine(transform.position, transform.position + steeringFunction(), Color.green);
        Debug.DrawLine(transform.position, transform.position + velocity, Color.blue);

        return Vector3.ClampMagnitude(velocity + steeringFunction()+ ObstacleAvoidance() * 1.5f /*+ AvoidOtherFish(FishManager.instance.allFish)*/, maxVelocity);
    }

    #endregion

    Vector3 SetAngle(Vector3 vector, float angle)
    {
        float length = vector.magnitude;
        vector.x = Mathf.Cos(angle) * length;
        vector.y = Mathf.Sin(angle) * length;
        return vector;
    }

    void UpdateWanderTarget()
    {
        Vector3 circleCenter = velocity.normalized * wanderCircleDistance;

        // Vector de desplazamiento inicial en el círculo
        Vector3 displacement = new Vector3(0, -1, 0) * wanderCircleRadius;

        // Pequeño cambio de ángulo aleatorio dentro del rango máximo
        float randomAngle = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);
        displacement = SetAngle(displacement, randomAngle * Mathf.Deg2Rad);

        // Combinar el centro del círculo con el desplazamiento rotado
        Vector3 wanderForce = circleCenter + displacement;

        // Calcular la nueva posición de wander (relativa al agente)
        wanderPosition = transform.position + wanderForce;
    }

    #region Obstacle
    Vector3 AvoidOtherFish(List<SteeringBehaviour> allFish)
    {
        Vector3 avoidance = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in allFish)
        {
            // No te compares a ti mismo
            if (other == this) continue;

            // Los peces pequeños evitan a los grandes
            if (this.fishType == FishEntry.FishType.LITTLE && other.fishType == FishEntry.FishType.BIG)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                float avoidRadius = 2.0f; // ajusta según la escala

                if (distance < avoidRadius)
                {
                    // Vector de alejamiento
                    Vector3 fleeDir = (transform.position - other.transform.position).normalized;
                    avoidance += fleeDir / distance; // más fuerte cuanto más cerca
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
            avoidance /= neighborCount;

        return avoidance;
    }

    Vector3 ObstacleAvoidance()
    {
        Vector3 steering = Vector3.zero;
        Vector3 ahead = transform.position + velocity.normalized * maxSeeAhead;

        //Vector3 ahead2 = ahead / 2;
        Vector3 ahead2 = transform.position + velocity.normalized * (maxSeeAhead * 0.5f);

        SteeringObstacle mostThreatening = FindMostThreateningObstacle(ahead, ahead2);

        if (mostThreatening != null)
        {
            steering.x = ahead.x - mostThreatening.transform.position.x;
            steering.y = ahead.y - mostThreatening.transform.position.y;
            steering = steering.normalized * maxAvoidForce;
        }
        else
        {
            steering *= 0;
        }

        Debug.DrawLine(transform.position, transform.position + steering, Color.red);

        return steering;
    }

    SteeringObstacle FindMostThreateningObstacle(Vector3 ahead, Vector3 ahead2)
    {
        SteeringObstacle mostThreatening = null;

        foreach (var obstacle in SteeringObstacleManager.instance.obstacles)
        {
            bool collision = LineIntersectsCircle(ahead, ahead2, obstacle);

            if (collision && (mostThreatening == null ||
                Vector3.Distance(transform.position, obstacle.transform.position) <
                Vector3.Distance(transform.position, mostThreatening.transform.position)))
            {
                mostThreatening = obstacle;
            }
        }

        return mostThreatening;
    }

    private bool LineIntersectsCircle(Vector3 ahead, Vector3 ahead2, SteeringObstacle obstacle)
    {
        /*// the property "center" of the obstacle is a Vector3D. 
        return Vector3.Distance(obstacle.transform.position, ahead) <= obstacle.GetRadius() ||
            Vector3.Distance(obstacle.transform.position, ahead2) <= obstacle.GetRadius();*/
        float radius = obstacle.GetRadius();
        Vector3 center = obstacle.transform.position;

        return Vector3.Distance(center, ahead) <= radius || Vector3.Distance(center, ahead2) <= radius;
    }

    #endregion
}