using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    #region VARS
    private List<GameObject> allFishes;
    [SerializeField, Tooltip("Rango para asustar a los peces")] private float fleeRadius = 2f;

    Vector2 mousePosition;
    GameObject goTarget;
    #endregion

    private void Start()
    {
        allFishes = FishManager.instance.allFishes;

        goTarget = new GameObject("Target");
        goTarget.transform.SetParent(transform);
    }
    private void FixedUpdate()
    {
        MouseInteractionMethod();
    }

    private void MouseInteractionMethod()
    {
        // Obtener posición del ratón en coordenadas de mundo
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (GameObject fish in allFishes)
        {
            SteeringBehaviour sb = fish.GetComponent<SteeringBehaviour>();
            if (sb == null) 
                continue; 

            float distance = Vector2.Distance(mousePos, sb.transform.position);
            if (distance <= fleeRadius)
            {
                if (sb.target == null)
                    sb.target = goTarget.transform;
                sb.target.position = (Vector3)mousePos;

                if (Input.GetMouseButtonDown(0) && (sb.fishType == FishEntry.FishType.FISH || sb.fishType == FishEntry.FishType.BIG)) 
                {   // Clic izquierdo
                    sb.ChangeBehaviourState(SteeringBehaviour.FishStates.SEEK);
                }

                if (sb.fishType == FishEntry.FishType.LITTLE) 
                    sb.ChangeBehaviourState(SteeringBehaviour.FishStates.FLEE);
               
            }
            else if (sb.GetState() != SteeringBehaviour.FishStates.WANDER)
            {
                sb.ChangeBehaviourState(SteeringBehaviour.FishStates.WANDER);
            }

        }
    }
}
